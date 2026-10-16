using System;

namespace UniversitySystem
{
    // Абстракция: общие данные и поведение всех людей в университете.
    // Класс абстрактный - создать просто "человека" нельзя,
    // можно создать только студента или преподавателя.
    abstract class Person
    {
        // Инкапсуляция: прочитать данные можно откуда угодно,
        // а изменить - только внутри класса (private set)
        public int Id { get; private set; }
        public string FullName { get; private set; }
        public int Age { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }

        // protected - этот конструктор могут вызвать только наследники
        protected Person(int id, string fullName, int age, string phone, string email)
        {
            Id = id;
            FullName = fullName;
            Age = age;
            Phone = phone;
            Email = email;
        }

        // каждый наследник обязан сказать, кто он
        public abstract string GetRole();

        // Полиморфизм: общая часть информации,
        // наследники переопределяют метод и добавляют свои данные
        public virtual void ShowInfo()
        {
            Console.WriteLine($"{GetRole()} №{Id}");
            Console.WriteLine($"  ФИО:               {FullName}");
            Console.WriteLine($"  Возраст:           {Age}");
            Console.WriteLine($"  Телефон:           {Phone}");
            Console.WriteLine($"  Email:             {Email}");
        }

        // краткая информация в одну строку - для списков
        public override string ToString()
        {
            return $"№{Id} {FullName}, возраст {Age}";
        }
    }
}
