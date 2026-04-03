class Program
{
    static void Main(string[] args)
    {
        var paypal = new PayPalPaymentProcessor();
        paypal.ProcessPayment(100m);
    }
}

public interface IPaymentGateaway
{
    void ProcessPayment(decimal amount);
}

public class CreditCardGateaway : IPaymentGateaway
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Обработка {amount} через карту");
    }
}

public class PayPalGateaway : IPaymentGateaway
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Обработка {amount} через PayPal");
    }
}

public class BitcoinGateaway : IPaymentGateaway
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Обработка {amount} через биткоин");
    }
}

public abstract class PaymentProcessor 
{
    public void ProcessPayment(decimal amount)
    {
        var gateaway = CreateGateaway();

        gateaway.ProcessPayment(amount);
    }
    protected abstract IPaymentGateaway CreateGateaway();
}

public class CreditCardPaymentProcessor : PaymentProcessor
{
    protected override IPaymentGateaway CreateGateaway()
    {
        return new CreditCardGateaway();
    }
}

public class PayPalPaymentProcessor : PaymentProcessor
{
    protected override IPaymentGateaway CreateGateaway()
    {
        return new PayPalGateaway();
    }
}
public class BitcoinPaymentProcessor : PaymentProcessor
{
    protected override IPaymentGateaway CreateGateaway()
    {
        return new BitcoinGateaway();
    }
}

