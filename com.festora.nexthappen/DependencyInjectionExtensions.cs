using Microsoft.EntityFrameworkCore;
using com.festora.nexthappen.iam.Infrastructure.Persistence;
using com.festora.nexthappen.iam.Domain.Repositories;
using com.festora.nexthappen.iam.Infrastructure.Persistence.Repositories;
using com.festora.nexthappen.iam.Application.UseCases;

using com.festora.nexthappen.@event.Infrastructure.Persistence;
using com.festora.nexthappen.@event.Domain.Repositories;
using com.festora.nexthappen.@event.Infrastructure.Persistence.Repositories;
using com.festora.nexthappen.@event.Application.Services;

using com.festora.nexthappen.engagement.Infrastructure.Persistence;
using com.festora.nexthappen.engagement.Domain.Repositories;
using com.festora.nexthappen.engagement.Infrastructure.Persistence.Repositories;
using com.festora.nexthappen.engagement.Application.Services;

using com.festora.nexthappen.ticket.Infrastructure.Persistence;
using com.festora.nexthappen.ticket.Domain.Repositories;
using com.festora.nexthappen.ticket.Infrastructure.Persistence.Repositories;
using com.festora.nexthappen.ticket.Application.Services;

namespace com.festora.nexthappen;

public static class 
    DependencyInjectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        // --- IAM Module ---
        services.AddDbContext<IamDbContext>(opt => opt.UseMySql(connectionString, serverVersion));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<RegisterUser>();
        services.AddScoped<LoginUser>();
        services.AddScoped<com.festora.nexthappen.iam.Infrastructure.Security.JwtTokenGenerator>();

        // --- Event Module ---
        services.AddDbContext<EventDbContext>(opt => opt.UseMySql(connectionString, serverVersion));
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IAssignedStandRepository, AssignedStandRepository>();
        services.AddScoped<EventService>();
        services.AddScoped<StandService>();

        // --- Engagement Module ---
        services.AddDbContext<EngagementDbContext>(opt => opt.UseMySql(connectionString, serverVersion));
        services.AddScoped<ISavedEventRepository, SavedEventRepository>();
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<SavedEventService>();
        services.AddScoped<MetricService>();
        services.AddScoped<ReviewService>();

        // --- Ticket Module ---
        services.AddDbContext<TicketDbContext>(opt => opt.UseMySql(connectionString, serverVersion));
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<TicketService>();
        services.AddScoped<SalesService>();
        services.AddScoped<PaymentService>();

        return services;
    }
}
