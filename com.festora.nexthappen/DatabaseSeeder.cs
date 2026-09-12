using com.festora.nexthappen.@event.Domain.Entities;
using com.festora.nexthappen.@event.Domain.ValueObjects;
using com.festora.nexthappen.@event.Infrastructure.Persistence;
using com.festora.nexthappen.iam.Domain.Entities;
using com.festora.nexthappen.iam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace com.festora.nexthappen;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var iamDb = scope.ServiceProvider.GetRequiredService<IamDbContext>();
        var eventDb = scope.ServiceProvider.GetRequiredService<EventDbContext>();

        // 1. Seed Demo Users
        var organizerId = Guid.Parse("f402db94-8448-43da-ab30-ebfa9e606900");
        var userId = Guid.Parse("bc1543c2-c24d-4e06-9b6e-fb5865acf1e6");

        if (!await iamDb.Users.AnyAsync(u => u.Email == "organizador@nexthappen.demo"))
        {
            iamDb.Users.Add(new User
            {
                Id = organizerId,
                FullName = "Ferias Lima (Demo)",
                Email = "organizador@nexthappen.demo",
                PasswordHash = "$2a$11$iKN/kbOJSNv/cw0V.gV0S.sur.Fh0eiMZWdfOwM28aIDFoVKBX/66", // Demo1234!
                Role = "Organizer",
                TermsAcceptedAt = DateTime.UtcNow,
                TermsVersion = "v1.0-2026"
            });
        }

        if (!await iamDb.Users.AnyAsync(u => u.Email == "usuario@nexthappen.demo"))
        {
            iamDb.Users.Add(new User
            {
                Id = userId,
                FullName = "Dario Salcedo (Demo)",
                Email = "usuario@nexthappen.demo",
                PasswordHash = "$2a$11$K6NMszSae6TUiZrja5MKb.M5ccEbJaDCL2328hcckOBESFhNoFkMq", // Demo1234!
                Role = "User",
                TermsAcceptedAt = DateTime.UtcNow,
                TermsVersion = "v1.0-2026"
            });
        }

        await iamDb.SaveChangesAsync();

        // 2. Seed Demo Events
        if (!await eventDb.Events.AnyAsync())
        {
            var events = new List<Event>
            {
                new Event(
                    organizer: organizerId.ToString(),
                    title: "Feria Gastronómica de Barranco",
                    description: "Más de 40 emprendimientos de comida peruana, food trucks, música en vivo y talleres de cocina. Un fin de semana para disfrutar los sabores de Lima.",
                    price: 25.00m,
                    quantity: 196,
                    category: "Gastronomía",
                    address: "Parque Municipal de Barranco",
                    location: "-12.1467,-77.0206|Parque Municipal de Barranco, Lima",
                    photos: new[]
                    {
                        "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=900&q=80&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=900&q=80&auto=format&fit=crop"
                    },
                    dateRange: EventDateRange.Create(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4)),
                    isPublic: true
                ),
                new Event(
                    organizer: organizerId.ToString(),
                    title: "NightMarket Tech & Makers",
                    description: "Comunidad maker, robótica, impresión 3D y videojuegos indie. Demostraciones, networking y charlas de innovación.",
                    price: 20.00m,
                    quantity: 250,
                    category: "Tecnología",
                    address: "Surco",
                    location: "-12.1550,-76.9917|Av. Caminos del Inca, Surco, Lima",
                    photos: new[]
                    {
                        "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=900&q=80&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1531482615713-2afd69097998?w=900&q=80&auto=format&fit=crop"
                    },
                    dateRange: EventDateRange.Create(DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(6)),
                    isPublic: true
                ),
                new Event(
                    organizer: organizerId.ToString(),
                    title: "Festival Indie: Sonidos de la Ciudad",
                    description: "Bandas emergentes de rock, indie y fusión andina en un escenario íntimo. Descubre el nuevo sonido limeño antes que nadie.",
                    price: 45.00m,
                    quantity: 300,
                    category: "Música y Conciertos",
                    address: "Anfiteatro del Parque de la Exposición",
                    location: "-12.0664,-77.0378|Parque de la Exposición, Cercado de Lima",
                    photos: new[]
                    {
                        "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=900&q=80&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1501281668745-f7f57925c3b4?w=900&q=80&auto=format&fit=crop"
                    },
                    dateRange: EventDateRange.Create(DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(8)),
                    isPublic: true
                ),
                new Event(
                    organizer: organizerId.ToString(),
                    title: "Expo Café & Barismo",
                    description: "Cafés de especialidad de todo el Perú, catas guiadas y competencia de baristas. Aprende a preparar el café perfecto.",
                    price: 30.00m,
                    quantity: 120,
                    category: "Gastronomía",
                    address: "San Isidro",
                    location: "-12.0972,-77.0365|Calle Los Libertadores, San Isidro, Lima",
                    photos: new[]
                    {
                        "https://images.unsplash.com/photo-1442512595331-e89e73853f31?w=900&q=80&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1447933601403-0c6688de566e?w=900&q=80&auto=format&fit=crop"
                    },
                    dateRange: EventDateRange.Create(DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(11)),
                    isPublic: true
                ),
                new Event(
                    organizer: organizerId.ToString(),
                    title: "Noche de Danza y Folklore",
                    description: "Espectáculo de danzas tradicionales peruanas con agrupaciones invitadas. Una celebración de nuestra cultura viva.",
                    price: 35.00m,
                    quantity: 200,
                    category: "Cultural",
                    address: "Callao Monumental",
                    location: "-12.0566,-77.1181|Callao Monumental, Callao",
                    photos: new[]
                    {
                        "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?w=900&q=80&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=900&q=80&auto=format&fit=crop"
                    },
                    dateRange: EventDateRange.Create(DateTime.UtcNow.AddDays(12), DateTime.UtcNow.AddDays(13)),
                    isPublic: true
                ),
                new Event(
                    organizer: organizerId.ToString(),
                    title: "Mercado de Arte y Diseño Independiente",
                    description: "Ilustradores, ceramistas y diseñadores exhiben y venden sus piezas. Charlas de portafolio y arte en vivo durante toda la jornada.",
                    price: 15.00m,
                    quantity: 150,
                    category: "Arte y Diseño",
                    address: "Casa Cultural, Miraflores",
                    location: "-12.1211,-77.0297|Av. Larco, Miraflores, Lima",
                    photos: new[]
                    {
                        "https://images.unsplash.com/photo-1536924940846-227afb31e2a5?w=900&q=80&auto=format&fit=crop",
                        "https://images.unsplash.com/photo-1513151233558-d860c5398176?w=900&q=80&auto=format&fit=crop"
                    },
                    dateRange: EventDateRange.Create(DateTime.UtcNow.AddDays(14), DateTime.UtcNow.AddDays(15)),
                    isPublic: true
                )
            };

            eventDb.Events.AddRange(events);
            await eventDb.SaveChangesAsync();
        }
    }
}
