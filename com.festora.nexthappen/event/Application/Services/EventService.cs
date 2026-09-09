using com.festora.nexthappen.@event.Application.DTOs;
using com.festora.nexthappen.@event.Domain.Entities;
using com.festora.nexthappen.@event.Domain.Repositories;
using com.festora.nexthappen.@event.Domain.ValueObjects;

namespace com.festora.nexthappen.@event.Application.Services;

public class EventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<Event> CreateAsync(CreateEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("El título es obligatorio.");

        var dateRange = EventDateRange.Create(request.StartDate, request.EndDate);

        var newEvent = new Event(
            request.Organizer, request.Title, request.Description,
            request.Price, request.Quantity, request.Category,
            request.Address, request.Location,
            request.Photos, dateRange, request.IsPublic
        );

        await _repository.AddAsync(newEvent);
        await _repository.SaveChangesAsync();
        return newEvent;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
        => await _repository.GetAllAsync();

    public async Task<Event?> GetByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<Event>> GetPublicAsync()
        => await _repository.GetPublicEventsAsync();

    public async Task<Event?> UpdateAsync(Guid id, UpdateEventRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        var range = EventDateRange.Create(request.StartDate, request.EndDate);
        existing.UpdateDetails(
            request.Organizer, request.Title, request.Description,
            request.Price, request.Quantity, request.Category,
            request.Address, request.Location,
            request.Photos, range, request.IsPublic
        );

        await _repository.UpdateAsync(existing);
        return existing;
    }

    public async Task<bool> ReserveSeatsAsync(Guid id, int quantity)
        => await _repository.ReserveSeatsAsync(id, quantity);

    public async Task<bool> ReleaseSeatsAsync(Guid id, int quantity)
        => await _repository.ReleaseSeatsAsync(id, quantity);

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) return false;

        await _repository.DeleteByIdAsync(id);
        return true;
    }
}

