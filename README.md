# Sistema de Gestão de Franquias — Web API em C# / ASP.NET Core

Trabalho Acadêmico da disciplina de **Desenvolvimento Web Back-end**.

---

## 👤 Identificação do Aluno

- **Aluno:** JOÃO PAULO ANDRADE MATHIAS
- **RU:** 5180040
- **Professor:** Rodrigo da S. do Nascimento
- **Repositório GitHub:** [https://github.com/Joaopaulojpam/Trabalho-de-C-](https://github.com/Joaopaulojpam/Trabalho-de-CSharp)

---

## 📋 Descrição do Projeto

O **Sistema de Gestão de Franquias** é uma API REST desenvolvida em **C#** e **ASP.NET Core** para centralizar as operações de uma rede de franquias e suas respectivas unidades franqueadas. A solução resolve o problema de descentralização e falta de padronização na gestão de franqueados, permitindo o controle completo de:

- **Autenticação e Perfis de Acesso** (Administrador, Gestor da Unidade e Operador de Caixa);
- **Cadastro de Unidades Franqueadas e Franqueados** (com validação estrita de CNPJ único);
- **Catálogo Padronizado de Produtos e Serviços** (organizado por categorias e fornecedores homologados);
- **Controle de Estoque por Unidade** (com validação de saldo não negativo e alertas de estoque crítico);
- **Registro de Vendas e Itens** (com cálculo automático do valor total e baixa automática no estoque);
- **Cálculo Automatizado de Royalties** (apurado sobre o faturamento de vendas no mês e percentual contratual da unidade);
- **Central de Chamados e Suporte** (com prioridades, categorias e acompanhamento de status);
- **Módulo de Relatórios e Indicadores Gerenciais** (ranking de unidades, faturamento, produtos mais vendidos e visão geral da rede).

---

## 🛠️ Tecnologias e Recursos Utilizados

- **Linguagem:** C# 12 / .NET 8
- **Framework Web:** ASP.NET Core Web API
- **ORM / Acesso a Dados:** Entity Framework Core 8
- **Banco de Dados:** SQLite (banco relacional portátil com inicialização e *Seed* automático de dados)
- **Segurança e Criptografia:** JWT (JSON Web Tokens) e BCrypt para hashing seguro de senhas
- **Documentação Interativa:** Swagger / OpenAPI com suporte nativo a autenticação Bearer
- **Padrões de Arquitetura:** Clean/Layered Architecture, Dependency Injection, DTOs, Asynchronous Programming (`async`/`await`), Middleware Global de Tratamento de Erros

---

## 🏛️ Arquitetura do Projeto

O código está organizado seguindo a separação de responsabilidades recomendada no escopo acadêmico:

```
TRABALHO DE C#/
├── Franquias.sln                                  # Arquivo de Solução Visual Studio
├── Franquias.Api/                                 # Projeto Principal ASP.NET Core API
│   ├── Controllers/                               # Controladores REST com anotações Swagger
│   │   ├── AuthController.cs                      # Login e consulta de usuário autenticado
│   │   ├── CategoriasController.cs                # CRUD de categorias de produtos
│   │   ├── ChamadosController.cs                  # Abertura, filtros e resolução de chamados
│   │   ├── EstoquesController.cs                  # Consulta de saldo, movimentações e estoque crítico
│   │   ├── FornecedoresController.cs              # CRUD e busca de fornecedores homologados
│   │   ├── ProdutosController.cs                  # Catálogo de produtos e serviços
│   │   ├── RelatoriosController.cs                # Faturamento, ranking e indicadores gerenciais
│   │   ├── RoyaltiesController.cs                 # Apuração e liquidação de royalties
│   │   ├── UnidadesController.cs                  # CRUD de franquias e ativação/inativação
│   │   └── UsuariosController.cs                  # Gestão de usuários e permissões por perfil
│   ├── Models/                                    # Entidades de Domínio do Banco de Dados
│   │   ├── Enums/                                 # Enums (Perfis, Status, Prioridades, Tipos)
│   │   ├── Usuario.cs, Franqueadora.cs, UnidadeFranqueada.cs
│   │   ├── Categoria.cs, ProdutoServico.cs, Fornecedor.cs
│   │   ├── Estoque.cs, MovimentacaoEstoque.cs
│   │   ├── Venda.cs, ItemVenda.cs, Royalty.cs, ChamadoSuporte.cs
│   │   └── ...
│   ├── DTOs/                                      # Data Transfer Objects (Requests e Responses)
│   ├── Services/                                  # Camada de Negócio e Regras do Sistema
│   │   ├── Interfaces/                            # Contratos dos serviços
│   │   └── Implementations/                       # Lógica de cálculo, validações e persistência
│   ├── Data/                                      # Persistência e Banco de Dados
│   │   ├── AppDbContext.cs                        # Contexto do EF Core e mapeamentos Fluent API
│   │   └── DbInitializer.cs                       # Seed com dados de exemplo realistas
│   ├── Middleware/                                # Middleware global para tratamento de exceções
│   ├── Configurations/                            # Configurações de JWT, Swagger e Injeção de Dependências
│   ├── appsettings.json                           # Parâmetros de conexão e JWT
│   └── Program.cs                                 # Ponto de entrada da aplicação
├── requests.http                                  # Coleção de testes para VS Code / Visual Studio
├── postman_collection.json                        # Coleção completa pronta para importação no Postman
├── .gitignore                                     # Arquivos ignorados pelo Git
└── README.md                                      # Documentação completa de uso
```

---

## 🚀 Como Executar o Projeto Passo a Passo

### 1. Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior instalado.
- Visual Studio 2022, Visual Studio Code ou JetBrains Rider.

### 2. Clonar o Repositório
```bash
git clone https://github.com/Joaopaulojpam/Trabalho-de-C-.git
cd Trabalho-de-C-
```

### 3. Restaurar Pacotes e Compilar a Solução
```bash
dotnet restore
dotnet build
```

### 4. Executar a Web API
```bash
cd Franquias.Api
dotnet run
```

Ao iniciar, a API criará e alimentará automaticamente o banco de dados SQLite (`franquias.db`) com dados completos de teste!

### 5. Acessar a Documentação Interativa do Swagger
Abra o navegador no endereço:
👉 **[http://localhost:5000](http://localhost:5000)** ou **[https://localhost:5001](https://localhost:5001)**

*(A interface do Swagger abre diretamente na raiz da aplicação).*

---

## 🔐 Usuários e Credenciais Pré-Cadastrados para Testes

O banco já possui usuários criados com senhas criptografadas com **BCrypt**:

| Perfil | Nome | E-mail | Senha | Unidade Vinculada | Permissões |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Administrador** | Administrador Geral | `admin@franquias.com` | `Admin@123` | Matriz (Acesso Total) | Acesso irrestrito a todos os módulos, usuários, relatórios e parametrizações |
| **Gestor** | Carlos Eduardo Santos | `gestor.sp@franquias.com` | `Gestor@123` | Unidade SP - Jardins | Gerencia estoque, abre chamados, consulta royalties e relatórios de sua unidade |
| **Operador** | Lucas Oliveira | `operador.sp@franquias.com` | `Operador@123` | Unidade SP - Jardins | Realiza vendas, consultas de produtos e movimentações operacionais de caixa |
| **Gestor** | Juliana Ferreira | `gestor.rj@franquias.com` | `Gestor@123` | Unidade RJ - Copacabana | Gerencia unidade do Rio de Janeiro |

---

## 🔑 Como se Autenticar no Swagger

1. No Swagger, localize o endpoint `POST /api/Auth/login`.
2. Clique em **Try it out** e insira as credenciais do Administrador:
   ```json
   {
     "email": "admin@franquias.com",
     "senha": "Admin@123"
   }
   ```
3. Clique em **Execute** e copie o valor da propriedade `"token"` retornado na resposta.
4. Role até o topo da página do Swagger e clique no botão verde **Authorize** (com ícone de cadeado).
5. No campo *Value*, digite: `Bearer SEU_TOKEN_AQUI` (substituindo pelo token copiado).
6. Clique em **Authorize** e feche a janela. Todos os endpoints agora estão autenticados e liberados para teste!

---

## 📌 Principais Endpoints da API

### 🔐 1. Autenticação & Usuários
- `POST /api/Auth/login` — Autentica o usuário e retorna o Token JWT.
- `GET /api/Auth/me` — Retorna dados do usuário atualmente conectado.
- `GET /api/Usuarios` — Lista todos os usuários cadastrados (*Admin*).
- `POST /api/Usuarios` — Cadastra novo usuário (*Admin*).
- `PUT /api/Usuarios/{id}` — Atualiza dados e perfil do usuário (*Admin*).
- `PATCH /api/Usuarios/{id}/status` — Ativa ou inativa o usuário (*Admin*).

### 🏢 2. Unidades Franqueadas
- `GET /api/Unidades` — Lista unidades (com filtros por `ativo`, `cidade`, `uf` e `termoBusca`).
- `GET /api/Unidades/{id}` — Obtém detalhes da unidade.
- `POST /api/Unidades` — Cadastra nova unidade franqueada (*Admin*).
- `PUT /api/Unidades/{id}` — Atualiza informações da unidade (*Admin/Gestor*).
- `PATCH /api/Unidades/{id}/status` — Ativa/Inativa a unidade (*Admin*).

### ☕ 3. Catálogo, Categorias e Fornecedores
- `GET /api/Produtos` — Lista catálogo com filtros por categoria, status, tipo e busca textual.
- `POST /api/Produtos` — Cadastra novo produto/serviço (*Admin*).
- `GET /api/Categorias` — Lista categorias de produtos.
- `POST /api/Categorias` — Cadastra nova categoria (*Admin*).
- `GET /api/Fornecedores` — Lista fornecedores homologados.
- `POST /api/Fornecedores` — Cadastra fornecedor (*Admin*).

### 📦 4. Controle de Estoque
- `GET /api/Estoques/unidade/{unidadeId}` — Consulta saldo de estoque da unidade.
- `GET /api/Estoques/unidade/{unidadeId}?apenasCriticos=true` — Lista apenas itens com **estoque crítico** (abaixo do mínimo).
- `POST /api/Estoques/movimentar` — Realiza entrada, saída ou ajuste com validação de saldo não negativo.
- `GET /api/Estoques/movimentacoes` — Histórico de auditoria de movimentações.

### 🛒 5. Vendas
- `POST /api/Vendas` — Registra nova venda com itens, cálculo automático do total e baixa imediata no estoque.
- `GET /api/Vendas` — Consulta histórico de vendas por período e unidade.
- `GET /api/Vendas/{id}` — Detalhes completos da venda com itens e subtotais.
- `POST /api/Vendas/{id}/cancelar` — Cancela venda e estorna estoque.

### 💰 6. Royalties e Financeiro
- `POST /api/Royalties/gerar` — Calcula e apura o royalty da unidade com base no faturamento do mês e percentual configurado.
- `GET /api/Royalties` — Consulta lançamentos de royalties por período, unidade e status.
- `PATCH /api/Royalties/{id}/pagar` — Registra o pagamento e liquidação do royalty.
- `GET /api/Royalties/resumo` — Relatório financeiro consolidado de repasses da rede.

### 🎫 7. Chamados e Suporte
- `POST /api/Chamados` — Abertura de chamado pela unidade franqueada.
- `GET /api/Chamados` — Listagem com filtros por status e prioridade (*Baixa, Media, Alta, Critica*).
- `PATCH /api/Chamados/{id}/status` — Atualiza o status e registra a solução técnica do chamado.
- `GET /api/Chamados/contagem-status` — Quantitativo de chamados por situação.

### 📊 8. Relatórios e Indicadores Gerenciais
- `GET /api/Relatorios/faturamento-unidades` — Faturamento e royalties por unidade no período.
- `GET /api/Relatorios/ranking-unidades` — Ranking das franquias que mais faturam na rede.
- `GET /api/Relatorios/produtos-mais-vendidos` — Top produtos e serviços com maior saída.
- `GET /api/Relatorios/estoque-critico` — Visão geral da rede com todos os produtos com estoque em nível de alerta.
- `GET /api/Relatorios/indicadores-gerais` — Dashboard consolidado da franqueadora.

---

## ⚖️ Regras de Negócio Obrigatórias Implementadas

1. **Unicidade de CNPJ:** Não é permitido o cadastro de duas unidades franqueadas com o mesmo CNPJ.
2. **Unicidade de E-mail:** Bloqueio de duplicidade de e-mails de usuários.
3. **Bloqueio de Vendas em Unidade Inativa:** Uma unidade com status inativo é terminantemente impedida de registrar vendas.
4. **Validação de Venda:** A venda deve pertencer a uma unidade válida e conter pelo menos um item.
5. **Cálculo Automático de Preço:** O total da venda é calculado a partir das quantidades e preços unitários dos itens.
6. **Controle Estrito de Estoque:** Bloqueio de vendas ou saídas caso o saldo do produto seja insuficiente (não permite saldo negativo).
7. **Baixa Automática no Estoque:** A confirmação da venda gera baixa automática no saldo da unidade e auditoria de movimentação.
8. **Cálculo Automatizado de Royalties:** O royalty é apurado multiplicando o faturamento total de vendas no mês pelo percentual contratual da franquia.
9. **Controle de Acesso Baseado em Perfis:** Endpoints protegidos com `[Authorize(Roles = "...")]` garantindo que cada usuário acesse apenas operações permitidas.
10. **Preservação de Histórico (Soft Delete):** Inativação de registros críticos através de flag `Ativo` mantendo integridade referencial.

---

## 🧪 Testes Automatizados via HTTP / Postman

- **Arquivo `requests.http`:** Pode ser executado diretamente no Visual Studio ou VS Code (com a extensão REST Client). O arquivo já contém o fluxo completo de autenticação e chamadas parametrizadas.
- **Arquivo `postman_collection.json`:** Pode ser importado diretamente no Postman. Ao executar o endpoint de login, o token JWT é salvo automaticamente nas variáveis da coleção!

---

## 📤 Comandos Git para Envio ao Repositório

Para enviar o código para o seu repositório GitHub:

```bash
# 1. Inicializar o repositório git local
git init

# 2. Adicionar o repositório remoto
git remote add origin https://github.com/Joaopaulojpam/Trabalho-de-C-.git

# 3. Adicionar todos os arquivos e realizar o commit
git add .
git commit -m "Entrega Trabalho Academico C# - Gestao de Franquias - Joao Paulo Mathias RU 5180040"

# 4. Enviar para o branch principal
git branch -M main
git push -u origin main
```

---

*Desenvolvido por JOÃO PAULO ANDRADE MATHIAS (RU: 5180040) — 2026.*
