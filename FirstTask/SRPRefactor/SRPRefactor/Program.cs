// Пример нарушения SRP
// var user = new User { Name = "Иван", Email = "ivan@test.com" };
// user.Validate();
// user.Save();
// user.SendEmail();
//
// internal class User
// {
//     public string Name { get; set; }
//     public string Email { get; set; }
//
//     public void Validate()
//     {
//         if (string.IsNullOrEmpty(Name)) throw new Exception("Нет имени");
//     }
//
//     public void Save()
//     {
//         Console.WriteLine($"Сохраняю {Name} в базу данных...");
//     }
//     
//     public void SendEmail()
//     {
//         Console.WriteLine($"Отправляю письмо на {Email}...");
//     }
// }


var user = new User { Name = "Иван", Email = "ivan@test.com" };
        
var repo = new UserRepository();
var emailService = new EmailService();
var validator = new Validator();

validator.Validate(user);
repo.Save(user);
emailService.Send(user);

class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

internal class UserRepository
{
    public void Save(User user)
    {
        Console.WriteLine($"[БД] Сохранен: {user.Name}");
    }
}

internal class EmailService
{
    public void Send(User user)
    {
        Console.WriteLine($"[Email] Отправлено: {user.Email}");
    }
}

internal class Validator
{
    public bool Validate(User user)
    {
        return string.IsNullOrEmpty(user.Name);
    }
}