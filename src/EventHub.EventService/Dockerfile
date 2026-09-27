FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/EventHub.EventService/EventHub.EventService.csproj", "src/EventHub.EventService/"]

RUN dotnet restore "src/EventHub.EventService/EventHub.EventService.csproj"

COPY . .

WORKDIR "/src/src/EventHub.EventService"

RUN dotnet publish "EventHub.EventService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "EventHub.EventService.dll"]