using EWallet.Models;
using Microsoft.Data.SqlClient;
using EWallet.ViewModels;

namespace EWallet.Data.Repositories;

public class AccountRepository
{
  private readonly SqlConnectionFactory _connectionFactory;

  public AccountRepository(SqlConnectionFactory connectionFactory)
  {
    _connectionFactory = connectionFactory;
  }

  public async Task<int> CreateAsync(int userId)
  {
    string accountNumber =
        "EW" + Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();

    const string query = @"
            INSERT INTO Accounts (id_user, numero_compte)
            OUTPUT INSERTED.id_account
            VALUES (@UserId, @AccountNumber);
        ";

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);

    command.Parameters.AddWithValue("@UserId", userId);
    command.Parameters.AddWithValue("@AccountNumber", accountNumber);

    
    object? result = await command.ExecuteScalarAsync();

     if (result is not int accountId)
    {
    throw new InvalidOperationException(
        "Failed to create account or retrieve its ID.");
    }

    return accountId;

  }

  public async Task<Account?> GetByUserIdAsync(int userId)
  {
    const string query = @"
            SELECT
                id_account,
                id_user,
                numero_compte,
                type_account,
                solde,
                devise,
                date_creation,
                statut
            FROM Accounts
            WHERE id_user = @UserId
            AND is_deleted = 0;
        ";

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);

    command.Parameters.AddWithValue("@UserId", userId);

    await using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
      return null;

    return new Account
    {
      IdAccount = reader.GetInt32(0),
      IdUser = reader.GetInt32(1),
      NumeroCompte = reader.GetString(2),
      TypeAccount = reader.GetString(3),
      Solde = reader.GetDecimal(4),
      Devise = reader.GetString(5),
      DateCreation = reader.GetDateTime(6),
      Statut = reader.GetString(7)
    };
  }
  
   public async Task<List<Account>> GetAllAsync(int page = 1, int pageSize = 10)
   {
    var accounts = new List<Account>();

    if (page < 1)
        page = 1;

    if (pageSize < 1)
        pageSize = 10;

    const string query = @"
        SELECT id_account, id_user, numero_compte,
               type_account, solde, devise,
               date_creation, statut
        FROM dbo.Accounts
        WHERE is_deleted = 0
        ORDER BY id_account DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

    int offset = (page - 1) * pageSize;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);

    command.Parameters.AddWithValue("@Offset", offset);
    command.Parameters.AddWithValue("@PageSize", pageSize);

    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        accounts.Add(new Account
        {
            IdAccount = reader.GetInt32(0),
            IdUser = reader.GetInt32(1),
            NumeroCompte = reader.GetString(2),
            TypeAccount = reader.GetString(3),
            Solde = reader.GetDecimal(4),
            Devise = reader.GetString(5),
            DateCreation = reader.GetDateTime(6),
            Statut = reader.GetString(7)
        });
    }

    return accounts;
}

  public async Task<Account?> GetByIdAsync(int accountId)
  {
    const string query = @"
        SELECT id_account, id_user, numero_compte,
               type_account, solde, devise,
               date_creation, statut
        FROM Accounts
        WHERE id_account = @AccountId
        AND is_deleted = 0";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);
    command.Parameters.AddWithValue("@AccountId", accountId);

    await using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
        return null;

    return new Account
    {
        IdAccount = reader.GetInt32(0),
        IdUser = reader.GetInt32(1),
        NumeroCompte = reader.GetString(2),
        TypeAccount = reader.GetString(3),
        Solde = reader.GetDecimal(4),
        Devise = reader.GetString(5),
        DateCreation = reader.GetDateTime(6),
        Statut = reader.GetString(7)
    };
 }

  public async Task<AccountDetailsViewModel?> GetAccountDetailsAsync(int accountId)
  {
    const string query = @"
        SELECT a.id_account, a.id_user, a.numero_compte,
               a.type_account, a.solde, a.devise,
               a.date_creation, a.statut,
               u.Nom, u.Prenom, u.Email
        FROM dbo.Accounts a
        INNER JOIN dbo.AspNetUsers u ON a.id_user = u.Id
        WHERE a.id_account = @AccountId
           AND a.is_deleted = 0";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);
    command.Parameters.AddWithValue("@AccountId", accountId);

    await using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
        return null;

    return new AccountDetailsViewModel
    {
        IdAccount = reader.GetInt32(0),
        IdUser = reader.GetInt32(1),
        NumeroCompte = reader.GetString(2),
        TypeAccount = reader.GetString(3),
        Solde = reader.GetDecimal(4),
        Devise = reader.GetString(5),
        DateCreation = reader.GetDateTime(6),
        Statut = reader.GetString(7),
        Nom = reader.GetString(8),
        Prenom = reader.GetString(9),
        Email = reader.IsDBNull(10) ? "" : reader.GetString(10)
    };
 } 
  public async Task<bool> UpdateStatusAsync(int accountId, string newStatus)
  {
    if (newStatus != "Active" && newStatus != "Blocked")
        return false;

    const string query = @"
        UPDATE dbo.Accounts
        SET statut = @NewStatus
        WHERE id_account = @AccountId
          AND statut IN ('Active', 'Blocked')
          AND is_deleted = 0";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);

    command.Parameters.AddWithValue("@NewStatus", newStatus);
    command.Parameters.AddWithValue("@AccountId", accountId);

    int affectedRows = await command.ExecuteNonQueryAsync();

    return affectedRows > 0;
}

  public async Task<bool> SoftDeleteAsync(int accountId)
  {
    const string query = @"
        UPDATE dbo.Accounts
        SET is_deleted = 1,
            statut = 'Closed'
        WHERE id_account = @AccountId
          AND is_deleted = 0
          AND solde = 0
          AND statut IN ('Active', 'Blocked')";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);
    command.Parameters.AddWithValue("@AccountId", accountId);

    int affectedRows = await command.ExecuteNonQueryAsync();

    return affectedRows > 0;
}

  public async Task<int> GetTotalCountAsync()
 {
    const string query = @"
        SELECT COUNT(*)
        FROM dbo.Accounts
        WHERE is_deleted = 0";

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var command =
        new SqlCommand(query, connection);

    return Convert.ToInt32(
        await command.ExecuteScalarAsync());
}
}