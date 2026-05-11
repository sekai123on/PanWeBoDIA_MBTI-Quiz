using MbtiApi.Domain.Entities;

namespace MbtiApi.Application.Interfaces;

public interface IResultRepository
{
    Task<TestResult?> GetBySessionIdAsync(Guid sessionId, CancellationToken ct = default);
    Task AddAsync(TestResult result, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
