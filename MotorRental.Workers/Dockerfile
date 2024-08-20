FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MotorRental.Workers/MotorRental.Workers.csproj", "src/MotorRental.Workers/"]
COPY ["src/MotorRental.Application/MotorRental.Application.csproj", "src/MotorRental.Application/"]
COPY ["src/MotorRental.Domain/MotorRental.Domain.csproj", "src/MotorRental.Domain/"]
COPY ["src/MotorRental.Infrastructure/MotorRental.Infrastructure.csproj", "src/MotorRental.Infrastructure/"]
RUN dotnet restore "./src/MotorRental.Workers/MotorRental.Workers.csproj"
COPY . .
WORKDIR "/src/src/MotorRental.Workers"
RUN dotnet build "./MotorRental.Workers.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MotorRental.Workers.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MotorRental.Workers.dll"]