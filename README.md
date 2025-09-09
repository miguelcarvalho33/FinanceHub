# FinanceHub

Um gestor de finanças pessoais em .NET 8 que processa extratos bancários em PDF e expõe os dados através de uma API RESTful.

---

## Backlog de Funcionalidades (TODO)

Esta é a lista de funcionalidades planeadas para o futuro.

### 1. Motor de Categorização Inteligente com Base em Contactos (VCF)

- **Objetivo:** Categorizar automaticamente transferências MBWay com base numa lista de contactos importada de um ficheiro VCF, para evitar a criação de regras manuais para cada pessoa.
- **Tarefas:**
    - [ ] **Modelo de Dados:** Criar uma nova tabela/modelo `Contact` na base de dados (`Id`, `Name`, `PhoneNumber`).
    - [ ] **Parser de VCF:** Criar um novo serviço (`VcfParserService`) capaz de ler um ficheiro `.vcf` e extrair uma lista de contactos.
    - [ ] **Serviço de Background para VCF:** Criar um novo serviço (`VcfProcessingService`) que monitoriza uma pasta à procura de ficheiros `.vcf` e atualiza a tabela `Contacts` na base de dados (ex: mensalmente, substituindo os contactos antigos).
    - [ ] **Melhorar o `CategorizationService`:**
        - [ ] Atualizar a lógica de `ApplyMbwayRules` para, depois de procurar por regras manuais, procurar na tabela `Contacts` se os últimos 4 dígitos de uma transferência correspondem a algum contacto.
        - [ ] Se encontrar um contacto, a descrição da transação deve ser limpa para "MBWay - [Nome do Contacto]".
        - [ ] Atribuir uma categoria *default* (ex: "Transferências MBWay") para estas transações.
        - [ ] Lidar com casos em que o nome do destinatário já vem na descrição do extrato.

---
