"""
Tools (Skills) do agente Python Developer.
Cada tool é uma habilidade que o agente pode usar para executar tarefas.
"""

import os
import subprocess
from crewai.tools import tool


@tool("Ler Arquivo")
def read_file(file_path: str) -> str:
    """Lê o conteúdo de um arquivo do projeto. Use para analisar código existente."""
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            content = f.read()
        total_lines = len(content.splitlines())
        return f"📄 Arquivo: {file_path} ({total_lines} linhas)\n\n{content}"
    except FileNotFoundError:
        return f"❌ Arquivo não encontrado: {file_path}"
    except Exception as e:
        return f"❌ Erro ao ler arquivo: {e}"


@tool("Escrever Arquivo")
def write_file(file_path: str, content: str) -> str:
    """Cria ou sobrescreve um arquivo com o conteúdo fornecido. Use para gerar código novo."""
    try:
        os.makedirs(os.path.dirname(file_path), exist_ok=True)
        with open(file_path, "w", encoding="utf-8") as f:
            f.write(content)
        return f"✅ Arquivo criado/atualizado: {file_path}"
    except Exception as e:
        return f"❌ Erro ao escrever arquivo: {e}"


@tool("Listar Arquivos")
def list_files(directory: str) -> str:
    """Lista todos os arquivos de um diretório. Use para entender a estrutura do projeto."""
    try:
        result = []
        for root, dirs, files in os.walk(directory):
            dirs[:] = [d for d in dirs if d not in ("__pycache__", ".git", "node_modules", ".venv", "venv")]
            level = root.replace(directory, "").count(os.sep)
            indent = "│   " * level
            result.append(f"{indent}├── {os.path.basename(root)}/")
            sub_indent = "│   " * (level + 1)
            for file in sorted(files):
                result.append(f"{sub_indent}├── {file}")
        return "\n".join(result) if result else "📁 Diretório vazio"
    except Exception as e:
        return f"❌ Erro ao listar: {e}"


@tool("Executar Comando")
def run_command(command: str) -> str:
    """Executa um comando no terminal. Use para rodar testes, instalar pacotes, git, etc."""
    try:
        result = subprocess.run(
            command,
            shell=True,
            capture_output=True,
            text=True,
            timeout=120,
            cwd=os.getcwd(),
        )
        output = ""
        if result.stdout:
            output += f"📤 STDOUT:\n{result.stdout}\n"
        if result.stderr:
            output += f"⚠️ STDERR:\n{result.stderr}\n"
        output += f"Exit code: {result.returncode}"
        return output
    except subprocess.TimeoutExpired:
        return "❌ Comando excedeu o tempo limite (120s)"
    except Exception as e:
        return f"❌ Erro ao executar: {e}"


@tool("Analisar Código Python")
def analyze_python_code(file_path: str) -> str:
    """Executa análise estática de um arquivo Python com pylint/flake8. Use para identificar problemas."""
    checks = []

    flake8 = subprocess.run(
        f"python -m flake8 {file_path} --max-line-length=120 --statistics",
        shell=True, capture_output=True, text=True,
    )
    if flake8.stdout:
        checks.append(f"🔍 Flake8:\n{flake8.stdout}")
    else:
        checks.append("✅ Flake8: Nenhum problema encontrado")

    with open(file_path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    issues = []
    for i, line in enumerate(lines, 1):
        if len(line.rstrip()) > 120:
            issues.append(f"  Linha {i}: muito longa ({len(line.rstrip())} chars)")
        if "import *" in line:
            issues.append(f"  Linha {i}: wildcard import (import *)")
        if line.strip().startswith("except:"):
            issues.append(f"  Linha {i}: except genérico (bare except)")
        if "TODO" in line or "FIXME" in line or "HACK" in line:
            issues.append(f"  Linha {i}: marcador encontrado: {line.strip()}")

    if issues:
        checks.append(f"🔍 Análise custom:\n" + "\n".join(issues))
    else:
        checks.append("✅ Análise custom: Código limpo")

    return "\n\n".join(checks)


@tool("Rodar Testes Python")
def run_tests(test_path: str = "tests/") -> str:
    """Executa testes unitários com pytest. Use para validar que o código funciona."""
    result = subprocess.run(
        f"python -m pytest {test_path} -v --tb=short",
        shell=True, capture_output=True, text=True, timeout=120,
    )
    output = ""
    if result.stdout:
        output += result.stdout
    if result.stderr:
        output += f"\n{result.stderr}"
    return output if output else "Nenhum teste encontrado"


@tool("Consultar Banco de Dados")
def query_database(connection_string: str, query: str) -> str:
    """Executa uma query SQL em um banco de dados. Use para troubleshooting e análise de dados."""
    try:
        import sqlite3
        conn = sqlite3.connect(connection_string)
        cursor = conn.cursor()
        cursor.execute(query)

        if query.strip().upper().startswith("SELECT"):
            columns = [desc[0] for desc in cursor.description]
            rows = cursor.fetchall()
            result = f"Colunas: {columns}\n"
            result += f"Total: {len(rows)} registros\n\n"
            for row in rows[:50]:
                result += f"  {row}\n"
            return result
        else:
            conn.commit()
            return f"✅ Query executada. Rows affected: {cursor.rowcount}"
    except Exception as e:
        return f"❌ Erro no banco: {e}"
    finally:
        conn.close()
