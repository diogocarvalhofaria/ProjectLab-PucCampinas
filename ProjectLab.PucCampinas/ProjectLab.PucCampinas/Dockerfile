# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o resto e compila
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Estágio de Execução (Imagem Final Leve)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Expondo as portas padrão
EXPOSE 8000
EXPOSE 8081

ENTRYPOINT ["dotnet", "ProjectLab.PucCampinas.dll"]