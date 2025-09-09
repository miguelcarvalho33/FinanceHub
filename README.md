# FinanceHub

Um gestor de finanças pessoais em .NET 8 que processa extratos bancários em PDF e expõe os dados através de uma API RESTful.

---

## Backlog de Funcionalidades

Esta é a lista de funcionalidades e melhorias planeadas para o projeto, organizadas por épicos.

### Épico: Categorização Inteligente com Base em Contactos (VCF)

- **Objetivo:** Categorizar automaticamente transferências MBWay com base numa lista de contactos importada de um ficheiro VCF, para evitar a criação de regras manuais para cada pessoa.
- **Tarefas:**
    - [ ] **Modelo de Dados:** Criar uma nova tabela/modelo `Contact` na base de dados (`Id`, `Name`, `PhoneNumber`).
    - [ ] **Parser de VCF:** Criar um novo serviço (`VcfParserService`) capaz de ler um ficheiro `.vcf` e extrair uma lista de contactos.
    - [ ] **Serviço de Background para VCF:** Criar um novo serviço (`VcfProcessingService`) que monitoriza uma pasta à procura de ficheiros `.vcf` e atualiza a tabela `Contacts`.
    - [ ] **Melhorar o `CategorizationService`:**
        - [ ] Atualizar a lógica de `ApplyMbwayRules` para, depois de procurar por regras manuais, procurar na tabela `Contacts`.
        - [ ] Se encontrar um contacto, a descrição da transação deve ser limpa para "MBWay - [Nome do Contacto]".
        - [ ] Atribuir uma categoria *default* (ex: "Transferências MBWay") para estas transações.

### Épico: Evolução do Motor de Regras

- **Objetivo:** Tornar o sistema de regras mais flexível, poderoso e autónomo, reduzindo a necessidade de manutenção manual.
- **Tarefas:**
    - [ ] **Evoluir para um Motor de Regras Flexível:** Substituir as atuais `DescriptionRule` e `MbwayRule` por uma única entidade `CategorizationRule` com suporte para lógicas de **Keyword**, **Regex** e um sistema de **Prioridade**.
    - [ ] **Sugestão de Regras Automáticas:** Criar um serviço que analisa transações "Não Categorizadas" e sugere ativamente a criação de novas regras para padrões recorrentes (ex: "Spotify", "Uber").
    - [ ] **(v2.0) Implementar Motor de Machine Learning:** Usar ML.NET para treinar um modelo que aprende com as transações já categorizadas para prever a categoria de novas transações.

### Épico: Interface e Usabilidade

- **Objetivo:** Criar as ferramentas visuais necessárias para uma gestão simples da aplicação, sem interação direta com a base de dados.
- **Tarefas:**
    - [ ] **UI de Gestão de Regras e Categorias:** Desenvolver uma interface web (Blazor/Razor Pages) para gerir (CRUD) categorias e as novas `CategorizationRule`.
    - [ ] **Dashboard de Análise Visual:** Criar uma página de dashboard na aplicação com gráficos interativos (despesas por categoria, evolução de saldo, etc.).
    - [ ] **Adição Manual de Transações:** Implementar um formulário para inserir transações manuais (ex: despesas em dinheiro).

### Épico: Funcionalidades Financeiras Avançadas

- **Objetivo:** Adicionar funcionalidades que permitam uma gestão financeira mais proativa, como orçamentação e previsibilidade.
- **Tarefas:**
    - [ ] **Deteção de Transações Recorrentes:** Identificar e marcar automaticamente pagamentos recorrentes (Netflix, ginásio, renda).
    - [ ] **Sistema de Orçamentos (Budgets):** Permitir a definição de orçamentos mensais por categoria e visualizar o progresso dos gastos.
    - [ ] **Divisão de Transações (Split Transactions):** Permitir que uma única transação seja dividida por múltiplas categorias.

### Épico: Expansão da Captura de Dados

- **Objetivo:** Aumentar as fontes de dados que o FinanceHub consegue processar.
- **Tarefas:**
    - [ ] **Importador de Ficheiros CSV:** Adicionar a funcionalidade de importar transações a partir de ficheiros CSV.
    - [ ] **(Longo Prazo) Integração com Open Banking (PSD2):** Investigar a ligação direta a APIs de bancos para importar transações em tempo real.

---
