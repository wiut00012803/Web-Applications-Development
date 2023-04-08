set language english;
GO

/*******************************************************************************
   Create Tables
********************************************************************************/
CREATE TABLE [dbo].[Product]
(
    [ProductId] INT NOT NULL IDENTITY,
    [Title] NVARCHAR(160) NOT NULL,
    [ProviderId] INT NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId])
);
GO
CREATE TABLE [dbo].[Provider]
(
    [ProviderId] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(120),
    CONSTRAINT [PK_Provider] PRIMARY KEY CLUSTERED ([ProviderId])
);
GO
CREATE TABLE [dbo].[Customer]
(
    [CustomerId] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(40) NOT NULL,
    [LName] NVARCHAR(20) NOT NULL,
    [Company] NVARCHAR(80),
    [Address] NVARCHAR(70),
    [City] NVARCHAR(40),
    [State] NVARCHAR(40),
    [Country] NVARCHAR(40),
    [PostalCode] NVARCHAR(10),
    [Phone] NVARCHAR(24),
    [Fax] NVARCHAR(24),
    [Email] NVARCHAR(60) NOT NULL,
    [SupportRepId] INT,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerId])
);
GO
CREATE TABLE [dbo].[Employee]
(
    [EmployeeId] INT NOT NULL IDENTITY,
    [LName] NVARCHAR(20) NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    [Title] NVARCHAR(30),
    [ReportsTo] INT,
    [BirthDate] DATETIME,
    [StartDate] DATETIME,
    [Address] NVARCHAR(70),
    [City] NVARCHAR(40),
    [State] NVARCHAR(40),
    [Country] NVARCHAR(40),
    [PostalCode] NVARCHAR(10),
    [Phone] NVARCHAR(24),
    [Fax] NVARCHAR(24),
    [Email] NVARCHAR(60),
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([EmployeeId])
);
GO
CREATE TABLE [dbo].[Section]
(
    [SectionId] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(120),
    CONSTRAINT [PK_Section] PRIMARY KEY CLUSTERED ([SectionId])
);
GO
CREATE TABLE [dbo].[Invoice]
(
    [InvoiceId] INT NOT NULL IDENTITY,
    [CustomerId] INT NOT NULL,
    [InvoiceDate] DATETIME NOT NULL,
    [billAddress] NVARCHAR(70),
    [billCity] NVARCHAR(40),
    [billState] NVARCHAR(40),
    [billCountry] NVARCHAR(40),
    [billPostalCode] NVARCHAR(10),
    [Total] NUMERIC(10,2) NOT NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([InvoiceId])
);
GO
CREATE TABLE [dbo].[InvoiceLine]
(
    [InvoiceLineId] INT NOT NULL IDENTITY,
    [InvoiceId] INT NOT NULL,
    [CurentId] INT NOT NULL,
    [UnitPrice] NUMERIC(10,2) NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT [PK_InvoiceLine] PRIMARY KEY CLUSTERED ([InvoiceLineId])
);
GO
CREATE TABLE [dbo].[Type]
(
    [TypeId] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(120),
    CONSTRAINT [PK_Type] PRIMARY KEY CLUSTERED ([TypeId])
);
GO
CREATE TABLE [dbo].[Cart]
(
    [CartId] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(120),
    CONSTRAINT [PK_Cart] PRIMARY KEY CLUSTERED ([CartId])
);
GO
CREATE TABLE [dbo].[CartCurent]
(
    [CartId] INT NOT NULL,
    [CurentId] INT NOT NULL,
    CONSTRAINT [PK_CartCurent] PRIMARY KEY NONCLUSTERED ([CartId], [CurentId])
);
GO
CREATE TABLE [dbo].[Curent]
(
    [CurentId] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(200) NOT NULL,
    [ProductId] INT,
    [TypeId] INT NOT NULL,
    [SectionId] INT,
    [Farmer] NVARCHAR(220),
    [Weight] INT NOT NULL,
    [Amount] INT,
    [UnitPrice] NUMERIC(10,2) NOT NULL,
    CONSTRAINT [PK_Curent] PRIMARY KEY CLUSTERED ([CurentId])
);
GO


/*******************************************************************************
   Create Primary Key Unique Indexes
********************************************************************************/

/*******************************************************************************
   Create Foreign Keys
********************************************************************************/
ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_ProductProviderId]
    FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Provider] ([ProviderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_ProductProviderId] ON [dbo].[Product] ([ProviderId]);
GO
ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [FK_CustomerSupportRepId]
    FOREIGN KEY ([SupportRepId]) REFERENCES [dbo].[Employee] ([EmployeeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_CustomerSupportRepId] ON [dbo].[Customer] ([SupportRepId]);
GO
ALTER TABLE [dbo].[Employee] ADD CONSTRAINT [FK_EmployeeReportsTo]
    FOREIGN KEY ([ReportsTo]) REFERENCES [dbo].[Employee] ([EmployeeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_EmployeeReportsTo] ON [dbo].[Employee] ([ReportsTo]);
GO
ALTER TABLE [dbo].[Invoice] ADD CONSTRAINT [FK_InvoiceCustomerId]
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_InvoiceCustomerId] ON [dbo].[Invoice] ([CustomerId]);
GO
ALTER TABLE [dbo].[InvoiceLine] ADD CONSTRAINT [FK_InvoiceLineInvoiceId]
    FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoice] ([InvoiceId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_InvoiceLineInvoiceId] ON [dbo].[InvoiceLine] ([InvoiceId]);
GO
ALTER TABLE [dbo].[InvoiceLine] ADD CONSTRAINT [FK_InvoiceLineCurentId]
    FOREIGN KEY ([CurentId]) REFERENCES [dbo].[Curent] ([CurentId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_InvoiceLineCurentId] ON [dbo].[InvoiceLine] ([CurentId]);
GO
ALTER TABLE [dbo].[CartCurent] ADD CONSTRAINT [FK_CartCurentCartId]
    FOREIGN KEY ([CartId]) REFERENCES [dbo].[Cart] ([CartId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [dbo].[CartCurent] ADD CONSTRAINT [FK_CartCurentCurentId]
    FOREIGN KEY ([CurentId]) REFERENCES [dbo].[Curent] ([CurentId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_CartCurentCurentId] ON [dbo].[CartCurent] ([CurentId]);
GO
ALTER TABLE [dbo].[Curent] ADD CONSTRAINT [FK_CurentProductId]
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_CurentProductId] ON [dbo].[Curent] ([ProductId]);
GO
ALTER TABLE [dbo].[Curent] ADD CONSTRAINT [FK_CurentSectionId]
    FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Section] ([SectionId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_CurentSectionId] ON [dbo].[Curent] ([SectionId]);
GO
ALTER TABLE [dbo].[Curent] ADD CONSTRAINT [FK_CurentTypeId]
    FOREIGN KEY ([TypeId]) REFERENCES [dbo].[Type] ([TypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
CREATE INDEX [IFK_CurentTypeId] ON [dbo].[Curent] ([TypeId]);
GO