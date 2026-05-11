using Microsoft.AspNetCore.Mvc;
using MbtiApi.Application.DTOs.Request;
using MbtiApi.Application.DTOs.Response;
using MbtiApi.Application.Interfaces;

namespace MbtiApi.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class QuizController : ControllerBase
{
    private readonly IQuizService   _quizService;
    private readonly IResultService _resultService;

    public QuizController(IQuizService quizService, IResultService resultService)
    {
        _quizService   = quizService;
        _resultService = resultService;
    }

    /// <summary>Fetch all active questions in the requested language.</summary>
    /// <param name="lang">en (default) or my (Myanmar)</param>
    [HttpGet("questions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<QuestionResponse>>), 200)]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] string lang = "en",
        CancellationToken ct = default)
    {
        var questions = await _quizService.GetQuestionsAsync(lang, ct);
        return Ok(ApiResponse<IEnumerable<QuestionResponse>>.Ok(questions));
    }

    /// <summary>Start a new test session.</summary>
    [HttpPost("sessions")]
    [ProducesResponseType(typeof(ApiResponse<SessionResponse>), 201)]
    public async Task<IActionResult> StartSession(
        [FromBody] StartSessionRequest request,
        CancellationToken ct = default)
    {
        var session = await _quizService.StartSessionAsync(request, ct);
        return CreatedAtAction(nameof(GetResult), new { sessionId = session.SessionId },
            ApiResponse<SessionResponse>.Ok(session));
    }

    /// <summary>Submit all answers for a session and receive the MBTI result.</summary>
    [HttpPost("sessions/{sessionId}/submit")]
    [ProducesResponseType(typeof(ApiResponse<ResultResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> SubmitAnswers(
        Guid sessionId,
        [FromBody] SubmitAllAnswersRequest request,
        CancellationToken ct = default)
    {
        if (sessionId != request.SessionId)
            return BadRequest(ApiResponse<object>.Fail("Session ID mismatch."));

        var result = await _quizService.SubmitAnswersAsync(request, ct);
        return Ok(ApiResponse<ResultResponse>.Ok(result));
    }

    /// <summary>Retrieve the result for a completed session.</summary>
    [HttpGet("sessions/{sessionId}/result")]
    [ProducesResponseType(typeof(ApiResponse<ResultResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetResult(Guid sessionId, CancellationToken ct = default)
    {
        var result = await _resultService.GetResultAsync(sessionId, ct);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Result not found for this session."));

        return Ok(ApiResponse<ResultResponse>.Ok(result));
    }
}
