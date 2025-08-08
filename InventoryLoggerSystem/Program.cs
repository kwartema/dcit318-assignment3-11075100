using System;
using System.IO;

namespace InventoryLoggerSystem
{
    public class InventoryLogger
    {
        private readonly string logFilePath;

        public InventoryLogger(string filePath)
        {
            logFilePath = filePath;
        }

        public void LogItem(string itemName, int quantity)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Item: {itemName} | Quantity: {quantity}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            Console.WriteLine("Item logged successfully.");
        }

        public void DisplayLog()
        {
            if (File.Exists(logFilePath))
            {
                Console.WriteLine("\n--- Inventory Log ---");
                string[] logs = File.ReadAllLines(logFilePath);
                foreach (var log in logs)
                {
                    Console.WriteLine(log);
                }
            }
            else
            {
                Console.WriteLine("No log file found.");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventory_log.txt");
            InventoryLogger logger = new InventoryLogger(logFile);

            while (true)
            {
                Console.WriteLine("\n1. Log new item");
                Console.WriteLine("2. View log");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter item name: ");
                        string name = Console.ReadLine();

                        Console.Write("Enter quantity: ");
                        if (int.TryParse(Console.ReadLine(), out int qty))
                        {
                            logger.LogItem(name, qty);
                        }
                        else
                        {
                            Console.WriteLine("Invalid quantity.");
                        }
                        break;

                    case "2":
                        logger.DisplayLog();
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
