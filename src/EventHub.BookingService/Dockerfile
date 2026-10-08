FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/EventHub.BookingService/EventHub.BookingService.csproj", "src/EventHub.BookingService/"]
RUN dotnet restore "src/EventHub.BookingService/EventHub.BookingService.csproj"

COPY . .
WORKDIR "/src/src/EventHub.BookingService"

RUN dotnet build "EventHub.BookingService.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/build .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EventHub.BookingService.dll"]