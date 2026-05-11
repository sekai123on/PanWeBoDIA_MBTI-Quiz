using MbtiApi.Domain.Entities;

namespace MbtiApi.Application.Interfaces;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Question question, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
