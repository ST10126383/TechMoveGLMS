FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TechMoveGLMS/TechMoveGLMS.csproj", "TechMoveGLMS/"]
RUN dotnet restore "TechMoveGLMS/TechMoveGLMS.csproj"
COPY . .
WORKDIR "/src/TechMoveGLMS"
RUN dotnet build "TechMoveGLMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechMoveGLMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.dll"]