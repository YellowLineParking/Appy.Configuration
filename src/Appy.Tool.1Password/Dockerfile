FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

ARG configuration=Release

WORKDIR /
COPY ["src/Appy.Tool.1Password/Appy.Tool.1Password.csproj", "src/Appy.Tool.1Password/"]
COPY ["src/Appy.Configuration/Appy.Configuration.csproj", "src/Appy.Configuration/"]
COPY ["src/Appy.Configuration.1Password/Appy.Configuration.1Password.csproj", "src/Appy.Configuration.1Password/"]
COPY ["src/Appy.Infrastructure.1Password/Appy.Infrastructure.1Password.csproj", "src/Appy.Infrastructure.1Password/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Build.targets", "src/"]
RUN dotnet restore "src/Appy.Tool.1Password/Appy.Tool.1Password.csproj"
COPY . .
WORKDIR "/src/Appy.Tool.1Password"
RUN dotnet build "Appy.Tool.1Password.csproj" -c $configuration -o /app/build

FROM build AS publish

ARG configuration=Release

RUN dotnet publish "Appy.Tool.1Password.csproj" -c $configuration -o /app/publish

FROM base AS final
# Install 1Password CLI (op)
ENV OP_VERSION 1.8.0
RUN buildDeps="curl gnupg dirmngr unzip" \
    && runDeps="ca-certificates" \
    && apt-get update \
    && apt-get install -y --no-install-recommends $runDeps $buildDeps \
    && cd /root \
    && curl -sSL -o op.zip https://cache.agilebits.com/dist/1P/op/pkg/v$OP_VERSION/op_linux_amd64_v$OP_VERSION.zip \
    && unzip op.zip \
    && gpg --keyserver hkp://keyserver.ubuntu.com --receive-keys 3FEF9748469ADBE15DA7CA80AC2D62742012EA22 \
    && gpg --verify op.sig op \
    && mv op /usr/local/bin \
    && rm -Rf * .gnupg \
    && apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false -o APT::AutoRemove::SuggestsImportant=false $buildDeps \
    && rm -r /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Appy.Tool.1Password.dll"]

# docker build . -f src/Appy.Tool.1Password/Dockerfile -t appy-op:0.6.1
# docker run --rm -it -e "HOME=$HOME" -v "$HOME:$HOME" -p 6000:6000 --name appytest appy-op:0.6.0 -s -vt Development -env DEV -a -api 6000

