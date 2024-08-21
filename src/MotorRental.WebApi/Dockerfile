FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MotorRental.WebApi/MotorRental.WebApi.csproj", "src/MotorRental.WebApi/"]
COPY ["src/MotorRental.Application/MotorRental.Application.csproj", "src/MotorRental.Application/"]
COPY ["src/MotorRental.Domain/MotorRental.Domain.csproj", "src/MotorRental.Domain/"]
COPY ["src/MotorRental.Infrastructure/MotorRental.Infrastructure.csproj", "src/MotorRental.Infrastructure/"]
RUN dotnet restore "./src/MotorRental.WebApi/MotorRental.WebApi.csproj"
COPY . .
WORKDIR "/src/src/MotorRental.WebApi"
RUN dotnet build "MotorRental.WebApi.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS migrations
RUN dotnet tool install --version 8.0.7 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet-ef database update --project "/src/src/MotorRental.Infrastructure/" --startup-project "/src/src/MotorRental.WebApi"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MotorRental.WebApi.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MotorRental.WebApi.dll"]