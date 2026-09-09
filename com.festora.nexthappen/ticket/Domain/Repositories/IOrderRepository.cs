using com.festora.nexthappen.ticket.Domain.Entities;

namespace com.festora.nexthappen.ticket.Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order?> GetBySessionIdAsync(string stripeSessionId);
    Task UpdateAsync(Order order);
}
