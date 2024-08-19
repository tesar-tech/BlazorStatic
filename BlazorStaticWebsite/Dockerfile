FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorStaticWebsite/BlazorStaticWebsite.csproj", "BlazorStaticWebsite/"]
COPY ["src/BlazorStatic.csproj", "src/"]
RUN dotnet restore "BlazorStaticWebsite/BlazorStaticWebsite.csproj"
COPY . .
WORKDIR "/src/BlazorStaticWebsite"
RUN dotnet build "BlazorStaticWebsite.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BlazorStaticWebsite.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorStaticWebsite.dll"]
