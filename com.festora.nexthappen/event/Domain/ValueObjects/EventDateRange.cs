namespace com.festora.nexthappen.@event.Domain.ValueObjects;

public record EventDateRange(DateTime StartDate, DateTime EndDate)
{
    public static EventDateRange Create(DateTime start, DateTime end)
    {
        if (start.Year < 1000 || end.Year < 1000)
            throw new ArgumentException("Las fechas deben ser válidas (no pueden ser la fecha por defecto).");
            
        if (end < start)
            throw new ArgumentException("La fecha final no puede ser menor que la inicial.");
            
        return new EventDateRange(start, end);
    }
}
