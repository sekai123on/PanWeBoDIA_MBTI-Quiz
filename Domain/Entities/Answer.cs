using MbtiApi.Domain.Enums;

namespace MbtiApi.Domain.Entities;

public class Answer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SessionId { get; set; }
    public TestSession Session { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    /// <summary>A = first pole (E/S/T/J), B = second pole (I/N/F/P).</summary>
    public DimensionChoice Choice { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
