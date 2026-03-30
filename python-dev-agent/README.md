# Python Dev Agent

Time de agentes IA para desenvolvimento Python usando CrewAI.

## Agentes disponíveis

| Agente | Papel | Skills (Tools) |
|--------|-------|----------------|
| **Desenvolvedor Python** | Cria código, APIs, automações | Ler/escrever arquivos, terminal, análise, testes, banco |
| **Code Reviewer** | Revisa código, identifica problemas | Ler arquivos, análise estática |
| **Engenheiro de Testes** | Escreve testes unitários | Ler/escrever arquivos, rodar testes |

## Cenários prontos

1. **Ciclo completo** — Dev cria → Reviewer revisa → Tester testa
2. **Code Review** — Analisa um arquivo existente
3. **Troubleshooting** — Investiga e resolve um problema
4. **Automação** — Cria scripts de automação

## Setup

```bash
# 1. Instalar dependências
pip install -r requirements.txt

# 2. Configurar chave de API
cp .env.example .env
# Edite .env e preencha OPENAI_API_KEY

# 3. Executar
python main.py
```

## Estrutura

```
python-dev-agent/
├── main.py          # Ponto de entrada — menu com cenários
├── agents.py        # Definição dos agentes (perfil, objetivo, ferramentas)
├── tasks.py         # Tarefas que os agentes executam
├── tools/
│   └── code_tools.py  # Skills: ler, escrever, analisar, testar, query DB
├── requirements.txt
├── .env.example
└── README.md
```

## Como funciona

```
Você define a tarefa (ex: "Criar CRUD de Clientes")
    ↓
CrewAI distribui para os agentes
    ↓
Agente usa as tools (skills) para executar
    ↓
Resultado é retornado
```

## Customização

Para criar novos agentes, edite `agents.py`. Para novas tarefas, edite `tasks.py`.
Para novas skills, adicione funções com `@tool` em `tools/code_tools.py`.
