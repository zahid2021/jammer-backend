using MySql.Data.MySqlClient;
using System.Data;

namespace Jammer.DBContext
{
    public class DapperContext : IDapperContext
    {
        IConfiguration _configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }

}
