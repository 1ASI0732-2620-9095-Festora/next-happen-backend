namespace com.festora.nexthappen.ticket.Domain.Repositories;

public interface ITicketRepository
{
    Task AddAsync(com.festora.nexthappen.ticket.Domain.Entities.Ticket ticket);
    Task AddRangeAsync(IEnumerable<com.festora.nexthappen.ticket.Domain.Entities.Ticket> tickets);
    Task<List<com.festora.nexthappen.ticket.Domain.Entities.Ticket>> GetByUserIdAsync(Guid userId);
    Task<com.festora.nexthappen.ticket.Domain.Entities.Ticket?> GetByIdAsync(Guid id);
    Task<com.festora.nexthappen.ticket.Domain.Entities.Ticket?> GetByQrCodeAsync(string qrCode);
    Task<com.festora.nexthappen.ticket.Domain.Entities.Ticket?> GetByShortCodeAsync(string shortCode);
    Task<bool> ShortCodeExistsAsync(string shortCode);
    Task<List<com.festora.nexthappen.ticket.Domain.Entities.Ticket>> GetByEventIdAsync(Guid eventId);
    Task<List<com.festora.nexthappen.ticket.Domain.Entities.Ticket>> GetByEventIdsAsync(IEnumerable<Guid> eventIds);
    Task UpdateAsync(com.festora.nexthappen.ticket.Domain.Entities.Ticket ticket);
    Task<bool> CancelAsync(Guid ticketId);
}
