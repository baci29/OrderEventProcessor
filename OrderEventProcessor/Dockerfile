# Pou�ije ofici�ln� .NET runtime obraz jako z�klad
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Pou�ije ofici�ln� .NET SDK obraz pro sestaven� aplikace
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderEventProcessor/OrderEventProcessor.csproj", "OrderEventProcessor/"]
RUN dotnet restore "OrderEventProcessor/OrderEventProcessor.csproj"
COPY . .
WORKDIR "/src/OrderEventProcessor"
RUN dotnet build "OrderEventProcessor.csproj" -c Release -o /app/build

# Publikuje aplikaci
FROM build AS publish
RUN dotnet publish "OrderEventProcessor.csproj" -c Release -o /app/publish

# Pou�ije runtime obraz pro fin�ln� f�zi
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderEventProcessor.dll"]
