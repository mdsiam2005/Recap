

// --- Data Layer (Repository) ---
namespace MyApp.Data
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public IEnumerable<Product> GetAll() => _products;

        public Product GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        public void Add(Product product) => _products.Add(product);

        public void Update(Product product)
        {
            var existing = GetById(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Price = product.Price;
            }
        }

        public void Delete(int id)
        {
            var product = GetById(id);
            if (product != null)
                _products.Remove(product);
        }
    }
}

// --- Business Layer (Service) ---
namespace MyApp.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        void RemoveProduct(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Product> GetAllProducts() => _repository.GetAll();

        public Product GetProductById(int id) => _repository.GetById(id);

        public void CreateProduct(Product product) => _repository.Add(product);

        public void UpdateProduct(Product product) => _repository.Update(product);

        public void RemoveProduct(int id) => _repository.Delete(id);
    }
}

// --- Presentation Layer (Controller) ---
namespace MyApp.Controllers
{
    public class ProductController
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        public void ListProducts()
        {
            var products = _service.GetAllProducts();
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id}: {product.Name} - {product.Price:C}");
            }
        }

        public void AddProduct(string name, decimal price)
        {
            var newProduct = new Product { Id = new Random().Next(1, 1000), Name = name, Price = price };
            _service.CreateProduct(newProduct);
            Console.WriteLine("Product added successfully.");
        }

        public void DeleteProduct(int id)
        {
            _service.RemoveProduct(id);
            Console.WriteLine($"Product with ID {id} removed.");
        }
    }
}

// --- Dependency Injection Setup ---
using Microsoft.Extensions.DependencyInjection;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Dependency Injection container
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<IProductService, ProductService>()
                .AddTransient<ProductController>()
                .BuildServiceProvider();

            // Example Usage
            var controller = serviceProvider.GetService<ProductController>();
            controller.AddProduct("Sample Product", 99.99m);
            controller.ListProducts();
        }
    }
}
