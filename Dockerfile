# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["com.festora.nexthappen/com.festora.nexthappen.csproj", "com.festora.nexthappen/"]
RUN dotnet restore "com.festora.nexthappen/com.festora.nexthappen.csproj"

# Copy source and publish release build
COPY . .
WORKDIR "/src/com.festora.nexthappen"
RUN dotnet publish "com.festora.nexthappen.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage (Alpine for ultra-lightweight image)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache icu-libs tzdata curl
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "com.festora.nexthappen.dll"]
