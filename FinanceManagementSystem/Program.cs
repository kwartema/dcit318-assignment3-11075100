using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // a. Record type to represent financial data
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b. Interface for transaction processing
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c. Concrete processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
        }
    }

    // d. Base class Account
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }

    // e. Sealed class SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
            }
        }
    }

    // f. FinanceApp
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Create SavingsAccount
            var account = new SavingsAccount("ACC001", 1000m);

            // ii. Create 3 transactions
            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 200m, "Entertainment");

            // iii. Process each transaction
            ITransactionProcessor mobileProcessor = new MobileMoneyProcessor();
            ITransactionProcessor bankProcessor = new BankTransferProcessor();
            ITransactionProcessor cryptoProcessor = new CryptoWalletProcessor();

            mobileProcessor.Process(t1);
            account.ApplyTransaction(t1);
            _transactions.Add(t1);

            bankProcessor.Process(t2);
            account.ApplyTransaction(t2);
            _transactions.Add(t2);

            cryptoProcessor.Process(t3);
            account.ApplyTransaction(t3);
            _transactions.Add(t3);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
