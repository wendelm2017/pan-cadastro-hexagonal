"""
Python Dev Chat — Assistente de IA personalizado para desenvolvimento Python.
Backend FastAPI com streaming de respostas via OpenAI API.
"""

import os
from dotenv import load_dotenv
from fastapi import FastAPI, Request
from fastapi.responses import HTMLResponse, StreamingResponse
from fastapi.staticfiles import StaticFiles
from openai import OpenAI

load_dotenv()

app = FastAPI(title="Python Dev Chat")
app.mount("/static", StaticFiles(directory="static"), name="static")

client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))
MODEL = os.getenv("OPENAI_MODEL", "gpt-4o")

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
8. Se não souber algo, diga que não sabe — nunca invente"""

conversations: dict[str, list[dict]] = {}


@app.get("/", response_class=HTMLResponse)
async def index():
    with open("static/index.html", "r", encoding="utf-8") as f:
        return HTMLResponse(content=f.read())


@app.post("/api/chat")
async def chat(request: Request):
    body = await request.json()
    message = body.get("message", "")
    session_id = body.get("session_id", "default")

    if session_id not in conversations:
        conversations[session_id] = []

    conversations[session_id].append({"role": "user", "content": message})

    # Mantém últimas 50 mensagens para não estourar o contexto
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
