# Use the official .NET 10 preview 3 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview.3 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY bamboo-card-task.sln ./
COPY BamboCardTask.API/*.csproj ./BamboCardTask.API/
COPY BambooCardTask.JwtGenerator/*.csproj ./BambooCardTask.JwtGenerator/
COPY BambooCardTask.Test/*.csproj ./BambooCardTask.Test/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/BamboCardTask.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview.3 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the default port (adjust if needed)
EXPOSE 5117

# Set environment variables if needed
# ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BambooCardTask.Api.dll"]
