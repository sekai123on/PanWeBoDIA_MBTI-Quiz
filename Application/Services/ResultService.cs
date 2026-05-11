using MbtiApi.Application.DTOs.Response;
using MbtiApi.Application.Interfaces;

namespace MbtiApi.Application.Services;

public class ResultService : IResultService
{
    private readonly IResultRepository _resultRepo;

    public ResultService(IResultRepository resultRepo)
    {
        _resultRepo = resultRepo;
    }

    public async Task<ResultResponse?> GetResultAsync(Guid sessionId, CancellationToken ct = default)
    {
        var result = await _resultRepo.GetBySessionIdAsync(sessionId, ct);
        if (result is null) return null;

        return new ResultResponse(
            result.SessionId,
            result.MbtiType,
            result.EiScore,
            result.SnScore,
            result.TfScore,
            result.JpScore,
            Description: MbtiDescriptions.Get(result.MbtiType),
            result.CalculatedAt
        );
    }
}
