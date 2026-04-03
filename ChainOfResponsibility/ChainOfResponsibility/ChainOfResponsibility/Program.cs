using System;

public class Request
{
    public string Token { get; set; } = "";
    public string Role { get; set; } = "";
    public string Data { get; set; } = "";
}

public abstract class Middleware
{
    protected Middleware? _next;
    public Middleware SetNext(Middleware next) { _next = next; return next; }
    public abstract bool Handle(Request request);

    protected bool HandleNext(Request request)
    {
        return _next?.Handle(request) ?? true;
    }
}

public class LoggingMiddleware : Middleware
{
    public override bool Handle(Request request)
    {
        Console.WriteLine($"[LOG] Входящий запрос | Токен: {(string.IsNullOrEmpty(request.Token))} | Роль: {request.Role} | Данные: {request.Data}");
        return HandleNext(request);
    }
}

public class AuthenticationMiddleware : Middleware
{
    public override bool Handle(Request request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            Console.WriteLine("[AUTH] Отклонено: отсутствует токен аутентификации.");
            return false;
        }
        Console.WriteLine("[AUTH] Аутентификация успешна.");
        return HandleNext(request);
    }
}

public class AuthorizationMiddleware : Middleware
{
    public override bool Handle(Request request)
    {
        if (request.Role != "Admin" && request.Role != "User")
        {
            Console.WriteLine("🛡️ [AUTH] Отклонено: недостаточно прав доступа.");
            return false;
        }
        Console.WriteLine($"🛡️ [AUTH] Доступ разрешён для роли: {request.Role}.");
        return HandleNext(request);
    }
}

public class ValidationMiddleware : Middleware
{
    public override bool Handle(Request request)
    {
        if (string.IsNullOrEmpty(request.Data) || request.Data.Length < 3)
        {
            Console.WriteLine("[VALID] Отклонено: данные невалидны или слишком короткие.");
            return false;
        }
        Console.WriteLine("[VALID] Валидация данных пройдена.");
        return HandleNext(request);
    }
}

public class RequestHandler : Middleware
{
    public override bool Handle(Request request)
    {
        Console.WriteLine($"[HANDLER] Запрос успешно обработан! Ответ: 200 OK. Результат: {request.Data}");
        return true;
    }
}

class Program
{
    static void Main()
    {
        var logging = new LoggingMiddleware();
        var auth = new AuthenticationMiddleware();
        var authz = new AuthorizationMiddleware();
        var valid = new ValidationMiddleware();
        var handler = new RequestHandler();

        logging.SetNext(auth).SetNext(authz).SetNext(valid).SetNext(handler);
        Middleware pipeline = logging;

        Console.WriteLine("=== Сценарий 1: Валидный запрос ===");
        Run(pipeline, new Request { Token = "valid_token_123", Role = "Admin", Data = "Order #101" });

        Console.WriteLine("\n=== Сценарий 2: Нет токена (сбой на Auth) ===");
        Run(pipeline, new Request { Token = "", Role = "Admin", Data = "Order #102" });

        Console.WriteLine("\n=== Сценарий 3: Нет прав (сбой на Authz) ===");
        Run(pipeline, new Request { Token = "valid_token", Role = "Guest", Data = "Order #103" });

        Console.WriteLine("\n=== Сценарий 4: Невалидные данные (сбой на Validation) ===");
        Run(pipeline, new Request { Token = "valid_token", Role = "User", Data = "AB" });

        Console.WriteLine("\n=== Сценарий 5: Укороченная цепочка (только Auth -> Handler) ===");
        var shortPipeline = new AuthenticationMiddleware();
        shortPipeline.SetNext(new RequestHandler());
        Run(shortPipeline, new Request { Token = "test", Role = "", Data = "" });
    }

    static void Run(Middleware chain, Request req)
    {
        bool success = chain.Handle(req);
        Console.WriteLine($"[ИТОГ] {(success ? "Запрос принят в обработку" : "Запрос отклонён")}");
    }
}