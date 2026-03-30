"""
Definição das Tasks (Tarefas) — o que cada agente deve fazer.
Cada task é uma unidade de trabalho que pode ser executada por um agente.
"""

from crewai import Task, Agent


def task_create_api_endpoint(agent: Agent, description: str) -> Task:
    """Tarefa: criar um endpoint de API."""
    return Task(
        description=(
            f"Crie um endpoint de API REST com FastAPI para: {description}\n\n"
            "Requisitos:\n"
            "- Use arquitetura hexagonal (ports & adapters)\n"
            "- Implemente validação com Pydantic\n"
            "- Trate erros com exceções customizadas\n"
            "- Documente com docstrings\n"
            "- Siga PEP 8 e boas práticas Python\n"
            "- Use type hints em tudo\n"
            "- Retorne respostas padronizadas (sucesso, mensagem, dados)"
        ),
        expected_output=(
            "Código Python completo do endpoint com: model, schema, "
            "repository (port + adapter), service (use case), router (controller). "
            "Todos os arquivos criados e organizados."
        ),
        agent=agent,
    )


def task_review_code(agent: Agent, file_path: str) -> Task:
    """Tarefa: revisar código existente."""
    return Task(
        description=(
            f"Faça code review completo do arquivo: {file_path}\n\n"
            "Analise:\n"
            "1. Qualidade do código (Clean Code, SOLID, DRY)\n"
            "2. Bugs ou comportamentos inesperados\n"
            "3. Problemas de segurança\n"
            "4. Performance e escalabilidade\n"
            "5. Tratamento de erros\n"
            "6. Documentação e type hints\n"
            "7. Conformidade com PEP 8\n\n"
            "Para cada problema, indique a linha, o problema e a correção sugerida."
        ),
        expected_output=(
            "Relatório de code review com: resumo geral, lista de problemas "
            "encontrados (severidade, linha, descrição, sugestão de correção), "
            "e pontos positivos do código."
        ),
        agent=agent,
    )


def task_write_tests(agent: Agent, file_path: str) -> Task:
    """Tarefa: escrever testes unitários."""
    return Task(
        description=(
            f"Escreva testes unitários completos para: {file_path}\n\n"
            "Requisitos:\n"
            "- Use pytest com fixtures\n"
            "- Padrão AAA (Arrange, Act, Assert)\n"
            "- Cubra: happy path, edge cases, erros esperados\n"
            "- Use mock/patch para dependências externas\n"
            "- Nomes descritivos (test_deve_retornar_erro_quando_cpf_invalido)\n"
            "- Mínimo 80% de cobertura do arquivo alvo"
        ),
        expected_output=(
            "Arquivo de testes completo com todos os cenários cobertos, "
            "organizado por classe/função testada, pronto para rodar com pytest."
        ),
        agent=agent,
    )


def task_troubleshoot(agent: Agent, problem_description: str) -> Task:
    """Tarefa: investigar e resolver um problema."""
    return Task(
        description=(
            f"Investigue e resolva o seguinte problema: {problem_description}\n\n"
            "Passos:\n"
            "1. Analise os logs e código relacionado\n"
            "2. Identifique a causa raiz\n"
            "3. Proponha a correção\n"
            "4. Implemente a correção\n"
            "5. Valide com testes"
        ),
        expected_output=(
            "Relatório com: causa raiz identificada, correção aplicada, "
            "testes de validação e recomendações para evitar recorrência."
        ),
        agent=agent,
    )


def task_create_automation(agent: Agent, description: str) -> Task:
    """Tarefa: criar script de automação."""
    return Task(
        description=(
            f"Crie um script de automação para: {description}\n\n"
            "Requisitos:\n"
            "- Script bem estruturado com funções\n"
            "- Tratamento de erros robusto\n"
            "- Logging adequado\n"
            "- Configuração via variáveis de ambiente\n"
            "- Documentação de uso (--help)\n"
            "- Type hints e docstrings"
        ),
        expected_output=(
            "Script Python completo, documentado, com tratamento de erros "
            "e pronto para uso em produção."
        ),
        agent=agent,
    )
