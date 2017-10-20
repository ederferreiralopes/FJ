create database finderjobs;

use finderjobs;

CREATE TABLE `Usuario` (
	`Id`             	 INT NOT NULL AUTO_INCREMENT,
    `Login`          	 VARCHAR (12) NOT NULL,
    `Senha`          	 VARCHAR (12) NOT NULL,
    `Tipo`           	 VARCHAR (12) NOT NULL,
    `Nome`           	 VARCHAR (50) NOT NULL,
    `Email`          	 VARCHAR (50) NOT NULL,
    `Celular`        	 VARCHAR (20) NULL,
    `CpfCnpj`        	 VARCHAR (20) NOT NULL,
    `EnderecoCep`        VARCHAR (8) NOT NULL,
	`EnderecoNumero` 	 VARCHAR (10) NULL,
    `DataCadastro`   	 VARCHAR (20) NOT NULL,    
    `Habilidades`    	 VARCHAR(300) NULL,
	`Pago`           	 BIT          NOT NULL,
    `Anonimo`        	 BIT          NOT NULL,
	`EnderecoLogradouro` VARCHAR(140) NULL,
	`EnderecoBairro` 	 VARCHAR(140) NULL,
	`EnderecoCidade` 	 VARCHAR(140) NULL,
	`EnderecoUF`     	 CHAR(2)      NULL,
	`Ativo`          	 BIT          NOT NULL,	
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


CREATE TABLE `Candidato` (
	`Id`             INT           NOT NULL AUTO_INCREMENT,
    `UsuarioId`      INT           NOT NULL,   
    `Profissao`      VARCHAR(100)  NOT NULL,
	`Curriculo`      VARCHAR(300)  NOT NULL,
	`Habilidades`    VARCHAR(300)  NOT NULL,	
	`Ativo`          BIT           NOT NULL,	
  PRIMARY KEY (`Id`),
  FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

CREATE TABLE `Arquivo` (
	`Id`             INT           NOT NULL AUTO_INCREMENT,
    `UsuarioId`      INT           NOT NULL,   
    `Caminho`        VARCHAR(255)  NOT NULL,
	`Tipo`           VARCHAR(20)   NOT NULL,
	`Nome`           VARCHAR(120)  NOT NULL,	
	`Ativo`          BIT           NOT NULL,	
  PRIMARY KEY (`Id`),
  FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


CREATE TABLE `Habilidade` (
	`Id`             INT NOT NULL AUTO_INCREMENT,
    `Nome`           VARCHAR (50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


CREATE TABLE `Empresa` (
	`Id`             INT NOT NULL AUTO_INCREMENT,
    `Ramo`           VARCHAR (50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


CREATE TABLE `Vaga` (
	`Id`             INT           NOT NULL AUTO_INCREMENT,
    `EmpresaId`      INT           NOT NULL,
    `Descricao`      VARCHAR (100) NULL,
    `Cep`            VARCHAR (10)  NOT NULL,  
    `Habilidades`    VARCHAR(300)  NOT NULL,     
	`DataCadastro`   DATETIME      NOT NULL,
	`Ativo`          BIT           NOT NULL,	
  PRIMARY KEY (`Id`),
  FOREIGN KEY (EmpresaId) REFERENCES Usuario(Id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;



CREATE TABLE `Cedente` (
	`Id`             INT           NOT NULL AUTO_INCREMENT,
    `Codigo`         INT           NOT NULL,
    `NossoNumero`    INT           NOT NULL,
    `CpfCnpj`        VARCHAR (14)  NULL,
    `Nome`           VARCHAR (50)  NOT NULL,  
    `Agencia`        VARCHAR(10)   NOT NULL,     
	`Conta`          VARCHAR(10)   NOT NULL,     
	`DigitoConta`    VARCHAR(2)    NOT NULL,     	
	`Ativo`          BIT           NOT NULL,	
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


CREATE TABLE `ConfiguracaoBoleto` (
	`Id`                        INT           NOT NULL AUTO_INCREMENT,
    `CedenteId`               INT           NOT NULL,
	`CodigoBanco`               INT           NOT NULL,
    `Vencimento`                VARCHAR (10)  NOT NULL,    
    `ValorBoleto`               NUMERIC (18,2) NULL,
    `NumeroDocumento`           VARCHAR (50)  NOT NULL,  
    `CodigoEspecieDocumento`    VARCHAR (50)  NOT NULL,
	`Descricao`                 VARCHAR(50)  NOT NULL,  
    `CodigoCarteira`            INT          NOT NULL,  		
	`MostrarCodigoCarteira`     BIT           NOT NULL,
    `MostrarComprovanteEntrega` BIT           NOT NULL,
    `Ativo`                     BIT           NOT NULL,	
  PRIMARY KEY (`Id`),
  FOREIGN KEY (CedenteId) REFERENCES Cedente(Id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


CREATE TABLE `UsuarioHabilidade` (
	`Id`             INT           NOT NULL AUTO_INCREMENT,
    `UsuarioId`      INT           NOT NULL,   
	`HabilidadeId`   INT           NOT NULL,
  PRIMARY KEY (`Id`),
  FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id),
  FOREIGN KEY (HabilidadeId) REFERENCES Habilidade(Id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;


