#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN echo "This is my base image" > /tmp/image_type

FROM base As mydebug
RUN echo "This is my debug image" > /tmp/image_type

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EDA.Producer/EDA.Producer.csproj", "EDA.Producer/"]
RUN dotnet restore "./EDA.Producer/EDA.Producer.csproj"
COPY . .
WORKDIR "/src/EDA.Producer"
RUN dotnet build "./EDA.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EDA.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EDA.Producer.dll"]