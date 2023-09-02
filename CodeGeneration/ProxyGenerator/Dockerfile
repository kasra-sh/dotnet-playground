FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProxyGenerator/ProxyGenerator.csproj", "ProxyGenerator/"]
RUN dotnet restore "ProxyGenerator/ProxyGenerator.csproj"
COPY . .
WORKDIR "/src/ProxyGenerator"
RUN dotnet build "ProxyGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProxyGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProxyGenerator.dll"]
