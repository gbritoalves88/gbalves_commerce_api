# GBAlves Commerce — E-commerce Full Stack

Projeto de e-commerce desenvolvido como uma **base reutilizável para criação de lojas virtuais**.

A proposta é manter uma estrutura funcional de comércio eletrônico, com vitrine, catálogo de produtos, carrinho, checkout, pagamentos e painel administrativo, permitindo reutilizar o projeto em novos negócios com adaptações de identidade visual, catálogo e configurações comerciais.

O projeto é dividido em dois repositórios:

- **Frontend:** Angular 21.
- **Backend:** ASP.NET Core 9.

## 🎯 Objetivo

O GBAlves Commerce foi desenvolvido como um projeto-base para acelerar a criação de lojas virtuais.

Em vez de iniciar cada novo e-commerce do zero, a ideia é aproveitar uma estrutura já desenvolvida, mantendo funcionalidades comuns e adaptando:

- Identidade visual.
- Logo e cores.
- Banners e imagens.
- Categorias e produtos.
- Menus e páginas institucionais.
- Dados comerciais e informações de contato.
- Configurações de pagamento e frete.

O objetivo é reduzir o tempo de desenvolvimento de novos projetos e manter uma arquitetura organizada, que permita evoluções sem precisar reconstruir toda a aplicação.

## 🏗️ Arquitetura

O sistema utiliza uma arquitetura cliente-servidor, com frontend independente e backend disponibilizado por meio de uma API REST.

### Repositórios

| Módulo | Repositório |
|---|---|
| Frontend | [gbalves_commerce](https://github.com/gbritoalves88/gbalves_commerce/tree/main/frontend) |
| Backend | [gbalves_commerce_api](https://github.com/gbritoalves88/gbalves_commerce_api) |

### Visão geral

```text
                  CLIENTE
                     |
                     v
          +----------------------+
          |   Angular 21        |
          |                      |
          | - Vitrine            |
          | - Produtos           |
          | - Carrinho           |
          | - Checkout           |
          | - Área do cliente    |
          | - Painel admin       |
          +----------+-----------+
                     |
                  HTTP / JWT
                     |
                     v
          +----------------------+
          |   ASP.NET Core 9     |
          |                      |
          | - Controllers        |
          | - Application        |
          | - Domain             |
          | - Infrastructure     |
          +----------+-----------+
                     |
          +----------+-----------+
          |          |           |
          v          v           v
      PostgreSQL  Mercado Pago  Cloudinary
                     |
                 Melhor Envio
```

## ✨ Funcionalidades

### Loja virtual

- Página inicial com banners e produtos em destaque.
- Catálogo de produtos.
- Pesquisa de produtos.
- Filtro por categoria.
- Página de detalhes do produto.
- Exibição de imagens, preços e disponibilidade.
- Menus dinâmicos.
- Páginas institucionais.
- Layout responsivo.

### Carrinho de compras

- Adicionar produtos ao carrinho.
- Atualizar quantidades.
- Remover produtos.
- Calcular subtotal.
- Exibir quantidade de itens.
- Persistência local do carrinho utilizando `localStorage`.
- Validação de estoque durante as operações de compra no backend.

### Cadastro e autenticação

- Cadastro de clientes.
- Login com e-mail e senha.
- Autenticação JWT.
- Persistência da sessão no frontend.
- Interceptor HTTP para envio do token.
- Guards para proteção das rotas.
- Controle de acesso por nível de usuário.

### Checkout

- Identificação do cliente.
- Preenchimento de endereço.
- Consulta de CEP.
- Validação de CPF e telefone.
- Cálculo de frete.
- Seleção da modalidade de entrega.
- Escolha da forma de pagamento.
- Criação do pedido.

### Pagamentos

Integração com o Mercado Pago para processamento de pagamentos.

Modalidades implementadas:

- PIX com geração de QR Code.
- Cartão de crédito.
- Tokenização de cartão pelo SDK do Mercado Pago.
- Consulta e atualização de status.
- Recebimento de notificações por webhook.
- Validação de assinatura das notificações.
- Registro de eventos de pagamento.
- Tratamento de pagamentos pendentes, aprovados, recusados e cancelados.

### Entregas e frete

Integração com o Melhor Envio.

- Consulta de CEP.
- Cálculo de frete por destino.
- Consulta de modalidades de envio.
- Exibição de transportadora, preço e prazo.
- Registro de informações de envio no pedido.
- Atualização do código de rastreamento.

### Área do cliente

- Visualização do perfil.
- Atualização dos dados cadastrais.
- Consulta de pedidos.
- Acompanhamento do status dos pedidos.
- Visualização dos detalhes da compra.

### Painel administrativo

O painel administrativo permite gerenciar a operação da loja.

**Dashboard**
- Resumo dos pedidos.
- Faturamento.
- Quantidade de produtos.
- Quantidade de clientes.
- Exportação de relatórios.

**Produtos**
- Cadastro.
- Edição.
- Exclusão.
- Controle de estoque.
- Destaques.
- Imagens.

**Categorias**
- Cadastro.
- Edição.
- Exclusão.
- Organização do catálogo.

**Pedidos**
- Consulta de pedidos.
- Visualização dos detalhes.
- Atualização de status.
- Cancelamento.
- Atualização de código de rastreamento.
- Verificação de pagamento.

**Banners**
- Cadastro e gerenciamento.
- Ativação e desativação.
- Exibição de banners ativos na vitrine.

**Menus**
- Criação de menus.
- Gerenciamento de itens.
- Organização por localização.
- Ativação e desativação.

**Configurações da loja**
- Nome.
- Logo.
- Cores.
- Informações comerciais.
- Dados de contato.

## 🧰 Tecnologias utilizadas

### Frontend

| Tecnologia | Utilização |
|---|---|
| Angular 21 | Framework frontend |
| TypeScript 5.9 | Linguagem principal |
| Angular Material 21 | Componentes de interface |
| Angular CDK | Recursos de interface |
| RxJS 7.8 | Programação reativa |
| Angular Router | Navegação |
| Angular Signals | Gerenciamento de estado reativo |
| Swiper 14 | Carrosséis e sliders |
| SCSS | Estilização |
| Vitest 4 | Testes unitários |

### Backend

| Tecnologia | Utilização |
|---|---|
| C# | Linguagem principal |
| .NET 9 | Plataforma de desenvolvimento |
| ASP.NET Core Web API | API REST |
| Entity Framework Core 9 | ORM |
| PostgreSQL | Banco de dados |
| Npgsql | Provider PostgreSQL para EF Core |
| JWT Bearer | Autenticação |
| BCrypt.Net | Hash de senhas |
| Serilog | Logging |
| Swashbuckle | Swagger / OpenAPI |
| ClosedXML | Geração de planilhas Excel |

### Integrações externas

| Serviço | Utilização |
|---|---|
| Mercado Pago | Pagamentos PIX e cartão |
| Melhor Envio | Cálculo de frete |
| Cloudinary | Armazenamento de imagens |
| SMTP | Envio de e-mails |

## 📁 Estrutura do backend

O backend utiliza separação de responsabilidades em quatro projetos.

```text
GBAlves.Commerce.Api/
│
├── GBAlves.Commerce.Api/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
│
├── GBAlves.Commerce.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
│
├── GBAlves.Commerce.Domain/
│   ├── Entities/
│   └── Enums/
│
├── GBAlves.Commerce.Infrastructure/
│   ├── Data/
│   ├── Migrations/
│   └── Payment/
│
├── GBAlves.Commerce.sln
└── dockerfile
```

### Responsabilidades

**API**
- Exposição dos endpoints REST.
- Autenticação e autorização.
- Configuração de dependências.
- Middlewares e políticas da aplicação.

**Application**
- Serviços de aplicação.
- Regras de orquestração dos casos de uso.
- Interfaces e contratos.
- DTOs de entrada e saída.

**Domain**
- Entidades do negócio.
- Enums.
- Estruturas centrais do domínio.

**Infrastructure**
- Persistência com Entity Framework Core.
- Configuração do banco.
- Migrations.
- Integrações com serviços externos.

## 📁 Estrutura do frontend

O frontend utiliza organização por funcionalidades (*feature-based*), com componentes standalone.

```text
frontend/
│
├── src/
│   ├── app/
│   │   ├── features/
│   │   │   ├── admin/
│   │   │   │   ├── admin-layout/
│   │   │   │   ├── dashboard/
│   │   │   │   ├── produtos/
│   │   │   │   ├── categorias/
│   │   │   │   ├── pedidos/
│   │   │   │   ├── banners/
│   │   │   │   ├── menus/
│   │   │   │   └── configuracoes/
│   │   │   │
│   │   │   ├── home/
│   │   │   ├── produtos/
│   │   │   ├── carrinho/
│   │   │   ├── checkout/
│   │   │   ├── pagamento/
│   │   │   ├── pedido-confirmado/
│   │   │   ├── login/
│   │   │   ├── cadastro/
│   │   │   ├── perfil/
│   │   │   └── institucional/
│   │   │
│   │   └── shared/
│   │       ├── guards/
│   │       ├── interceptors/
│   │       ├── models/
│   │       ├── services/
│   │       ├── header/
│   │       ├── footer/
│   │       └── produto-card/
│   │
│   ├── environments/
│   ├── styles.scss
│   └── main.ts
│
├── angular.json
├── package.json
└── tsconfig.json
```

## 🔄 Fluxo de compra

O fluxo principal da aplicação funciona da seguinte maneira:

1. O cliente acessa a vitrine e consulta os produtos.
2. Seleciona os produtos desejados e adiciona ao carrinho.
3. Informa os dados de entrega.
4. O frontend consulta o CEP e solicita as opções de frete.
5. O cliente escolhe a modalidade de entrega.
6. O pedido é criado na API.
7. O cliente escolhe PIX ou cartão de crédito.
8. A API solicita o processamento do pagamento ao Mercado Pago.
9. O pagamento é acompanhado por meio do retorno do gateway e dos webhooks.
10. O pedido tem seu status atualizado conforme o processamento.
11. O cliente pode acompanhar a compra na área de pedidos.

## 🗄️ Banco de dados

O backend utiliza PostgreSQL com Entity Framework Core.

O contexto `AppDbContext` gerencia as entidades:

| Entidade | Finalidade |
|---|---|
| `ConfiguracaoLoja` | Configurações da loja |
| `Usuario` | Usuários e autenticação |
| `Cliente` | Dados dos clientes |
| `Categoria` | Categorias de produtos |
| `Produto` | Catálogo e estoque |
| `Pedido` | Pedidos realizados |
| `ItemPedido` | Itens de cada pedido |
| `Menu` | Menus da loja |
| `ItemMenu` | Itens dos menus |
| `Banner` | Banners da vitrine |
| `Pagamento` | Informações de pagamentos |
| `LogPagamento` | Logs de processamento |
| `EventoPagamento` | Eventos de pagamento |

As alterações de estrutura do banco são controladas por migrations do Entity Framework Core.

## 🔐 Autenticação e autorização

A autenticação utiliza JWT Bearer.

O sistema possui três níveis de acesso:

| Perfil | Permissões gerais |
|---|---|
| Admin | Gerenciamento completo |
| Operador | Operação de produtos, categorias e pedidos |
| Cliente | Compras, perfil e consulta dos próprios pedidos |

O backend utiliza políticas de autorização para proteger os endpoints.

No frontend, a proteção de navegação é realizada por guards e o token é enviado por um interceptor HTTP.

A validação efetiva de permissões é sempre realizada no backend.

## 🚀 Executando localmente

### Pré-requisitos

- Node.js compatível com Angular 21.
- npm 10.9.7 ou compatível.
- .NET SDK 9.
- PostgreSQL.
- Git.

Para utilizar todas as funcionalidades, também serão necessárias credenciais dos serviços externos.

### 1. Clonar os repositórios

Frontend:

```bash
git clone https://github.com/gbritoalves88/gbalves_commerce.git
```

Backend:

```bash
git clone https://github.com/gbritoalves88/gbalves_commerce_api.git
```

### 2. Configurar o backend

Entre na pasta do backend:

```bash
cd gbalves_commerce_api
```

Restaure os pacotes:

```bash
dotnet restore GBAlves.Commerce.sln
```

Configure as variáveis de ambiente ou o arquivo de configuração local.

Principais configurações utilizadas:

| Configuração | Finalidade |
|---|---|
| `ConnectionStrings:DefaultConnection` | Conexão PostgreSQL |
| `Jwt:Key` | Segredo JWT |
| `Jwt:Issuer` | Emissor do token |
| `Jwt:Audience` | Audiência do token |
| `Cloudinary:Url` | Credenciais Cloudinary |
| `MercadoPago:AccessToken` | Token de acesso |
| `MercadoPago:PublicKey` | Chave pública |
| `MercadoPago:WebhookSecret` | Segredo de validação do webhook |
| `Email:Host` | Servidor SMTP |
| `Email:Port` | Porta SMTP |
| `Email:Username` | Usuário SMTP |
| `Email:Password` | Senha SMTP |
| `Email:From` | Remetente |
| `MelhorEnvio:Token` | Token Melhor Envio |
| `MelhorEnvio:CepOrigem` | CEP de origem |
| `AppUrl` | URL da aplicação |

As configurações podem ser fornecidas por variáveis de ambiente usando a convenção do ASP.NET Core, por exemplo:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
Cloudinary__Url
MercadoPago__AccessToken
MercadoPago__WebhookSecret
MelhorEnvio__Token
```

Não versione credenciais, tokens ou senhas.

### 3. Preparar o banco

Com o PostgreSQL disponível e a connection string configurada, aplique as migrations:

```bash
dotnet ef database update \
  --project GBAlves.Commerce.Infrastructure \
  --startup-project GBAlves.Commerce.Api
```

Caso necessário, instale a ferramenta:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Executar a API

```bash
dotnet run --project GBAlves.Commerce.Api
```

O endereço local configurado no frontend para desenvolvimento é:

```text
http://localhost:5184/api
```

O Swagger é habilitado no ambiente de desenvolvimento.

### 5. Configurar o frontend

Entre na pasta:

```bash
cd gbalves_commerce/frontend
```

Instale as dependências:

```bash
npm install
```

Confira:

```text
src/environments/environment.ts
```

A configuração de desenvolvimento utiliza:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5184/api',
  mercadoPagoPublicKey: 'SUA_CHAVE_PUBLICA'
};
```

A chave pública do Mercado Pago deve corresponder ao ambiente de pagamento utilizado.

### 6. Executar o frontend

```bash
npm start
```

A aplicação ficará disponível em:

```text
http://localhost:4200
```

## 🏗️ Build e publicação

### Frontend

Build de produção:

```bash
npm run build
```

Os arquivos compilados são gerados no diretório `dist/`.

O projeto possui configuração de produção com URL da API hospedada no Render e configuração de publicação na Vercel.

### Backend

O backend possui Dockerfile com build em múltiplos estágios, utilizando imagens .NET 9 SDK e ASP.NET Runtime.

Para construir a imagem, a partir da raiz do repositório:

```bash
docker build -f dockerfile -t gbalves-commerce-api .
```

Para executar:

```bash
docker run -p 8080:8080 \
  -e DATABASE_URL="SUA_CONNECTION_STRING" \
  -e JWT_KEY="SEU_SEGREDO_JWT" \
  gbalves-commerce-api
```

As demais configurações externas também devem ser fornecidas ao container.

A API escuta na porta `8080` dentro do container.

## 📡 Principais grupos de endpoints

Os endpoints são organizados por controllers e utilizam o prefixo `/api`.

| Grupo | Exemplos de operações |
|---|---|
| Auth | Login |
| Usuarios | Cadastro, consulta e atualização |
| Produtos | Catálogo, pesquisa e administração |
| Categorias | Consulta e gerenciamento |
| Pedidos | Criação, consulta, status e rastreamento |
| Checkout | Criação de pagamento |
| Frete | Cálculo de frete |
| Banners | Consulta e gerenciamento |
| Menus | Consulta e gerenciamento |
| ConfiguracaoLoja | Consulta e atualização da loja |
| Dashboard | Indicadores administrativos |
| Upload | Envio de imagens |
| Webhook | Notificações de pagamento |
| Relatorio | Exportação Excel |

Exemplos de rotas:

```text
POST /api/Auth/login

GET  /api/Produtos
GET  /api/Produtos/destaques
GET  /api/Produtos/slug/{slug}

POST /api/Pedidos
GET  /api/Pedidos/meus

POST /api/Checkout
POST /api/Frete/calcular

GET  /api/Banners/ativos
GET  /api/ConfiguracaoLoja

POST /api/Webhook/pagamento
GET  /api/Relatorio/excel
```

A documentação Swagger apresenta os contratos completos quando executada em ambiente de desenvolvimento.

## 🔧 Adaptação para uma nova loja

O projeto foi pensado para ser reutilizado como ponto de partida.

Para criar uma nova loja, a adaptação pode seguir este roteiro:

1. Copiar o frontend e o backend para novos repositórios.
2. Alterar nome, logo, cores e identidade visual.
3. Configurar banners e menus.
4. Cadastrar categorias e produtos.
5. Ajustar textos e páginas institucionais.
6. Configurar a URL da API no frontend.
7. Configurar banco de dados e credenciais dos serviços.
8. Configurar Mercado Pago e Melhor Envio.
9. Configurar domínio, CORS e hospedagem.
10. Validar o fluxo completo de compra antes da publicação.

Cada nova loja deve utilizar suas próprias credenciais e configuração de infraestrutura.

O modelo atual permite personalização das configurações de uma loja, mas não implementa isolamento de dados entre múltiplas lojas em uma única instalação.

## 🧪 Testes

Frontend:

```bash
npm test
```

O projeto utiliza Vitest por meio da configuração do Angular CLI.

## 📌 Considerações

O GBAlves Commerce é um projeto de base para desenvolvimento de lojas virtuais, construído com foco em reutilização, organização e integração entre frontend, backend e serviços externos.

Mais do que uma vitrine de produtos, o projeto contempla o fluxo de compra, processamento de pagamento, cálculo de frete e administração da loja.

---

Desenvolvido por [Gabriel Brito](https://github.com/gbritoalves88).
