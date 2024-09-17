FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API.Autenticacao/API.Autenticacao.csproj", "API.Autenticacao/"]
RUN dotnet restore "API.Autenticacao/API.Autenticacao.csproj"
COPY . .
WORKDIR "/src/API.Autenticacao"
RUN dotnet build "API.Autenticacao.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.Autenticacao.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.Autenticacao.dll"]