USE EWallet;
GO



-- Remove the old foreign key.
IF EXISTS (SELECT 1
           FROM   sys.foreign_keys
           WHERE  name = N'FK_Accounts_Users')
  BEGIN
    ALTER TABLE dbo.Accounts DROP CONSTRAINT FK_Accounts_Users;
  END
GO



-- Remove the obsolete custom Users table.
DROP TABLE IF EXISTS dbo.Users;
GO



-- Reference the Identity user table instead.
ALTER TABLE dbo.Accounts
  ADD CONSTRAINT FK_Accounts_AspNetUsers FOREIGN KEY (id_user) REFERENCES dbo.AspNetUsers (Id);
GO



PRINT 'Accounts now references ASP.NET Core Identity users.';