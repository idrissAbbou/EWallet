using Microsoft.Data.SqlClient;

namespace EWallet.Data;

public class SqlConnectionFactory
{
  private readonly string _connectionString;

  public SqlConnectionFactory(IConfiguration config)
  {
    _connectionString = config.GetConnectionString("EWalletConnection") ?? throw new InvalidOperationException("Connection string not found in the config");
  }

  public SqlConnection CreateConnection()
  {
    return new SqlConnection(_connectionString);
  }
}
