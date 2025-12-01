# Go Code Standards

## Estrutura do Projeto

- Estrutura baseada em Clean Architecture
- `/cmd` - Ponto de entrada da aplicação
- `/internal` - Código privado da aplicação
  - `/handler` - Handlers HTTP (controllers)
  - `/service` - Lógica de negócios
  - `/repository` - Acesso a dados
  - `/middleware` - Middlewares HTTP
  - `/domain` - Modelos e interfaces de domínio
- `/pkg` - Código que pode ser reutilizado por outros projetos

## Convenções de Código

- Seguir [Effective Go](https://golang.org/doc/effective_go)
- Usar [Go Code Review Comments](https://github.com/golang/go/wiki/CodeReviewComments)
- Pacotes em snake_case, funções e variáveis em camelCase
- Nomes de interfaces devem terminar com 'er' (ex: `UserService`)
- Evitar inicialização de variáveis com o valor zero

## Testes

- Todos os pacotes devem ter testes
- Usar tabela de testes para testes unitários
- Usar `testify` para asserções
- Manter cobertura de testes acima de 70%

## Logging

- Usar um pacote de logging estruturado (zerolog, zap)
- Níveis de log apropriados (debug, info, warning, error)
- Adicionar contexto aos logs (request_id, user_id, etc.)

## Gestão de Erros

- Usar `errors.Wrap` para adicionar contexto aos erros
- Retornar erros significativos, não apenas mensagens genéricas
- Usar códigos de erro HTTP apropriados
