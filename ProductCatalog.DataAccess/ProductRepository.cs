using Dapper;
using System.Data;

namespace ProductCatalog.DataAccess
{
    public class ProductRepository : IProductRepository
    {
        // The repository now holds a private reference to an IDbConnection.
        private readonly IDbConnection _dbConnection;

        // The constructor is updated to accept the IDbConnection object.
        public ProductRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // We now use the injected _dbConnection directly.
            var sql = "SELECT Id, Name, Description, Price FROM Products";
            return await _dbConnection.QueryAsync<Product>(sql);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Name, Description, Price FROM Products WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Product product)
        {
            var sql = "INSERT INTO Products (Name, Description, Price) VALUES (@Name, @Description, @Price); SELECT SCOPE_IDENTITY();";
            var id = await _dbConnection.ExecuteScalarAsync<int>(sql, product);
            return id;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var sql = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price WHERE Id = @Id;";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, product);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
