using System.Data;

namespace Jammer.DBContext
{
    public interface IDapperContext
    {
        public IDbConnection CreateConnection();
    }
}
