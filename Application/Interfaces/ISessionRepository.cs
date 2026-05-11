using MbtiApi.Domain.Entities;

namespace MbtiApi.Application.Interfaces;

public interface ISessionRepository
{
    Task<TestSession?> GetByIdWithAnswersAsync(Guid id, CancellationToken ct = default);
    Task<TestSession> CreateAsync(TestSession session, CancellationToken ct = default);
    Task AddAnswersAsync(Guid sessionId, IEnumerable<Answer> answers, CancellationToken ct = default);
    Task CompleteAsync(Guid sessionId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
