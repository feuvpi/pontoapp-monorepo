# Flutter Code Standards

## Estrutura do Projeto

- `/lib`
  - `/app` - Aplicação principal
    - `/feature` - Cada feature é uma pasta (ex: `/auth`, `/ponto`, `/empresa`)
      - `/controller` - Lógica de negócios, chamadas API
      - `/screen` - Telas da feature
      - `/widgets` - Widgets específicos da feature
      - `/models` - Modelos de dados para a feature
  - `/core` - Classes utilitárias, constantes, extensões
  - `/shared` - Recursos compartilhados
    - `/formatters` - Formatadores de texto, data, etc.
    - `/models` - Modelos comuns
    - `/widgets` - Widgets reutilizáveis
  - `/services` - Serviços (API, autenticação, etc.)
    - `/middlewares` - Middlewares (autorização, etc.)
  - `router.dart` - Roteamento da aplicação

## Design System

### Cores e Temas

Utilizamos um sistema centralizado de cores em `colors.dart` com duas paletas principais:
- `AppColorLight` - Tema claro
- `AppColorDark` - Tema escuro

### Fontes e Tamanhos

Tamanhos consistentes definidos em `AppFontSize` para manter a coerência visual:
- `displayLarge/Medium/Small` - Textos maiores para destaques
- `headlineLarge/Medium/Small` - Títulos principais
- `titleLarge/Medium/Small` - Subtítulos
- `bodyLarge/Medium/Small` - Textos comuns
- `labelLarge/Medium/Small` - Etiquetas e informações secundárias

### Espaçamento

Espaçamentos padronizados em `AppSpaceSize` para garantir consistência:
- Valores predefinidos (5, 10, 15, 20, 25)
- Uso consistente em margem e padding

## Convenções de Código

### Nomenclatura
- Classes e Widgets em PascalCase (`LoginScreen`, `CustomButton`)
- Variáveis e métodos em camelCase (`userName`, `fetchData()`)
- Arquivos em snake_case.dart (`auth_service.dart`, `home_screen.dart`)
- Um widget/classe por arquivo
- Prefixo `_` para membros privados

### Organização dos Widgets
- Seguir a estrutura StatelessWidget/StatefulWidget
- Métodos `build` devem ser concisos
- Extração de widgets complexos para métodos ou classes separadas
- Uso de const onde aplicável para otimização

## Navegação

Utilizamos `go_router` para navegação declarativa:
- Rotas nomeadas definidas como constantes estáticas em cada tela
- Transições personalizadas quando necessário
- Passagem de parâmetros via extra
- Middleware de autenticação para proteção de rotas

## Gerenciamento de Estado

- Gerenciamento de estado conforme complexidade do caso:
  - `setState` para estados locais simples
  - Provider ou Riverpod para estados globais
  - Divisão clara entre UI e lógica de negócios

## Boas Práticas

### UI/UX
- Botões com tamanho mínimo de 48x48 para melhor acessibilidade
- Feedback visual para todas as interações
- Suporte a temas claro/escuro
- Uso consistente da paleta de cores definida

### Performance
- Uso de const widgets
- Lazy loading para listas longas
- Carregamento assíncrono de dados
- Uso de placeholders durante carregamento

### Código
- DRY (Don't Repeat Yourself)
- Funções pequenas com propósito único
- Documentação com /// para classes e métodos públicos
- Tratamento adequado de erros e estados vazios

## Estilo & Temas

Todo o estilo do aplicativo é centralizado em `style.dart` que contém:
- Definição do tema claro e escuro
- Configuração de componentes (botões, inputs, cards)
- Tipografia baseada em Google Fonts (Manrope)
- Gradientes e efeitos visuais padronizados

## Exemplo de Implementação de Tela

```dart
// auth/screens/login_screen.dart
import 'package:flutter/material.dart';
import '/core/style.dart';
import '/core/colors.dart';
import '../controller/auth_controller.dart';

class LoginScreen extends StatelessWidget {
  static const String path = '/login';
  
  const LoginScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: BoxDecoration(
          gradient: AppColorLight.surfaceGradient,
        ),
        child: SafeArea(
          child: Center(
            child: _buildLoginForm(context),
          ),
        ),
      ),
    );
  }

  Widget _buildLoginForm(BuildContext context) {
    // Implementação do formulário
  }
}
