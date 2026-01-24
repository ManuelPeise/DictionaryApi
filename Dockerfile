# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy ALL project files (including missing Shared.Enums)
COPY ["sources/Shared.Models/Shared.Models.csproj", "sources/Shared.Models/"]
COPY ["sources/Shared.Enums/Shared.Enums.csproj", "sources/Shared.Enums/"]
COPY ["sources/Data.Database/Data.Database.csproj", "sources/Data.Database/"]
COPY ["sources/Data.Accessor/Data.Accessor.csproj", "sources/Data.Accessor/"]
COPY ["sources/Logic.Shared/Logic.Shared.csproj", "sources/Logic.Shared/"]
COPY ["sources/Logic.UserService/Logic.UserService.csproj", "sources/Logic.UserService/"]
COPY ["sources/Logic.Words/Logic.Words.csproj", "sources/Logic.Words/"]
COPY ["sources/Service.Api/Service.Api.csproj", "sources/Service.Api/"]
COPY ["sources/Web.Core/Web.Core.csproj", "sources/Web.Core/"]

# Restore dependencies for Web.Core (main entry point)
RUN dotnet restore "sources/Web.Core/Web.Core.csproj"

# Copy all source files
COPY sources/ sources/

# Build and Publish Web.Core project
WORKDIR "/src/sources/Web.Core"
RUN dotnet publish "Web.Core.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Web.Core.dll"]