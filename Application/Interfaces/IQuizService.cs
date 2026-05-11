using MbtiApi.Application.DTOs.Request;
using MbtiApi.Application.DTOs.Response;

namespace MbtiApi.Application.Interfaces;

public interface IQuizService
{
    Task<IEnumerable<QuestionResponse>> GetQuestionsAsync(string language, CancellationToken ct = default);
    Task<SessionResponse> StartSessionAsync(StartSessionRequest request, CancellationToken ct = default);
    Task<ResultResponse> SubmitAnswersAsync(SubmitAllAnswersRequest request, CancellationToken ct = default);
}

public interface IResultService
{
    Task<ResultResponse?> GetResultAsync(Guid sessionId, CancellationToken ct = default);
}
