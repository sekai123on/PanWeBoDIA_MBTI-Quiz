using Microsoft.EntityFrameworkCore;
using MbtiApi.Application.Interfaces;
using MbtiApi.Domain.Entities;
using MbtiApi.Infrastructure.Data;

namespace MbtiApi.Infrastructure.Repositories;

// ── QuestionRepository ────────────────────────────────────────────────────────
public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _db;
    public QuestionRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Question>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.Questions
            .Where(q => q.IsActive)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(ct);

    public async Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Questions.FindAsync(new object[] { id }, ct);

    public async Task AddAsync(Question question, CancellationToken ct = default) =>
        await _db.Questions.AddAsync(question, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}

// ── SessionRepository ─────────────────────────────────────────────────────────
public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _db;
    public SessionRepository(AppDbContext db) => _db = db;

    public async Task<TestSession?> GetByIdWithAnswersAsync(Guid id, CancellationToken ct = default) =>
        await _db.TestSessions
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<TestSession> CreateAsync(TestSession session, CancellationToken ct = default)
    {
        await _db.TestSessions.AddAsync(session, ct);
        return session;
    }

    public async Task AddAnswersAsync(Guid sessionId, IEnumerable<Answer> answers, CancellationToken ct = default)
    {
        await _db.Answers.AddRangeAsync(answers, ct);
    }

    public async Task CompleteAsync(Guid sessionId, CancellationToken ct = default)
    {
        var session = await _db.TestSessions.FindAsync(new object[] { sessionId }, ct);
        if (session is not null)
        {
            session.IsCompleted  = true;
            session.CompletedAt  = DateTime.UtcNow;
        }
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}

// ── ResultRepository ──────────────────────────────────────────────────────────
public class ResultRepository : IResultRepository
{
    private readonly AppDbContext _db;
    public ResultRepository(AppDbContext db) => _db = db;

    public async Task<TestResult?> GetBySessionIdAsync(Guid sessionId, CancellationToken ct = default) =>
        await _db.TestResults.FirstOrDefaultAsync(r => r.SessionId == sessionId, ct);

    public async Task AddAsync(TestResult result, CancellationToken ct = default) =>
        await _db.TestResults.AddAsync(result, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
