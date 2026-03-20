## ==================================================================
## MULTI-STAGE DOCKERFILE - Nutra API (.NET 9)
## ==================================================================
## Build: docker build -t nutra-api .
## Run:   docker run --env-file .env -p 5000:8080 nutra-api
## ==================================================================

# ── Stage 1: Build ──────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas .csproj primeiro (layer de cache para restore)
COPY ["nutra.api", "."]
RUN dotnet restore "nutra.api"

# Copia o restante do código-fonte
COPY . .

RUN dotnet publish "nutra.api" -c Release -o /app/publish /p:UseAppHost=false

# ── Stage 2: Runtime ────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Cria usuário não-root (segurança: nunca rodar como root)
RUN groupadd -r appuser && useradd -r -g appuser -s /sbin/nologin appuser

WORKDIR /app

# Copia artefatos do build
COPY --from=build /app/publish .

# Cria diretório de keys (DataProtection) com permissão correta
# VOLUME marca o ponto para Docker inicializar com as permissões corretas
RUN mkdir -p /app/keys && chown appuser:appuser /app/keys
VOLUME /app/keys

# Ajusta permissões
RUN chown -R appuser:appuser /app

# Troca para usuário não-root
USER appuser

# Healthcheck interno do container
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080

# Definir ambiente como Production por padrão
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Nutra.dll"]
