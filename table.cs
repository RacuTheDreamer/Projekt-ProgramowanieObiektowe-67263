using System.Text;
using Npgsql;
namespace DbProject
{

    public class Table(string tableName, string tableFields, string connectionString)
    {
        private bool _isTableCreatedSuccessfully = false;
        private string _tableFields = tableFields;
        private string _tableName = tableName;
        private string _connectionString = connectionString;

        public async Task CreateTableInDbAsync()
        {   

            var queryBuilder = new StringBuilder($"CREATE TABLE {_tableName} (");
            queryBuilder.Append(_tableFields);
            queryBuilder.Append(");");

            string query = queryBuilder.ToString();
            Console.WriteLine(query);
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(query, conn);
                await cmd.ExecuteNonQueryAsync();
                _isTableCreatedSuccessfully = true;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"succesfully [C]REATED TABLE {_tableName}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred:) upsie im sure its your fault, here is why (i hope it helps) {ex.Message}");
                Console.ResetColor();
            }

        }
        public async Task DeleteTableInDbAsync(string? tableName = null)
        {
            if (tableName == null)
            {
                tableName = _tableName;
            }


            Console.WriteLine(tableName);
            string query = $"DROP TABLE {tableName}";
            try
            {

                if (!_isTableCreatedSuccessfully)
                {
                    throw new InvalidOperationException("Table was not created successfully or already deleted.");
                }

                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(query, conn);
                await cmd.ExecuteNonQueryAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"succesfully [D]ELETED TABLE {tableName}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred:) upsie im sure its your fault, here is why maybe its readable {ex.Message}");
                Console.ResetColor();
            }
        }

        public async Task<int?> ExecuteCustomQueryAsync(string query)
        {
            try
            {
                if (!_isTableCreatedSuccessfully)
                {
                    Console.WriteLine("it looks like you have not created db using the [C] option so gl querying it");
                }

                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(query, conn);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Successfully queried the DB. Rows affected: {rowsAffected} if its -1 it works just certain queries return taht value check insert and see");
                Console.ResetColor();

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred:) upsie im sure its your fault, here is why maybe its readable {ex.Message}");
                Console.ResetColor();
                return null;
            }
        }
    }
}
