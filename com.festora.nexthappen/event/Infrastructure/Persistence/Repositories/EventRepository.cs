using com.festora.nexthappen.@event.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace com.festora.nexthappen.@event.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventDbContext _context;

    public EventRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(com.festora.nexthappen.@event.Domain.Entities.Event entity)
    {
        _context.Set<com.festora.nexthappen.@event.Domain.Entities.Event>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<com.festora.nexthappen.@event.Domain.Entities.Event>> GetAllAsync()
        => await _context.Set<com.festora.nexthappen.@event.Domain.Entities.Event>().ToListAsync();

    public async Task<com.festora.nexthappen.@event.Domain.Entities.Event?> GetByIdAsync(Guid id)
        => await _context.Set<com.festora.nexthappen.@event.Domain.Entities.Event>().FindAsync(id);

    public async Task<IEnumerable<com.festora.nexthappen.@event.Domain.Entities.Event>> GetPublicEventsAsync()
    {
        return await _context.Events
            .Where(e => e.IsPublic == true)
            .OrderBy(e => e.DateRange.StartDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(com.festora.nexthappen.@event.Domain.Entities.Event ev)
    {
        _context.Events.Update(ev);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM Events WHERE Id = {0}", id);
    }

    public async Task<bool> ReserveSeatsAsync(Guid id, int quantity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var ev = await _context.Events
                .FromSqlRaw("SELECT * FROM Events WHERE Id = {0} FOR UPDATE", id)
                .SingleOrDefaultAsync();

            if (ev == null) return false;

            ev.ReserveSeats(quantity);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> ReleaseSeatsAsync(Guid id, int quantity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var ev = await _context.Events
                .FromSqlRaw("SELECT * FROM Events WHERE Id = {0} FOR UPDATE", id)
                .SingleOrDefaultAsync();

            if (ev == null) return false;

            ev.ReleaseSeats(quantity);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

