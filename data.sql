IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [ParkingTickets] (
    [Id] int NOT NULL IDENTITY,
    [VehicleType] int NOT NULL,
    [LicensePlate] nvarchar(max) NULL,
    [TicketNumber] int NULL,
    [TimeIn] datetime2 NOT NULL,
    [TimeOut] datetime2 NULL,
    [Fee] decimal(10,2) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ParkingTickets] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022032048_InitialCreate', N'9.0.10');

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251202081037_AddUserTable', N'9.0.10');

CREATE TABLE [MonthlyTickets] (
    [Id] int NOT NULL IDENTITY,
    [CustomerName] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [LicensePlate] nvarchar(max) NOT NULL,
    [VehicleType] int NOT NULL,
    [ExpirationDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL,
    CONSTRAINT [PK_MonthlyTickets] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251202111055_AddMonthlyTicketTable', N'9.0.10');

COMMIT;
GO

