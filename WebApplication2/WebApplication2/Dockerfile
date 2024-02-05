#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApplication2/WebApplication2.csproj", "WebApplication2/"]
RUN dotnet restore "./WebApplication2/./WebApplication2.csproj"
COPY . .
WORKDIR "/src/WebApplication2"
RUN dotnet build "./WebApplication2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApplication2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# To modify this image to grab a NuGet package instead of building from source
# Change the COPY line to copy from wherever the NuGet package is stored instead of making it from source code
# This might still be a multistage build, but the first stage would be to grab the NuGet package
# and the second stage would be to copy the NuGet package into the final image

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication2.dll"]