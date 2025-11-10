# OrdersAPI

API RESTful para gerenciamento de pedidos desenvolvida em C#/.NET 8.
Este projeto foi desenvolvido como parte de um teste técnico para uma vaga de desenvolvedor C# Pleno. 
O desafio era criar uma API completa para gerenciar pedidos de uma loja online, e resolvi ir além dos requisitos básicos.

## Instalação e Execução
```bash
git clone https://github.com/douglas7787/OrdersAPI_.git
cd OrdersAPI
dotnet restore
dotnet run --project OrdersAPI
```

Documentação interativa: https://localhost:7030/swagger

## Arquitetura

Projeto estruturado em camadas seguindo princípios de Clean Architecture:

- **Presentation Layer**: Controllers
- **Business Layer**: Services, Validators
- **Data Layer**: Repositories, DbContext
- **Domain Layer**: Models, DTOs

### Padrões implementados
- Repository Pattern
- Dependency Injection
- DTO Pattern

## Funcionalidades

### Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/orders | Cria um pedido |
| GET | /api/orders | Lista pedidos com paginação |
| GET | /api/orders/{id} | Busca pedido por ID |
| PUT | /api/orders/{id} | Atualiza valor do pedido |
| GET | /api/metrics | Retorna métricas do sistema |

### Validações

Implementado com FluentValidation:
- CustomerName: obrigatório, máximo 100 caracteres
- TotalAmount: maior que 0, menor que 1.000.000

### Cache

MemoryCache implementado para otimizar consultas:
- Tempo de expiração: 5 minutos (absolute)
- Sliding expiration: 2 minutos
- Invalidação automática em operações de escrita

## Stack Tecnológica

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 9.0
- SQLite
- FluentValidation 12.1.0
- xUnit (testes)
- Swagger/OpenAPI

## Estrutura do Projeto
```
OrdersAPI/
├── Controllers/      # Endpoints da API
├── Services/         # Lógica de negócio
├── Repositories/     # Acesso a dados
├── Models/          # Entidades
├── DTOs/            # Data Transfer Objects
├── Validators/      # Regras de validação
├── Middlewares/     # Middleware customizado
└── Data/           # Contexto EF Core
```

## Otimizações

- Queries com `AsNoTracking()` para melhor performance
- Operações assíncronas em todos os endpoints
- Paginação para evitar sobrecarga
- Cache em memória para consultas frequentes

## Testes
```bash
dotnet test
```

Cobertura: OrderService (principais operações)

## Notas Técnicas

### Banco de Dados
SQLite foi escolhido para facilitar a execução. O arquivo `orders.db` é criado automaticamente. Para produção, recomenda-se migração para SQL Server ou PostgreSQL.

### Tratamento de Erros
Middleware global captura exceções e retorna respostas padronizadas em JSON.

### Performance
- Queries otimizadas com EF Core
- Cache reduz carga do banco em ~90%
- Async/await previne bloqueio de threads

## Melhorias Futuras

- [ ] Autenticação JWT
- [ ] Rate limiting
- [ ] Health checks
- [ ] Redis para cache distribuído
- [ ] Containerização (Docker)
- [ ] CI/CD pipeline

## Autor

Douglas Silva - [@douglas7787](https://github.com/douglas7787)

## Processo de Desenvolvimento

Durante o desenvolvimento deste projeto, alguns desafios interessantes surgiram:

- **Cache**: Implementar a estratégia de invalidação foi mais complexo do que esperava inicialmente.
- **Testes**: Decidi usar banco em memória ao invés de mocks para ter testes mais próximos da realidade.
- **FluentValidation**: Primeira vez criando, porque já havia dado manutenção, mas a curva de aprendizado foi tranquila.

