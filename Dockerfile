FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && \
    apt-get install -y tzdata && \
    ln -snf /usr/share/zoneinfo/Asia/Tehran /etc/localtime && \
    echo "Asia/Tehran" > /etc/timezone

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["sport-app-backend.csproj", "./"]
RUN dotnet restore "./sport-app-backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "sport-app-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "sport-app-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# تنظیم کانکشن‌استرینگ
ENV ConnectionString__DefaultConnection="server=charsetdb,3306;database=jolly_jepsen;user=root;password=clJAkjn3tjKqClnnmsn3dAi6;Connection Lifetime=300;Max Pool Size=100;Connection Timeout=30;"

ENTRYPOINT ["dotnet", "sport-app-backend.dll"]
