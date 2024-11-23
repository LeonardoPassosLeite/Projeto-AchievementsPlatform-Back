# Imagem base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Imagem base para o build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto e restaurar as dependências
COPY ["AchievementsPlatform.csproj", "./"]
RUN dotnet restore "./AchievementsPlatform.csproj"

# Copiar o restante do código e construir o projeto
COPY . .
WORKDIR "/src"
RUN dotnet build "AchievementsPlatform.csproj" -c Release -o /app/build

# Publicação
FROM build AS publish
RUN dotnet publish "AchievementsPlatform.csproj" -c Release -o /app/publish

# Configuração final para o runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AchievementsPlatform.dll"]