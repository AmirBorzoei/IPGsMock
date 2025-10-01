FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["IPGsMock/IPGsMock.csproj", "IPGsMock/"]
RUN dotnet restore "IPGsMock/IPGsMock.csproj"
COPY . .
COPY "../README.md" "../."
WORKDIR "/src/IPGsMock"
RUN dotnet build "IPGsMock.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "IPGsMock.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IPGsMock.dll"]
