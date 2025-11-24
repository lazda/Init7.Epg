# -------------------------
# Stage 1: Build (native ARM64 on Pi)
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim-arm64v8 AS build
#
#WORKDIR /src

# Install Git + CA certificates for restore
#RUN apt-get update && \
    #apt-get install -y --no-install-recommends git ca-certificates && \
    #rm -rf /var/lib/apt/lists/*

# Clone the repository
#RUN git clone https://github.com/lazda/Init7.Epg.git .

#COPY . .

## Restore solution
#RUN dotnet restore "./Init7.Epg.csproj" --runtime linux-arm64
#
## Build & publish
#RUN dotnet publish "./Init7.Epg.csproj" \
    #-c Raspberry \
    #-f net8.0 \
    #-r linux-arm64 \
    #--no-restore \
    ##--disable-build-servers \
    #--no-self-contained \
    #-o /app/publish

# -------------------------
# Stage 2: Runtime
# -------------------------

COPY linux-arm64 /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0-bookworm-slim-arm64v8

WORKDIR /app

# Copy published binaries
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Init7.Epg.dll"]


#FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim-arm64v8 AS build
#
#ARG BUILD_CONFIGURATION=OpenWRT
#ARG TARGETARCH
#WORKDIR /src
#
#COPY . .
#RUN dotnet restore "./Init7.Epg.csproj" -r linux-arm64
#
#RUN dotnet build "./Init7.Epg.csproj" -c $BUILD_CONFIGURATION -o /app/build --arch $TARGETARCH --no-restore
#
#FROM  --platform=$BUILDPLATFORM build AS publish
#ARG BUILD_CONFIGURATION=OpenWRT
#ARG TARGETARCH
#RUN dotnet publish "./Init7.Epg.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --arch $TARGETARCH --os linux --no-self-contained
#
#FROM mcr.microsoft.com/dotnet/runtime:9.0-bookworm-slim-arm64v8 AS runtime
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Init7.Epg.dll"]