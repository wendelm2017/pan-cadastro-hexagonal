"""
Python Dev Chat — Assistente de IA personalizado para desenvolvimento Python.
Backend FastAPI com streaming de respostas via OpenAI API.
Integração com Azure DevOps para navegar código dos repositórios.
"""

import os
from dotenv import load_dotenv
from fastapi import FastAPI, Request, HTTPException
from fastapi.responses import HTMLResponse, StreamingResponse
from fastapi.staticfiles import StaticFiles
from openai import OpenAI
from azure_devops import AzureDevOpsClient, AzureDevOpsConfig

load_dotenv()

app = FastAPI(title="Python Dev Chat")
app.mount("/static", StaticFiles(directory="static"), name="static")

client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))
MODEL = os.getenv("OPENAI_MODEL", "gpt-4o")

devops_config = AzureDevOpsConfig.from_env()
devops_client = AzureDevOpsClient(devops_config) if devops_config else None

SYSTEM_PROMPT = """Você é um Desenvolvedor Python Sênior altamente qualificado, atuando como assistente de desenvolvimento. Responda sempre em português brasileiro.

## Suas Responsabilidades
- Desenvolver, manter e evoluir aplicações e APIs em Python
- Criar automações, scripts e integrações entre sistemas
- Garantir código limpo, organizado e bem documentado
- Implementar testes unitários e participar de code reviews
- Atuar na análise de problemas, troubleshooting e melhorias de performance
- Colaborar na definição de soluções técnicas escaláveis

## Seus Conhecimentos Técnicos
- Python (domínio avançado)
- Frameworks: FastAPI, Flask, Django
- APIs REST (design, implementação, documentação)
- Bancos de dados relacionais (PostgreSQL, SQL Server, MySQL) e NoSQL (MongoDB, Redis, DynamoDB)
- Git e versionamento de código
- Testes unitários (pytest, unittest, mock)
- CI/CD (GitHub Actions, Azure DevOps Pipelines, Jenkins)
- Cloud: AWS (S3, Lambda, ECS, Glue, RDS), Azure (App Service, Functions, Azure SQL), GCP
- Docker e Kubernetes
- Mensageria: Kafka, RabbitMQ, SQS
- Dados: Pandas, PySpark, SQLAlchemy
- Arquitetura: hexagonal, microsserviços, clean architecture, SOLID, DRY, KISS

## Seu Perfil de Comportamento
- Forte capacidade analítica — analisa problemas a fundo antes de propor soluções
- Proativo e autônomo — sugere melhorias mesmo quando não solicitado
- Boa comunicação — explica conceitos complexos de forma clara e didática
- Comprometido com qualidade — nunca entrega código sem tratamento de erros e sem considerar edge cases
- Sempre sugere testes quando entrega código
- Usa type hints em todo código Python
- Segue PEP 8 e boas práticas da comunidade Python

## Regras ao Responder
1. Sempre responda em português brasileiro
2. Quando escrever código, use type hints, docstrings e tratamento de erros
3. Sugira testes unitários quando entregar código
4. Se identificar problemas no código do usuário, aponte de forma construtiva
5. Use markdown para formatar as respostas (código em blocos, listas, tabelas quando apropriado)
6. Seja direto e prático, sem enrolação
7. Quando relevante, mencione alternativas e trade-offs
8. Se não souber algo, diga que não sabe — nunca invente
9. Quando o usuário referenciar um arquivo do Azure DevOps (com @), analise o conteúdo completo do arquivo que foi injetado no contexto"""

conversations: dict[str, list[dict]] = {}


@app.get("/", response_class=HTMLResponse)
async def index():
    with open("static/index.html", "r", encoding="utf-8") as f:
        return HTMLResponse(content=f.read())


@app.get("/api/devops/status")
async def devops_status():
    """Verifica se a integração com Azure DevOps está configurada."""
    if not devops_client:
        return {"connected": False, "message": "Azure DevOps não configurado. Preencha AZURE_DEVOPS_ORG, AZURE_DEVOPS_PROJECT e AZURE_DEVOPS_PAT no .env"}
    try:
        repos = devops_client.list_repositories()
        return {"connected": True, "organization": devops_config.organization, "project": devops_config.project, "repositories": len(repos)}
    except Exception as e:
        return {"connected": False, "message": f"Erro ao conectar: {str(e)}"}


@app.get("/api/devops/repos")
async def list_repos():
    """Lista repositórios do projeto Azure DevOps."""
    if not devops_client:
        raise HTTPException(400, "Azure DevOps não configurado")
    try:
        return devops_client.list_repositories()
    except Exception as e:
        raise HTTPException(500, str(e))


@app.get("/api/devops/repos/{repo_name}/branches")
async def list_branches(repo_name: str):
    """Lista branches de um repositório."""
    if not devops_client:
        raise HTTPException(400, "Azure DevOps não configurado")
    try:
        return devops_client.list_branches(repo_name)
    except Exception as e:
        raise HTTPException(500, str(e))


@app.get("/api/devops/repos/{repo_name}/files")
async def list_files(repo_name: str, branch: str = "main", path: str = "/"):
    """Lista arquivos de um diretório no repositório."""
    if not devops_client:
        raise HTTPException(400, "Azure DevOps não configurado")
    try:
        return devops_client.list_files(repo_name, branch, path)
    except Exception as e:
        raise HTTPException(500, str(e))


@app.get("/api/devops/repos/{repo_name}/content")
async def get_file_content(repo_name: str, path: str, branch: str = "main"):
    """Retorna o conteúdo de um arquivo do repositório."""
    if not devops_client:
        raise HTTPException(400, "Azure DevOps não configurado")
    try:
        content = devops_client.get_file_content(repo_name, path, branch)
        return {"path": path, "branch": branch, "content": content}
    except Exception as e:
        raise HTTPException(500, str(e))


@app.get("/api/devops/repos/{repo_name}/tree")
async def get_tree(repo_name: str, branch: str = "main", path: str = "/"):
    """Retorna a árvore de arquivos do repositório."""
    if not devops_client:
        raise HTTPException(400, "Azure DevOps não configurado")
    try:
        return devops_client.get_file_tree(repo_name, branch, path)
    except Exception as e:
        raise HTTPException(500, str(e))


@app.post("/api/chat")
async def chat(request: Request):
    body = await request.json()
    message = body.get("message", "")
    session_id = body.get("session_id", "default")
    attached_files = body.get("attached_files", [])

    if session_id not in conversations:
        conversations[session_id] = []

    user_content = message
    if attached_files:
        file_context = "\n\n--- ARQUIVOS REFERENCIADOS DO AZURE DEVOPS ---\n"
        for f in attached_files:
            file_context += f"\n### 📄 {f['path']} (branch: {f.get('branch', 'main')})\n```\n{f['content']}\n```\n"
        file_context += "--- FIM DOS ARQUIVOS ---\n"
        user_content = file_context + "\n" + message

    conversations[session_id].append({"role": "user", "content": user_content})

    history = conversations[session_id][-50:]
    messages = [{"role": "system", "content": SYSTEM_PROMPT}] + history

    async def generate():
        assistant_message = ""
        stream = client.chat.completions.create(
            model=MODEL,
            messages=messages,
            stream=True,
            temperature=0.7,
            max_tokens=4096,
        )
        for chunk in stream:
            if chunk.choices[0].delta.content:
                content = chunk.choices[0].delta.content
                assistant_message += content
                yield content

        conversations[session_id].append(
            {"role": "assistant", "content": assistant_message}
        )

    return StreamingResponse(generate(), media_type="text/plain")


@app.post("/api/clear")
async def clear(request: Request):
    body = await request.json()
    session_id = body.get("session_id", "default")
    conversations.pop(session_id, None)
    return {"status": "ok"}


if __name__ == "__main__":
    import uvicorn

    port = int(os.getenv("PORT", 8000))
    uvicorn.run(app, host="0.0.0.0", port=port)
