using com.festora.nexthappen.@event.Application.DTOs;
using com.festora.nexthappen.@event.Application.Services;
using com.festora.nexthappen.@event.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.festora.nexthappen.@event.API.Controllers;

[ApiController]
[Route("api/events")]
public class EventController : ControllerBase
{
    private readonly EventService _service;

    public EventController(EventService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                request.Organizer = userId;
            }

            var ev = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ToResponse(ev));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            var details = ex.InnerException?.Message ?? "";
            return BadRequest(new { error = $"Error de BD o interno: {ex.Message} | {details}" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await _service.GetByIdAsync(id);
        return ev is null ? NotFound() : Ok(ToResponse(ev));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userRole == "Organizer" && !string.IsNullOrEmpty(userId))
        {
            var events = await _service.GetAllAsync();
            events = events.Where(e => e.Organizer == userId);
            return Ok(events.Select(ToResponse));
        }

        if (userRole == "Admin")
        {
            var events = await _service.GetAllAsync();
            return Ok(events.Select(ToResponse));
        }

        // Public/unauthenticated discovery
        var publicEvents = await _service.GetPublicAsync();
        return Ok(publicEvents.Select(ToResponse));
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicEvents()
    {
        var events = await _service.GetPublicAsync();
        return Ok(events.Select(ToResponse));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request)
    {
        try
        {
            var existingEvent = await _service.GetByIdAsync(id);
            if (existingEvent == null) return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && existingEvent.Organizer != userId)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(userId) && userRole != "Admin")
            {
                request.Organizer = userId;
            }

            var result = await _service.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(ToResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingEvent = await _service.GetByIdAsync(id);
        if (existingEvent == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userRole != "Admin" && existingEvent.Organizer != userId)
        {
            return Forbid();
        }

        bool deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/reserve")]
    public async Task<IActionResult> Reserve(Guid id, [FromBody] ReserveEventRequest request)
    {
        var success = await _service.ReserveSeatsAsync(id, request.Quantity);
        if (!success)
        {
            return BadRequest(new { error = "No hay suficientes cupos disponibles o el evento no existe." });
        }
        return Ok();
    }

    // Libera cupos previamente reservados (uso interno: pago expirado o reembolso).
    [HttpPost("{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id, [FromBody] ReserveEventRequest request)
    {
        var success = await _service.ReleaseSeatsAsync(id, request.Quantity);
        if (!success)
        {
            return BadRequest(new { error = "No se pudieron liberar los cupos o el evento no existe." });
        }
        return Ok();
    }

    // ── Helper ──
    private static EventResponse ToResponse(Event ev) => new()
    {
        Id = ev.Id,
        Organizer = ev.Organizer,
        Title = ev.Title,
        Description = ev.Description,
        Price = ev.Price,
        Quantity = ev.Quantity,
        Category = ev.Category,
        Address = ev.Address,
        Location = ev.Location,
        Photos = ev.Photos,
        StartDate = ev.DateRange.StartDate,
        EndDate = ev.DateRange.EndDate,
        IsPublic = ev.IsPublic
    };
}

