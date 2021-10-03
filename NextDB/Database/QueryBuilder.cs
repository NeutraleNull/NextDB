using MySql.Data.MySqlClient;

namespace NextDB.Database
{
    public class QueryBuilder
    {
        public void BuildQuery(string query, MySqlConnection connection)
        {
            var command = new MySqlCommand(query, connection);
            command.Parameters.Add(new MySqlParameter());
        }
    }
}