USE EWallet;


GO
INSERT INTO dbo.Categories (
  nom_categorie,
  type_categorie,
  description
)
SELECT s.nom_categorie,
       s.type_categorie,
       s.description
FROM   (VALUES (N'Personnel', N'PERSONAL', N'Personal money transfers'), (N'Achats', N'SHOPPING', N'Payments related to purchases'), (N'Factures', N'BILLS', N'Bill payments'), (N'Autre', N'OTHER', N'Other transfers')) AS s(nom_categorie, type_categorie, description)
WHERE  NOT EXISTS (SELECT 1
                   FROM   dbo.Categories AS c
                   WHERE  c.nom_categorie = s.nom_categorie);


GO
PRINT 'Seed data inserted successfully.';