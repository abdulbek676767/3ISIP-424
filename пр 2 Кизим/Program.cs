using System;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        List<TextStats> history = new List<TextStats>();

        while (true)
        {
            Console.WriteLine("\n1-Ввести новый текст  2-Статистика по прошлым текстам  0-Выход");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    string text;
                    while (true)
                    {
                        Console.WriteLine("Введите текст (минимум 100 символов):");
                        text = Console.ReadLine();
                        if (text != null && text.Length >= 100) break;
                        Console.WriteLine("Текст слишком короткий, попробуйте еще раз");
                    }

                    TextStats stats = new TextStats(text);
                    history.Add(stats);
                    stats.Print();
                    break;

                case "2":
                    if (history.Count == 0)
                    {
                        Console.WriteLine("Вы еще не вводили тексты");
                        break;
                    }
                    for (int i = 0; i < history.Count; i++)
                    {
                        Console.WriteLine($"\n===== Текст №{i + 1} =====");
                        history[i].Print();
                    }
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Нет такого пункта");
                    break;
            }
        }
    }
}

class TextStats
{
    public string Text;
    public int WordsCount;
    public string ShortWord = "";
    public string LongWord = "";
    public int Sentences;
    public int Vowels;
    public int Consonants;
    public string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz";
    public int[] LetterCount;

    public TextStats(string text)
    {
        Text = text;
        LetterCount = new int[Alphabet.Length];
        CountWords();
        CountSentences();
        CountLetters();
    }

    // разбиваем текст на слова (слово - это буквы подряд)
    void CountWords()
    {
        List<string> words = new List<string>();
        string word = "";
        for (int i = 0; i < Text.Length; i++)
        {
            if (char.IsLetter(Text[i]))
            {
                word = word + Text[i];
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

        WordsCount = words.Count;

        if (words.Count > 0)
        {
            ShortWord = words[0];
            LongWord = words[0];
            for (int i = 1; i < words.Count; i++)
            {
                if (words[i].Length < ShortWord.Length)
                    ShortWord = words[i];
                if (words[i].Length > LongWord.Length)
                    LongWord = words[i];
            }
        }
    }

    // считаем предложения по знакам . ! ?
    // предложение засчитывается, если перед знаком были буквы (так ... или ?! считаются один раз)
    void CountSentences()
    {
        Sentences = 0;
        bool hasLetters = false;
        for (int i = 0; i < Text.Length; i++)
        {
            if (char.IsLetter(Text[i]))
                hasLetters = true;
            if ((Text[i] == '.' || Text[i] == '!' || Text[i] == '?') && hasLetters)
            {
                Sentences++;
                hasLetters = false;
            }
        }
        // если в конце нет точки, последнее предложение тоже считаем
        if (hasLetters)
            Sentences++;
    }

    // гласные, согласные и частота каждой буквы
    void CountLetters()
    {
        string vowelsList = "аеёиоуыэюяaeiouy";
        for (int i = 0; i < Text.Length; i++)
        {
            char c = char.ToLower(Text[i]);
            if (char.IsLetter(c))
            {
                if (vowelsList.Contains(c))
                    Vowels++;
                else if (c != 'ъ' && c != 'ь')
                    Consonants++;
            }

            int index = Alphabet.IndexOf(c);
            if (index != -1)
                LetterCount[index]++;
        }
    }

    public void Print()
    {
        Console.WriteLine($"Текст: {Text}");
        Console.WriteLine($"Количество слов: {WordsCount}");
        Console.WriteLine($"Самое короткое слово: {ShortWord}");
        Console.WriteLine($"Самое длинное слово: {LongWord}");
        Console.WriteLine($"Количество предложений: {Sentences}");
        Console.WriteLine($"Гласных букв: {Vowels}");
        Console.WriteLine($"Согласных букв: {Consonants}");
        Console.WriteLine("Частота букв:");
        for (int i = 0; i < Alphabet.Length; i++)
        {
            if (LetterCount[i] > 0)
                Console.WriteLine($"{Alphabet[i]} - {LetterCount[i]}");
        }
    }
}
