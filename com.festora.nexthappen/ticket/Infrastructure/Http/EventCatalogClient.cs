using com.festora.nexthappen.@event.Application.Services;

namespace com.festora.nexthappen.ticket.Infrastructure.Http;

/// <summary>
/// Adaptador de catálogo de eventos para el módulo de tickets.
/// En el monolito modular delega directamente en EventService en proceso.
/// </summary>
public class EventCatalogClient
{
    private readonly EventService _eventService;
    private readonly ILogger<EventCatalogClient> _logger;

    public EventCatalogClient(EventService eventService, ILogger<EventCatalogClient> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    public record EventInfo(Guid Id, string? Title, decimal? Price, string? Organizer);

    /// <summary>Obtiene los datos del evento (precio, título, organizador).</summary>
    public async Task<EventInfo?> GetEventAsync(Guid eventId)
    {
        var ev = await _eventService.GetByIdAsync(eventId);
        if (ev is null) return null;
        return new EventInfo(ev.Id, ev.Title, ev.Price, ev.Organizer);
    }

    /// <summary>Reserva cupos con bloqueo pesimista. Devuelve false si no hay stock.</summary>
    public Task<bool> ReserveSeatsAsync(Guid eventId, int quantity)
        => _eventService.ReserveSeatsAsync(eventId, quantity);

    /// <summary>Devuelve cupos al inventario (pago expirado o reembolso).</summary>
    public async Task<bool> ReleaseSeatsAsync(Guid eventId, int quantity)
    {
        try
        {
            return await _eventService.ReleaseSeatsAsync(eventId, quantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Ticket] No se pudieron liberar {Qty} cupos del evento {EventId}", quantity, eventId);
            return false;
        }
    }
}
