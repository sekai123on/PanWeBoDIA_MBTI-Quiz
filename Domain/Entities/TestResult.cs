namespace MbtiApi.Domain.Entities;

public class TestResult
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SessionId { get; set; }
    public TestSession Session { get; set; } = null!;

    /// <summary>One of the 16 MBTI types, e.g. "INTJ", "ENFP".</summary>
    public string MbtiType { get; set; } = string.Empty;

    // Dimension scores (percentage leaning toward the A-pole)
    public int EiScore { get; set; } // higher = more E
    public int SnScore { get; set; } // higher = more S
    public int TfScore { get; set; } // higher = more T
    public int JpScore { get; set; } // higher = more J

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
