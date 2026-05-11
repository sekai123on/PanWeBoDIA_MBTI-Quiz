namespace MbtiApi.Application.DTOs.Response;

public record QuestionResponse(
    Guid Id,
    string Dimension,
    int OrderIndex,
    string Text,
    string OptionA,
    string OptionB
);

public record SessionResponse(
    Guid SessionId,
    string Language,
    DateTime StartedAt
);

public record ResultResponse(
    Guid SessionId,
    string MbtiType,
    int EiScore,
    int SnScore,
    int TfScore,
    int JpScore,
    string Description,
    DateTime CalculatedAt
);

public record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Error = null
)
{
    public static ApiResponse<T> Ok(T data) => new(true, data);
    public static ApiResponse<T> Fail(string error) => new(false, default, error);
}
