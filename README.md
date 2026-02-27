# PanCadastro

API + SPA para cadastro de Pessoa Física, Pessoa Jurídica e Endereço.

Backend em .NET 8 com arquitetura hexagonal, frontend em Angular 18 com PrimeNG.

## Por que hexagonal?

Optei por hexagonal (Ports & Adapters) porque facilita trocar infraestrutura sem mexer em regra de negócio. O Domain não referencia EF Core, MongoDB, nada externo — só FluentValidation. Se amanhã precisar trocar SQL Server por Postgres, é só criar um novo adapter.

Na prática, o projeto ficou assim:

- **PanCadastro.Domain** — Entidades, Value Objects (CPF, CNPJ, CEP), exceções, interfaces de porta
- **PanCadastro.Application** — Services com os casos de uso, DTOs, AutoMapper
- **PanCadastro.Adapters.Driven** — Repositórios EF Core, client ViaCEP, MongoDB pra cache de CEP
- **PanCadastro.Adapters.Driving** — Controllers REST, middleware de erro, Program.cs
- **PanCadastro.CrossCutting** — Composition root, registra tudo no DI

A regra de dependência é simples: Driving → Application → Domain ← Driven. O CrossCutting conecta as pontas.

## Stack

**Backend:** .NET 8, EF Core 8, MongoDB Driver, AutoMapper 13, FluentValidation 11, Serilog, Swagger

**Frontend:** Angular 18 (standalone components), PrimeNG 17.18, PrimeFlex, RxJS, TypeScript 5.4

**Infra:** Docker Compose (SQL Server 2022 + MongoDB 7), migrations automáticas, seed de dados em dev

## Pré-requisitos

- .NET 8 SDK
- Node.js 18+
- Docker e Docker Compose

## Como rodar

A forma mais rápida:

```bash
chmod +x setup.sh
./setup.sh
```

O script sobe os bancos, espera ficarem prontos, roda restore/build/test, aplica migrations e instala o frontend. Depois é só iniciar:

```bash
# terminal 1 - backend
cd pan-cadastro-backend/src/PanCadastro.Adapters.Driving
dotnet run

# terminal 2 - frontend
cd pan-cadastro-frontend
npx ng serve
```

API: http://localhost:5000/swagger
Frontend: http://localhost:4200

Se preferir subir tudo via Docker (backend + bancos):

```bash
docker-compose up -d
```

Aí só o frontend fica local.

## Testes

108 testes unitários cobrindo Domain e Application:

```bash
dotnet test
```

- **76 testes de Domain** — Value Objects (validação de dígitos CPF/CNPJ, formatação, igualdade por valor), entidades (factory method, soft delete, vínculo de endereços), validações
- **32 testes de Application** — Services com Moq (CRUD completo, happy path + edge cases), unicidade CPF/CNPJ, mock ViaCEP, config do AutoMapper

## Endpoints principais

**Pessoa Física** — `GET/POST/PUT/DELETE /api/pessoasfisicas`

**Pessoa Jurídica** — `GET/POST/PUT/DELETE /api/pessoasjuridicas`

**Endereço** — `GET/POST/PUT/DELETE /api/enderecos` + `GET /api/enderecos/cep/{cep}` (consulta ViaCEP)

**Health** — `GET /health`

Os endpoints de endereço também filtram por pessoa: `/api/enderecos/pessoa-fisica/{id}` e `/api/enderecos/pessoa-juridica/{id}`.

## Validações

Implementei validação pesada no Domain pra não depender de annotation do framework:

- **CPF** — Algoritmo completo de dígitos verificadores, rejeita sequências repetidas (111.111.111-11)
- **CNPJ** — Multiplicadores FEBRABAN, 14 dígitos
- **CEP** — 8 dígitos numéricos
- **Email** — Validação básica de formato
- **Datas** — Nascimento não pode ser futura nem > 150 anos, abertura PJ não pode ser futura

## Tratamento de erros

Middleware global que converte exceções do domain em respostas HTTP padronizadas. DomainException vira 400, NotFoundException vira 404, o resto 500 (logado via Serilog). Formato:

```json
{
  "sucesso": false,
  "mensagem": "CPF 529.982.247-25 já está cadastrado.",
  "dados": null
}
```

## Frontend

SPA com Angular 18 usando standalone components e lazy loading por rota. Principais funcionalidades:

- CRUD completo de PF e PJ com tabela paginada
- Endereços vinculados em dialog (nested CRUD)
- Consulta automática ViaCEP — digita o CEP e preenche rua, bairro, cidade, UF
- Máscaras de input pra CPF, CNPJ e CEP
- Interceptor global de erros com toast (PrimeNG)
- Tela de auditoria com leitura de logs do Serilog (filtro por severidade e busca)

## Estrutura do frontend

```
pan-cadastro-frontend/src/app/
├── core/
│   ├── interceptors/     # error.interceptor.ts
│   ├── models/           # interfaces e constantes
│   └── services/         # HTTP clients (PF, PJ, Endereço, Auditoria)
├── features/
│   ├── auditoria/        # visualização de logs
│   ├── home/             # dashboard
│   ├── pessoa-fisica/    # CRUD PF
│   └── pessoa-juridica/  # CRUD PJ
└── shared/               # componentes compartilhados
```

## Seed de dados

Em ambiente Development, a API executa seed automático com 2 Pessoas Físicas e 1 Pessoa Jurídica com endereços vinculados. Facilita pra testar sem ter que cadastrar tudo na mão.

## O que eu faria com mais tempo

- Testes de integração com TestContainers (SQL Server + MongoDB em container de teste)
- Cache distribuído com Redis no lugar do MongoDB pra ViaCEP
- Paginação server-side nas listagens
- CI/CD com GitHub Actions
- Autenticação JWT