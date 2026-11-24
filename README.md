# Plateforme E-Wallet

## 1. Présentation du projet

E-Wallet est une plateforme de portefeuille électronique permettant aux utilisateurs de gérer leurs transactions financières, de suivre leurs budgets et de contrôler leurs dépenses.

La plateforme intègre un modèle de Machine Learning basé sur **XGBoost** pour détecter les transactions bancaires potentiellement frauduleuses.

## 2. Fonctionnalités

- Gestion des comptes utilisateurs.
- Gestion des portefeuilles électroniques.
- Gestion et suivi des transactions financières.
- Suivi du solde du portefeuille.
- Gestion des budgets personnels.
- Historique et suivi des dépenses.
- Détection des fraudes basée sur l'intelligence artificielle.

## 3. Technologies utilisées

| Composant | Technologie |
|---|---|
| Frontend | HTML, Tailwind CSS, Flowbite |
| Backend | ASP.NET Core MVC, ADO.NET |
| Base de données | Microsoft SQL Server |
| Intelligence artificielle / ML | Python, XGBoost |
| Gestion de versions | Git, GitHub |

## 4. Architecture

L'application repose sur une architecture client-serveur organisée en plusieurs couches :

- **Couche présentation :** ASP.NET Core MVC, Razor Views, Tailwind CSS et Flowbite.
- **Couche applicative :** Contrôleurs et services assurant la logique métier.
- **Couche d'accès aux données :** ADO.NET pour communiquer avec la base de données.
- **Couche données :** SQL Server pour stocker les utilisateurs, les portefeuilles et les transactions.
- **Couche IA :** Modèle XGBoost pour la détection des transactions frauduleuses.

```mermaid
flowchart TB
    USER["User / Browser"]

    subgraph WEB["ASP.NET Core MVC Application"]
        direction TB
        CTRL["Presentation Layer<br/>Controllers + Razor Views"]
        SERVICE["Business Layer<br/>Application Services"]
        REPO["Data Access Layer<br/>ADO.NET Repositories"]

        CTRL --> SERVICE
        SERVICE --> REPO
    end

    DB[("SQL Server Database")]

    subgraph ML["Python ML Service"]
        API["FastAPI<br/>Prediction Endpoint"]
        MODEL["XGBoost Model<br/>model.pkl"]
        API --> MODEL
    end

    USER <-->|HTTP / HTTPS| CTRL
    REPO <-->|SQL Queries| DB
    SERVICE <-->|HTTP REST / JSON| API
```

## 5 La conception

## 5.1. Conception  Uml

## 5.1.1 Diagramme de Cas d'Utilisation
```mermaid
flowchart LR
    %% Acteurs
    U(["👤 Utilisateur"])
    C(["👤 Client"])
    A(["👤 Admin"])
    IA(["🤖 Système IA<br/>Détection de fraude"])

    %% Héritage
    C -. "hérite de" .-> U
    A -. "hérite de" .-> U

    subgraph SYS["Système WalletDB - E-Wallet"]
        direction LR
        %% Cas communs (Utilisateur)
        UC1(["S'authentifier"])
        UC2(["Gérer son profil"])

        %% Cas Client (Compte unique 1,1)
        UC6(["S'inscrire"])
        UC3(["Consulter son compte"])
        UC4(["Consulter l'historique"])
        UC5(["Consulter le solde"])
        UC7(["Effectuer une transaction"])
        UC8(["Vérifier le solde disponible"])
        UC9(["Classifier la transaction"])
        UC10(["Analyser le risque de fraude"])

        %% Cas Admin
        UC11(["Gérer les comptes"])
        UC12(["Gérer les utilisateurs"])
        UC13(["Gérer les catégories"])
        UC14(["Valider ou bloquer une transaction suspecte"])
        UC15(["Consulter les alertes de fraude"])
    end

    %% Utilisateur (commun)
    U --- UC1
    U --- UC2

    %% Client (avec son compte unique 1,1)
    C --- UC6
    C --- UC3
    C --- UC4
    C --- UC7

    %% Admin
    A --- UC11
    A --- UC12
    A --- UC13
    A --- UC15

    %% Include / Extend
    UC3 -. "include" .-> UC5
    UC7 -. "include" .-> UC8
    UC7 -. "include" .-> UC9
    UC7 -. "include" .-> UC10
    UC14 -. "extend" .-> UC15

    %% Connexion IA
    UC10 --- IA
```
## 5.1.2 Diagramme de Séquence

```mermaid
sequenceDiagram
    actor C as Client
    participant Sys as Système E-Wallet
    participant Acc as Compte (ACCOUNT)
    participant Tr as Transaction (TRANSACTION)
    participant IA as Système IA (FRAUDDETECTION)

    C->>Sys: 1. Saisir virement (montant, destinataire, catégorie)
    
    %% Vérification du solde (entité ACCOUNT)
    Sys->>Acc: 2. Vérifier solde (compte émetteur)
    Acc-->>Sys: Solde suffisant

    %% Analyse de fraude (entité FRAUDDETECTION)
    Sys->>IA: 3. Analyser le risque de fraude
    IA-->>Sys: Score risque & décision IA (OK)

    alt Transaction valide (Pas de fraude)
        %% Enregistrement (entités TRANSACTION et CATEGORIE)
        Sys->>Tr: 4. Créer transaction (liée à id_categorie)
        Tr-->>Sys: Transaction enregistrée
        
        %% Mise à jour des comptes (Envoie / Reçoit)
        Sys->>Acc: 5. Débiter compte émetteur & Créditer destinataire
        Acc-->>Sys: Soldes mis à jour
        
        Sys-->>C: 6. Confirmation de la transaction
    else Transaction suspecte (Fraude détectée)
        Sys->>IA: Enregistrer alerte (score_risque, raison)
        Sys-->>C: Transaction bloquée (En attente validation Admin)
    end
```

## 5.1.3 Diagramme de Classes

```mermaid
classDiagram
    direction LR

    class User {
        -int id_user
        -string nom
        -string prenom
        -string email
        -string mot_de_passe
        -string telephone
        -Role role
        +sAuthentifier() bool
        +modifierProfil() void
        +consulterCompte() Account
    }

    class Account {
        -int id_account
        -string numero_compte
        -string type_account
        -decimal solde
        -datetime date_creation
        -string statut
        -string devise
        +consulterSolde() decimal
        +debiter(montant) void
        +crediter(montant) void
        +verifierSolde(montant) bool
    }

    class Categorie {
        -int id_categorie
        -string nom_categorie
        -string type_categorie
        -string description
        +classifier(transaction) void
    }

    class Transaction {
        -int id_transaction
        -decimal montant
        -datetime date_transaction
        -string description
        -StatutTransaction statut
        -TypeTransaction type_transaction
        +effectuer() bool
        +annuler() void
        +consulterHistorique() List~Transaction~
    }

    class FraudDetection {
        -int id_fraude
        -decimal score_risque
        -string raison
        -datetime date_detection
        -boolean decision_ia
        +analyser(transaction) decimal
        +decider() string
    }

    class Role {
        <<enumeration>>
        Client
        Admin
    }

    class StatutTransaction {
        <<enumeration>>
        Validee
        Bloquee
        EnAttente
    }

    class TypeTransaction {
        <<enumeration>>
        Depense
        Revenu
    }

    User "1" --> "0..1" Account : possède
    Account "1" --> "0..*" Transaction : "envoie (compte émetteur)"
    Account "1" --> "0..*" Transaction : "reçoit (compte destinataire)"
    Categorie "1" --> "0..*" Transaction : classifie
    Transaction "1" --> "1" FraudDetection : génère
```





 ## 5.2. Conception de la base de données

La base de données repose principalement sur les entités suivantes :

FraudDetectionResults (_id_fraud_, score_risque, raison, date_detection, decision_IA, #id_transaction)  -- #id_transaction UNIQUE

## 5.2.1 Modèle Conceptuel de Données (MCD)

```mermaid
classDiagram
    direction LR

    class USER {
         id_user
         nom
         prenom
         email
         mot_de_passe
         telephone
         role
    }

    class ACCOUNT {
         id_account
         numero_compte
         type_account
         solde
         date_creation
         statut
         devise
    }

    class TRANSACTION {
         id_transaction
         montant
         date_transaction
         type_transaction
         description
         statut
    }

    class CATEGORIE {
         id_categorie
         nom_categorie
         type_categorie
         description
    }

    class FRAUDDETECTION {
         id_fraude
         score_risque
         raison
         date_detection
         decision_ia
    }

    %% Relations et cardinalités du MCD d'origine
    USER "0..1" -- "1,1" ACCOUNT : "Possède"
    ACCOUNT "1,1" -- "0..N" TRANSACTION : "Envoie"
    ACCOUNT "1,1" -- "0..N" TRANSACTION : "Reçoit"
    CATEGORIE "0..N" -- "1,1" TRANSACTION : "Classifie"
    TRANSACTION "1,1" -- "1,1" FRAUDDETECTION : "Génère"
```

## 5.2.2 Modèle Logique de Données (MLD)

```mermaid
erDiagram
    USER {
        int id_user PK
        string nom
        string prenom
        string email
        string mot_de_passe
        string telephone
        string role
    }

    ACCOUNT {
        int id_account PK
        string numero_compte
        string type_account
        decimal solde
        datetime date_creation
        string statut
        string devise
        int id_user FK
    }

    CATEGORIE {
        int id_categorie PK
        string nom_categorie
        string type_categorie
        string description
    }

    TRANSACTION {
        int id_transaction PK
        decimal montant
        datetime date_transaction
        string type_transaction
        string description
        string statut
        int id_account_emetteur FK
        int id_account_destinataire FK
        int id_categorie FK
    }

    FRAUDDETECTION {
        int id_fraude PK
        decimal score_risque
        string raison
        datetime date_detection
        boolean decision_ia
        int id_transaction FK
    }

    USER ||--o| ACCOUNT : "possède"
    ACCOUNT ||--o{ TRANSACTION : "envoie"
    ACCOUNT ||--o{ TRANSACTION : "reçoit"
    CATEGORIE ||--o{ TRANSACTION : "classifie"
    TRANSACTION ||--|| FRAUDDETECTION : "génère"

```
## 5.2.3 Modèle physique de données (MPD)

```sql
-- =============================================
-- MPD - WalletDB (SQL Server)
-- =============================================
CREATE DATABASE WalletDB;
GO
USE WalletDB;
GO

-- ---------------------------------------------
-- Table USERS
-- ---------------------------------------------
CREATE TABLE Users (
    id_user         INT IDENTITY(1,1) NOT NULL,
    nom             NVARCHAR(100)  NOT NULL,
    prenom          NVARCHAR(100)  NOT NULL,
    email           NVARCHAR(150)  NOT NULL,
    mot_de_passe    NVARCHAR(255)  NOT NULL,
    telephone       NVARCHAR(20)   NULL,
    role            NVARCHAR(10)   NOT NULL DEFAULT 'Client',
    CONSTRAINT PK_Users PRIMARY KEY (id_user),
    CONSTRAINT UQ_Users_Email UNIQUE (email),
    CONSTRAINT CK_Users_Role CHECK (role IN ('Client', 'Admin'))
);
GO

-- ---------------------------------------------
-- Table ACCOUNTS
-- ---------------------------------------------
CREATE TABLE Accounts (
    id_account      INT IDENTITY(1,1) NOT NULL,
    numero_compte   NVARCHAR(30)   NOT NULL,
    type_account    NVARCHAR(50)   NOT NULL,
    solde           DECIMAL(18,2)  NOT NULL DEFAULT 0,
    date_creation   DATE           NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    statut          NVARCHAR(20)   NOT NULL DEFAULT 'Actif',
    devise          CHAR(3)        NOT NULL DEFAULT 'MAD',
    id_user         INT            NOT NULL,
    CONSTRAINT PK_Accounts PRIMARY KEY (id_account),
    CONSTRAINT UQ_Accounts_Numero UNIQUE (numero_compte),
    CONSTRAINT FK_Accounts_Users FOREIGN KEY (id_user)
        REFERENCES Users(id_user)
        ON DELETE CASCADE
 CHECK (statut IN ('Actif', 'Inactif', 'Bloque'))
);
GO

-- ---------------------------------------------
-- Table CATEGORIES
-- ---------------------------------------------
CREATE TABLE Categories (
    id_categorie    INT IDENTITY(1,1) NOT NULL,
    nom_categorie   NVARCHAR(100)  NOT NULL,
    type_categorie  NVARCHAR(50)   NOT NULL,
    description     NVARCHAR(255)  NULL,
    CONSTRAINT PK_Categories PRIMARY KEY (id_categorie)
    CHECK (type_categorie IN ('Depense', 'Revenu'))
);
GO

-- ---------------------------------------------
-- Table TRANSACTIONS
-- ---------------------------------------------
CREATE TABLE Transactions (
    id_transaction          INT IDENTITY(1,1) NOT NULL,
    montant                 DECIMAL(18,2)  NOT NULL,
    date_transaction        DATETIME       NOT NULL DEFAULT GETDATE(),
    type_transaction        NVARCHAR(50)   NOT NULL,
    description             NVARCHAR(255)  NULL,
    statut                  NVARCHAR(10)   NOT NULL,
    id_account_emetteur     INT            NOT NULL,
    id_account_destinataire INT            NOT NULL,
    id_categorie            INT            NOT NULL,
    CONSTRAINT PK_Transactions PRIMARY KEY (id_transaction),
    CONSTRAINT CK_Transactions_Montant CHECK (montant > 0),
    CONSTRAINT CK_Transactions_Statut CHECK (statut IN ('Depense', 'Revenu')),
    CONSTRAINT CK_Transactions_Comptes CHECK (id_account_emetteur <> id_account_destinataire),
    -- NO ACTION pour eviter les "multiple cascade paths" sur Accounts
    CONSTRAINT FK_Transactions_Emetteur FOREIGN KEY (id_account_emetteur)
        REFERENCES Accounts(id_account),
    CONSTRAINT FK_Transactions_Destinataire FOREIGN KEY (id_account_destinataire)
        REFERENCES Accounts(id_account),
    CONSTRAINT FK_Transactions_Categories FOREIGN KEY (id_categorie)
        REFERENCES Categories(id_categorie)

);
GO

-- ---------------------------------------------
-- Table FRAUDDETECTION
-- ---------------------------------------------
CREATE TABLE FraudDetection (
    id_fraude       INT IDENTITY(1,1) NOT NULL,
    score_risque    DECIMAL(5,2)   NOT NULL,
    raison          NVARCHAR(255)  NULL,
    date_detection  DATETIME       NOT NULL DEFAULT GETDATE(),
    decision_ia     boolean   NOT NULL,
    id_transaction  INT            NOT NULL,
    CONSTRAINT PK_FraudDetection PRIMARY KEY (id_fraude),
    CONSTRAINT UQ_FraudDetection_Transaction UNIQUE (id_transaction),
    CONSTRAINT CK_FraudDetection_Score CHECK (score_risque BETWEEN 0 AND 100),
    CONSTRAINT FK_FraudDetection_Transactions FOREIGN KEY (id_transaction)
        REFERENCES Transactions(id_transaction)
        ON DELETE CASCADE
);
GO

-- ---------------------------------------------
-- Index pour les performances
-- ---------------------------------------------
CREATE INDEX IX_Accounts_User ON Accounts(id_user);
CREATE INDEX IX_Transactions_Emetteur ON Transactions(id_account_emetteur);
CREATE INDEX IX_Transactions_Destinataire ON Transactions(id_account_destinataire);
CREATE INDEX IX_Transactions_Categorie ON Transactions(id_categorie);
CREATE INDEX IX_Transactions_Date ON Transactions(date_transaction);
GO
```


## 6. Prérequis

- .NET SDK
- Microsoft SQL Server
- SQL Server Management Studio (SSMS)
- Git
- Python (pour le composant Machine Learning)

## 7. Installation

Cloner le dépôt Git :

```bash
git clone git@github.com:idrissAbbou/e_wallet.git
cd e_wallet
```

Restaurer les dépendances du projet :

```bash
dotnet restore
npm install
```

## 8. Configuration

Les éléments suivants doivent être configurés avant le lancement de l'application :

- **Connexion à la base de données :** Configurer la chaîne de connexion SQL Server.
- **Variables d'environnement :** Configurer les paramètres sensibles de l'application.
- **Modèle IA :** Configurer l'intégration du modèle XGBoost.
- **Services externes :** Configurer les éventuelles API nécessaires.

Les identifiants de connexion et les clés API ne doivent pas être publiés dans le dépôt Git.

## 9. Exécution de l'application

Lancer l'application avec la commande suivante :

```bash
dotnet run
```

Accéder ensuite à l'URL locale affichée dans le terminal.

## 10. Structure du projet

```text
e_wallet/
├── Controllers/
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── BudgetsController.cs
│   ├── CategoriesController.cs
│   ├── DashboardController.cs
│   ├── HomeController.cs
│   ├── TransactionsController.cs
│   └── WalletsController.cs
├── Data/
│   ├── Database/
│   │   └── DbConnectionFactory.cs
│   ├── Interfaces/
│   │   └── IUserRepository.cs
│   └── Repositories/
│       ├── DatabaseRepository.cs
│       └── UserRepository.cs
├── Database/
│   └── Scripts/
│       ├── 001_CreateDatabase.sql
│       ├── 002_CreateTables.sql
│       ├── 004_AddConstraintes.sql
│       ├── 004_CreateIndexes.sql
│       ├── 005_dataSeed.sql
│       └── CheckScripts.sql
├── Models/
│   ├── Entities/
│   │   └── User.cs
│   └── ErrorViewModel.cs
├── Properties/
│   └── launchSettings.json
├── Views/
│   ├── Account/             # Connexion, inscription et profil
│   ├── Admin/               # Pages d'administration et de fraude
│   ├── Budgets/
│   ├── Categories/
│   ├── Dashboard/
│   ├── Home/
│   ├── Shared/              # Layouts, navigation et composants Razor
│   ├── Transactions/
│   ├── Wallets/
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/
│   │   ├── site.css         # CSS Tailwind compilé, servi par l'application
│   │   └── tailwind.css     # Source Tailwind et composants personnalisés
│   ├── js/
│   │   └── site.js          # Thème et navigation responsive
│   ├── lib/                 # Bibliothèques front-end tierces
│   └── favicon.ico
├── appsettings.json
├── appsettings.Development.json
├── build_frontend.py        # Génération auxiliaire de pages front-end
├── e_wallet.csproj
├── generate_frontend.py     # Génération auxiliaire de pages et config Tailwind
├── fix_missing_frontend.py  # Génération auxiliaire de pages front-end manquantes
├── package.json             # Commandes et dépendances Tailwind CSS
├── package-lock.json
├── postcss.config.js
├── Program.cs               # Démarrage, configuration DI et routes MVC
├── tailwind.config.js
├── readme.md
└── .gitignore
```

### Dossiers et fichiers importants

- **`Data/`** contient l'accès aux données : création des connexions SQL Server, interfaces de dépôt et implémentations des dépôts.
- **`Database/Scripts/`** contient les scripts SQL de création de la base et des tables, les contraintes, les index, les données de test et les scripts de vérification.
- **`Views/Shared/`** regroupe les layouts utilisateur, administrateur et authentification, ainsi que les partial views partagées.
- **`wwwroot/css/tailwind.css`** est la source CSS. La commande `npm run build:css` génère **`wwwroot/css/site.css`**, utilisé par les pages.
- **`wwwroot/js/site.js`** gère les interactions communes de thème et de menu responsive.
- **`Program.cs`** configure les services et le routage MVC. **`e_wallet.csproj`** définit le projet .NET et ses dépendances.
- **`appsettings.json`** et **`appsettings.Development.json`** contiennent la configuration de l'application. Ne stockez pas de mots de passe ni d'autres secrets dans les fichiers suivis par Git.
- **`package.json`**, **`package-lock.json`**, **`tailwind.config.js`** et **`postcss.config.js`** définissent l'outillage de génération CSS.
- Les dossiers **`bin/`**, **`obj/`** et **`node_modules/`** sont des dossiers générés ou restaurés et ne font pas partie des sources à modifier manuellement.

## 11. Utilisation

La plateforme permettra aux utilisateurs de gérer leurs portefeuilles électroniques, d'effectuer et de consulter leurs transactions financières, de suivre leurs dépenses et leurs budgets, ainsi que de recevoir des alertes en cas de détection de transactions potentiellement frauduleuses.

## 12. Membres de l'équipe

| Nom | Rôle |
|---|---|
| Achraf Bayh | Développement |
| Chaymae El Azhari | Développement |
| Amale Assay | Développement |
| Idriss Abbou | Développement |
