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

    private string text = "";
    private int wordsCount;
    private string shortWord = "";
    private string longWord = "";
    private int sentences;
    private int vowels;
    private int consonants;
    private string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz";
    private int[] letterCount;

    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\n1-Ввести новый текст  2-Статистика по прошлым текстам  0-Выход");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ReadText();
                    Analyze();
                    string result = GetStatistics();
                    history.Add(result);
                    Console.WriteLine(result);
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

    private void ReadText()
    {
        while (true)
        {
            Console.WriteLine("Введите текст (минимум 100 символов):");
            text = Console.ReadLine();
            if (text != null && text.Length >= 100) break;
            Console.WriteLine("Текст слишком короткий, попробуйте еще раз");
        }
    }

    // перед новым текстом обнуляем старые значения
    private void Analyze()
    {
        wordsCount = 0;
        shortWord = "";
        longWord = "";
        sentences = 0;
        vowels = 0;
        consonants = 0;
        letterCount = new int[alphabet.Length];

        CountWords();
        CountSentences();
        CountLetters();
    }

    // разбиваем текст на слова (слово - это буквы подряд)
    private void CountWords()
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

        wordsCount = words.Count;

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
    }

    // считаем предложения по знакам . ! ?
    // предложение засчитывается, если перед знаком были буквы (так ... или ?! считаются один раз)
    private void CountSentences()
    {
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
    }

    // гласные, согласные и частота каждой буквы
    private void CountLetters()
    {
        string vowelsList = "аеёиоуыэюяaeiouy";
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

            int index = alphabet.IndexOf(c);
            if (index != -1)
                letterCount[index]++;
        }
    }

    // собираем всю статистику в одну строку, чтобы сохранить ее в список
    private string GetStatistics()
    {
        string result = $"Текст: {text}\n";
        result += $"Количество слов: {wordsCount}\n";
        result += $"Самое короткое слово: {shortWord}\n";
        result += $"Самое длинное слово: {longWord}\n";
        result += $"Количество предложений: {sentences}\n";
        result += $"Гласных букв: {vowels}\n";
        result += $"Согласных букв: {consonants}\n";
        result += "Частота букв:";
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
