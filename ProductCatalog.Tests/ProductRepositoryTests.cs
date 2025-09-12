using Moq;
using Dapper;
using System.Data;
using Moq.Dapper;
using ProductCatalog.DataAccess;
using System.Data.Common;

namespace ProductCatalog.Tests
{
    /// <summary>
    /// Unit tests for the ProductRepository class.
    /// This test class uses Moq to simulate the IDbConnection and isolate the repository's logic.
    /// </summary>
    public class ProductRepositoryTests
    {
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            _mockConnection = new Mock<IDbConnection>();
            _repository = new ProductRepository(_mockConnection.Object);
        }

        [Fact(Skip = "Skipping test")]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Description = "Portable computer", Price = 1200.00m },
            new Product { Id = 2, Name = "Keyboard", Description = "Mechanical keyboard", Price = 150.00m }
        };

            // Use Moq.Dapper to set up the mock. This tells the mock connection
            // to return our predefined list of products when QueryAsync is called.
            _mockConnection.SetupDapper(c => c.QueryAsync<Product>(It.IsAny<string>(), null, null, null, null))
                           .ReturnsAsync(expectedProducts);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(result);
            Assert.Equal(2, productList.Count());
        }

        [Fact(Skip = "Skipping test")]
        public async Task GetByIdAsync_ReturnsCorrectProduct()
        {
            // Arrange
            var expectedProduct = new Product { Id = 1, Name = "Laptop", Price = 1200.00m };

            // Setup the mock to return a single product when QuerySingleOrDefaultAsync is called.
            _mockConnection.SetupDapper(c => c.QuerySingleOrDefaultAsync<Product>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                           .ReturnsAsync(expectedProduct);

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Laptop", result.Name);
        }
        [Fact(Skip = "Skipping test")]
        public async Task CreateAsync_CreatesProductAndReturnsId()
        {
            // Arrange
            var newProduct = new Product { Name = "Mouse", Description = "Wireless mouse", Price = 50.00m };
            var expectedId = 3;

            // Setup the mock to return the expected ID for ExecuteScalarAsync<int>.
            _mockConnection.SetupDapper(c => c.ExecuteScalarAsync<int>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()
                ))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _repository.CreateAsync(newProduct);

            // Assert
            Assert.Equal(expectedId, result);
        }

        [Fact(Skip = "Skipping test")]
        public async Task UpdateAsync_UpdatesProductAndReturnsTrue()
        {
            // Arrange
            var productToUpdate = new Product { Id = 1, Name = "Laptop", Description = "Updated", Price = 1300.00m };

            _mockConnection.SetupDapper(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                           .ReturnsAsync(1);

            // Act
            var result = await _repository.UpdateAsync(productToUpdate);

            // Assert
            Assert.True(result);
        }

        [Fact(Skip = "Skipping test")]
        public async Task DeleteAsync_DeletesProductAndReturnsTrue()
        {
            // Arrange
            var productId = 1;

            _mockConnection.SetupDapper(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                           .ReturnsAsync(1);

            // Act
            var result = await _repository.DeleteAsync(productId);

            // Assert
            Assert.True(result);
        }
    }
}