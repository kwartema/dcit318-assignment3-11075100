using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    // a. Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b. ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"[Electronic] ID={Id}, Name={Name}, Qty={Quantity}, Brand={Brand}, Warranty={WarrantyMonths} months";
    }

    // c. GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"[Grocery] ID={Id}, Name={Name}, Qty={Quantity}, Expiry={ExpiryDate.ToShortDateString()}";
    }

    // e. Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d. Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items.Remove(id);
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items[id].Quantity = newQuantity;
        }
    }

    // f. WarehouseManager
    public class WarehouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(1, "Rice", 50, DateTime.Now.AddMonths(6)));
            _groceries.AddItem(new GroceryItem(2, "Milk", 30, DateTime.Now.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Updated stock for {item.Name}. New quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item with ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WarehouseManager();

            manager.SeedData();

            Console.WriteLine("Grocery Items:");
            manager.PrintAllItems(manager.GroceriesRepo);

            Console.WriteLine("\nElectronic Items:");
            manager.PrintAllItems(manager.ElectronicsRepo);

            Console.WriteLine("\nTesting exceptions:");
            try
            {
                manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12)); // duplicate
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            manager.RemoveItemById(manager.GroceriesRepo, 99); // non-existent

            try
            {
                manager.ElectronicsRepo.UpdateQuantity(2, -5); // invalid quantity
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
