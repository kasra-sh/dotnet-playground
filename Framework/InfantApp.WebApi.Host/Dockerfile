FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Framework/InfantApp.WebApi.Host/InfantApp.WebApi.Host.csproj", "InfantApp.WebApi.Host/"]
RUN dotnet restore "Framework/InfantApp.WebApi.Host/InfantApp.WebApi.Host.csproj"
COPY . .
WORKDIR "/src/InfantApp.WebApi.Host"
RUN dotnet build "InfantApp.WebApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InfantApp.WebApi.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InfantApp.WebApi.Host.dll"]
