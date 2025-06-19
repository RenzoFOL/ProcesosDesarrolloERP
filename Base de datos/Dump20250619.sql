-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: manasysdb
-- ------------------------------------------------------
-- Server version	8.3.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `manasysdb`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `manasysdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `manasysdb`;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20250418211239_AddIsEnabledAndRoleAssignedToUsers','8.0.4'),('20250418214357_InitWithApplicationUser','8.0.4'),('20250419015958_ExtendUserWithIsEnabledAndRoleAssigned','8.0.4');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('06d3d678-c4ee-4597-936a-a190ee193bf8','Gerente','GERENTE',NULL),('0e8cd022-5218-47d3-b3b7-2b24304e1470','Empleado','EMPLEADO',NULL),('7a989a8d-5d8f-4a1a-b6a3-db66ae8161ba','Administrador','ADMINISTRADOR',NULL),('918c69cc-ca9d-4f89-b8f4-0ac0acfdb916','Admin','ADMIN',NULL),('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','SuperAdministrador','SUPERADMINISTRADOR','superadmin-concurrency-stamp-001');
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('3c4568c7-7c35-478a-a5e7-c91379f8ffe8','06d3d678-c4ee-4597-936a-a190ee193bf8'),('4362725c-5c44-4a59-8f32-a8bdfd8920ca','06d3d678-c4ee-4597-936a-a190ee193bf8'),('972b1b73-b438-4070-b5ed-0f75ebea434c','06d3d678-c4ee-4597-936a-a190ee193bf8'),('b01198cc-a7ed-4ed6-ae6e-9a2f05778e55','06d3d678-c4ee-4597-936a-a190ee193bf8'),('b01198cc-a7ed-4ed6-ae6e-9a2f05778e55','0e8cd022-5218-47d3-b3b7-2b24304e1470'),('fb3ce85b-2e2d-4180-a818-f28c3c3bc304','0e8cd022-5218-47d3-b3b7-2b24304e1470');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsEnabled` tinyint(1) NOT NULL,
  `RoleAssigned` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('2a7a0258-a3a5-4c8e-81b4-e7f70bc79bd6',1,'SuperAdministrador','renol1099@gmail.com','RENOL1099@GMAIL.COM','renol1099@gmail.com','RENOL1099@GMAIL.COM',1,'AQAAAAIAAYagAAAAEIb8UvMbAVaEV0KhRYOYD4ga10ZJ0ZVEmLmm8fsEigBMkYidxb1fB3U0MTLzzvs+6g==','GQS46OJQ54HEIRHAWT74ATICB75H3MKD','92aa80ce-9b13-4122-9145-06980fe9d9ed',NULL,0,0,NULL,1,0),('3c4568c7-7c35-478a-a5e7-c91379f8ffe8',1,'Gerente','COrreo@gmail.com','CORREO@GMAIL.COM','COrreo@gmail.com','CORREO@GMAIL.COM',1,'AQAAAAIAAYagAAAAEKcV3g+PxCLzl1eT0bGG3j28VbCc+zyQofNAKfkn47jjdR+/0rWda9wyAprPRPblgw==','4KFFW6XAV5Q3KPLUAHUGWU772PZ53EHZ','351851b5-6158-4b7b-8ec6-c24fdba68271',NULL,0,0,NULL,1,0),('4362725c-5c44-4a59-8f32-a8bdfd8920ca',1,'Gerente','deyrex1099@gmail.com','DEYREX1099@GMAIL.COM','deyrex1099@gmail.com','DEYREX1099@GMAIL.COM',1,'AQAAAAIAAYagAAAAEPaDLy1+v4jF9hB3CbX65a1HEXNwsRzO2/F4UxjB2++DQgsOnAonA9tYlJd0ciMF1Q==','RUSEXVT62FQXKRYIK4CELOQPOKIOV42O','eb4e72a7-4c90-4e95-9cb0-e9908ecd8df0',NULL,0,0,NULL,1,0),('45ec5556-ea2c-4fb4-a9c7-9f56dfa344bf',0,'Administrador','lorenaomonserrath@gmail.com','LORENAOMONSERRATH@GMAIL.COM','lorenaomonserrath@gmail.com','LORENAOMONSERRATH@GMAIL.COM',0,'AQAAAAIAAYagAAAAENMROq3ijbh+h1jzrvvzGGkD3Iig/ZVevAy0exm6EVgLIwN9JwVVvQNvMsUJ6hdAIw==','GNUF57TKMNKHL3LPVZTV4SMHQLCGSKFH','d6a50d3a-09d5-40bb-b679-32c45a89b67d',NULL,0,0,NULL,1,0),('4e983d64-2843-46b6-863d-c1dca8d201a9',0,'Administrador','su123@gmail.com','SU123@GMAIL.COM','su123@gmail.com','SU123@GMAIL.COM',0,'AQAAAAIAAYagAAAAEPzvbPlLPagH2pVbwCkLIrAeq0fOUChFXvzGkvi7RnuHuEbqFgtWd9HYwi0BFn/1Sw==','Y5HDR2FUAWVMTQ5XJYIAA7UWYKLPCJGX','eeda203a-8f0b-43bf-b612-57ad3b4f030b',NULL,0,0,NULL,1,0),('972b1b73-b438-4070-b5ed-0f75ebea434c',1,'Gerente','prueba123@gmail.com','PRUEBA123@GMAIL.COM','prueba123@gmail.com','PRUEBA123@GMAIL.COM',1,'AQAAAAIAAYagAAAAEGUccp48DETKqLSK38gUDwe2rCSbObd1K+GqWVmQrjEqjiYNWOFcbH3QK32c7J5SNw==','LN333ON2DZZQ77SAO2T4D6EIBM2RNBEJ','0881199a-684f-403a-bf72-7ff9b1e97b1b',NULL,0,0,NULL,1,0),('b01198cc-a7ed-4ed6-ae6e-9a2f05778e55',1,'Gerente','renzo.ortizlara@gmail.com','RENZO.ORTIZLARA@GMAIL.COM','renzo.ortizlara@gmail.com','RENZO.ORTIZLARA@GMAIL.COM',1,'AQAAAAIAAYagAAAAEFTQG0kDs08y75TWGXOA7QVMu0hZL3k6auj492XT5MSdNTWmLXLQl4kGpSkXC7umEg==','YRI37HGLSXZY3L3ZFEA5HZ63HOERUOZP','8cc93bef-aff5-4fc5-aca9-92d8f5acca55',NULL,0,0,NULL,1,0),('c02b1c09-7178-41d5-8f11-ddc8c6635ec4',0,'Administrador','Renzo@gmail.com','RENZO@GMAIL.COM','Renzo@gmail.com','RENZO@GMAIL.COM',0,'AQAAAAIAAYagAAAAEH+dMw4/qOJIbZE1NNdyLqI/cZv8a97t5QZgAA/xaalL3Qp6K4NrJrormYMfzl8vCQ==','2FOQIN7G7DIVBE6547H7ZER3Y7GWOXGT','44d18067-56c5-4dad-a2b9-32efbb509868',NULL,0,0,NULL,1,0),('c56ab27b-6daa-498b-a2dc-1092602b107c',0,'Administrador','listillosuv@gmail.com','LISTILLOSUV@GMAIL.COM','listillosuv@gmail.com','LISTILLOSUV@GMAIL.COM',0,'AQAAAAIAAYagAAAAECDWJ6gVlD0KP7joucSD8icByAaMCeh8mSlUYaGa9OQg/GmZQoeonloWEvtzSySy+Q==','7VDV6VPFXEBZN5JOWINDYUUHSRSVFHMN','09afb9e6-f7fc-4f40-8cf4-32cb6a87a98a',NULL,0,0,NULL,1,0),('fb3ce85b-2e2d-4180-a818-f28c3c3bc304',1,'Empleado','renolf1099@gmail.com','RENOLF1099@GMAIL.COM','renolf1099@gmail.com','RENOLF1099@GMAIL.COM',1,'AQAAAAIAAYagAAAAEFG6NIVfG3iGLPrXK1SufuBqU2mmHzu+aZkoaE+vG5LTPtSIChpa7FTq1A/q319mfQ==','LFE7MON3FGSEKN6A5PN256BPOJCTJCLC','0e0c102d-ee00-4473-96de-acfce2d856b2',NULL,0,0,NULL,1,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'manasysdb'
--

--
-- Dumping routines for database 'manasysdb'
--

--
-- Current Database: `manasys_inventario`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `manasys_inventario` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `manasys_inventario`;

--
-- Table structure for table `categoria`
--

DROP TABLE IF EXISTS `categoria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categoria` (
  `ID_Categoria` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  PRIMARY KEY (`ID_Categoria`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categoria`
--

LOCK TABLES `categoria` WRITE;
/*!40000 ALTER TABLE `categoria` DISABLE KEYS */;
INSERT INTO `categoria` VALUES (3,'Lapiz'),(4,'ELectronica'),(5,'SUS1'),(6,'SUS'),(7,'Lapiz123'),(8,'SSS'),(9,'SSSS'),(10,'Papel'),(11,'Caja');
/*!40000 ALTER TABLE `categoria` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entrada`
--

DROP TABLE IF EXISTS `entrada`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entrada` (
  `ID_Entrada` int NOT NULL AUTO_INCREMENT,
  `fkID_Producto` varchar(10) DEFAULT NULL,
  `Cantidad` int NOT NULL,
  `Precio_Entrada` decimal(10,2) DEFAULT NULL,
  `Precio_Venta` decimal(10,2) DEFAULT NULL,
  `FechaEntrada` date NOT NULL,
  PRIMARY KEY (`ID_Entrada`),
  KEY `fkID_Producto` (`fkID_Producto`),
  CONSTRAINT `entrada_ibfk_1` FOREIGN KEY (`fkID_Producto`) REFERENCES `producto` (`ID_Producto`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entrada`
--

LOCK TABLES `entrada` WRITE;
/*!40000 ALTER TABLE `entrada` DISABLE KEYS */;
INSERT INTO `entrada` VALUES (3,'123',12,32.00,123.00,'2025-05-24'),(4,'123',12,32.00,123.00,'2025-05-31'),(5,'123',1000,32.00,123.00,'2025-05-31'),(6,'1',28,12.00,1551.00,'2025-05-30'),(7,'12345',12333,12.00,122.00,'2025-06-19'),(8,'1',122,12.00,122.00,'2025-06-18');
/*!40000 ALTER TABLE `entrada` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `producto`
--

DROP TABLE IF EXISTS `producto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `producto` (
  `ID_Producto` varchar(10) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Descripcion` text,
  `fkID_Categoria` int DEFAULT NULL,
  `Stock` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID_Producto`),
  KEY `fkID_Categoria` (`fkID_Categoria`),
  CONSTRAINT `producto_ibfk_1` FOREIGN KEY (`fkID_Categoria`) REFERENCES `categoria` (`ID_Categoria`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `producto`
--

LOCK TABLES `producto` WRITE;
/*!40000 ALTER TABLE `producto` DISABLE KEYS */;
INSERT INTO `producto` VALUES ('1','Lapiz Mapped digi','ssasq',4,0),('123','Lapiz','SuS',3,0),('12345','Lapiz 12','Lapiz H12',3,0),('45','Papel2','Papel2',10,0);
/*!40000 ALTER TABLE `producto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `salida`
--

DROP TABLE IF EXISTS `salida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `salida` (
  `ID_Salida` int NOT NULL AUTO_INCREMENT,
  `fkID_Producto` varchar(10) DEFAULT NULL,
  `Cantidad` int NOT NULL,
  `FechaSalida` date NOT NULL,
  PRIMARY KEY (`ID_Salida`),
  KEY `fkID_Producto` (`fkID_Producto`),
  CONSTRAINT `salida_ibfk_1` FOREIGN KEY (`fkID_Producto`) REFERENCES `producto` (`ID_Producto`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `salida`
--

LOCK TABLES `salida` WRITE;
/*!40000 ALTER TABLE `salida` DISABLE KEYS */;
INSERT INTO `salida` VALUES (1,'123',6,'2025-05-23'),(2,'123',1,'2025-05-23'),(3,'123',17,'2025-05-23'),(4,'123',2,'2025-05-23'),(5,'1',10,'2025-05-23'),(6,'123',1,'2025-05-26'),(7,'123',2,'2025-05-26'),(8,'123',4,'2025-05-26'),(9,'123',3,'2025-05-26'),(10,'123',1,'2025-05-26'),(11,'123',10,'2025-05-26'),(12,'123',123,'2025-05-26'),(14,'1',1,'2025-05-28'),(15,'123',123,'2025-05-28'),(16,'123',125,'2025-05-28'),(17,'1',1,'2025-05-28'),(18,'1',1,'2025-05-28'),(19,'123',1,'2025-05-28'),(20,'1',1,'2025-05-28'),(21,'1',1,'2025-05-28'),(22,'1',1,'2025-05-28'),(23,'1',1,'2025-05-28'),(24,'123',1,'2025-05-28'),(25,'1',1,'2025-05-28'),(26,'123',1,'2025-05-28'),(27,'1',1,'2025-05-28'),(28,'1',1,'2025-05-28'),(29,'123',1,'2025-05-28'),(30,'1',1,'2025-05-28'),(31,'123',1,'2025-05-28'),(32,'1',1,'2025-05-28'),(35,'123',6,'2025-05-29'),(37,'123',2,'2025-06-10'),(39,'1',3,'2025-06-18');
/*!40000 ALTER TABLE `salida` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `validar_salida_stock` BEFORE INSERT ON `salida` FOR EACH ROW BEGIN
    DECLARE stock_actual INT;
    SELECT 
        IFNULL(SUM(e.Cantidad), 0) - IFNULL(SUM(s.Cantidad), 0)
    INTO stock_actual
    FROM Producto p
    LEFT JOIN Entrada e ON p.ID_Producto = e.fkID_Producto
    LEFT JOIN Salida s ON p.ID_Producto = s.fkID_Producto
    WHERE p.ID_Producto = NEW.fkID_Producto;

    IF stock_actual < NEW.Cantidad THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Stock insuficiente para realizar la salida';
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `vistastock`
--

DROP TABLE IF EXISTS `vistastock`;
/*!50001 DROP VIEW IF EXISTS `vistastock`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vistastock` AS SELECT 
 1 AS `ID_Producto`,
 1 AS `Nombre`,
 1 AS `fkID_Categoria`,
 1 AS `StockActual`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vistastockprecio`
--

DROP TABLE IF EXISTS `vistastockprecio`;
/*!50001 DROP VIEW IF EXISTS `vistastockprecio`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vistastockprecio` AS SELECT 
 1 AS `ID_Producto`,
 1 AS `Nombre`,
 1 AS `Descripcion`,
 1 AS `fkID_Categoria`,
 1 AS `NombreCategoria`,
 1 AS `StockActual`,
 1 AS `PrecioVenta`*/;
SET character_set_client = @saved_cs_client;

--
-- Dumping events for database 'manasys_inventario'
--

--
-- Dumping routines for database 'manasys_inventario'
--

--
-- Current Database: `manasysdb`
--

USE `manasysdb`;

--
-- Current Database: `manasys_inventario`
--

USE `manasys_inventario`;

--
-- Final view structure for view `vistastock`
--

/*!50001 DROP VIEW IF EXISTS `vistastock`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vistastock` AS select `p`.`ID_Producto` AS `ID_Producto`,`p`.`Nombre` AS `Nombre`,`p`.`fkID_Categoria` AS `fkID_Categoria`,(ifnull(`e`.`TotalEntradas`,0) - ifnull(`s`.`TotalSalidas`,0)) AS `StockActual` from ((`producto` `p` left join (select `entrada`.`fkID_Producto` AS `fkID_Producto`,sum(`entrada`.`Cantidad`) AS `TotalEntradas` from `entrada` group by `entrada`.`fkID_Producto`) `e` on((`p`.`ID_Producto` = `e`.`fkID_Producto`))) left join (select `salida`.`fkID_Producto` AS `fkID_Producto`,sum(`salida`.`Cantidad`) AS `TotalSalidas` from `salida` group by `salida`.`fkID_Producto`) `s` on((`p`.`ID_Producto` = `s`.`fkID_Producto`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vistastockprecio`
--

/*!50001 DROP VIEW IF EXISTS `vistastockprecio`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vistastockprecio` AS select `p`.`ID_Producto` AS `ID_Producto`,`p`.`Nombre` AS `Nombre`,`p`.`Descripcion` AS `Descripcion`,`p`.`fkID_Categoria` AS `fkID_Categoria`,`c`.`Nombre` AS `NombreCategoria`,(ifnull(`e`.`TotalEntradas`,0) - ifnull(`s`.`TotalSalidas`,0)) AS `StockActual`,coalesce(`e_precio`.`Precio_Venta`,0) AS `PrecioVenta` from ((((`producto` `p` left join `categoria` `c` on((`p`.`fkID_Categoria` = `c`.`ID_Categoria`))) left join (select `entrada`.`fkID_Producto` AS `fkID_Producto`,sum(`entrada`.`Cantidad`) AS `TotalEntradas` from `entrada` group by `entrada`.`fkID_Producto`) `e` on((`p`.`ID_Producto` = `e`.`fkID_Producto`))) left join (select `salida`.`fkID_Producto` AS `fkID_Producto`,sum(`salida`.`Cantidad`) AS `TotalSalidas` from `salida` group by `salida`.`fkID_Producto`) `s` on((`p`.`ID_Producto` = `s`.`fkID_Producto`))) left join (select `e1`.`fkID_Producto` AS `fkID_Producto`,`e1`.`Precio_Venta` AS `Precio_Venta` from (`entrada` `e1` join (select `entrada`.`fkID_Producto` AS `fkID_Producto`,max(`entrada`.`FechaEntrada`) AS `UltimaFecha` from `entrada` group by `entrada`.`fkID_Producto`) `e2` on(((`e1`.`fkID_Producto` = `e2`.`fkID_Producto`) and (`e1`.`FechaEntrada` = `e2`.`UltimaFecha`))))) `e_precio` on((`p`.`ID_Producto` = `e_precio`.`fkID_Producto`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-06-19  0:29:52
