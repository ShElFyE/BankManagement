using System.IO;
using Newtonsoft.Json;
public class Program //если не вводить класс програм, то программа почему-то не работает
{
    static Bank bank = new Bank();
    static void Main(string[] args)
    {
        bank.LoadAccountsFromFile();
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("1 - Создать счет\n2 - Пополнить счет\n3 - Снять со счета\n4 - Перевести средства\n5 - Показать информацию о счете\n6 - Удалить счет\n7 - Выход");
            string input = Console.ReadLine();
            Console.WriteLine();
            switch (input)
            {
                case "1":
                    CreateAccount();
                    break;
                case "2":
                    Deposit();
                    break;
                case "3":
                    Withdraw();
                    break;
                case "4":
                    Transfer();
                    break;
                case "5":
                    ShowAccountInfo();
                    break;
                case "7":
                    exit = true;
                    break;
                case "6":
                    DeleteAccount();
                    break;
                default:
                    Console.WriteLine("Error");
                    break;
            }

            Console.WriteLine();
        }
    }
    static void CreateAccount()
    {
        Console.Write("Введите номер счета: ");
        string accountNumber = Console.ReadLine();
        Console.Write("Введите имя владельца счета: ");
        string accountOwner = Console.ReadLine();
        Console.Write("Введите начальный баланс: ");
        double initialBalance;
        if (double.TryParse(Console.ReadLine(), out initialBalance))
        {
            bank.OpenAccount(accountNumber, initialBalance, accountOwner);
            bank.SaveAccountsToFile();
        }
        else
        {
            Console.WriteLine("Некорректный ввод. Введите число для начального баланса.");
        }
    }
    static void Deposit()
    {
        Console.Write("Введите номер счета для пополнения: ");
        string accountNumber = Console.ReadLine();
        Console.Write("Введите сумму для пополнения: ");
        double amount;
        if (double.TryParse(Console.ReadLine(), out amount))
        {
            bank.Deposit(accountNumber, amount);
            bank.SaveAccountsToFile();
            Console.WriteLine($"Счет {accountNumber} пополнен на сумму {amount}.");
        }
        else
        {
            Console.WriteLine("Некорректный ввод. Введите число для суммы пополнения.");
        }
    }
    static void Withdraw()
    {
        Console.Write("Введите номер счета для снятия: ");
        string accountNumber = Console.ReadLine();
        Console.Write("Введите сумму для снятия: ");
        double amount;
        if (double.TryParse(Console.ReadLine(), out amount))
        {
            bank.Withdraw(accountNumber, amount);
            bank.SaveAccountsToFile();
            Console.WriteLine($"Со счета {accountNumber} списано {amount}.");
        }
        else
        {
            Console.WriteLine("Некорректный ввод. Введите число для суммы снятия.");
        }
    }
    static void Transfer()
    {
        Console.Write("Введите номер счета отправителя: ");
        string fromAccountNumber = Console.ReadLine();
        Console.Write("Введите номер счета получателя: ");
        string toAccountNumber = Console.ReadLine();
        Console.Write("Введите сумму для перевода: ");
        double amount;
        if (double.TryParse(Console.ReadLine(), out amount))
        {
            bank.Transfer(fromAccountNumber, toAccountNumber, amount);
            bank.SaveAccountsToFile();
            Console.WriteLine($"Перевод средств с {fromAccountNumber} на {toAccountNumber} выполнен успешно.");
        }
        else
        {
            Console.WriteLine("Некорректный ввод. Введите число для суммы перевода.");
        }
    }
    static void ShowAccountInfo()
    {
        Console.Write("Введите номер счета для просмотра информации: ");
        string accountNumber = Console.ReadLine();
        BankAccount account = bank.FindAccount(accountNumber);
        if (account != null)
        {
            Console.WriteLine(account.GetAccountInfo());
        }
        else
        {
            Console.WriteLine("Счет с таким номером не найден.");
        }
    }
    static void DeleteAccount()
    {
        Console.Write("Введите номер счета для удаления: ");
        string accountNumber = Console.ReadLine();
        bank.DeleteAccount(accountNumber);
        bank.SaveAccountsToFile();
    }
}
public class BankAccount
{
    public string AccountNum { get; set; }
    public double Balance { get; set; }
    public string AccountOwn { get; set; }
    public BankAccount(string accountNum, double initialBalance, string accountOwn)
    {
        this.AccountNum = accountNum;
        this.Balance = initialBalance;
        this.AccountOwn = accountOwn;
    }
    public double GetBalance()
    {
        return Balance;
    }
    public void Deposit(double amount)
    {
        if (amount > 0)
        {
            Balance += amount;
        }
        else
        {
            Console.WriteLine("Сумма должна быть больше нуля");
        }
    }
    public void Withdraw(double amount)
    {
        if (amount > 0)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
            }
            else
            {
                Console.WriteLine("Недостаточно средств");
            }
        }
        else
        {
            Console.WriteLine("Сумма должна быть больше нуля");
        }
    }
    public string GetAccountInfo()
    {
        return $"Номер счета: {AccountNum}, Владелец счета: {AccountOwn}, Баланс: {Balance}";
    }
    }
public class Bank
{
    private List<BankAccount> accounts;
    static string accountsFilePath = "accounts.json";
    public Bank()
    {
        accounts = new List<BankAccount>();
    }
    public void OpenAccount(string accountNum, double initialBalance, string accountOwn)
    {
        if (FindAccount(accountNum) == null)
        {
            BankAccount newAccount = new BankAccount(accountNum, initialBalance, accountOwn);
            accounts.Add(newAccount);
            Console.WriteLine("Новый счет успешно открыт.");
        }
        else
        {
            Console.WriteLine("Счет с таким номером уже существует.");
        }
    }
    public void Deposit(string accountNum, double amount)
    {
        BankAccount account = FindAccount(accountNum);
        if (account != null)
        {
            account.Deposit(amount);
        }
        else
        {
            Console.WriteLine("Счет с таким номером не найден.");
        }
    }
    public void Withdraw(string accountNum, double amount)
    {
        BankAccount account = FindAccount(accountNum);
        if (account != null)
        {
            account.Withdraw(amount);
        }
        else
        {
            Console.WriteLine("Счет с таким номером не найден.");
        }
    }
    public void Transfer(string fromAccountNum, string toAccountNum, double amount)
    {
        BankAccount fromAccount = FindAccount(fromAccountNum);
        BankAccount toAccount = FindAccount(toAccountNum);

        if (fromAccount != null && toAccount != null)
        {
            if (fromAccount.GetBalance() >= amount)
            {
                fromAccount.Withdraw(amount);
                toAccount.Deposit(amount);
            }
            else
            {
                Console.WriteLine("Недостаточно средств на счете для перевода.");
            }
        }
        else
        {
            Console.WriteLine("Один из счетов не найден.");
        }
    }
    public void PrintAccountInfo()
    {
        Console.WriteLine("Информация о счетах:");
        foreach (var account in accounts)
        {
            Console.WriteLine(account.GetAccountInfo());
        }
    }
    public BankAccount FindAccount(string accountNum)
    {
        return accounts.FirstOrDefault(acc => acc.AccountNum == accountNum);
    }
    public void DeleteAccount(string accountNum)
    {
        BankAccount account = FindAccount(accountNum);
        if (account != null)
        {
            accounts.Remove(account);
            Console.WriteLine($"Счет {accountNum} успешно удален.");
        }
        else
        {
            Console.WriteLine("Счет с таким номером не найден.");
        }
    }
    public void SaveAccountsToFile()
    {
        string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
        File.WriteAllText(accountsFilePath, json);
    }
    public void LoadAccountsFromFile()
    {
        if (File.Exists(accountsFilePath))
        {
            string json = File.ReadAllText(accountsFilePath);
            accounts = JsonConvert.DeserializeObject<List<BankAccount>>(json);
        }
    }
}
