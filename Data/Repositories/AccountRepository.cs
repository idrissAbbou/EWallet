using EWallet.Models;
using Microsoft.Data.SqlClient;

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
            INSERT INTO Accounts ([IdUser], [NumeroCompte], [TypeAccount], [Solde], [Devise], [DateCreation], [Statut])
            OUTPUT INSERTED.[IdAccount]
            VALUES (@UserId, @AccountNumber, 'Courant', 0, 'MAD', GETDATE(), 'Actif');
        ";

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var command = new SqlCommand(query, connection);

    command.Parameters.AddWithValue("@UserId", userId);
    command.Parameters.AddWithValue("@AccountNumber", accountNumber);

    return (int)await command.ExecuteScalarAsync();
  }

  public async Task<Account?> GetByUserIdAsync(int userId)
  {
    const string query = @"
            SELECT [IdAccount], [IdUser], [NumeroCompte], [TypeAccount], [Solde], [Devise], [DateCreation], [Statut]
            FROM Accounts
            WHERE [IdUser] = @UserId;
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
}