using MySqlConnector;

namespace Jegyek
{
    public class DbConnection
    {
        public MySqlConnection Conn = new(new MySqlConnectionStringBuilder
        {
            Server = "192.168.50.28",
            Port = 3306,
            Database = "jegyek",
            UserID = "root",
            Password = "password"
        }.ConnectionString);
    }
}
