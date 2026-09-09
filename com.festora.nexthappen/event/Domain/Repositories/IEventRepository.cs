namespace com.festora.nexthappen.@event.Domain.Repositories;

public interface IEventRepository
{
    Task AddAsync(com.festora.nexthappen.@event.Domain.Entities.Event entity);
    Task<IEnumerable<com.festora.nexthappen.@event.Domain.Entities.Event>> GetAllAsync();
    Task<com.festora.nexthappen.@event.Domain.Entities.Event?> GetByIdAsync(Guid id);
    Task<IEnumerable<com.festora.nexthappen.@event.Domain.Entities.Event>> GetPublicEventsAsync();
    Task UpdateAsync(com.festora.nexthappen.@event.Domain.Entities.Event ev);
    Task DeleteByIdAsync(Guid id);
    Task<bool> ReserveSeatsAsync(Guid id, int quantity);
    Task<bool> ReleaseSeatsAsync(Guid id, int quantity);
    Task SaveChangesAsync();
}

