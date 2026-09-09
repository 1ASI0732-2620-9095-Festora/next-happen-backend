using System.Security.Claims;
using com.festora.nexthappen.engagement.Application.DTOs;
using com.festora.nexthappen.engagement.Application.Services;
using com.festora.nexthappen.engagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.festora.nexthappen.engagement.API.Controllers;

[ApiController]
[Route("api/users/saved-events")]
[Authorize]
public class SavedEventsController : ControllerBase
{
    private readonly SavedEventService _service;

    public SavedEventsController(SavedEventService service)
    {
        _service = service;
    }

    private Guid GetAuthenticatedUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid.TryParse(userIdStr, out var userId);
        return userId;
    }

    [HttpPost("{eventId:guid}")]
    public async Task<IActionResult> SaveEvent(Guid eventId)
    {
        var userId = GetAuthenticatedUserId();
        var success = await _service.SaveEventAsync(userId, eventId);
        return success ? Ok() : Conflict("Event already saved.");
    }

    [HttpDelete("{eventId:guid}")]
    public async Task<IActionResult> Delete(Guid eventId)
    {
        var userId = GetAuthenticatedUserId();
        var success = await _service.RemoveAsync(userId, eventId);
        return success ? Ok() : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetAuthenticatedUserId();
        var events = await _service.GetByUserAsync(userId);
        return Ok(events);
    }
}

[ApiController]
[Route("api/metrics")]
public class MetricsController : ControllerBase
{
    private readonly MetricService _service;

    public MetricsController(MetricService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] Metric body)
    {
        await _service.RegisterAsync(body.EventId, body.Action);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost("event-view/{eventId:guid}")]
    public async Task<IActionResult> RegisterEventView(Guid eventId)
    {
        await _service.RegisterAsync(eventId, "view-event");
        return Ok();
    }
}

[ApiController]
[Route("api/events/{eventId:guid}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _service;

    public ReviewsController(ReviewService service)
    {
        _service = service;
    }

    /// <summary>Lista pública de reseñas con promedio y distribución.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetForEvent(Guid eventId)
        => Ok(await _service.GetSummaryAsync(eventId));

    /// <summary>Crea o actualiza la reseña del usuario autenticado para el evento.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Guid eventId, [FromBody] CreateReviewRequest request)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var userName = User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue("name")
            ?? "Usuario";

        try
        {
            var review = await _service.UpsertAsync(eventId, userId, userName, request.Rating, request.Comment);
            return Ok(review);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetAuthenticatedUserId()
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        return userId;
    }
}
