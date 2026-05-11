using MbtiApi.Domain.Enums;

namespace MbtiApi.Application.DTOs.Request;

public record StartSessionRequest(string Language = "en");

public record SubmitAnswerRequest(
    Guid QuestionId,
    DimensionChoice Choice
);

public record SubmitAllAnswersRequest(
    Guid SessionId,
    List<SubmitAnswerRequest> Answers
);
