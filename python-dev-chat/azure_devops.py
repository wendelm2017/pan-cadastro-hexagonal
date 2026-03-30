"""
Integração com Azure DevOps API.
Permite navegar repositórios, branches e arquivos direto do chat.
Usa Personal Access Token (PAT) para autenticação.
"""

import os
import base64
import httpx
from dataclasses import dataclass


@dataclass
class AzureDevOpsConfig:
    organization: str
    project: str
    pat: str

    @property
    def base_url(self) -> str:
        return f"https://dev.azure.com/{self.organization}/{self.project}/_apis"

    @property
    def auth_header(self) -> dict:
        token = base64.b64encode(f":{self.pat}".encode()).decode()
        return {
            "Authorization": f"Basic {token}",
            "Content-Type": "application/json",
        }

    @classmethod
    def from_env(cls) -> "AzureDevOpsConfig | None":
        org = os.getenv("AZURE_DEVOPS_ORG")
        project = os.getenv("AZURE_DEVOPS_PROJECT")
        pat = os.getenv("AZURE_DEVOPS_PAT")
        if not all([org, project, pat]):
            return None
        return cls(organization=org, project=project, pat=pat)


class AzureDevOpsClient:
    """Cliente para acessar repositórios, branches e arquivos no Azure DevOps."""

    API_VERSION = "7.1"

    def __init__(self, config: AzureDevOpsConfig):
        self.config = config
        self.client = httpx.Client(
            headers=config.auth_header,
            timeout=30.0,
        )

    def _get(self, url: str, params: dict | None = None) -> dict:
        params = params or {}
        params["api-version"] = self.API_VERSION
        response = self.client.get(url, params=params)
        response.raise_for_status()
        return response.json()

    def list_repositories(self) -> list[dict]:
        url = f"{self.config.base_url}/git/repositories"
        data = self._get(url)
        return [
            {"id": r["id"], "name": r["name"], "url": r.get("webUrl", "")}
            for r in data.get("value", [])
        ]

    def list_branches(self, repo_name: str) -> list[dict]:
        url = f"{self.config.base_url}/git/repositories/{repo_name}/refs"
        data = self._get(url, {"filter": "heads/"})
        return [
            {
                "name": r["name"].replace("refs/heads/", ""),
                "commit": r.get("objectId", "")[:8],
            }
            for r in data.get("value", [])
        ]

    def list_files(self, repo_name: str, branch: str = "main", path: str = "/") -> list[dict]:
        url = f"{self.config.base_url}/git/repositories/{repo_name}/items"
        data = self._get(url, {
            "scopePath": path,
            "recursionLevel": "oneLevel",
            "versionDescriptor.version": branch,
            "versionDescriptor.versionType": "branch",
        })
        items = data.get("value", [])
        return [
            {
                "path": item["path"],
                "type": "folder" if item.get("isFolder") else "file",
                "size": item.get("size", 0),
            }
            for item in items
            if item["path"] != path
        ]

    def get_file_content(self, repo_name: str, file_path: str, branch: str = "main") -> str:
        url = f"{self.config.base_url}/git/repositories/{repo_name}/items"
        params = {
            "path": file_path,
            "versionDescriptor.version": branch,
            "versionDescriptor.versionType": "branch",
            "includeContent": "true",
            "api-version": self.API_VERSION,
        }
        response = self.client.get(url, params=params)
        response.raise_for_status()

        content_type = response.headers.get("content-type", "")
        if "application/json" in content_type:
            data = response.json()
            return data.get("content", response.text)
        return response.text

    def get_file_tree(self, repo_name: str, branch: str = "main", path: str = "/", max_depth: int = 3) -> list[dict]:
        """Retorna a árvore de arquivos recursiva até max_depth níveis."""
        url = f"{self.config.base_url}/git/repositories/{repo_name}/items"
        data = self._get(url, {
            "scopePath": path,
            "recursionLevel": "full",
            "versionDescriptor.version": branch,
            "versionDescriptor.versionType": "branch",
        })
        items = data.get("value", [])

        result = []
        for item in items:
            depth = item["path"].strip("/").count("/")
            if depth <= max_depth:
                result.append({
                    "path": item["path"],
                    "type": "folder" if item.get("isFolder") else "file",
                })
        return result

    def search_code(self, repo_name: str, search_text: str, branch: str = "main") -> list[dict]:
        """Busca texto no código do repositório usando a API de search."""
        search_url = f"https://almsearch.dev.azure.com/{self.config.organization}/{self.config.project}/_apis/search/codesearchresults"
        body = {
            "searchText": search_text,
            "$top": 20,
            "filters": {
                "Repository": [repo_name],
                "Branch": [branch],
            },
        }
        response = self.client.post(
            search_url,
            json=body,
            params={"api-version": "7.1"},
        )
        if response.status_code != 200:
            return [{"error": "Search API não disponível ou sem permissão"}]

        data = response.json()
        return [
            {
                "path": r.get("path", ""),
                "filename": r.get("fileName", ""),
                "matches": [h.get("content", "") for h in r.get("matches", {}).get("content", [])][:3],
            }
            for r in data.get("results", [])
        ]
