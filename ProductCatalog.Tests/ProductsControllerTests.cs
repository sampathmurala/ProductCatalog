using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductCatalog.Api.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProductCatalog.Tests
{
    /// <summary>
    /// Unit tests for the ProductsController.
    /// This class uses the Xunit framework for tests and Moq for mocking dependencies.
    /// </summary>
    public class ProductsControllerTests
    {
        // A mock object for the IProductRepository dependency.
        private readonly Mock<IProductRepository> _mockRepo;

        // The controller instance we are testing.
        private readonly ProductsController _controller;

        /// <summary>
        /// Initializes a new instance of the test class.
        /// This setup code runs before each test method.
        /// </summary>
        public ProductsControllerTests()
        {
            // Arrange: Create a new mock object for the repository.
            _mockRepo = new Mock<IProductRepository>();

            // Arrange: Create a new instance of the controller, injecting our mocked repository.
            // This ensures the controller uses our fake repository for its data operations.
            _controller = new ProductsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            // Create a list of dummy products that our mock repository will return.
            var expectedProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
            new Product { Id = 2, Name = "Mouse", Price = 25.00m }
        };

            // Setup the mock repository.
            // Tell the mock that when the GetAllAsync() method is called, it should return our dummy list.
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedProducts);

            // Act
            // Call the GetAll() action on our controller.
            var result = await _controller.GetAll();

            // Assert
            // 1. Verify that the result is an OkObjectResult. This confirms the controller returned a 200 OK status.
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            // 2. Verify that the value returned by the controller is a list of Products.
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            // 3. Verify that the number of products returned matches our dummy list.
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedProduct = new Product { Id = 1, Name = "Laptop", Price = 1200.00m };

            // Setup the mock to return the expected product when GetByIdAsync is called with Id 1.
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedProduct);

            // Act
            // Call the GetById() action with a valid ID.
            var result = await _controller.GetById(1);

            // Assert
            // Verify a 200 OK result and that the returned object is the correct product.
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(expectedProduct.Id, returnValue.Id);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            // Setup the mock to return null, simulating a product not being found in the database.
            _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Product)null);

            // Act
            // Call the GetById() action with an ID that we know will not be found.
            var result = await _controller.GetById(99);

            // Assert
            // Verify that the result is a NotFoundResult (404 Not Found).
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_WithValidProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newProduct = new Product { Name = "New Product", Price = 50.00m };
            var createdProductId = 3;

            // Setup the mock to return a new ID when CreateAsync is called with the new product.
            _mockRepo.Setup(repo => repo.CreateAsync(newProduct)).ReturnsAsync(createdProductId);

            // Act
            // Call the Create() action with a valid product object.
            var result = await _controller.Create(newProduct);

            // Assert
            // 1. Verify that the result is a CreatedAtActionResult.
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            // 2. Verify that the ActionName is correct (points to the GetById action).
            Assert.Equal("GetById", createdAtActionResult.ActionName);

            // 3. Verify that the returned object has the correct newly created ID.
            Assert.Equal(createdProductId, ((Product)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Update_WithExistingProduct_ReturnsNoContentResult()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Updated Product", Price = 75.00m };

            // Setup the mock to return true, indicating a successful update.
            _mockRepo.Setup(repo => repo.UpdateAsync(existingProduct)).ReturnsAsync(true);

            // Act
            // Call the Update() action with a product that exists.
            var result = await _controller.Update(1, existingProduct);

            // Assert
            // Verify that the result is a NoContentResult (204 No Content).
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WithMismatchedIds_ReturnsBadRequestResult()
        {
            // Arrange
            // Create a product object with a different ID than the one in the URL.
            var product = new Product { Id = 1, Name = "Mismatched", Price = 100.00m };

            // Act
            // Call the Update() action with mismatched IDs.
            var result = await _controller.Update(2, product);

            // Assert
            // Verify that the result is a BadRequestResult (400 Bad Request).
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_WithExistingProduct_ReturnsNoContentResult()
        {
            // Arrange
            // Setup the mock to return true, indicating a successful deletion.
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            // Call the Delete() action with an existing product ID.
            var result = await _controller.Delete(1);

            // Assert
            // Verify that the result is a NoContentResult (204 No Content).
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithNonExistingProduct_ReturnsNotFoundResult()
        {
            // Arrange
            // Setup the mock to return false, indicating the product to delete was not found.
            _mockRepo.Setup(repo => repo.DeleteAsync(99)).ReturnsAsync(false);

            // Act
            // Call the Delete() action with a non-existing product ID.
            var result = await _controller.Delete(99);

            // Assert
            // Verify that the result is a NotFoundResult (404 Not Found).
            Assert.IsType<NotFoundResult>(result);
        }
    }
}