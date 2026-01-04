FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MoneyKeeper.csproj", "."]
RUN dotnet restore "./MoneyKeeper.csproj"

COPY . .

RUN dotnet publish "MoneyKeeper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MoneyKeeper.dll"]