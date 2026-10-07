using System;
using System.Collections.Generic;

namespace TextStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isRunning = true;
            List<string> allStats = new List<string>();

            while (isRunning)
            {
                Console.WriteLine("Введите текст (не меньше 100 символов)");
                string input = Console.ReadLine();
                if (input == null || input.Length < 100)
                {
                    Console.WriteLine("В тексте меньше 100 символов");
                    continue;
                }

                string result = "";

                string[] wordsArray = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int countWords = wordsArray.Length;
                Console.WriteLine($"Количество слов: {countWords}\n");
                result += $"Количество слов: {countWords}\n";

                string minWord = wordsArray[0];
                string maxWord = wordsArray[0];
                for (int j = 1; j < wordsArray.Length; j++)
                {
                    if (wordsArray[j].Length < minWord.Length)
                    {
                        minWord = wordsArray[j];
                    }
                    if (wordsArray[j].Length > maxWord.Length)
                    {
                        maxWord = wordsArray[j];
                    }
                }
                Console.WriteLine($"Самое короткое слово: {minWord}\n");
                result += $"Самое короткое слово: {minWord}\n";

                int countSentences = input.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
                Console.WriteLine($"Количество предложений: {countSentences}\n");
                result += $"Количество предложений: {countSentences}\n";

                Console.WriteLine($"Самое длинное слово: {maxWord}\n");
                result += $"Самое длинное слово: {maxWord}\n";

                string vowelLetters = "аеёиоуыэюяaeiouyАЕЁИОУЫЭЮЯAEIOUY";

                int vowelCount = 0;
                int consonantCount = 0;

                foreach (char symbol in input)
                {
                    if (char.IsLetter(symbol))
                    {
                        bool isVowel = false;
                        foreach (char v in vowelLetters)
                        {
                            if (v == symbol)
                            {
                                isVowel = true;
                                break;
                            }
                        }

                        if (isVowel)
                        {
                            vowelCount++;
                        }
                        else
                        {
                            consonantCount++;
                        }
                    }
                }
                Console.WriteLine($"Гласные буквы: {vowelCount}");
                Console.WriteLine($"Согласные буквы: {consonantCount}\n");
                result += $"Гласные буквы: {vowelCount}\n";
                result += $"Согласные буквы: {consonantCount}\n";

                Dictionary<char, int> frequency = new Dictionary<char, int>();

                foreach (char c in input)
                {
                    if (char.IsLetter(c))
                    {
                        char letter = char.ToLower(c);

                        if (frequency.ContainsKey(letter))
                        {
                            frequency[letter]++;
                        }
                        else
                        {
                            frequency[letter] = 1;
                        }
                    }
                }
                Console.WriteLine("Частота букв:");
                result += "Частота букв:\n";
                foreach (KeyValuePair<char, int> item in frequency)
                {
                    Console.WriteLine($"Буква '{item.Key}': {item.Value}");
                    result += $"Буква '{item.Key}': {item.Value}\n";
                }

                allStats.Add(result);

                Console.WriteLine("\nПродолжить работу с новым текстом? (1 - да, 2 - нет)");
                string choice = Console.ReadLine();
                if (choice != "1")
                {
                    isRunning = false;
                }
            }

            Console.WriteLine("\nВывести статистику по прошлым текстам? (1 - да, 2 - нет)");
            string showStats = Console.ReadLine();
            if (showStats == "1")
            {
                for (int j = 0; j < allStats.Count; j++)
                {
                    Console.WriteLine($"Текст {j + 1}");
                    Console.WriteLine(allStats[j]);
                }
            }
        }
    }
}
