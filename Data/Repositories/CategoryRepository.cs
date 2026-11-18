using EWallet.Models;

namespace EWallet.Data.Repositories;

public class CategoryRepository
{
  private readonly SqlConnectionFactory _connectionFactory;
  public CategoryRepository(SqlConnectionFactory connectionFactory)
  {
    _connectionFactory = connectionFactory;
  }

  public async Task<List<Category>> GetAllAsync()
  {
    var categories = new List<Category>();

    var conn = _connectionFactory.CreateConnection();
    await conn.OpenAsync();

    var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT * FROM Categories";

    var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
      categories.Add(new Category()
      {
        IdCategorie = reader.GetInt32(0),
        NomCategorie = reader.GetString(1),
        TypeCategorie = reader.GetString(2),
        Description = reader.IsDBNull(3)
                    ? null
                    : reader.GetString(3)
      });
    }

    return categories;
  }
}
