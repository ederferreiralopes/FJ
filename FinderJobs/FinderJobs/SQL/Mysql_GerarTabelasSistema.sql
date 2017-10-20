create database recrutamento;

use recrutamento;
CREATE TABLE `candidato` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `DataCadastro` datetime NOT NULL,
  `Nome` longtext,
  `Cep` longtext,
  `Habilidades` longtext,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

use recrutamento;
CREATE TABLE `vaga` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `DataCadastro` datetime NOT NULL,
  `Empresa` longtext,
  `Descricao` longtext,
  `Cep` longtext,
  `Habilidades` longtext,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

use recrutamento;
CREATE TABLE `usuario` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Login` longtext,
  `Senha` longtext,
  `Email` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
