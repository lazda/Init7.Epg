# -------------------------
# Stage 1: Build (native ARM64 on Pi)
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim-arm64v8 AS build

WORKDIR /src

# Install Git + CA certificates for restore
RUN apt-get update && \
    apt-get install -y --no-install-recommends git ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# Clone the repository
RUN git clone https://github.com/lazda/Init7.Epg.git .

# Restore solution
RUN dotnet restore "Init7.Epg.sln" --runtime linux-arm64

# Build & publish
RUN dotnet publish "Init7.Epg/Init7.Epg.csproj" \
    -c OpenWRT \
    -f net9.0 \
    -r linux-arm64 \
    --self-contained false \
    -o /app/publish

# -------------------------
# Stage 2: Runtime
# -------------------------
FROM mcr.microsoft.com/dotnet/runtime:9.0-bookworm-slim-arm64v8

WORKDIR /app

# Copy published binaries
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Init7.Epg.dll"]
