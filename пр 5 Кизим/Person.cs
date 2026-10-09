using System;

// базовый класс для всех людей в университете (абстракция)
// от него наследуются Student и Teacher
abstract class Person
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public int Age { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }

    public Person(int id, string fullName, int age, string phone, string email)
    {
        Id = id;
        FullName = fullName;
        Age = age;
        Phone = phone;
        Email = email;
    }

    // каждый наследник сам говорит, кто он
    public abstract string GetRole();

    // общая информация, наследники дополняют ее своей (полиморфизм)
    public virtual void ShowInfo()
    {
        Console.WriteLine($"[{GetRole()}] №{Id}");
        Console.WriteLine($"  ФИО:      {FullName}");
        Console.WriteLine($"  Возраст:  {Age}");
        Console.WriteLine($"  Телефон:  {Phone}");
        Console.WriteLine($"  Email:    {Email}");
    }

    // короткая строка для списков
    public virtual string GetShortInfo()
    {
        return $"№{Id} {FullName}, {Age} лет";
    }
}
