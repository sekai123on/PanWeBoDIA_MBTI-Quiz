using MbtiApi.Application.DTOs.Request;
using MbtiApi.Application.DTOs.Response;
using MbtiApi.Application.Interfaces;
using MbtiApi.Domain.Entities;
using MbtiApi.Domain.Enums;

namespace MbtiApi.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuestionRepository _questionRepo;
    private readonly ISessionRepository _sessionRepo;
    private readonly IResultRepository _resultRepo;

    public QuizService(
        IQuestionRepository questionRepo,
        ISessionRepository sessionRepo,
        IResultRepository resultRepo)
    {
        _questionRepo = questionRepo;
        _sessionRepo  = sessionRepo;
        _resultRepo   = resultRepo;
    }

    // ── Get all active questions, localised ──────────────────────────────
    public async Task<IEnumerable<QuestionResponse>> GetQuestionsAsync(
        string language, CancellationToken ct = default)
    {
        var questions = await _questionRepo.GetAllActiveAsync(ct);
        bool isMy = language.Equals("my", StringComparison.OrdinalIgnoreCase);

        return questions
            .OrderBy(q => q.OrderIndex)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Dimension.ToString(),
                q.OrderIndex,
                Text:    isMy ? q.TextMy    : q.TextEn,
                OptionA: isMy ? q.OptionAMy : q.OptionAEn,
                OptionB: isMy ? q.OptionBMy : q.OptionBEn
            ));
    }

    // ── Start a new test session ─────────────────────────────────────────
    public async Task<SessionResponse> StartSessionAsync(
        StartSessionRequest request, CancellationToken ct = default)
    {
        var session = new TestSession
        {
            Language = request.Language.ToLower()
        };

        await _sessionRepo.CreateAsync(session, ct);
        await _sessionRepo.SaveChangesAsync(ct);

        return new SessionResponse(session.Id, session.Language, session.StartedAt);
    }

    // ── Submit all answers and compute MBTI result ───────────────────────
    public async Task<ResultResponse> SubmitAnswersAsync(
        SubmitAllAnswersRequest request, CancellationToken ct = default)
    {
        var session = await _sessionRepo.GetByIdWithAnswersAsync(request.SessionId, ct)
            ?? throw new KeyNotFoundException($"Session {request.SessionId} not found.");

        if (session.IsCompleted)
            throw new InvalidOperationException("Session is already completed.");

        // Persist answers
        var answers = request.Answers.Select(a => new Answer
        {
            SessionId  = request.SessionId,
            QuestionId = a.QuestionId,
            Choice     = a.Choice
        }).ToList();

        await _sessionRepo.AddAnswersAsync(request.SessionId, answers, ct);
        await _sessionRepo.CompleteAsync(request.SessionId, ct);

        // Load questions to know which dimension each answer maps to
        var questions = (await _questionRepo.GetAllActiveAsync(ct))
            .ToDictionary(q => q.Id);

        // ── Scoring ──────────────────────────────────────────────────────
        // A-choice = first pole (E, S, T, J); score tracks % toward A-pole.
        var scores = CalculateScores(answers, questions);

        var result = new TestResult
        {
            SessionId = request.SessionId,
            MbtiType  = BuildType(scores),
            EiScore   = scores[Dimension.EI],
            SnScore   = scores[Dimension.SN],
            TfScore   = scores[Dimension.TF],
            JpScore   = scores[Dimension.JP]
        };

        await _resultRepo.AddAsync(result, ct);
        await _resultRepo.SaveChangesAsync(ct);
        await _sessionRepo.SaveChangesAsync(ct);

        return MapToResponse(result);
    }

    // ── Private helpers ──────────────────────────────────────────────────

    private static Dictionary<Dimension, int> CalculateScores(
        IEnumerable<Answer> answers,
        Dictionary<Guid, Question> questions)
    {
        var totals = new Dictionary<Dimension, (int aCount, int total)>
        {
            [Dimension.EI] = (0, 0),
            [Dimension.SN] = (0, 0),
            [Dimension.TF] = (0, 0),
            [Dimension.JP] = (0, 0)
        };

        foreach (var answer in answers)
        {
            if (!questions.TryGetValue(answer.QuestionId, out var q)) continue;

            var (aCount, total) = totals[q.Dimension];
            totals[q.Dimension] = (
                aCount + (answer.Choice == DimensionChoice.A ? 1 : 0),
                total + 1
            );
        }

        // Convert to 0-100 percentage toward A-pole
        return totals.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.total == 0
                ? 50
                : (int)Math.Round(kvp.Value.aCount * 100.0 / kvp.Value.total)
        );
    }

    private static string BuildType(Dictionary<Dimension, int> scores) =>
        string.Concat(
            scores[Dimension.EI] >= 50 ? "E" : "I",
            scores[Dimension.SN] >= 50 ? "S" : "N",
            scores[Dimension.TF] >= 50 ? "T" : "F",
            scores[Dimension.JP] >= 50 ? "J" : "P"
        );

    private static ResultResponse MapToResponse(TestResult r) =>
        new(
            r.SessionId,
            r.MbtiType,
            r.EiScore,
            r.SnScore,
            r.TfScore,
            r.JpScore,
            Description: MbtiDescriptions.Get(r.MbtiType),
            r.CalculatedAt
        );
}

// ── 16 type descriptions (short, can be expanded) ────────────────────────────
internal static class MbtiDescriptions
{
    private static readonly Dictionary<string, string> _map = new()
    {
        ["INTJ"] = "The Architect — strategic, independent, high standards.",
        ["INTP"] = "The Logician — analytical, curious, loves abstract ideas.",
        ["ENTJ"] = "The Commander — decisive, ambitious, natural leader.",
        ["ENTP"] = "The Debater — innovative, quick-witted, loves challenges.",
        ["INFJ"] = "The Advocate — insightful, principled, quietly determined.",
        ["INFP"] = "The Mediator — idealistic, empathetic, creative dreamer.",
        ["ENFJ"] = "The Protagonist — charismatic, inspiring, people-focused.",
        ["ENFP"] = "The Campaigner — enthusiastic, creative, sociable.",
        ["ISTJ"] = "The Logistician — reliable, practical, fact-minded.",
        ["ISFJ"] = "The Defender — supportive, patient, dedicated.",
        ["ESTJ"] = "The Executive — organised, traditional, strong-willed.",
        ["ESFJ"] = "The Consul — caring, social, eager to help.",
        ["ISTP"] = "The Virtuoso — practical, observant, bold experimenter.",
        ["ISFP"] = "The Adventurer — flexible, charming, artistic.",
        ["ESTP"] = "The Entrepreneur — energetic, perceptive, loves action.",
        ["ESFP"] = "The Entertainer — spontaneous, fun, lives in the moment.",
    };

    public static string Get(string type) =>
        _map.TryGetValue(type, out var desc) ? desc : "A unique MBTI profile.";
}
