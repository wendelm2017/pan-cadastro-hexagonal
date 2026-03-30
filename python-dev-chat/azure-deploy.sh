#!/bin/bash
# ============================================================
# Deploy do Python Dev Chat no Azure App Service (Tier Grátis)
# Pré-requisito: Azure CLI instalado e logado (az login)
# ============================================================

set -e

# ===== CONFIGURAÇÕES (altere se quiser) =====
RESOURCE_GROUP="rg-python-dev-chat"
LOCATION="eastus"
PLAN_NAME="plan-python-dev-chat"
APP_NAME="python-dev-chat-$(openssl rand -hex 3)"
RUNTIME="PYTHON:3.12"
# =============================================

echo ""
echo "============================================"
echo "  Deploy - Python Dev Chat no Azure"
echo "============================================"
echo ""
echo "App Name: $APP_NAME"
echo "URL final: https://$APP_NAME.azurewebsites.net"
echo ""

# 1. Criar Resource Group
echo "[1/6] Criando Resource Group..."
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --output none

# 2. Criar App Service Plan (GRÁTIS)
echo "[2/6] Criando App Service Plan (F1 - Grátis)..."
az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RESOURCE_GROUP \
    --sku F1 \
    --is-linux \
    --output none

# 3. Criar Web App
echo "[3/6] Criando Web App..."
az webapp create \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --plan $PLAN_NAME \
    --runtime $RUNTIME \
    --output none

# 4. Configurar variáveis de ambiente
echo "[4/6] Configurando variáveis de ambiente..."
echo ""
read -sp "Cole sua OPENAI_API_KEY: " API_KEY
echo ""

az webapp config appsettings set \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings \
        OPENAI_API_KEY="$API_KEY" \
        SCM_DO_BUILD_DURING_DEPLOYMENT=true \
        PORT=8000 \
    --output none

# 5. Configurar startup command
echo "[5/6] Configurando startup command..."
az webapp config set \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --startup-file "startup.sh" \
    --output none

# 6. Deploy do código
echo "[6/6] Fazendo deploy do código (pode levar 2-3 min)..."
az webapp up \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --runtime $RUNTIME

echo ""
echo "============================================"
echo "  Deploy concluído!"
echo "============================================"
echo ""
echo "  URL: https://$APP_NAME.azurewebsites.net"
echo ""
echo "  IMPORTANTE: O tier grátis (F1) dorme após"
echo "  20min sem acesso. O primeiro request depois"
echo "  de dormir demora ~15-20s para acordar."
echo ""
echo "  Para ver logs:"
echo "    az webapp log tail --name $APP_NAME --resource-group $RESOURCE_GROUP"
echo ""
echo "  Para deletar tudo (parar de gastar):"
echo "    az group delete --name $RESOURCE_GROUP --yes"
echo ""
