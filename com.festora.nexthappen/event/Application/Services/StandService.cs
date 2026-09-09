using com.festora.nexthappen.@event.Domain.Entities;
using com.festora.nexthappen.@event.Domain.Repositories;

namespace com.festora.nexthappen.@event.Application.Services;

public class StandService
{
    private readonly IAssignedStandRepository _repo;

    public StandService(IAssignedStandRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AssignedStand>> GetByEventAsync(Guid eventId)
        => await _repo.GetByEventIdAsync(eventId);

    public async Task<AssignedStand> AssignAsync(Guid eventId, string name, string category)
    {
        var stand = new AssignedStand(eventId, name, category);
        await _repo.AddAsync(stand);
        return stand;
    }

    public async Task<AssignedStand?> UpdateAsync(Guid id, string name, string category)
    {
        var stand = await _repo.GetByIdAsync(id);
        if (stand == null) return null;

        stand.Update(name, category);
        await _repo.UpdateAsync(stand);
        return stand;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var stand = await _repo.GetByIdAsync(id);
        if (stand == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
