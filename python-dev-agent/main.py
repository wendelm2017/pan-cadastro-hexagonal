"""
Python Dev Agent — Time de agentes IA para desenvolvimento Python.

Uso:
    python main.py

Antes de rodar:
    1. Copie .env.example para .env
    2. Preencha sua OPENAI_API_KEY
    3. pip install -r requirements.txt
"""

from dotenv import load_dotenv
from crewai import Crew, Process

from agents import create_python_developer, create_code_reviewer, create_test_engineer
from tasks import (
    task_create_api_endpoint,
    task_review_code,
    task_write_tests,
    task_troubleshoot,
    task_create_automation,
)

load_dotenv()


def run_full_development_cycle():
    """
    Exemplo 1: Ciclo completo de desenvolvimento.
    O Dev cria o código → Reviewer revisa → Tester escreve testes.
    Os agentes trabalham em sequência, um passando o resultado para o outro.
    """
    dev = create_python_developer()
    reviewer = create_code_reviewer()
    tester = create_test_engineer()

    dev_task = task_create_api_endpoint(
        agent=dev,
        description="CRUD de Clientes com campos: nome, email, CPF, telefone. "
                    "Validar CPF, email obrigatório, nome mínimo 3 caracteres.",
    )

    review_task = task_review_code(
        agent=reviewer,
        file_path="output/routers/cliente_router.py",
    )
    review_task.context = [dev_task]

    test_task = task_write_tests(
        agent=tester,
        file_path="output/services/cliente_service.py",
    )
    test_task.context = [dev_task]

    crew = Crew(
        agents=[dev, reviewer, tester],
        tasks=[dev_task, review_task, test_task],
        process=Process.sequential,
        verbose=True,
    )

    result = crew.kickoff()
    print("\n" + "=" * 60)
    print("RESULTADO DO CICLO COMPLETO")
    print("=" * 60)
    print(result)


def run_code_review_only():
    """
    Exemplo 2: Só code review de um arquivo existente.
    """
    reviewer = create_code_reviewer()

    review_task = task_review_code(
        agent=reviewer,
        file_path="caminho/para/seu/arquivo.py",  # ← troque pelo seu arquivo
    )

    crew = Crew(
        agents=[reviewer],
        tasks=[review_task],
        process=Process.sequential,
        verbose=True,
    )

    result = crew.kickoff()
    print("\n" + "=" * 60)
    print("RESULTADO DO CODE REVIEW")
    print("=" * 60)
    print(result)


def run_troubleshooting():
    """
    Exemplo 3: Investigar e resolver um problema.
    """
    dev = create_python_developer()

    debug_task = task_troubleshoot(
        agent=dev,
        problem_description=(
            "A API está retornando 500 Internal Server Error no endpoint "
            "POST /api/clientes quando o campo email está vazio. "
            "Deveria retornar 422 com mensagem de validação."
        ),
    )

    crew = Crew(
        agents=[dev],
        tasks=[debug_task],
        process=Process.sequential,
        verbose=True,
    )

    result = crew.kickoff()
    print("\n" + "=" * 60)
    print("RESULTADO DO TROUBLESHOOTING")
    print("=" * 60)
    print(result)


def run_automation():
    """
    Exemplo 4: Criar um script de automação.
    """
    dev = create_python_developer()

    auto_task = task_create_automation(
        agent=dev,
        description=(
            "Script que monitora um bucket S3, quando um arquivo CSV novo "
            "chega, lê o arquivo, valida os dados (CPF, email, campos "
            "obrigatórios) e insere os registros válidos no banco PostgreSQL. "
            "Registros inválidos vão para uma tabela de erros com o motivo."
        ),
    )

    crew = Crew(
        agents=[dev],
        tasks=[auto_task],
        process=Process.sequential,
        verbose=True,
    )

    result = crew.kickoff()
    print("\n" + "=" * 60)
    print("RESULTADO DA AUTOMAÇÃO")
    print("=" * 60)
    print(result)


if __name__ == "__main__":
    print("🐍 Python Dev Agent — Selecione o cenário:\n")
    print("1. Ciclo completo (Dev → Review → Testes)")
    print("2. Code Review de arquivo")
    print("3. Troubleshooting de problema")
    print("4. Criar automação/script")

    choice = input("\nEscolha (1-4): ").strip()

    scenarios = {
        "1": run_full_development_cycle,
        "2": run_code_review_only,
        "3": run_troubleshooting,
        "4": run_automation,
    }

    if choice in scenarios:
        scenarios[choice]()
    else:
        print("❌ Opção inválida. Use 1, 2, 3 ou 4.")
