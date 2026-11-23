USE EWallet;
GO

-- EWallet - Accounts Soft Delete
-- Add is_deleted only if the column does not exist

IF OBJECT_ID(N'dbo.Accounts', N'U') IS NULL
BEGIN
    THROW 50001, 'Table dbo.Accounts does not exist.', 1;
END;
GO

IF COL_LENGTH('dbo.Accounts', 'is_deleted') IS NULL
BEGIN
    ALTER TABLE dbo.Accounts
    ADD is_deleted BIT NOT NULL
        CONSTRAINT DF_Accounts_IsDeleted DEFAULT (0);
END;
GO
