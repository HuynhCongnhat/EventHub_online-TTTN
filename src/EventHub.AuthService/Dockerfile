# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file
COPY ["src/EventHub.AuthService/EventHub.AuthService.csproj", "src/EventHub.AuthService/"]

# Restore
RUN dotnet restore "src/EventHub.AuthService/EventHub.AuthService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/EventHub.AuthService"
RUN dotnet build "EventHub.AuthService.csproj" -c Release -o /app/build


# =========================
# Publish stage
# =========================
FROM build AS publish

RUN dotnet publish "EventHub.AuthService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EventHub.AuthService.dll"]