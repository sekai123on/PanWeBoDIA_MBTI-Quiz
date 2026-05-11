using MbtiApi.Domain.Enums;

namespace MbtiApi.Domain.Entities;

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Which MBTI axis this question measures.</summary>
    public Dimension Dimension { get; set; }

    /// <summary>Order index within the full questionnaire.</summary>
    public int OrderIndex { get; set; }

    // ── English ──────────────────────────────────────────────────────────
    public string TextEn { get; set; } = string.Empty;
    public string OptionAEn { get; set; } = string.Empty; // maps to A-pole: E / S / T / J
    public string OptionBEn { get; set; } = string.Empty; // maps to B-pole: I / N / F / P

    // ── Myanmar / Burmese ────────────────────────────────────────────────
    public string TextMy { get; set; } = string.Empty;
    public string OptionAMy { get; set; } = string.Empty;
    public string OptionBMy { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
