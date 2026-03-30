# Python Dev Chat

Chat web com assistente IA personalizado para desenvolvimento Python. Funciona como um "clone" customizado do ChatGPT/Claude, focado em desenvolvimento Python.

## Stack

- **Backend:** FastAPI + OpenAI API (streaming)
- **Frontend:** HTML/CSS/JS (sem framework, leve e rápido)
- **Deploy:** Docker ou direto com Python

## Features

- Chat com streaming de respostas (resposta aparece em tempo real)
- Syntax highlighting para blocos de código
- Botão de copiar código
- Markdown renderizado (tabelas, listas, negrito, etc.)
- Responsivo (funciona no celular)
- Histórico de conversa por sessão
- Sugestões de perguntas na tela inicial
- Tema escuro

## Rodar local

```bash
# 1. Instalar dependências
pip install -r requirements.txt

# 2. Configurar chave
cp .env.example .env
# Edite .env e preencha OPENAI_API_KEY

# 3. Rodar
python app.py

# 4. Acessar
# http://localhost:8000
```

## Rodar com Docker

```bash
docker build -t python-dev-chat .
docker run -p 8000:8000 -e OPENAI_API_KEY=sk-sua-chave python-dev-chat
```

## Deploy no Azure App Service (grátis)

```bash
# 1. Login no Azure
az login

# 2. Criar resource group
az group create --name rg-python-dev-chat --location eastus

# 3. Criar App Service Plan (grátis)
az appservice plan create --name plan-python-dev-chat --resource-group rg-python-dev-chat --sku F1 --is-linux

# 4. Criar Web App
az webapp create --name python-dev-chat --resource-group rg-python-dev-chat --plan plan-python-dev-chat --runtime "PYTHON:3.12"

# 5. Configurar a chave
az webapp config appsettings set --name python-dev-chat --resource-group rg-python-dev-chat --settings OPENAI_API_KEY=sk-sua-chave

# 6. Deploy
az webapp up --name python-dev-chat --resource-group rg-python-dev-chat

# 7. Acessar
# https://python-dev-chat.azurewebsites.net
```

## Deploy no Railway (alternativa rápida)

1. Faça fork deste repo
2. Acesse railway.app e conecte o repo
3. Adicione a variável `OPENAI_API_KEY`
4. Deploy automático
