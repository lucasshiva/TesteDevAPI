Teste para Desenvolvedor Pleno

📌 Objetivo do Projeto

Este projeto consiste na criação de uma **API RESTful** em **C# com ASP.NET Core e SQLite**, além de um **aplicativo WinForms** que consome essa API. O objetivo é aplicar boas práticas de desenvolvimento para garantir eficiência, segurança e escalabilidade.

🔧 Tecnologias Utilizadas

- **ASP.NET Core** (Desenvolvimento da API)
- **Entity Framework Core** (ORM para persistência de dados)
- **SQLite** (Banco de dados leve para armazenamento)
- **JWT (JSON Web Token)** (Autenticação e segurança) **(Opcional, caso fizer será um diferencial para o teste)**
- **WinForms** (Interface gráfica para consumo da API)
- **HttpClient** (Consumo de API no cliente WinForms)
- **ILogger** (Monitoramento e logs) **(Opcional, caso fizer será um diferencial para o teste)**
- **xUnit** (Testes unitários)

📂 Estrutura do Projeto

```
📁 MinhaApiComSQLite
│── 📁 Controllers
│── 📁 Services
│── 📁 Repositories
│── 📁 DTOs
│── 📁 Models
│── 📁 Data (Contexto do banco de dados)
│── 📁 Tests (Testes unitários)
│── Program.cs
│── Startup.cs
```

🚀 Como Executar o Projeto

1️⃣ Clonando o Repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd TesteDevAPI
```

2️⃣ Configurando o Banco de Dados

- O banco de dados **SQLite** já está configurado no projeto.
- Para aplicar as migrações, execute:

```bash
dotnet ef database update
```
A API estará disponível em `http://localhost:5000`.

📌 Funcionalidades Implementadas

API

✅ CRUD de Produtos e Categorias\
✅ Autenticação via JWT\ (Opcional, caso fizer será um diferencial para o teste)
✅ Paginação de produtos\ 
✅ Registro de logs com ILogger\ (Opcional, caso fizer será um diferencial para o teste)
✅ Histórico de preços e relatórios (Opcional, caso fizer será um diferencial para o teste)

Aplicação WinForms

✅ Interface gráfica com **DataGridView**\
✅ Botões para **Criar, Atualizar e Excluir** produtos\
✅ Consumo da API com **HttpClient**\
✅ Uso de **Models** para manipulação de dados

📜 Exemplo de Requisição

Criar Produto (POST)

```json
POST /api/produtos
{
  "nome": "Produto Exemplo",
  "preco": 50.00,
  "categoriaId": 1
}
```

Neste teste, você deverá desenvolver uma API RESTful em C# com ASP.NET Core e 
SQLite, aplicando boas práticas de arquitetura e desenvolvimento para garantir 
eficiência, segurança e manutenibilidade. 

**1. Requisitos Funcionais** 
  - Implementar os métodos CRUD para a entidade Produto, com os seguintes 
  atributos: 
      - Id (auto gerado pelo banco de dados) 
      - Nome (string, deve ser descritivo e único) 
      - Preço (decimal, maior que zero) 
      - CategoriaId (relacionamento com a entidade Categoria)
        
  - Implementar os métodos CRUD para a entidade Categoria, com os seguintes 
  atributos: 
      - Id (auto gerado pelo banco de dados) 
      - o Nome (string, deve ser descritivo e único)
        
  - Implementar autenticação JWT, garantindo que apenas usuários autenticados 
  possam acessar endpoints protegidos. **(Opcional, caso fizer será um diferencial para o teste)**

  - Implementar paginação para a listagem de produtos.
    
  - Implementar logs e monitoramento utilizando ILogger<T>. **(Opcional, caso fizer será um diferencial para o teste)**
    
  - Implementar um endpoint que permita consultar o histórico de preços de um 
  produto. **(Opcional, caso fizer será um diferencial para o teste)**

  - Criar um endpoint que retorne relatórios e estatísticas, como: 
      - Total de produtos cadastrados 
      - Média de preços dos produtos 
      - Valor total dos produtos no estoque
        
  - Aplicar validações rigorosas na entrada de dados.
    
  - Criar um aplicativo WinForms que consuma a API, com as seguintes 
  funcionalidades: 
    - Interface gráfica com DataGridView para listar produtos. 
    - Botões para Criar, Atualizar e Excluir produtos com base no Grid View. 
    - Uso de HttpClient para realizar as requisições à API. 
    o Models para manipular os dados obtidos da API.

**2. Requisitos Técnicos**
    - Utilizar ASP.NET Core para desenvolver a API. 
    - Utilizar Entity Framework Core com SQLite para persistência de dados. 
    - Aplicar arquitetura em camadas separadas (Controllers, Services, Repositories, 
DTOs). 
    - Criar testes unitários para validar as funcionalidades críticas. 
    - Utilizar WinForms para criar o aplicativo cliente que consome a API. 
    
**3. Regras de Negócio Avançadas** 
    - O nome do produto deve ser armazenado sempre com a primeira letra 
    maiúscula. 
    - O preço do produto não pode ser negativo ou igual a zero. 

**4. Instruções**
   - Criar uma documentação mínima explicando como rodar o projeto e exemplos de 
    requisições. 
   - Desenvolver o aplicativo WinForms, garantindo integração com a API. 
   - Enviar um link para o repositório atualizado.
       
   - Paginação 
      A paginação permite que grandes volumes de dados sejam retornados de forma eficiente, 
      evitando sobrecarregar o banco de dados e melhorando a experiência do usuário. 
      Exemplo de implementação no ASP.NET Core: 

```csharp
public async Task<IActionResult> GetProdutos(int pageNumber = 1, int pageSize = 10) 
{ 
    var produtos = await _context.Produtos 
        .OrderBy(p => p.Nome) 
        .Skip((pageNumber - 1) * pageSize) 
        .Take(pageSize) 
        .ToListAsync(); 
        
    return Ok(produtos); 
}
```

Chamando o endpoint: GET /api/produtos?pageNumber=1&pageSize=10

**(Opcional, caso fizer será um diferencial para o teste)**
Monitoramento e Logs 

Para registrar eventos importantes, podemos utilizar ILogger<T> no ASP.NET Core: 

```csharp
public class ProdutoService 
{ 
 private readonly ILogger<ProdutoService> _logger; 
 public ProdutoService(ILogger<ProdutoService> logger) 
 { 
 _logger = logger; 
 } 
 public void AdicionarProduto(Produto produto) 
 { 
 _logger.LogInformation($"Produto {produto.Nome} adicionado em 
{DateTime.UtcNow}"); 
 } 
}
```

Os logs podem ser visualizados no console ou configurados para serem salvos em 
arquivos.

✅ Critérios de Avaliação

- Implementação correta dos requisitos funcionais e técnicos.
- Uso de boas práticas de código e arquitetura.
- Cobertura de testes unitários.
- Documentação clara e objetiva.

---

✉️ **Dúvidas? Entre em contato!**

