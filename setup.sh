#!/bin/bash
# ============================================================
# PanCadastro — Script de Setup Completo
# Sobe infra (Docker), roda migrations, e inicia backend+frontend
# ============================================================

set -e

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${GREEN}═══════════════════════════════════════${NC}"
echo -e "${GREEN}  PanCadastro — Setup Completo         ${NC}"
echo -e "${GREEN}═══════════════════════════════════════${NC}"

# 1. Infra
echo -e "\n${YELLOW}[1/5] Subindo SQL Server e MongoDB via Docker...${NC}"
docker-compose up -d sqlserver mongodb
echo "Aguardando SQL Server ficar healthy (pode levar ~30s)..."
until docker-compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Pan@2024Str0ng!" -Q "SELECT 1" -C -b &>/dev/null; do
  sleep 2
  echo -n "."
done
echo -e "\n${GREEN}SQL Server pronto!${NC}"

# 2. Backend restore + build
echo -e "\n${YELLOW}[2/5] Restaurando pacotes e compilando backend...${NC}"
dotnet restore
dotnet build --no-restore

# 3. Tests
echo -e "\n${YELLOW}[3/5] Rodando testes...${NC}"
dotnet test --no-build --verbosity minimal

# 4. EF Core Migrations
echo -e "\n${YELLOW}[4/5] Verificando migrations...${NC}"
dotnet tool install --global dotnet-ef 2>/dev/null || true
export PATH="$PATH:$HOME/.dotnet/tools"
cd pan-cadastro-backend/src/PanCadastro.Adapters.Driving
dotnet ef database update --project ../PanCadastro.Adapters.Driven --startup-project . 2>/dev/null || echo "Migrations serão aplicadas no startup da API"
cd ../../..

# 5. Frontend
echo -e "\n${YELLOW}[5/5] Instalando dependências do frontend...${NC}"
cd pan-cadastro-frontend
npm install
echo -e "${GREEN}Frontend pronto!${NC}"
echo -e "\n${GREEN}═══════════════════════════════════════${NC}"
echo -e "${GREEN}  Setup concluído com sucesso!          ${NC}"
echo -e "${GREEN}═══════════════════════════════════════${NC}"
echo "Para iniciar, abra dois terminais:"
echo "  (o setup já subiu SQL Server e MongoDB via Docker)"
echo ""
echo "  Terminal 1 (API):"
echo "    cd pan-cadastro-backend/src/PanCadastro.Adapters.Driving && dotnet run"
echo ""
echo "  Terminal 2 (Frontend):"
echo "    cd pan-cadastro-frontend && npx ng serve"
echo ""
echo "  API Swagger: http://localhost:5000/swagger"
echo "  Frontend:    http://localhost:4200"
echo ""