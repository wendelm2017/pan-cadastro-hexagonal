FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["pan-cadastro-backend/src/PanCadastro.Domain/PanCadastro.Domain.csproj", "PanCadastro.Domain/"]
COPY ["pan-cadastro-backend/src/PanCadastro.Application/PanCadastro.Application.csproj", "PanCadastro.Application/"]
COPY ["pan-cadastro-backend/src/PanCadastro.Adapters.Driven/PanCadastro.Adapters.Driven.csproj", "PanCadastro.Adapters.Driven/"]
COPY ["pan-cadastro-backend/src/PanCadastro.Adapters.Driving/PanCadastro.Adapters.Driving.csproj", "PanCadastro.Adapters.Driving/"]
COPY ["pan-cadastro-backend/src/PanCadastro.CrossCutting/PanCadastro.CrossCutting.csproj", "PanCadastro.CrossCutting/"]
RUN dotnet restore "PanCadastro.Adapters.Driving/PanCadastro.Adapters.Driving.csproj"

COPY pan-cadastro-backend/src/ .
RUN dotnet publish "PanCadastro.Adapters.Driving/PanCadastro.Adapters.Driving.csproj" \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 5000

RUN adduser --disabled-password --gecos "" appuser
RUN mkdir -p /app/logs && chown appuser:appuser /app/logs
USER appuser

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PanCadastro.Adapters.Driving.dll"]
