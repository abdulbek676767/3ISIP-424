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

        // разбиваем текст на слова (слово - это буквы подряд)
        List<string> words = new List<string>();
        string word = "";
        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsLetter(text[i]))
            {
                word = word + text[i];
            }
            else
            {
                if (word != "")
                {
                    words.Add(word);
                    word = "";
                }
            }
        }
        if (word != "")
            words.Add(word);

        Console.WriteLine($"Количество слов: {words.Count}");

        if (words.Count > 0)
        {
            string shortWord = words[0];
            string longWord = words[0];
            for (int i = 1; i < words.Count; i++)
            {
                if (words[i].Length < shortWord.Length)
                    shortWord = words[i];
                if (words[i].Length > longWord.Length)
                    longWord = words[i];
            }
            Console.WriteLine($"Самое короткое слово: {shortWord}");
            Console.WriteLine($"Самое длинное слово: {longWord}");
        }
    }
}
