// --- Domain Layer ---
namespace MyApp.Domain
{
    // Core Entity
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // Core Repository Interface
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);
    }
}




// --- Application Layer ---
using MyApp.Domain;

namespace MyApp.Application
{
    // Use Case Interface
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAll();
        Customer GetById(int id);
        void Create(string name, string email);
        void Update(int id, string name, string email);
        void Remove(int id);
    }

    // Use Case Implementation
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Customer> GetAll() => _repository.GetAllCustomers();

        public Customer GetById(int id) => _repository.GetCustomerById(id);

        public void Create(string name, string email)
        {
            var customer = new Customer { Id = new Random().Next(1, 1000), Name = name, Email = email };
            _repository.AddCustomer(customer);
        }

        public void Update(int id, string name, string email)
        {
            var customer = _repository.GetCustomerById(id);
            if (customer != null)
            {
                customer.Name = name;
                customer.Email = email;
                _repository.UpdateCustomer(customer);
            }
        }

        public void Remove(int id) => _repository.DeleteCustomer(id);
    }
}


// --- Infrastructure Layer ---
using MyApp.Domain;

namespace MyApp.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new();

        public IEnumerable<Customer> GetAllCustomers() => _customers;

        public Customer GetCustomerById(int id) => _customers.FirstOrDefault(c => c.Id == id);

        public void AddCustomer(Customer customer) => _customers.Add(customer);

        public void UpdateCustomer(Customer customer)
        {
            var existing = GetCustomerById(customer.Id);
            if (existing != null)
            {
                existing.Name = customer.Name;
                existing.Email = customer.Email;
            }
        }

        public void DeleteCustomer(int id)
        {
            var customer = GetCustomerById(id);
            if (customer != null)
                _customers.Remove(customer);
        }
    }
}



// --- Presentation Layer ---
using MyApp.Application;

namespace MyApp.Presentation
{
    public class ConsoleUI
    {
        private readonly ICustomerService _service;

        public ConsoleUI(ICustomerService service)
        {
            _service = service;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("1. List Customers");
                Console.WriteLine("2. Add Customer");
                Console.WriteLine("3. Update Customer");
                Console.WriteLine("4. Delete Customer");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ListCustomers();
                        break;
                    case "2":
                        AddCustomer();
                        break;
                    case "3":
                        UpdateCustomer();
                        break;
                    case "4":
                        DeleteCustomer();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        private void ListCustomers()
        {
            var customers = _service.GetAll();
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.Id}: {customer.Name} ({customer.Email})");
            }
        }

        private void AddCustomer()
        {
            Console.Write("Enter Name: ");
            var name = Console.ReadLine();
            Console.Write("Enter Email: ");
            var email = Console.ReadLine();
            _service.Create(name, email);
            Console.WriteLine("Customer added successfully.");
        }

        private void UpdateCustomer()
        {
            Console.Write("Enter Customer ID to Update: ");
            if (int.TryParse(Console.ReadLine(), out var id))
            {
                Console.Write("Enter New Name: ");
                var name = Console.ReadLine();
                Console.Write("Enter New Email: ");
                var email = Console.ReadLine();
                _service.Update(id, name, email);
                Console.WriteLine("Customer updated successfully.");
            }
        }

        private void DeleteCustomer()
        {
            Console.Write("Enter Customer ID to Delete: ");
            if (int.TryParse(Console.ReadLine(), out var id))
            {
                _service.Remove(id);
                Console.WriteLine("Customer deleted successfully.");
            }
        }
    }
}




