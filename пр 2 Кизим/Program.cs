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

        // считаем предложения по знакам . ! ?
        // предложение засчитывается, если перед знаком были буквы (так ... или ?! считаются один раз)
        int sentences = 0;
        bool hasLetters = false;
        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsLetter(text[i]))
                hasLetters = true;
            if ((text[i] == '.' || text[i] == '!' || text[i] == '?') && hasLetters)
            {
                sentences++;
                hasLetters = false;
            }
        }
        // если в конце нет точки, последнее предложение тоже считаем
        if (hasLetters)
            sentences++;
        Console.WriteLine($"Количество предложений: {sentences}");

        // гласные и согласные
        string vowelsList = "аеёиоуыэюяaeiouy";
        int vowels = 0;
        int consonants = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = char.ToLower(text[i]);
            if (char.IsLetter(c))
            {
                if (vowelsList.Contains(c))
                    vowels++;
                else if (c != 'ъ' && c != 'ь')
                    consonants++;
            }
        }
        Console.WriteLine($"Гласных букв: {vowels}");
        Console.WriteLine($"Согласных букв: {consonants}");
    }
}
