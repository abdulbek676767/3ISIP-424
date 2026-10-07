using System;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        string text;
        while (true)
        {
            Console.WriteLine("Введите текст (минимум 100 символов):");
            text = Console.ReadLine();
            if (text != null && text.Length >= 100) break;
            Console.WriteLine("Текст слишком короткий, попробуйте еще раз");
        }

        Console.WriteLine($"Длина текста: {text.Length} символов");
    }
}
