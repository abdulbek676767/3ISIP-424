using System;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        TextAnalyzer analyzer = new TextAnalyzer();
        analyzer.Start();
    }
}

class TextAnalyzer
{
    // сюда сохраняется статистика по всем текстам
    private List<string> history = new List<string>();

    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\n1-Ввести новый текст  2-Статистика по прошлым текстам  0-Выход");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    string text = ReadText();
                    Analyze(text);
                    break;

                case "2":
                    ShowHistory();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Нет такого пункта");
                    break;
            }
        }
    }

    private string ReadText()
    {
        while (true)
        {
            Console.WriteLine("Введите текст (минимум 100 символов):");
            string text = Console.ReadLine();
            if (text != null && text.Length >= 100)
                return text;
            Console.WriteLine("Текст слишком короткий, попробуйте еще раз");
        }
    }

    // считаем всю статистику, выводим ее и сохраняем в список
    private void Analyze(string text)
    {
        string result = $"Текст: {text}\n";
        result += CountWords(text);
        result += CountSentences(text);
        result += CountVowels(text);
        result += LetterFrequency(text);

        Console.WriteLine(result);
        history.Add(result);
    }

    // разбиваем текст на слова (слово - это буквы подряд)
    private string CountWords(string text)
    {
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

        string shortWord = "";
        string longWord = "";
        if (words.Count > 0)
        {
            shortWord = words[0];
            longWord = words[0];
            for (int i = 1; i < words.Count; i++)
            {
                if (words[i].Length < shortWord.Length)
                    shortWord = words[i];
                if (words[i].Length > longWord.Length)
                    longWord = words[i];
            }
        }

        string result = $"Количество слов: {words.Count}\n";
        result += $"Самое короткое слово: {shortWord}\n";
        result += $"Самое длинное слово: {longWord}\n";
        return result;
    }

    // считаем предложения по знакам . ! ?
    // предложение засчитывается, если перед знаком были буквы (так ... или ?! считаются один раз)
    private string CountSentences(string text)
    {
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

        return $"Количество предложений: {sentences}\n";
    }

    // гласные и согласные
    private string CountVowels(string text)
    {
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

        string result = $"Гласных букв: {vowels}\n";
        result += $"Согласных букв: {consonants}\n";
        return result;
    }

    // частота каждой буквы
    private string LetterFrequency(string text)
    {
        string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz";
        int[] letterCount = new int[alphabet.Length];
        for (int i = 0; i < text.Length; i++)
        {
            char c = char.ToLower(text[i]);
            int index = alphabet.IndexOf(c);
            if (index != -1)
                letterCount[index]++;
        }

        string result = "Частота букв:";
        for (int i = 0; i < alphabet.Length; i++)
        {
            if (letterCount[i] > 0)
                result += $"\n{alphabet[i]} - {letterCount[i]}";
        }
        return result;
    }

    private void ShowHistory()
    {
        if (history.Count == 0)
        {
            Console.WriteLine("Вы еще не вводили тексты");
            return;
        }
        for (int i = 0; i < history.Count; i++)
        {
            Console.WriteLine($"\n===== Текст №{i + 1} =====");
            Console.WriteLine(history[i]);
        }
    }
}
