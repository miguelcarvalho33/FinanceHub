-- Script gerado automaticamente a partir de financehub.db
CREATE DATABASE FinanceHub;
GO
USE FinanceHub;
GO

-- Tabela: __EFMigrationsLock
CREATE TABLE [__EFMigrationsLock] (
[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
[Timestamp] NVARCHAR(MAX) NOT NULL
);
GO

-- Tabela: __EFMigrationsHistory
CREATE TABLE [__EFMigrationsHistory] (
[MigrationId] NVARCHAR(MAX) NOT NULL PRIMARY KEY IDENTITY(1,1),
[ProductVersion] NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO [__EFMigrationsHistory] VALUES (N'20250908232309_InitialCreate', N'9.0.8');
INSERT INTO [__EFMigrationsHistory] VALUES (N'20250909023538_RenamedPhoneNumberToSuffixInMbwayRule', N'9.0.8');
INSERT INTO [__EFMigrationsHistory] VALUES (N'20250909222451_AddContactsTable', N'9.0.8');
GO

-- Tabela: Categories
CREATE TABLE [Categories] (
[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
[Name] NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO [Categories] VALUES (1, N'Não Categorizado');
INSERT INTO [Categories] VALUES (2, N'Habitação');
INSERT INTO [Categories] VALUES (3, N'Contas da Casa');
INSERT INTO [Categories] VALUES (4, N'Manutenção da Casa');
INSERT INTO [Categories] VALUES (5, N'Decoração e Mobiliário');
INSERT INTO [Categories] VALUES (6, N'Animais de Estimação');
INSERT INTO [Categories] VALUES (7, N'Supermercado');
INSERT INTO [Categories] VALUES (8, N'Transporte');
INSERT INTO [Categories] VALUES (9, N'Saúde e Bem-Estar');
INSERT INTO [Categories] VALUES (10, N'Cuidados Pessoais');
INSERT INTO [Categories] VALUES (11, N'Educação');
INSERT INTO [Categories] VALUES (12, N'Vestuário e Acessórios');
INSERT INTO [Categories] VALUES (13, N'Restaurantes e Cafés');
INSERT INTO [Categories] VALUES (14, N'Take-Away e Entregas');
INSERT INTO [Categories] VALUES (15, N'Assinaturas e Subscrições');
INSERT INTO [Categories] VALUES (16, N'Hobbies');
INSERT INTO [Categories] VALUES (17, N'Viagens e Férias');
INSERT INTO [Categories] VALUES (18, N'Eventos e Cultura');
INSERT INTO [Categories] VALUES (19, N'Créditos');
INSERT INTO [Categories] VALUES (20, N'Seguros');
INSERT INTO [Categories] VALUES (21, N'Impostos');
INSERT INTO [Categories] VALUES (22, N'Taxas e Comissões Bancárias');
INSERT INTO [Categories] VALUES (23, N'Serviços Profissionais');
INSERT INTO [Categories] VALUES (24, N'Presentes e Doações');
INSERT INTO [Categories] VALUES (25, N'Salário');
INSERT INTO [Categories] VALUES (26, N'Rendimentos Extra');
INSERT INTO [Categories] VALUES (27, N'Transferências');
INSERT INTO [Categories] VALUES (28, N'Poupança e Investimento');
INSERT INTO [Categories] VALUES (29, N'Levantamentos');
INSERT INTO [Categories] VALUES (30, N'Pagamento de Serviços');
INSERT INTO [Categories] VALUES (31, N'Despesas Gerais');
INSERT INTO [Categories] VALUES (32, N'Compras Gerais');
GO

-- Tabela: sqlite_sequence
CREATE TABLE [sqlite_sequence] (
[name] NVARCHAR(MAX),
[seq] NVARCHAR(MAX)
);
GO

INSERT INTO [sqlite_sequence] VALUES (N'Categories', 33);
INSERT INTO [sqlite_sequence] VALUES (N'Transactions', 2049);
INSERT INTO [sqlite_sequence] VALUES (N'DescriptionRules', 73);
INSERT INTO [sqlite_sequence] VALUES (N'Contacts', 589);
GO

-- Tabela: DescriptionRules
CREATE TABLE [DescriptionRules] (
[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
[TextToFind] NVARCHAR(MAX) NOT NULL,
[CleanDescription] NVARCHAR(MAX) NOT NULL,
[CategoryId] INT NOT NULL
);
GO

INSERT INTO [DescriptionRules] VALUES (1, N'EDP COMERCIAL', N'Fatura EDP RCB43', 3);
INSERT INTO [DescriptionRules] VALUES (2, N'MEO SA', N'Fatura MEO RCB43', 3);
INSERT INTO [DescriptionRules] VALUES (3, N'PRESTACAO', N'Prestação de Crédito RCB43', 3);
INSERT INTO [DescriptionRules] VALUES (4, N'C CLASSIC', N'Cartão de Crédito', 6);
INSERT INTO [DescriptionRules] VALUES (5, N'SANT CONSUMER PT', N'Crédito Santander', 6);
INSERT INTO [DescriptionRules] VALUES (6, N'MONTEPIO CREDITO', N'Crédito Carro Montepio', 6);
INSERT INTO [DescriptionRules] VALUES (7, N'LIDL', N'Supermercado - Lidl', 7);
INSERT INTO [DescriptionRules] VALUES (8, N'CONTINENTE', N'Supermercado - Continente', 7);
INSERT INTO [DescriptionRules] VALUES (9, N'PINGO DOCE', N'Supermercado - Pingo Doce', 7);
INSERT INTO [DescriptionRules] VALUES (10, N'MERCADONA', N'Supermercado - Mercadona', 7);
INSERT INTO [DescriptionRules] VALUES (11, N'ALGARTALHOS', N'Supermercado - Algartalhos', 7);
INSERT INTO [DescriptionRules] VALUES (12, N'ALDI', N'Supermercado - Aldi', 7);
INSERT INTO [DescriptionRules] VALUES (13, N'UBER *TRIP', N'Transporte - Uber', 8);
INSERT INTO [DescriptionRules] VALUES (14, N'BP ', N'Combustível - BP', 8);
INSERT INTO [DescriptionRules] VALUES (15, N'DIESEL SERVICE', N'Combustível', 8);
INSERT INTO [DescriptionRules] VALUES (16, N'RYANAIR', N'Viagem - Ryanair', 8);
INSERT INTO [DescriptionRules] VALUES (17, N'WELLS SAUDE', N'Saúde - Wells', 9);
INSERT INTO [DescriptionRules] VALUES (18, N'GYMNASIUM', N'Ginásio', 9);
INSERT INTO [DescriptionRules] VALUES (19, N'ZARA', N'Roupa - Zara', 12);
INSERT INTO [DescriptionRules] VALUES (20, N'LEFTIES', N'Roupa - Lefties', 12);
INSERT INTO [DescriptionRules] VALUES (21, N'PARFOIS', N'Acessórios - Parfois', 12);
INSERT INTO [DescriptionRules] VALUES (22, N'GLOVO', N'Entrega - Glovo', 14);
INSERT INTO [DescriptionRules] VALUES (23, N'UBER *EATS', N'Entrega - Uber Eats', 14);
INSERT INTO [DescriptionRules] VALUES (24, N'DOMINOS', N'Pizzaria - Domino''s', 14);
INSERT INTO [DescriptionRules] VALUES (25, N'KFC', N'Restaurante - KFC', 14);
INSERT INTO [DescriptionRules] VALUES (26, N'NETFLIX', N'Subscrição - Netflix', 15);
INSERT INTO [DescriptionRules] VALUES (27, N'SPOTIFY', N'Subscrição - Spotify', 15);
INSERT INTO [DescriptionRules] VALUES (28, N'GOOGLE', N'Serviços Google', 15);
INSERT INTO [DescriptionRules] VALUES (29, N'HUMBLEBUNDLE', N'Subscrição - Humble Bundle', 15);
INSERT INTO [DescriptionRules] VALUES (30, N'OPENAI', N'Subscrição - OpenAI/ChatGPT', 15);
INSERT INTO [DescriptionRules] VALUES (31, N'PGSHARP', N'Subscrição - PGSharp', 15);
INSERT INTO [DescriptionRules] VALUES (32, N'TRF.IMED. DE', N'Transferência Recebida', 1);
INSERT INTO [DescriptionRules] VALUES (33, N'TRF.IMED. P/', N'Transferência Enviada', 1);
INSERT INTO [DescriptionRules] VALUES (34, N'TRF MBWAY-', N'Transferência MBWay', 1);
INSERT INTO [DescriptionRules] VALUES (35, N'PAGAMENTOS VOD', N'VODAFONE - ALGARVE', 3);
INSERT INTO [DescriptionRules] VALUES (36, N'VILACOMB', N'Combustível', 8);
INSERT INTO [DescriptionRules] VALUES (37, N'BX VALOR', N'Movimento de Baixo Valor', 22);
INSERT INTO [DescriptionRules] VALUES (38, N'COBRANCA PRESTACAO', N'Prestação de Crédito RCB43', 3);
INSERT INTO [DescriptionRules] VALUES (39, N'COMISSAO COMPRAS FORA', N'Comissão Compras Estrangeiro', 22);
INSERT INTO [DescriptionRules] VALUES (40, N'3CKET', N'Compra de Bilhetes', 18);
INSERT INTO [DescriptionRules] VALUES (41, N'ALGARTALHOS', N'Supermercado - Algartalhos', 7);
INSERT INTO [DescriptionRules] VALUES (42, N'BUTCHERS', N'Restaurante - Butchers', 13);
INSERT INTO [DescriptionRules] VALUES (43, N'REALVSEGUROS', N'Seguro RCB43', 20);
INSERT INTO [DescriptionRules] VALUES (44, N'IMPOSTO SELO', N'Imposto de Selo', 22);
INSERT INTO [DescriptionRules] VALUES (45, N'FIDELIDADE COMPANHI', N'Seguro RCB43', 20);
INSERT INTO [DescriptionRules] VALUES (46, N'COMPRAS C.DEB UBER', N'Entrega - Uber Eats', 14);
INSERT INTO [DescriptionRules] VALUES (47, N'COMPRAS C.DEB', N'Compras', 32);
INSERT INTO [DescriptionRules] VALUES (70, N'REEMBOLSOS IRS', N'Reembolso IRS', 26);
INSERT INTO [DescriptionRules] VALUES (71, N'EISNT', N'Pagamento Formação', 11);
INSERT INTO [DescriptionRules] VALUES (72, N'ATOS IT SOLUTIONS', N'Recibo Vencimento', 25);
INSERT INTO [DescriptionRules] VALUES (73, N'PAGAMENTO DE ISSMD', N'Imposto de Selo', 22);
GO

-- Tabela: MbwayRules
CREATE TABLE [MbwayRules] (
[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
[PhoneNumberSuffix] NVARCHAR(MAX) NOT NULL,
[ContactName] NVARCHAR(MAX) NOT NULL,
[CategoryId] INT NOT NULL
);
GO

-- Tabela: Transactions
CREATE TABLE [Transactions] (
[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
[MovementDate] NVARCHAR(MAX) NOT NULL,
[ValueDate] NVARCHAR(MAX) NOT NULL,
[OriginalDescription] NVARCHAR(MAX) NOT NULL,
[Amount] NVARCHAR(MAX) NOT NULL,
[Bank] NVARCHAR(MAX) NOT NULL,
[CategoryId] INT,
[CleanDescription] NVARCHAR(MAX),
[Hash] NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO [Transactions] VALUES (2007, N'2025-05-01 00:00:00', N'2025-04-29 00:00:00', N'COMPRAS C.DEB NETFLIX 1600349004', N'-17.99', N'Caixa Geral de Depósitos', 15.0, N'Subscrição - Netflix', N'de513b479e6efd0a7b7b6a02af0e2b50e7103eac11604209bc89bb95bc5ec6aa');
INSERT INTO [Transactions] VALUES (2008, N'2025-05-01 00:00:00', N'2025-04-29 00:00:00', N'COMPRAS C.DEB TASKINH 1600338041', N'-22.6', N'Caixa Geral de Depósitos', 32.0, N'Compras', N'42707508519af529db6e7902f68bfd2da8b69990583d4abf54085bd3737aced7');
INSERT INTO [Transactions] VALUES (2009, N'2025-05-02 00:00:00', N'2025-04-29 00:00:00', N'COMPRA OPENAI CHATGP 1600285151', N'-21.72', N'Caixa Geral de Depósitos', 15.0, N'Subscrição - OpenAI/ChatGPT', N'0b6c45f21274aef210637017a9f3ef5dceeb6c576c4879f7393cd4f15b0cee87');
INSERT INTO [Transactions] VALUES (2010, N'2025-05-02 00:00:00', N'2025-04-29 00:00:00', N'IMPOSTO SELO S COMISS', N'-0.03', N'Caixa Geral de Depósitos', 22.0, N'Imposto de Selo', N'a3ed8a2394fd8b87fa84742354689b6b5dddab5f5d8d56274706d456c9e211bd');
INSERT INTO [Transactions] VALUES (2011, N'2025-05-02 00:00:00', N'2025-04-29 00:00:00', N'COMISSAO COMPRAS FORA', N'-0.65', N'Caixa Geral de Depósitos', 22.0, N'Comissão Compras Estrangeiro', N'c5fe133589e3b58bfc32ae442354245d10d8947b5b2241453f36950113db5a83');
INSERT INTO [Transactions] VALUES (2012, N'2025-05-02 00:00:00', N'2025-05-02 00:00:00', N'REALVSEGUROS', N'-10.18', N'Caixa Geral de Depósitos', 20.0, N'Seguro RCB43', N'87b26354f09cbf1f9d88000c44ca5cc0af0a9732bfbd4075b24237148c19bf01');
INSERT INTO [Transactions] VALUES (2013, N'2025-05-02 00:00:00', N'2025-05-02 00:00:00', N'GYMNASIUM', N'-34.9', N'Caixa Geral de Depósitos', 9.0, N'Ginásio', N'588edbc2fc48de9b3bc17842ad2db733b4021414adf7d27b5bd58ca6fbef5417');
INSERT INTO [Transactions] VALUES (2014, N'2025-05-02 00:00:00', N'2025-05-02 00:00:00', N'COMPRA PARFOIS SA 0000235180', N'-26.08', N'Caixa Geral de Depósitos', 12.0, N'Acessórios - Parfois', N'cd8bedb0b00434d81eb9241920386b57c79a095d769b8e938ff82bd7e81a6acf');
INSERT INTO [Transactions] VALUES (2015, N'2025-05-03 00:00:00', N'2025-05-02 00:00:00', N'C CLASSIC 0160492749', N'-41.23', N'Caixa Geral de Depósitos', 6.0, N'Cartão de Crédito', N'19862d873fb31364d601db32bf894b6d4e987bcb268731fe396f8e9dd3eb818a');
INSERT INTO [Transactions] VALUES (2016, N'2025-05-05 00:00:00', N'2025-05-01 00:00:00', N'COMPRAS C.DEB UBER 1601396194', N'-28.59', N'Caixa Geral de Depósitos', 14.0, N'Entrega - Uber Eats', N'3671cf261f3993d45e7125bef675cb93609094b025819049d3b03455810e859d');
INSERT INTO [Transactions] VALUES (2017, N'2025-05-05 00:00:00', N'2025-05-05 00:00:00', N'COBRANCA PRESTACAO 9376504975', N'-615.57', N'Caixa Geral de Depósitos', 3.0, N'Prestação de Crédito RCB43', N'db98e5e466a3b74eff1fcfec3c9fbafb1ce702ccbfe790a6d25b87603ce6573d');
INSERT INTO [Transactions] VALUES (2018, N'2025-05-05 00:00:00', N'2025-05-05 00:00:00', N'FIDELIDADE COMPANHI', N'-12.88', N'Caixa Geral de Depósitos', 20.0, N'Seguro RCB43', N'9d7ea28077eeec303da4c702f239b49ec8bac015f261153def5b323aa6490652');
INSERT INTO [Transactions] VALUES (2019, N'2025-05-05 00:00:00', N'2025-05-05 00:00:00', N'SANT CONSUMER PT', N'-56.25', N'Caixa Geral de Depósitos', 6.0, N'Crédito Santander', N'6f333b09dd775eb1de80e29bac98d2c6c50a857780387b55cae96c4a52ec386b');
INSERT INTO [Transactions] VALUES (2020, N'2025-05-05 00:00:00', N'2025-05-05 00:00:00', N'MONTEPIO CREDITO IN', N'-187.06', N'Caixa Geral de Depósitos', 6.0, N'Crédito Carro Montepio', N'1359bf798dfc990f9f52a0553541e313edcd1f061b4eb7de710d82e3c0415d8f');
INSERT INTO [Transactions] VALUES (2021, N'2025-05-07 00:00:00', N'2025-05-06 00:00:00', N'BX VALOR 03-TRANSACCO', N'-21.69', N'Caixa Geral de Depósitos', 22.0, N'Movimento de Baixo Valor', N'ca13fb507a6d15eefbd61eeb1a30816230bff22601519bd30563bf2a99659063');
INSERT INTO [Transactions] VALUES (2022, N'2025-05-08 00:00:00', N'2025-05-07 00:00:00', N'BX VALOR 03-TRANSACCO', N'-6.6', N'Caixa Geral de Depósitos', 22.0, N'Movimento de Baixo Valor', N'6d46bc8c464a42b436b8b0b7c69a39ddfd9bf776a8956a470050dc51033db101');
INSERT INTO [Transactions] VALUES (2023, N'2025-05-13 00:00:00', N'2025-05-13 00:00:00', N'TRF REEMBOLSOS IRS', N'2029.29', N'Caixa Geral de Depósitos', 26.0, N'Reembolso IRS', N'42a03c52cac75c425ab6c7d01e877f8816b285cbfec962f168edeea4ba6e05c0');
INSERT INTO [Transactions] VALUES (2024, N'2025-05-14 00:00:00', N'2025-05-14 00:00:00', N'COMPRA PAGAMENTOS VOD 0000235180', N'-46.16', N'Caixa Geral de Depósitos', 3.0, N'VODAFONE - ALGARVE', N'660acb8dcc0be65ffa753250e09a672199de6700cfd1a207b40735e7e39f87be');
INSERT INTO [Transactions] VALUES (2025, N'2025-05-15 00:00:00', N'2025-05-13 00:00:00', N'COMPRAS C.DEB SPOTIFY 1606048955', N'-7.99', N'Caixa Geral de Depósitos', 15.0, N'Subscrição - Spotify', N'f21d63a983093076f2fbdffa30f902c43099f44c9c11430aefba986f32faa2df');
INSERT INTO [Transactions] VALUES (2026, N'2025-05-15 00:00:00', N'2025-05-14 00:00:00', N'BX VALOR 03-TRANSACCO', N'-14.94', N'Caixa Geral de Depósitos', 22.0, N'Movimento de Baixo Valor', N'c9ce8c24e5084b4a18b1b79a0e69f297d24aa000714ecc544b4f179ac62a38b7');
INSERT INTO [Transactions] VALUES (2027, N'2025-05-17 00:00:00', N'2025-05-17 00:00:00', N'COMPRA LIDL AGRADECE 0000235180', N'-10.37', N'Caixa Geral de Depósitos', 7.0, N'Supermercado - Lidl', N'7c4d78b0fa8876a4713e97f4cf45498a5d2e8789fc03a929e1fea3be29f5da42');
INSERT INTO [Transactions] VALUES (2028, N'2025-05-18 00:00:00', N'2025-05-18 00:00:00', N'COMPRA BUTCHERS 0000235180', N'-263.7', N'Caixa Geral de Depósitos', 13.0, N'Restaurante - Butchers', N'805a03648f2b17c60402b70ba2788a9cd25d775b2753406d1acfc2e768a18dca');
INSERT INTO [Transactions] VALUES (2029, N'2025-05-19 00:00:00', N'2025-05-19 00:00:00', N'C CLASSIC', N'-972.68', N'Caixa Geral de Depósitos', 6.0, N'Cartão de Crédito', N'36051606198b9cb055b63268be6299ff99d599dabe3ef2415ffbf8ebb53059be');
INSERT INTO [Transactions] VALUES (2030, N'2025-05-19 00:00:00', N'2025-05-19 00:00:00', N'TRF EISNT ENG INF E S', N'90.0', N'Caixa Geral de Depósitos', 11.0, N'Pagamento Formação', N'0ca02839b006c2ea425340929c964281aa5b0b53087987ceb49dea494f5ac359');
INSERT INTO [Transactions] VALUES (2031, N'2025-05-20 00:00:00', N'2025-05-20 00:00:00', N'COMPRA EDP COMERCIAL 0000235180', N'-88.27', N'Caixa Geral de Depósitos', 3.0, N'Fatura EDP RCB43', N'4e04f6986619f365f5357bfd3635150f475b18d178b481ed36719f9c4ac4e58f');
INSERT INTO [Transactions] VALUES (2032, N'2025-05-20 00:00:00', N'2025-05-20 00:00:00', N'TRF MBWAY 964XXX072 0000235180', N'-65.0', N'Caixa Geral de Depósitos', NULL, N'TRF MBWAY 964XXX072 0000235180', N'b4ea45f8c06c170cafe06b946d4a027647fe40722960f7a45040ac840e6fbf80');
INSERT INTO [Transactions] VALUES (2033, N'2025-05-22 00:00:00', N'2025-05-21 00:00:00', N'BX VALOR 03-TRANSACCO', N'-74.65', N'Caixa Geral de Depósitos', 22.0, N'Movimento de Baixo Valor', N'05bc24be55b7918f4d7407b9eb7abd74c203eff678e71b965e19e0cffa6fe289');
INSERT INTO [Transactions] VALUES (2034, N'2025-05-25 00:00:00', N'2025-05-25 00:00:00', N'COMPRA ALGARTALHOS SU 0000235180', N'-8.73', N'Caixa Geral de Depósitos', 7.0, N'Supermercado - Algartalhos', N'3811e267ff1f724dee7c41924bdef42eaf47ef44d960e23adcbe89451a3307b3');
INSERT INTO [Transactions] VALUES (2035, N'2025-05-26 00:00:00', N'2025-05-26 00:00:00', N'COMPRA LIDL LOULE CAL 0000235180', N'-18.47', N'Caixa Geral de Depósitos', 7.0, N'Supermercado - Lidl', N'c6669be1e78af5d3f6487cfebfbeedea9f9c09b2286b2c83acd347baa483af51');
INSERT INTO [Transactions] VALUES (2036, N'2025-05-27 00:00:00', N'2025-05-25 00:00:00', N'COMPRAS C.DEB GLOVO 1610947672', N'-19.11', N'Caixa Geral de Depósitos', 14.0, N'Entrega - Glovo', N'0bc9723321eee693798a2fa92909f5346adcc8bfc8d0c10f060a5a173df7a964');
INSERT INTO [Transactions] VALUES (2037, N'2025-05-27 00:00:00', N'2025-05-27 00:00:00', N'COMPRA LIDL AGRADECE 0000235180', N'-10.54', N'Caixa Geral de Depósitos', 7.0, N'Supermercado - Lidl', N'e15132891f34b786a5b65e48efa363f9b8b1a2555868ab9dace9526ff0ce33a6');
INSERT INTO [Transactions] VALUES (2038, N'2025-05-27 00:00:00', N'2025-05-27 00:00:00', N'Trf Mbway 965XXX085', N'-60.0', N'Caixa Geral de Depósitos', NULL, N'Trf Mbway 965XXX085', N'054780d995b05387a6126671f602c7b4b8e5411d16e728028d48f2dc95b61c60');
INSERT INTO [Transactions] VALUES (2039, N'2025-05-27 00:00:00', N'2025-05-27 00:00:00', N'COMPRA CONTINENTE MOD 0000235180', N'-8.6', N'Caixa Geral de Depósitos', 7.0, N'Supermercado - Continente', N'fcfac3e7b0786d0dd394028a3bad01e509250f02a12dccc9ecb4d232b1417b8b');
INSERT INTO [Transactions] VALUES (2040, N'2025-05-27 00:00:00', N'2025-05-27 00:00:00', N'COMPRA VILACOMB LDA 0000235180', N'-41.8', N'Caixa Geral de Depósitos', 8.0, N'Combustível', N'ca9f219cbce57d888fbb389e625f182a9b75bb8fb2daf0908ebe710c9321a893');
INSERT INTO [Transactions] VALUES (2041, N'2025-05-28 00:00:00', N'2025-05-25 00:00:00', N'COMPRAS C.DEB GOOGLE 1610915620', N'-1.99', N'Caixa Geral de Depósitos', 15.0, N'Serviços Google', N'fd344a85d772d7d57efed751c30b2218d5043464d8f018a9e89522052d81cbe7');
INSERT INTO [Transactions] VALUES (2042, N'2025-05-28 00:00:00', N'2025-05-28 00:00:00', N'TRF ATOS IT SOLUTIONS', N'1158.72', N'Caixa Geral de Depósitos', 25.0, N'Recibo Vencimento', N'fef591ce6c174a9c454e9cb516801dd38c6654a78ebf07ba32cd840fc1d8cb73');
INSERT INTO [Transactions] VALUES (2043, N'2025-05-28 00:00:00', N'2025-05-28 00:00:00', N'Trf Mbway 910XXX194', N'-25.0', N'Caixa Geral de Depósitos', NULL, N'Trf Mbway 910XXX194', N'71452edfd7ab398ba20833db01dd533dd651e7cdd94b35a202f0babae13b5b50');
INSERT INTO [Transactions] VALUES (2044, N'2025-05-29 00:00:00', N'2025-05-29 00:00:00', N'COMPRA 3CKET 0000235180', N'-10.0', N'Caixa Geral de Depósitos', 18.0, N'Compra de Bilhetes', N'2b71cb0b3eae5734d2f6152cbc5ddc63ad8c3359a1dfdf3c88875d923609b3e5');
INSERT INTO [Transactions] VALUES (2045, N'2025-05-29 00:00:00', N'2025-05-29 00:00:00', N'COMPRA 3CKET 0000235180', N'-10.0', N'Caixa Geral de Depósitos', 18.0, N'Compra de Bilhetes', N'2b71cb0b3eae5734d2f6152cbc5ddc63ad8c3359a1dfdf3c88875d923609b3e5');
INSERT INTO [Transactions] VALUES (2046, N'2025-05-29 00:00:00', N'2025-05-28 00:00:00', N'COMPRAS C.DEB NETFLIX 1611784246', N'-17.99', N'Caixa Geral de Depósitos', 15.0, N'Subscrição - Netflix', N'e70ea0d10f9d4b80e985b8b03d9a2dd44f294cf0aec9aceb6db8aa140a8d6581');
INSERT INTO [Transactions] VALUES (2047, N'2025-05-29 00:00:00', N'2025-05-29 00:00:00', N'MEO SA', N'-79.59', N'Caixa Geral de Depósitos', 3.0, N'Fatura MEO RCB43', N'1ad061b55888496aa3f3289708fcd918586e7dc60d6d6aad09213eef0392c3dc');
INSERT INTO [Transactions] VALUES (2048, N'2025-05-29 00:00:00', N'2025-05-29 00:00:00', N'Trf Mbway 965XXX947', N'-14.0', N'Caixa Geral de Depósitos', NULL, N'Trf Mbway 965XXX947', N'1b20b6cd60a9a947f12ce9bcd46082486b0dcf5a5fec5a2e2c6ba9edb8331f30');
INSERT INTO [Transactions] VALUES (2049, N'2025-05-30 00:00:00', N'2025-05-30 00:00:00', N'Trf Mbway 965XXX947', N'-7.0', N'Caixa Geral de Depósitos', NULL, N'Trf Mbway 965XXX947', N'187ad5042d059048c237767a5ad6a6a179e71cede0540c8ca25c21bc7361ffb1');
GO

-- Tabela: Contacts
CREATE TABLE [Contacts] (
[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
[Name] NVARCHAR(MAX) NOT NULL,
[PhoneNumber] NVARCHAR(MAX) NOT NULL
);
GO

INSERT INTO [Contacts] VALUES (354, N'Guilherme Fonseca MCV UAlg', N'913289107');
INSERT INTO [Contacts] VALUES (355, N'Madalena Cinca', N'918267693');
INSERT INTO [Contacts] VALUES (356, N'João Pacheco', N'926185860');
INSERT INTO [Contacts] VALUES (357, N'Joana Marques', N'910704178');
INSERT INTO [Contacts] VALUES (358, N'Ana Beatriz Calado', N'915539071');
INSERT INTO [Contacts] VALUES (359, N'Ana Carolina Assis', N'926239611');
INSERT INTO [Contacts] VALUES (360, N'Ashley Pinto', N'960441449');
INSERT INTO [Contacts] VALUES (361, N'Avó Gracinda', N'966082148');
INSERT INTO [Contacts] VALUES (362, N'Avó Luísa', N'969942472');
INSERT INTO [Contacts] VALUES (363, N'Avô Manuel', N'968528854');
INSERT INTO [Contacts] VALUES (364, N'Beatriz Adagas', N'927090387');
INSERT INTO [Contacts] VALUES (365, N'Beatriz Gonçalves', N'963629424');
INSERT INTO [Contacts] VALUES (366, N'Bernardo Marques', N'939510672');
INSERT INTO [Contacts] VALUES (367, N'Afonso Alves', N'910635194');
INSERT INTO [Contacts] VALUES (368, N'Beatriz Girão', N'964234681');
INSERT INTO [Contacts] VALUES (369, N'Afilhado Bernardo 🎓', N'917554360');
INSERT INTO [Contacts] VALUES (370, N'Afilhado João 🎓', N'938246902');
INSERT INTO [Contacts] VALUES (371, N'João Raposo', N'926782297');
INSERT INTO [Contacts] VALUES (372, N'Afilhado Pedro 🎓', N'914865965');
INSERT INTO [Contacts] VALUES (373, N'Carolina Vinagre', N'925400554');
INSERT INTO [Contacts] VALUES (374, N'Casa', N'268628244');
INSERT INTO [Contacts] VALUES (375, N'Casa Avós Portalegre', N'245082075');
INSERT INTO [Contacts] VALUES (376, N'D. Maria Emília', N'965628089');
INSERT INTO [Contacts] VALUES (377, N'Daniel Mendonça', N'935369500');
INSERT INTO [Contacts] VALUES (378, N'David Ferreira', N'925215933');
INSERT INTO [Contacts] VALUES (379, N'Diogo Meireles', N'965172254');
INSERT INTO [Contacts] VALUES (380, N'Dona Preciosa', N'969322791');
INSERT INTO [Contacts] VALUES (381, N'Dona Rosalina', N'960255461');
INSERT INTO [Contacts] VALUES (382, N'Eduardo Medeiros', N'927182257');
INSERT INTO [Contacts] VALUES (383, N'Elisa Cipriano', N'965007073');
INSERT INTO [Contacts] VALUES (384, N'Esther Gutierrez Sosa', N'662114267');
INSERT INTO [Contacts] VALUES (385, N'Fernanda Louro', N'968091903');
INSERT INTO [Contacts] VALUES (386, N'Filipa Godinho', N'918948342');
INSERT INTO [Contacts] VALUES (387, N'Francisco Figueira', N'932589075');
INSERT INTO [Contacts] VALUES (388, N'Francisco Rodrigues', N'925188832');
INSERT INTO [Contacts] VALUES (389, N'Gonçalo Barradas 🍻', N'961416646');
INSERT INTO [Contacts] VALUES (390, N'Gonçalo Jesus', N'962517912');
INSERT INTO [Contacts] VALUES (391, N'Guilherme Guedes', N'925589509');
INSERT INTO [Contacts] VALUES (392, N'Hospital', N'268637200');
INSERT INTO [Contacts] VALUES (393, N'Hélder Meira', N'917622708');
INSERT INTO [Contacts] VALUES (394, N'Ioana Bodnar', N'914098512');
INSERT INTO [Contacts] VALUES (395, N'Isabel Rato', N'967819125');
INSERT INTO [Contacts] VALUES (396, N'João Galvão', N'913447707');
INSERT INTO [Contacts] VALUES (397, N'João Marono', N'968193346');
INSERT INTO [Contacts] VALUES (398, N'João Matos', N'926609223');
INSERT INTO [Contacts] VALUES (399, N'João Piteira', N'926231126');
INSERT INTO [Contacts] VALUES (400, N'João Rento', N'968359051');
INSERT INTO [Contacts] VALUES (401, N'Laura Cambóias', N'966717831');
INSERT INTO [Contacts] VALUES (402, N'Luigy Marques', N'965166069');
INSERT INTO [Contacts] VALUES (403, N'Luís Pereira', N'927265631');
INSERT INTO [Contacts] VALUES (404, N'Luís Roma', N'961882550');
INSERT INTO [Contacts] VALUES (405, N'Marcos O Grande', N'964615014');
INSERT INTO [Contacts] VALUES (406, N'Maria AfiMadri', N'968005481');
INSERT INTO [Contacts] VALUES (407, N'Maxi Pizza Évora', N'965638331');
INSERT INTO [Contacts] VALUES (408, N'Miguel Beicinha', N'967224472');
INSERT INTO [Contacts] VALUES (409, N'Notável do Ano', N'965241947');
INSERT INTO [Contacts] VALUES (410, N'Miguel Ginga', N'934516677');
INSERT INTO [Contacts] VALUES (411, N'Miguel Luís CN', N'969844905');
INSERT INTO [Contacts] VALUES (412, N'Mãe', N'964395072');
INSERT INTO [Contacts] VALUES (413, N'Müco 🇹🇷', N'544171021');
INSERT INTO [Contacts] VALUES (414, N'Nuno Cerdeira', N'915929478');
INSERT INTO [Contacts] VALUES (415, N'Pai', N'966884770');
INSERT INTO [Contacts] VALUES (416, N'Pedro Ginga', N'927000397');
INSERT INTO [Contacts] VALUES (417, N'Pedro Louro', N'927816308');
INSERT INTO [Contacts] VALUES (418, N'Ricardo Ferreira', N'965056882');
INSERT INTO [Contacts] VALUES (419, N'Ricardo Oliveira', N'911873212');
INSERT INTO [Contacts] VALUES (420, N'Rita Cambóias', N'967137758');
INSERT INTO [Contacts] VALUES (421, N'Rodrigo Alexandre', N'965311063');
INSERT INTO [Contacts] VALUES (422, N'Rui Lagarto', N'925743194');
INSERT INTO [Contacts] VALUES (423, N'Rui Santos', N'965051474');
INSERT INTO [Contacts] VALUES (424, N'Rúben Teimas', N'935629857');
INSERT INTO [Contacts] VALUES (425, N'Sara Lagarto', N'915377316');
INSERT INTO [Contacts] VALUES (426, N'Sofia Lopes', N'967084402');
INSERT INTO [Contacts] VALUES (427, N'Olga Carvalho', N'963188202');
INSERT INTO [Contacts] VALUES (428, N'Tia Paula', N'918462438');
INSERT INTO [Contacts] VALUES (429, N'Miguel Renda', N'916061443');
INSERT INTO [Contacts] VALUES (430, N'Teresa Machado', N'967430293');
INSERT INTO [Contacts] VALUES (431, N'Gonçalo Barriga', N'926167515');
INSERT INTO [Contacts] VALUES (432, N'José Morais', N'964719568');
INSERT INTO [Contacts] VALUES (433, N'Nádia Medeiros', N'939471266');
INSERT INTO [Contacts] VALUES (434, N'Tiago Alves', N'962586135');
INSERT INTO [Contacts] VALUES (435, N'Igor Almeida', N'961784544');
INSERT INTO [Contacts] VALUES (436, N'Filipe Rodrigues', N'927342813');
INSERT INTO [Contacts] VALUES (437, N'Tiago Baião', N'925317592');
INSERT INTO [Contacts] VALUES (438, N'Pedro Geraldes', N'915800255');
INSERT INTO [Contacts] VALUES (439, N'Ivan Soares', N'926688540');
INSERT INTO [Contacts] VALUES (440, N'Beatriz Grilo', N'925940437');
INSERT INTO [Contacts] VALUES (441, N'Dr Guilhermina Siquenique', N'961333770');
INSERT INTO [Contacts] VALUES (442, N'Luis Rosmaninho Fotógr%', N'919372289');
INSERT INTO [Contacts] VALUES (443, N'Carolina Barros', N'916209496');
INSERT INTO [Contacts] VALUES (444, N'Ethel D''Assa Castel-Branco', N'969772324');
INSERT INTO [Contacts] VALUES (445, N'Joana Ciríaco', N'967927306');
INSERT INTO [Contacts] VALUES (446, N'Mariana Franco', N'913262232');
INSERT INTO [Contacts] VALUES (447, N'Manuela Reis', N'911583030');
INSERT INTO [Contacts] VALUES (448, N'Maria Ana Gonçalves', N'939891479');
INSERT INTO [Contacts] VALUES (449, N'Gabriela Silva', N'926278440');
INSERT INTO [Contacts] VALUES (450, N'Iuri Neto', N'917472587');
INSERT INTO [Contacts] VALUES (451, N'Filipe Coelho', N'932069491');
INSERT INTO [Contacts] VALUES (452, N'Emanuel Carboila', N'968260460');
INSERT INTO [Contacts] VALUES (453, N'Mariana Antunes BH', N'969233948');
INSERT INTO [Contacts] VALUES (454, N'Zé Costa', N'927126291');
INSERT INTO [Contacts] VALUES (455, N'Tio Rui', N'913183560');
INSERT INTO [Contacts] VALUES (456, N'Tomás Santos', N'967115229');
INSERT INTO [Contacts] VALUES (457, N'Vincent Van Acht', N'937968823');
INSERT INTO [Contacts] VALUES (458, N'Voicemail', N'962000000');
INSERT INTO [Contacts] VALUES (459, N'Zé Loureiro', N'966786956');
INSERT INTO [Contacts] VALUES (460, N'Tia Sónia', N'966336817');
INSERT INTO [Contacts] VALUES (461, N'Gonçalo Guedes', N'967100476');
INSERT INTO [Contacts] VALUES (462, N'Diana Guerra BioTec', N'968091339');
INSERT INTO [Contacts] VALUES (463, N'Didizinha', N'968620774');
INSERT INTO [Contacts] VALUES (464, N'Daniel Silvério', N'968946950');
INSERT INTO [Contacts] VALUES (465, N'João Miguel USA', N'258923516');
INSERT INTO [Contacts] VALUES (466, N'Gaspar Nascimento', N'960258762');
INSERT INTO [Contacts] VALUES (467, N'Beatriz Vizeu', N'969660544');
INSERT INTO [Contacts] VALUES (468, N'Lucas Albuquerque', N'914165321');
INSERT INTO [Contacts] VALUES (469, N'Flavia Lima', N'913477665');
INSERT INTO [Contacts] VALUES (470, N'Senhor Sérgio Faísca', N'969845228');
INSERT INTO [Contacts] VALUES (471, N'Sr Zé Mocidade', N'962319033');
INSERT INTO [Contacts] VALUES (472, N'Dr Carlos Espiga', N'965403367');
INSERT INTO [Contacts] VALUES (473, N'Miguel Luis Africa', N'705048857');
INSERT INTO [Contacts] VALUES (474, N'Rita Cruz', N'912818905');
INSERT INTO [Contacts] VALUES (475, N'Ana Timóteo', N'925215252');
INSERT INTO [Contacts] VALUES (476, N'Sr Paulo Talha Velha', N'964689696');
INSERT INTO [Contacts] VALUES (477, N'Luís Coradinho', N'961619045');
INSERT INTO [Contacts] VALUES (478, N'João Barreto', N'966330965');
INSERT INTO [Contacts] VALUES (479, N'Ana Rita Silva', N'965410085');
INSERT INTO [Contacts] VALUES (480, N'Paulinha', N'965537980');
INSERT INTO [Contacts] VALUES (481, N'Iva Russo', N'969955882');
INSERT INTO [Contacts] VALUES (482, N'Gilberto', N'964043483');
INSERT INTO [Contacts] VALUES (483, N'Tiago Capaz CC', N'913716027');
INSERT INTO [Contacts] VALUES (484, N'D Lara Gonzalez', N'910126845');
INSERT INTO [Contacts] VALUES (485, N'D Celeste Guerreiro', N'934909731');
INSERT INTO [Contacts] VALUES (486, N'Sr Luís Guerreiro', N'925370652');
INSERT INTO [Contacts] VALUES (487, N'Isabel CGD', N'266989252');
INSERT INTO [Contacts] VALUES (488, N'Sofia Carrapeta', N'969418006');
INSERT INTO [Contacts] VALUES (489, N'Gonçalo Almeida UA', N'963689946');
INSERT INTO [Contacts] VALUES (490, N'Hélder Neto', N'919582930');
INSERT INTO [Contacts] VALUES (491, N'Joel Rodrigues', N'911138699');
INSERT INTO [Contacts] VALUES (492, N'Carlos Pinto', N'937090955');
INSERT INTO [Contacts] VALUES (493, N'João Pais', N'926007257');
INSERT INTO [Contacts] VALUES (494, N'João Lopes', N'913757748');
INSERT INTO [Contacts] VALUES (495, N'Camila Assis Campanharo', N'999986245');
INSERT INTO [Contacts] VALUES (496, N'Sr. Chico Div Com', N'966705514');
INSERT INTO [Contacts] VALUES (497, N'Tomás Ornelas UM', N'967327639');
INSERT INTO [Contacts] VALUES (498, N'Eng Joaquim Godinho', N'969845232');
INSERT INTO [Contacts] VALUES (499, N'Rafael Mouta', N'967845191');
INSERT INTO [Contacts] VALUES (500, N'Rosa Fernandes', N'914539222');
INSERT INTO [Contacts] VALUES (501, N'Max Leiria', N'912333559');
INSERT INTO [Contacts] VALUES (502, N'André Canha', N'960179854');
INSERT INTO [Contacts] VALUES (503, N'João Neto', N'914144320');
INSERT INTO [Contacts] VALUES (504, N'Mário Ascenção', N'938376897');
INSERT INTO [Contacts] VALUES (505, N'Cyril Covão Coelho', N'918245459');
INSERT INTO [Contacts] VALUES (506, N'Teresa Pinto', N'927706220');
INSERT INTO [Contacts] VALUES (507, N'Dona Filomena Fernandes SAC', N'933720355');
INSERT INTO [Contacts] VALUES (508, N'Duarte Nuno', N'965553465');
INSERT INTO [Contacts] VALUES (509, N'Alex Pinto', N'255371404');
INSERT INTO [Contacts] VALUES (510, N'Telmo Maia', N'964725923');
INSERT INTO [Contacts] VALUES (511, N'Rodrigo Raziel', N'915932649');
INSERT INTO [Contacts] VALUES (512, N'Pedro Neves', N'966051923');
INSERT INTO [Contacts] VALUES (513, N'Ricardo Jara', N'924469166');
INSERT INTO [Contacts] VALUES (514, N'Rui Valente UTAD', N'911942356');
INSERT INTO [Contacts] VALUES (515, N'Gonçalo Gomes "O Preto%', N'919426051');
INSERT INTO [Contacts] VALUES (516, N'João Martins', N'968421915');
INSERT INTO [Contacts] VALUES (517, N'Tatiana Vieira', N'918622542');
INSERT INTO [Contacts] VALUES (518, N'Beatriz Capitão', N'968495137');
INSERT INTO [Contacts] VALUES (519, N'AAAMiguel Carvalho', N'965061100');
INSERT INTO [Contacts] VALUES (520, N'Yaroslav Kolodiy', N'968385056');
INSERT INTO [Contacts] VALUES (521, N'Carlos Pinto', N'144474056');
INSERT INTO [Contacts] VALUES (522, N'Henrique Nunes', N'969963060');
INSERT INTO [Contacts] VALUES (523, N'Catarina Maia', N'963149810');
INSERT INTO [Contacts] VALUES (524, N'João Santos EI', N'919917280');
INSERT INTO [Contacts] VALUES (525, N'Tio Daniel', N'961978022');
INSERT INTO [Contacts] VALUES (526, N'Filomena Grácio', N'961934601');
INSERT INTO [Contacts] VALUES (527, N'Luís Filipe Grácio', N'913946389');
INSERT INTO [Contacts] VALUES (528, N'Michael Brito', N'917443889');
INSERT INTO [Contacts] VALUES (529, N'André Grave', N'925547986');
INSERT INTO [Contacts] VALUES (530, N'Constança Braz', N'911841601');
INSERT INTO [Contacts] VALUES (531, N'João Pais Aveiro', N'915459330');
INSERT INTO [Contacts] VALUES (532, N'Cátia Carvalho', N'962294615');
INSERT INTO [Contacts] VALUES (533, N'João Duarte Jota Aveir%', N'917044695');
INSERT INTO [Contacts] VALUES (534, N'António José Grácio', N'965094990');
INSERT INTO [Contacts] VALUES (535, N'Rui Patinha Aveiro', N'913370244');
INSERT INTO [Contacts] VALUES (536, N'Diogo "Queijinho Babybel" Queijo', N'934543057');
INSERT INTO [Contacts] VALUES (537, N'Sr Carlos UE', N'967667751');
INSERT INTO [Contacts] VALUES (538, N'Pedro Pata Leiria', N'914454386');
INSERT INTO [Contacts] VALUES (539, N'Rute Santos', N'916458025');
INSERT INTO [Contacts] VALUES (540, N'Rúben Neves Leiria', N'918337376');
INSERT INTO [Contacts] VALUES (541, N'Alexandra Santos', N'969600639');
INSERT INTO [Contacts] VALUES (542, N'Vanessa Carboila', N'964023326');
INSERT INTO [Contacts] VALUES (543, N'Rafael Vitorino', N'968546495');
INSERT INTO [Contacts] VALUES (544, N'Russell Jara', N'931957122');
INSERT INTO [Contacts] VALUES (545, N'Madalena Gusmão', N'964379801');
INSERT INTO [Contacts] VALUES (546, N'Miguel "ChupaChino" Esteves', N'968460902');
INSERT INTO [Contacts] VALUES (547, N'Inês "ChupaChina" Marq%', N'965202740');
INSERT INTO [Contacts] VALUES (548, N'Dário', N'968694414');
INSERT INTO [Contacts] VALUES (549, N'João Velhinho', N'969007291');
INSERT INTO [Contacts] VALUES (550, N'Chico MCV UAlg', N'963875302');
INSERT INTO [Contacts] VALUES (551, N'Ricardo Frieza', N'913216063');
INSERT INTO [Contacts] VALUES (552, N'Jorge Baptista', N'916086494');
INSERT INTO [Contacts] VALUES (553, N'Ivandro UAlg', N'931793449');
INSERT INTO [Contacts] VALUES (554, N'Sergiy Chukh', N'964390883');
INSERT INTO [Contacts] VALUES (555, N'Quim Zé', N'961038135');
INSERT INTO [Contacts] VALUES (556, N'Daniela Santos - PoSF', N'912326717');
INSERT INTO [Contacts] VALUES (557, N'Pablo Del Rosario', N'917522168');
INSERT INTO [Contacts] VALUES (558, N'Filipa - PoSF', N'968441534');
INSERT INTO [Contacts] VALUES (559, N'Diogo Alexandre', N'965615445');
INSERT INTO [Contacts] VALUES (560, N'Rodrigo Vargues UAlg', N'914185948');
INSERT INTO [Contacts] VALUES (561, N'Ivo Mendes', N'938799137');
INSERT INTO [Contacts] VALUES (562, N'Bernardo Sampaio Aveiro', N'939689370');
INSERT INTO [Contacts] VALUES (563, N'Catarina Oliveira', N'960431811');
INSERT INTO [Contacts] VALUES (564, N'Ricardo Rosa', N'918998155');
INSERT INTO [Contacts] VALUES (565, N'D Elvira', N'964307988');
INSERT INTO [Contacts] VALUES (566, N'Rui Carriço', N'969392645');
INSERT INTO [Contacts] VALUES (567, N'Maria Nunes', N'968554050');
INSERT INTO [Contacts] VALUES (568, N'Panzer UA', N'910256196');
INSERT INTO [Contacts] VALUES (569, N'Ricardo UBI', N'916248139');
INSERT INTO [Contacts] VALUES (570, N'Dux Lusíada Albernaz P%', N'911763232');
INSERT INTO [Contacts] VALUES (571, N'Catarina Pereira', N'966173522');
INSERT INTO [Contacts] VALUES (572, N'Jorge Fontainhas', N'912821308');
INSERT INTO [Contacts] VALUES (573, N'Primo João Pedro', N'926717526');
INSERT INTO [Contacts] VALUES (574, N'Synd Barber', N'910680690');
INSERT INTO [Contacts] VALUES (575, N'Francisco Ribeiro', N'926327782');
INSERT INTO [Contacts] VALUES (576, N'Marielba Zacarias Silva', N'918645709');
INSERT INTO [Contacts] VALUES (577, N'MS João Fonseca Aveiro', N'938727589');
INSERT INTO [Contacts] VALUES (578, N'D. Aida', N'968609297');
INSERT INTO [Contacts] VALUES (579, N'Carolina Damaso - Notá%', N'966679275');
INSERT INTO [Contacts] VALUES (580, N'Miguel Duarte CN', N'912874902');
INSERT INTO [Contacts] VALUES (581, N'Dona Teresa Limpeza Quarteira', N'916517984');
INSERT INTO [Contacts] VALUES (582, N'Rita Matos Belejo', N'926920544');
INSERT INTO [Contacts] VALUES (583, N'Sines Hotel', N'269038980');
INSERT INTO [Contacts] VALUES (584, N'Pedro Leitão', N'928024956');
INSERT INTO [Contacts] VALUES (585, N'Ivan Pires', N'926956107');
INSERT INTO [Contacts] VALUES (586, N'Filipe Oliveira', N'963115230');
INSERT INTO [Contacts] VALUES (587, N'Inês Menino Porta-Nova', N'961289323');
INSERT INTO [Contacts] VALUES (588, N'Ana Caeiro', N'963782374');
INSERT INTO [Contacts] VALUES (589, N'João Tuna', N'966283082');
GO

