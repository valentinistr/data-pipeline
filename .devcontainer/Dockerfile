# Pin the base image digest so Codespaces/prebuilds get a reproducible layer cache.
# To update: docker pull mcr.microsoft.com/devcontainers/dotnet:2-10.0-noble
#            docker inspect mcr.microsoft.com/devcontainers/dotnet:2-10.0-noble --format '{{index .RepoDigests 0}}'
# Tag at time of pin: 2-10.0-noble
FROM mcr.microsoft.com/devcontainers/dotnet@sha256:0ad11caaa728e35cbc46b0592dd11ca0df8111e565a96471024a795f4d412c7c

ARG NODE_MAJOR=22

RUN curl -fsSL "https://deb.nodesource.com/setup_${NODE_MAJOR}.x" | bash - \
    && apt-get install -y nodejs \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*

RUN node --version && npm --version
