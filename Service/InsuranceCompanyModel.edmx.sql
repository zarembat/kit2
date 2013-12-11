
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/10/2013 19:30:57
-- Generated from EDMX file: C:\Users\Tomek\Desktop\K&T Insurance\Service\InsuranceCompanyModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [InsuranceCompany];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AdressSetHouseSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HouseSet] DROP CONSTRAINT [FK_AdressSetHouseSet];
GO
IF OBJECT_ID(N'[dbo].[FK_CarPolicy]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CarSet] DROP CONSTRAINT [FK_CarPolicy];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientAdress]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientSet] DROP CONSTRAINT [FK_ClientAdress];
GO
IF OBJECT_ID(N'[dbo].[FK_HousePolicy]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HouseSet] DROP CONSTRAINT [FK_HousePolicy];
GO
IF OBJECT_ID(N'[dbo].[FK_PolicyClient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PolicySet] DROP CONSTRAINT [FK_PolicyClient];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AdressSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdressSet];
GO
IF OBJECT_ID(N'[dbo].[CarSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CarSet];
GO
IF OBJECT_ID(N'[dbo].[ClientSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientSet];
GO
IF OBJECT_ID(N'[dbo].[EmployeeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeSet];
GO
IF OBJECT_ID(N'[dbo].[HouseSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HouseSet];
GO
IF OBJECT_ID(N'[dbo].[PolicySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PolicySet];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AdressSet'
CREATE TABLE [dbo].[AdressSet] (
    [AdressId] int IDENTITY(1,1) NOT NULL,
    [Town] nvarchar(max)  NOT NULL,
    [Street] nvarchar(max)  NOT NULL,
    [HouseNumber] nvarchar(max)  NOT NULL,
    [ZipCode] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CarSet'
CREATE TABLE [dbo].[CarSet] (
    [ObjectId] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Brand] nvarchar(max)  NOT NULL,
    [Year] int  NOT NULL,
    [VinNumber] nvarchar(max)  NOT NULL,
    [Engine] nvarchar(max)  NOT NULL,
    [Policy_PolicyId] int  NOT NULL
);
GO

-- Creating table 'ClientSet'
CREATE TABLE [dbo].[ClientSet] (
    [ClientId] int IDENTITY(1,1) NOT NULL,
    [Surname] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [PESEL] nvarchar(max)  NOT NULL,
    [AdressAdressId] int  NOT NULL
);
GO

-- Creating table 'EmployeeSet'
CREATE TABLE [dbo].[EmployeeSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Surname] nvarchar(max)  NOT NULL,
    [Role] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'HouseSet'
CREATE TABLE [dbo].[HouseSet] (
    [ObjectId] int IDENTITY(1,1) NOT NULL,
    [Year] int  NOT NULL,
    [Size] int  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Policy_PolicyId] int  NOT NULL,
    [AdressSet_AdressId] int  NOT NULL
);
GO

-- Creating table 'PolicySet'
CREATE TABLE [dbo].[PolicySet] (
    [PolicyId] int IDENTITY(1,1) NOT NULL,
    [Duration] int  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [ObjectType] nvarchar(max)  NOT NULL,
    [ClientClientId] int  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AdressId] in table 'AdressSet'
ALTER TABLE [dbo].[AdressSet]
ADD CONSTRAINT [PK_AdressSet]
    PRIMARY KEY CLUSTERED ([AdressId] ASC);
GO

-- Creating primary key on [ObjectId] in table 'CarSet'
ALTER TABLE [dbo].[CarSet]
ADD CONSTRAINT [PK_CarSet]
    PRIMARY KEY CLUSTERED ([ObjectId] ASC);
GO

-- Creating primary key on [ClientId] in table 'ClientSet'
ALTER TABLE [dbo].[ClientSet]
ADD CONSTRAINT [PK_ClientSet]
    PRIMARY KEY CLUSTERED ([ClientId] ASC);
GO

-- Creating primary key on [Id] in table 'EmployeeSet'
ALTER TABLE [dbo].[EmployeeSet]
ADD CONSTRAINT [PK_EmployeeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ObjectId] in table 'HouseSet'
ALTER TABLE [dbo].[HouseSet]
ADD CONSTRAINT [PK_HouseSet]
    PRIMARY KEY CLUSTERED ([ObjectId] ASC);
GO

-- Creating primary key on [PolicyId] in table 'PolicySet'
ALTER TABLE [dbo].[PolicySet]
ADD CONSTRAINT [PK_PolicySet]
    PRIMARY KEY CLUSTERED ([PolicyId] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AdressSet_AdressId] in table 'HouseSet'
ALTER TABLE [dbo].[HouseSet]
ADD CONSTRAINT [FK_AdressSetHouseSet]
    FOREIGN KEY ([AdressSet_AdressId])
    REFERENCES [dbo].[AdressSet]
        ([AdressId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdressSetHouseSet'
CREATE INDEX [IX_FK_AdressSetHouseSet]
ON [dbo].[HouseSet]
    ([AdressSet_AdressId]);
GO

-- Creating foreign key on [AdressAdressId] in table 'ClientSet'
ALTER TABLE [dbo].[ClientSet]
ADD CONSTRAINT [FK_ClientAdress]
    FOREIGN KEY ([AdressAdressId])
    REFERENCES [dbo].[AdressSet]
        ([AdressId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientAdress'
CREATE INDEX [IX_FK_ClientAdress]
ON [dbo].[ClientSet]
    ([AdressAdressId]);
GO

-- Creating foreign key on [Policy_PolicyId] in table 'CarSet'
ALTER TABLE [dbo].[CarSet]
ADD CONSTRAINT [FK_CarPolicy]
    FOREIGN KEY ([Policy_PolicyId])
    REFERENCES [dbo].[PolicySet]
        ([PolicyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CarPolicy'
CREATE INDEX [IX_FK_CarPolicy]
ON [dbo].[CarSet]
    ([Policy_PolicyId]);
GO

-- Creating foreign key on [ClientClientId] in table 'PolicySet'
ALTER TABLE [dbo].[PolicySet]
ADD CONSTRAINT [FK_PolicyClient]
    FOREIGN KEY ([ClientClientId])
    REFERENCES [dbo].[ClientSet]
        ([ClientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PolicyClient'
CREATE INDEX [IX_FK_PolicyClient]
ON [dbo].[PolicySet]
    ([ClientClientId]);
GO

-- Creating foreign key on [Policy_PolicyId] in table 'HouseSet'
ALTER TABLE [dbo].[HouseSet]
ADD CONSTRAINT [FK_HousePolicy]
    FOREIGN KEY ([Policy_PolicyId])
    REFERENCES [dbo].[PolicySet]
        ([PolicyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HousePolicy'
CREATE INDEX [IX_FK_HousePolicy]
ON [dbo].[HouseSet]
    ([Policy_PolicyId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------