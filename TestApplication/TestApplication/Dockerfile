# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./TestApplication/TestApplication.csproj", "./"]
RUN dotnet restore "TestApplication.csproj"
COPY ./TestApplication .
RUN dotnet publish "./TestApplication.csproj" -c Release -o /app/publish

# Test Stage
FROM build AS test
ENTRYPOINT [ "dotnet", "test", "--logger:trx", "--filter" ]
CMD [""]