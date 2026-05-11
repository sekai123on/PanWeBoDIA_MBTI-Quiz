namespace MbtiApi.Domain.Entities;

public class TestSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Optional — null for anonymous / guest users.</summary>
    public Guid? UserId { get; set; }

    public string Language { get; set; } = "en"; // "en" | "my"

    public bool IsCompleted { get; set; } = false;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public TestResult? Result { get; set; }
}
