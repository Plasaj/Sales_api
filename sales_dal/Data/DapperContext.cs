using sales_dal.Interfaces;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace sales_dal.Data
{
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("SqlConnection");
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
