using System;
using System.Collections.Generic;
using System.Linq;
public abstract class Operation
{
    public double Amount { get; }
    public string SourceAccount { get; }
    public string SourceClient { get; }

    protected Operation(double amount, string sourceAccount, string sourceClient)
    {
        Amount = amount;
        SourceAccount = sourceAccount;
        SourceClient = sourceClient;
    }

    public abstract void Execute(Bank bank);
}
public class Deposit : Operation
{
    public Deposit(string sourceAccount, double amount) : base(amount, sourceAccount, "") { }

    public override void Execute(Bank bank)
    {
        var account = bank.FindAccount(SourceAccount);
        if (account != null)
        {
            account.Balance += Amount;
            account.History.Add($"Depósito ${Amount} a [{SourceAccount}/{account.Client.Name}]");
        }
    }
}
public class Withdrawal : Operation
{
    public Withdrawal(string sourceAccount, double amount) : base(amount, sourceAccount, "") { }

    public override void Execute(Bank bank)
    {
        var account = bank.FindAccount(SourceAccount);
        if (account != null && account.Balance >= Amount)
        {
            account.Balance -= Amount;
            account.History.Add($"Retiro ${Amount} de [{SourceAccount}/{account.Client.Name}]");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes en la cuenta {SourceAccount}");
        }
    }
}
public class Payment : Operation
{
    public Payment(string sourceAccount, double amount) : base(amount, sourceAccount, "") { }

    public override void Execute(Bank bank)
    {
        var account = bank.FindAccount(SourceAccount);
        if (account != null && account.Balance >= Amount)
        {
            account.Balance -= Amount;
            account.AccumulatePoints(Amount);
            account.History.Add($"Pago ${Amount} con [{SourceAccount}/{account.Client.Name}]");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes en la cuenta {SourceAccount}");
        }
    }
}
public class Transfer : Operation
{
    public string DestinationAccount { get; }

    public Transfer(string sourceAccount, string destinationAccount, double amount) : base(amount, sourceAccount, "") 
    {
        DestinationAccount = destinationAccount;
    }

    public override void Execute(Bank bank)
    {
        var sourceAccount = bank.FindAccount(SourceAccount);
        var destinationAccount = bank.FindAccount(DestinationAccount);

        if (sourceAccount != null && destinationAccount != null && sourceAccount.Balance >= Amount)
        {
            sourceAccount.Balance -= Amount;
            destinationAccount.Balance += Amount;
            sourceAccount.History.Add($"Transferencia ${Amount} de [{SourceAccount}/{sourceAccount.Client.Name}] a [{DestinationAccount}/{destinationAccount.Client.Name}]");
            destinationAccount.History.Add($"Transferencia ${Amount} de [{SourceAccount}/{sourceAccount.Client.Name}] a [{DestinationAccount}/{destinationAccount.Client.Name}]");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes o cuenta de destino no encontrada en la transferencia de [{SourceAccount}] a [{DestinationAccount}]");
        }
    }
}
public abstract class Account
{
    public string Number { get; }
    public double Balance { get; set; }
    public Client Client { get; }
    public List<string> History { get; }
    private double _points;

    public double Points 
    {
        get { return _points; }
        set { _points = value; }
    }

    protected Account(string number, double balance, Client client)
    {
        Number = number;
        Balance = balance;
        Client = client;
        History = new List<string>();
    }

    public abstract void AccumulatePoints(double amount);
}
public class GoldAccount : Account
{
    public GoldAccount(string number, double balance, Client client) : base(number, balance, client) { }

    public override void AccumulatePoints(double amount)
    {
        double percentage = amount > 1000 ? 0.05 : 0.03;
        Points += amount * percentage;
    }
}

public class SilverAccount : Account
{
    public SilverAccount(string number, double balance, Client client) : base(number, balance, client) { }

    public override void AccumulatePoints(double amount)
    {
        Points += amount * 0.02;
    }
}

public class BronzeAccount : Account
{
    public BronzeAccount(string number, double balance, Client client) : base(number, balance, client) { }

    public override void AccumulatePoints(double amount)
    {
        Points += amount * 0.01;
    }
}
public class Client
{
    public string Name { get; }
    public List<Account> Accounts { get; }
    public Bank Bank { get; set; }

    public Client(string name)
    {
        Name = name;
        Accounts = new List<Account>();
    }

    public void Add(Account account)
    {
        Accounts.Add(account);
    }
}
public class Bank
{
    public string Name { get; }
    private List<Client> Clients { get; }
    private List<Operation> Operations { get; }

    public Bank(string name)
    {
        Name = name;
        Clients = new List<Client>();
        Operations = new List<Operation>();
    }

    public void Add(Client client)
    {
        Clients.Add(client);
        client.Bank = this;
    }

    public Account FindAccount(string accountNumber)
    {
        foreach (var client in Clients)
        {
            var account = client.Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account != null)
                return account;
        }
        return null;
    }

    public void Register(Operation operation)
    {
        Operations.Add(operation);
        operation.Execute(this);
    }

    public void Report()
    {
        Console.WriteLine($"Banco: {Name} | Clientes: {Clients.Count}");
        foreach (var client in Clients)
        {
            double totalBalance = client.Accounts.Sum(a => a.Balance);
            double totalPoints = client.Accounts.Sum(a => a.Points);

            Console.WriteLine($"\n  Cliente: {client.Name} | Saldo Total: $ {totalBalance:F2} | Puntos Total: $ {totalPoints:F2}");
            foreach (var account in client.Accounts)
            {
                Console.WriteLine($"    Cuenta: {account.Number} | Saldo: $ {account.Balance:F2} | Puntos: $ {account.Points:F2}");
                foreach (var operation in account.History)
                {
                    Console.WriteLine($"     -  {operation}");
                }
            }
        }
    }
}

var raul = new Client("Raul Perez");
raul.Add(new GoldAccount("10001", 1000, raul));
raul.Add(new SilverAccount("10002", 2000, raul));

var sara = new Client("Sara Lopez");
sara.Add(new SilverAccount("10003", 3000, sara));
sara.Add(new SilverAccount("10004", 4000, sara));

var luis = new Client("Luis Gomez");
luis.Add(new BronzeAccount("10005", 5000, luis));

var nac = new Bank("Banco Nac");
nac.Add(raul);
nac.Add(sara);

var tup = new Bank("Banco TUP");
tup.Add(luis);

nac.Register(new Deposit("10001", 100));
nac.Register(new Withdrawal("10002", 200));
nac.Register(new Transfer("10001", "10002", 300));
nac.Register(new Transfer("10003", "10004", 500));
nac.Register(new Payment("10002", 400));

tup.Register(new Deposit("10005", 100));
tup.Register(new Withdrawal("10005", 200));
tup.Register(new Transfer("10005", "10002", 300));
tup.Register(new Payment("10005", 400));

nac.Report();
tup.Report();