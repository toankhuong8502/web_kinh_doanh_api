FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebKinhDoanh.sln ./
COPY src/WebKinhDoanh.Domain/WebKinhDoanh.Domain.csproj src/WebKinhDoanh.Domain/
COPY src/WebKinhDoanh.Application/WebKinhDoanh.Application.csproj src/WebKinhDoanh.Application/
COPY src/WebKinhDoanh.Infrastructure/WebKinhDoanh.Infrastructure.csproj src/WebKinhDoanh.Infrastructure/
COPY src/WebKinhDoanh.Api/WebKinhDoanh.Api.csproj src/WebKinhDoanh.Api/

RUN dotnet restore WebKinhDoanh.sln

COPY src/ src/
RUN dotnet publish src/WebKinhDoanh.Api/WebKinhDoanh.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends ca-certificates && rm -rf /var/lib/apt/lists/*
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebKinhDoanh.Api.dll"]
