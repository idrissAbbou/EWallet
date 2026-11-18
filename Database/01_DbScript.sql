
/* =====================================================
   1. DATABASE
   ===================================================== */

IF DB_ID(N'EWallet') IS NULL
    CREATE DATABASE EWallet;
GO

USE EWallet;
GO

/* =====================================================
   2. USERS
   ===================================================== */

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        id_user INT IDENTITY(1,1) PRIMARY KEY,
        nom NVARCHAR(80) NOT NULL,
        prenom NVARCHAR(80) NOT NULL,
        email NVARCHAR(256) NOT NULL UNIQUE,
        password_hash NVARCHAR(500) NOT NULL,
        telephone NVARCHAR(30) NULL,

        role_user NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Users_Role DEFAULT N'Client',

        statut_user NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Users_Status DEFAULT N'Active',

        date_creation DATETIME2 NOT NULL
            CONSTRAINT DF_Users_Date DEFAULT SYSUTCDATETIME(),

        CONSTRAINT CK_Users_Role
            CHECK (role_user IN (N'Admin', N'Client')),

        CONSTRAINT CK_Users_Status
            CHECK (statut_user IN (N'Active', N'Blocked'))
    );
END;
GO

/* =====================================================
   3. ACCOUNTS
   One user can have at most one account.
   ===================================================== */

IF OBJECT_ID(N'dbo.Accounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Accounts
    (
        id_account INT IDENTITY(1,1) PRIMARY KEY,

        id_user INT NOT NULL UNIQUE,

        numero_compte NVARCHAR(30) NOT NULL UNIQUE,

        type_account NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Accounts_Type DEFAULT N'Wallet',

        solde DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_Accounts_Balance DEFAULT 0,

        devise CHAR(3) NOT NULL
            CONSTRAINT DF_Accounts_Currency DEFAULT 'MAD',

        date_creation DATETIME2 NOT NULL
            CONSTRAINT DF_Accounts_Date DEFAULT SYSUTCDATETIME(),

        statut NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Accounts_Status DEFAULT N'Active',

        CONSTRAINT FK_Accounts_Users
            FOREIGN KEY (id_user)
            REFERENCES dbo.Users(id_user),

        CONSTRAINT CK_Accounts_Balance
            CHECK (solde >= 0),

        CONSTRAINT CK_Accounts_Status
            CHECK (statut IN (N'Active', N'Blocked', N'Closed'))
    );
END;
GO

/* =====================================================
   4. CATEGORIES
   ===================================================== */

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        id_categorie INT IDENTITY(1,1) PRIMARY KEY,

        nom_categorie NVARCHAR(80) NOT NULL UNIQUE,

        type_categorie NVARCHAR(30) NOT NULL,

        description NVARCHAR(250) NULL
    );
END;
GO

/* =====================================================
   5. TRANSACTIONS
   ===================================================== */

IF OBJECT_ID(N'dbo.Transactions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transactions
    (
        id_transaction INT IDENTITY(1,1) PRIMARY KEY,

        reference_transaction NVARCHAR(50) NOT NULL UNIQUE,

        id_compte_emetteur INT NOT NULL,

        id_compte_destinataire INT NOT NULL,

        id_categorie INT NOT NULL,

        montant DECIMAL(18,2) NOT NULL,

        date_transaction DATETIME2 NOT NULL
            CONSTRAINT DF_Transactions_Date DEFAULT SYSUTCDATETIME(),

        date_validation DATETIME2 NULL,

        type_transaction NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Transactions_Type DEFAULT N'Transfer',

        description NVARCHAR(250) NULL,

        statut NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Transactions_Status DEFAULT N'Pending',

        CONSTRAINT FK_Transactions_Sender
            FOREIGN KEY (id_compte_emetteur)
            REFERENCES dbo.Accounts(id_account),

        CONSTRAINT FK_Transactions_Receiver
            FOREIGN KEY (id_compte_destinataire)
            REFERENCES dbo.Accounts(id_account),

        CONSTRAINT FK_Transactions_Categories
            FOREIGN KEY (id_categorie)
            REFERENCES dbo.Categories(id_categorie),

        CONSTRAINT CK_Transactions_Amount
            CHECK (montant > 0),

        CONSTRAINT CK_Transactions_DifferentAccounts
            CHECK (id_compte_emetteur <> id_compte_destinataire),

        CONSTRAINT CK_Transactions_Status
            CHECK (statut IN (
                N'Pending',
                N'Completed',
                N'Failed',
                N'Cancelled'
            ))
    );
END;
GO

/* =====================================================
   6. FRAUD DETECTIONS
   One transaction can have zero or one fraud result.
   ===================================================== */

IF OBJECT_ID(N'dbo.FraudDetections', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FraudDetections
    (
        id_fraude INT IDENTITY(1,1) PRIMARY KEY,

        id_transaction INT NOT NULL UNIQUE,

        score_risque DECIMAL(5,2) NOT NULL,

        raison NVARCHAR(500) NULL,

        date_detection DATETIME2 NOT NULL
            CONSTRAINT DF_Fraud_Date DEFAULT SYSUTCDATETIME(),

        decision_ia NVARCHAR(20) NOT NULL,

        CONSTRAINT FK_Fraud_Transactions
            FOREIGN KEY (id_transaction)
            REFERENCES dbo.Transactions(id_transaction),

        CONSTRAINT CK_Fraud_Score
            CHECK (score_risque BETWEEN 0 AND 100),

        CONSTRAINT CK_Fraud_Decision
            CHECK (decision_ia IN (
                N'Approved',
                N'Review',
                N'Blocked'
            ))
    );
END;
GO

/* =====================================================
   7. INDEXES
   Improve transaction history queries.
   ===================================================== */

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Transactions_Sender_Date'
      AND object_id = OBJECT_ID(N'dbo.Transactions')
)
BEGIN
    CREATE INDEX IX_Transactions_Sender_Date
    ON dbo.Transactions(id_compte_emetteur, date_transaction);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Transactions_Receiver_Date'
      AND object_id = OBJECT_ID(N'dbo.Transactions')
)
BEGIN
    CREATE INDEX IX_Transactions_Receiver_Date
    ON dbo.Transactions(id_compte_destinataire, date_transaction);
END;
GO

PRINT 'EWallet database created successfully.';
