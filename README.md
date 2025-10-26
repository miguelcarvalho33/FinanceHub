# FinanceHub

Um gestor de finanças pessoais em .NET 8 que processa extratos bancários em PDF, faturas e outros documentos, expondo os dados através de uma API RESTful para análise.

## Backlog de Funcionalidades

Esta é a lista de funcionalidades e melhorias planeadas para o projeto, organizadas por épicos.

---

### Épico: Captura de Dados Avançada (Faturas e Recibos)

-   **Objetivo:** Expandir drasticamente as fontes de dados do FinanceHub para além dos extratos bancários, permitindo a leitura de faturas de serviços, documentos salariais e até recibos de compras físicas através de imagens.

-   **Tarefas:**
    -   [ ] **Sub-Épico: Leitura de Faturas de Serviços (Utilities)**
        -   [ ] **Parser para Faturas EDP:** Criar um `EdpInvoiceParser` para ler o valor total e data de faturas da EDP.
        -   [ ] **Parser para Faturas de Telecomunicações:** Implementar parsers para a MEO (`MeoInvoiceParser`) e Vodafone (`VodafoneInvoiceParser`).
        -   [ ] **Modelo de Dados `Invoice`:** Criar uma nova entidade para armazenar dados específicos de faturas.
        -   [ ] **Atualizar o `PdfProcessingService`:** Habilitar o serviço de background para processar estes novos tipos de PDF.
    -   [ ] **Sub-Épico: Processamento de Documentos Pessoais**
        -   [ ] **Parser de Recibo de Vencimento:** Desenvolver um `PayslipParser` para extrair salário líquido/bruto, deduções e contribuições.
        -   [ ] **Parser de Declaração de IRS:** Criar um `IrsDeclarationParser` para extrair o valor de reembolso ou a pagar.
    -   [ ] **Sub-Épico: Leitura de Recibos de Compras com OCR**
        -   [ ] **Investigação de Bibliotecas OCR:** Avaliar bibliotecas .NET (`IronOcr`, `Tesseract`) ou serviços cloud (`Azure Vision`, `Google Vision AI`).
        -   [ ] **Endpoint de Upload de Imagem:** Criar um endpoint na API (`POST /api/receipts/upload`).
        -   [ ] **Serviço de OCR (`OcrService`):** Criar um serviço que processa a imagem e extrai o texto.
        -   [ ] **Criação de Transação a partir de Recibo:** Desenvolver a lógica para criar uma transação a partir do texto extraído pelo OCR.
    -   [ ] **Importador de Ficheiros CSV:** Adicionar a funcionalidade de importar transações a partir de ficheiros CSV.
    -   [ ] **(Longo Prazo) Integração com Open Banking (PSD2):** Investigar a ligação direta a APIs de bancos para importar transações em tempo real.

---

### Épico: Categorização Inteligente com Base em Contactos (VCF)

-   **Objetivo:** Categorizar automaticamente transferências MBWay com base numa lista de contactos importada de um ficheiro VCF, para evitar a criação de regras manuais para cada pessoa.
-   **Tarefas:**
    -   [ ] **Modelo de Dados:** Criar uma nova tabela/modelo `Contact` na base de dados (`Id`, `Name`, `PhoneNumber`).
    -   [ ] **Parser de VCF:** Criar um novo serviço (`VcfParserService`) capaz de ler um ficheiro `.vcf`.
    -   [ ] **Serviço de Background para VCF:** Criar um novo serviço que monitoriza uma pasta à procura de ficheiros `.vcf` e atualiza a tabela `Contacts`.
    -   [ ] **Melhorar o `CategorizationService`:** Atualizar a lógica para usar a tabela `Contacts` como fallback às regras MBWay manuais.

---

### Épico: Evolução do Motor de Regras

-   **Objetivo:** Tornar o sistema de regras mais flexível, poderoso e autónomo, reduzindo a necessidade de manutenção manual.
-   **Tarefas:**
    -   [ ] **Evoluir para um Motor de Regras Flexível:** Substituir `DescriptionRule` e `MbwayRule` por uma única entidade `CategorizationRule` com suporte para **Keyword**, **Regex** e **Prioridade**.
    -   [ ] **Sugestão de Regras Automáticas:** Criar um serviço que analisa transações "Não Categorizadas" e sugere novas regras para padrões recorrentes.
    -   [ ] **(v2.0) Implementar Motor de Machine Learning:** Usar ML.NET para treinar um modelo que aprende com as transações já categorizadas para prever a categoria de novas transações.

---

### Épico: Funcionalidades Financeiras Avançadas

-   **Objetivo:** Adicionar funcionalidades que permitam uma gestão financeira mais proativa, como orçamentação e previsibilidade.
-   **Tarefas:**
    -   [ ] **Deteção de Transações Recorrentes:** Identificar e marcar automaticamente pagamentos recorrentes (Netflix, ginásio, renda).
    -   [ ] **Sistema de Orçamentos (Budgets):** Permitir a definição de orçamentos mensais por categoria e visualizar o progresso dos gastos.
    -   [ ] **Divisão de Transações (Split Transactions):** Permitir que uma única transação seja dividida por múltiplas categorias.

---

### Épico: Interface e Usabilidade

-   **Objetivo:** Criar as ferramentas visuais necessárias para uma gestão simples da aplicação, sem interação direta com a base de dados.
-   **Tarefas:**
    -   [ ] **UI de Gestão de Regras e Categorias:** Desenvolver uma interface web (Blazor/Razor Pages) para gerir (CRUD) categorias e regras.
    -   [ ] **Dashboard de Análise Visual:** Criar uma página de dashboard na aplicação com gráficos interativos.
    -   [ ] **Adição Manual de Transações:** Implementar um formulário para inserir transações manuais.

---

### Épico: Melhoria da API e Integrações

-   **Objetivo:** Aumentar a utilidade da API para além do consumo de dados brutos.
-   **Tarefas:**
    -   [ ] **Endpoints de Relatórios Avançados:** Criar endpoints para relatórios comuns (ex: `GET /api/reports/monthly-expenses`).
    -   [ ] **Autenticação e Segurança:** Implementar API Keys ou JWT para proteger os endpoints.
    -   [ ] **Webhooks:** Permitir que aplicações externas sejam notificadas em tempo real sobre novas transações.

---

### Épico: Qualidade de Código e Manutenção

-   **Objetivo:** Garantir a estabilidade e fiabilidade do projeto.
-   **Tarefas:**
    -   [ ] **Implementar Testes Unitários e de Integração:** Criar um projeto de testes para validar a lógica dos parsers e serviços.
    -   [ ] **Refatoração e Documentação:** Rever o código, adicionar documentação (Swagger/XML Comments) e garantir as boas práticas.

---
---

## Visão de Longo Prazo (Future Epics)

### Épico: Inteligência Financeira e Projeções

-   **Objetivo:** Transformar dados históricos em previsões e insights úteis, ajudando o utilizador a tomar melhores decisões financeiras de forma proativa.
-   **Tarefas:**
    -   [ ] **Previsão de Saldo (Cash Flow Forecasting):** Usar o histórico para projetar o saldo futuro e alertar para possíveis dificuldades.
    -   [ ] **Metas de Poupança (Savings Goals):** Permitir a criação de objetivos de poupança e acompanhar o progresso.
    -   [ ] **Deteção de Anomalias:** Alertar para atividades invulgares (ex: uma fatura de eletricidade muito mais alta que o normal).
    -   [ ] **"Health Score" Financeiro:** Desenvolver um índice de saúde financeira com base na taxa de poupança e outros indicadores.

### Épico: Experiência do Utilizador e Notificações

-   **Objetivo:** Tornar a aplicação mais interativa, envolvente e útil no dia a dia.
-   **Tarefas:**
    -   [ ] **Sistema de Alertas e Notificações:** Enviar alertas (email/push) para eventos importantes (ex: "Limite de orçamento atingido").
    -   [ ] **Relatórios Personalizáveis:** Permitir ao utilizador criar e guardar os seus próprios dashboards.
    -   [ ] **Gamificação:** Adicionar emblemas e conquistas por atingir metas financeiras.

### Épico: Expansão do Ecossistema e Conectividade

-   **Objetivo:** Integrar o FinanceHub com outras ferramentas para automatizar ainda mais a recolha de informação.
-   **Tarefas:**
    -   [ ] **Leitura de E-mails:** Criar um serviço para extrair automaticamente faturas e recibos digitais de e-mails.
    -   [ ] **Gestão Familiar (Multi-Utilizador):** Permitir a criação de um "agregado familiar" para gerir um orçamento comum.
    -   [ ] **Integração com Calendários:** Sincronizar datas de vencimento de faturas com Google Calendar ou Outlook Calendar.
