
public interface INotificationCommand
{
    void Execute();
    void Undo();
}

public class SendEmailCommand : INotificationCommand
{
    private readonly Order _order;
    public SendEmailCommand(Order order) => _order = order;
    public void Execute() => Console.WriteLine($"[EMAIL] Отправлено уведомление по заказу #{_order.Id}. Статус: {_order.Status}");
    public void Undo() => Console.WriteLine($"[EMAIL] Уведомление по заказу #{_order.Id} отменено.");
}

public class SendSmsCommand : INotificationCommand
{
    private readonly Order _order;
    public SendSmsCommand(Order order) => _order = order;
    public void Execute() => Console.WriteLine($"[SMS] Отправлено уведомление по заказу #{_order.Id}. Статус: {_order.Status}");
    public void Undo() => Console.WriteLine($"[SMS] Уведомление по заказу #{_order.Id} отменено.");
}

public class SendPushCommand : INotificationCommand
{
    private readonly Order _order;
    public SendPushCommand(Order order) => _order = order;
    public void Execute() => Console.WriteLine($"[PUSH] Отправлено уведомление по заказу #{_order.Id}. Статус: {_order.Status}");
    public void Undo() => Console.WriteLine($"[PUSH] Уведомление по заказу #{_order.Id} отменено.");
}


public class CommandHistory
{
    private readonly Stack<INotificationCommand> _history = new();

    public void Execute(INotificationCommand command)
    {
        command.Execute();
        _history.Push(command);
    }

    public void Undo()
    {
        if (_history.Count > 0)
        {
            var command = _history.Pop();
            command.Undo();
        }
        else
        {
            Console.WriteLine("Нечего отменять.");
        }
    }
}

public interface IOrderObserver
{
    void Update(Order order);
}

public class EmailNotifier : IOrderObserver
{
    private readonly CommandHistory _history;
    public EmailNotifier(CommandHistory history) => _history = history;

    public void Update(Order order)
    {
        var cmd = new SendEmailCommand(order);
        _history.Execute(cmd); // Наблюдатель создаёт и выполняет команду
    }
}

public class SmsNotifier : IOrderObserver
{
    private readonly CommandHistory _history;
    public SmsNotifier(CommandHistory history) => _history = history;

    public void Update(Order order)
    {
        var cmd = new SendSmsCommand(order);
        _history.Execute(cmd);
    }
}

public class PushNotifier : IOrderObserver
{
    private readonly CommandHistory _history;
    public PushNotifier(CommandHistory history) => _history = history;

    public void Update(Order order)
    {
        var cmd = new SendPushCommand(order);
        _history.Execute(cmd);
    }
}

public class Order
{
    public enum OrderStatus { Created, Processing, Shipped, Delivered, Cancelled }

    public int Id { get; } = new Random().Next(1000, 9999);
    private OrderStatus _status;
    private readonly List<IOrderObserver> _observers = new();

    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                NotifyObservers();
            }
        }
    }

    public void Subscribe(IOrderObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"Подписан наблюдатель: {observer.GetType().Name}");
    }

    public void Unsubscribe(IOrderObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"Отписан наблюдатель: {observer.GetType().Name}");
    }

    private void NotifyObservers()
    {
        Console.WriteLine($"\nЗаказ #{Id} изменил статус на: {Status}. Уведомляем подписчиков...");
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }
}

class Program
{
    static void Main()
    {
        var history = new CommandHistory(); 
        var order = new Order();
        
        var email = new EmailNotifier(history);
        var sms = new SmsNotifier(history);
        var push = new PushNotifier(history);

        order.Subscribe(email);
        order.Subscribe(sms);
        order.Subscribe(push);

        order.Status = Order.OrderStatus.Processing;

        order.Unsubscribe(push);
        
        order.Status = Order.OrderStatus.Shipped;
        
        Console.WriteLine("\n Отмена выполненных команд:");
        history.Undo(); 
        history.Undo(); 
        history.Undo(); 
        history.Undo(); 
    }
}