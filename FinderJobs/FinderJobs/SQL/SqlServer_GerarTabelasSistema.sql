CREATE DATABASE finderjobs

GO

USE [finderjobs]

GO

CREATE TABLE [dbo].[Candidato] (
    [Id] INT IDENTITY,
	[IdUsuario] INT NULL,
    [Habilidades] NVARCHAR (700) NULL,
    [Profissao]   NVARCHAR (50)  NULL,
    [Curriculo]   NVARCHAR (50)  NULL
);


GO

CREATE TABLE [dbo].[Habilidade] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [Nome] VARCHAR (40) NULL
);

GO

CREATE TABLE [dbo].[Usuario] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Login]        NVARCHAR (12) NULL,
    [Senha]        NVARCHAR (12) NULL,
    [Tipo]         NVARCHAR (12) NULL,
    [Nome]         NVARCHAR (50) NULL,
    [Email]        NVARCHAR (50) NULL,
    [Celular]      NVARCHAR (20) NULL,
    [CpfCnpj]       NVARCHAR (20) NULL,
    [Cep]          NVARCHAR (10) NULL,
	[EnderecoNumero] NVARCHAR (10) NULL,
    [DataCadastro] NVARCHAR (20) NULL,
    [Pago]         BIT           NULL,
    [Anonimo]      BIT           NULL,
    [Habilidades] VARCHAR(300) NULL, 
    [CaminhoArquivo] VARCHAR(240) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Vaga] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,		
	[IdEmpresa]    INT NOT NULL constraint fk_vaga_empresa foreign key references usuario(Id),
    [DataCadastro] DATETIME       NOT NULL,
    [Empresa]      NVARCHAR (MAX) NULL,
    [Descricao]    NVARCHAR (MAX) NULL,
    [Cep]          NVARCHAR (MAX) NULL,
    [Habilidades]  NVARCHAR (MAX) NULL
);

GO

CREATE TABLE [dbo].[Empresa] (
    [id] INT IDENTITY,
    [Ramo] NVARCHAR (50) NULL
);

CREATE TABLE [dbo].[ConfiguracaoBoleto] (
    [Id]                        INT             NOT NULL,
    [CodigoBanco]               INT             NOT NULL,
    [Vencimento]                VARCHAR (10)    NOT NULL,
    [ValorBoleto]               NUMERIC (18, 2) NOT NULL,
    [NumeroDocumento]           VARCHAR (50)    NOT NULL,
    [Descricao]                 VARCHAR (50)  NOT NULL,
    [CodigoCarteira]            INT             NOT NULL,
    [CodigoEspecieDocumento]    VARCHAR (10)    NOT NULL,
    [MostrarCodigoCarteira]     BIT             NOT NULL,
    [MostrarComprovanteEntrega] BIT             NOT NULL,
    [CedenteId]                 INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Cedente](
	[Id] [int] NOT NULL,
	[Codigo] [int] NOT NULL,
	[NossoNumero] [varchar](50) NOT NULL,
	[CpfCnpj] [varchar](14) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[Agencia] [varchar](10) NOT NULL,
	[Conta] [varchar](10) NOT NULL,
	[DigitoConta] [varchar](2) NOT NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
