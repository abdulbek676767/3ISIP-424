using System;
using System.Collections.Generic;
using System.Linq;

namespace RoguelikeGame
{
    // ===== Настройки игры (паттерн Singleton) =====

    // настройки, которые можно изменить из меню
    enum SettingType
    {
        ChestChance,
        DodgeChance,
        GoblinCritChance,
        MageFreezeChance,
        BossEveryTurns
    }

    // Singleton: у игры ровно один экземпляр настроек и одна точка доступа к нему - GameSettings.Instance.
    // Здесь собраны все шансы и случайные величины, чтобы менять баланс игры в одном месте.
    class GameSettings
    {
        // единственный экземпляр, создается один раз
        private static readonly GameSettings _instance = new GameSettings();

        // приватный конструктор - создать второй экземпляр нигде нельзя
        private GameSettings() { }

        public static GameSettings Instance
        {
            get { return _instance; }
        }

        // события хода
        public int ChestChance { get; private set; } = 50;      // шанс сундука, иначе враг (%)
        public int BossEveryTurns { get; private set; } = 10;   // босс каждые N ходов

        // игрок
        public int PlayerMaxHp { get; private set; } = 100;
        public int StartWeaponDamage { get; private set; } = 10;
        public int StartArmorDefense { get; private set; } = 3;
        public int DodgeChance { get; private set; } = 40;      // шанс уклониться в защите (%)
        public int BlockMinPercent { get; private set; } = 70;  // блок поглощает от 70%
        public int BlockMaxPercent { get; private set; } = 100; // до 100% от защиты доспеха

        // разброс любого удара: от 80% до 120% от атаки
        public int DamageMinPercent { get; private set; } = 80;
        public int DamageMaxPercent { get; private set; } = 120;

        // гоблин
        public int GoblinHp { get; private set; } = 25;
        public int GoblinAttack { get; private set; } = 7;
        public int GoblinDefense { get; private set; } = 1;
        public int GoblinCritChance { get; private set; } = 20; // (%)
        public double CritMultiplier { get; private set; } = 2.0;

        // скелет
        public int SkeletonHp { get; private set; } = 30;
        public int SkeletonAttack { get; private set; } = 6;
        public int SkeletonDefense { get; private set; } = 2;

        // маг
        public int MageHp { get; private set; } = 20;
        public int MageAttack { get; private set; } = 9;
        public int MageDefense { get; private set; } = 0;
        public int MageFreezeChance { get; private set; } = 25; // (%)

        // что выпадает из сундука (остальное - доспех)
        public int PotionChance { get; private set; } = 30;     // (%)
        public int WeaponChance { get; private set; } = 35;     // (%)
        public int WeaponMinDamage { get; private set; } = 8;
        public int WeaponMaxDamage { get; private set; } = 20;
        public int ArmorMinDefense { get; private set; } = 2;
        public int ArmorMaxDefense { get; private set; } = 10;

        // изменить настройку; false - если значение вне допустимых границ
        public bool Change(SettingType type, int value)
        {
            switch (type)
            {
                case SettingType.ChestChance:
                    if (!IsPercent(value)) return false;
                    ChestChance = value;
                    return true;
                case SettingType.DodgeChance:
                    if (!IsPercent(value)) return false;
                    DodgeChance = value;
                    return true;
                case SettingType.GoblinCritChance:
                    if (!IsPercent(value)) return false;
                    GoblinCritChance = value;
                    return true;
                case SettingType.MageFreezeChance:
                    if (!IsPercent(value)) return false;
                    MageFreezeChance = value;
                    return true;
                case SettingType.BossEveryTurns:
                    if (value < 2 || value > 50) return false;
                    BossEveryTurns = value;
                    return true;
                default:
                    return false;
            }
        }

        private bool IsPercent(int value)
        {
            return value >= 0 && value <= 100;
        }

        public void ShowInfo()
        {
            Console.WriteLine("--- Текущие настройки ---");
            Console.WriteLine($"  Шанс сундука (иначе враг):  {ChestChance}%");
            Console.WriteLine($"  Босс появляется каждые:     {BossEveryTurns} ходов");
            Console.WriteLine($"  Здоровье игрока:            {PlayerMaxHp}");
            Console.WriteLine($"  Шанс уклонения в защите:    {DodgeChance}%");
            Console.WriteLine($"  Блок в защите:              {BlockMinPercent}-{BlockMaxPercent}% от защиты доспеха");
            Console.WriteLine($"  Разброс урона:              {DamageMinPercent}-{DamageMaxPercent}% от атаки");
            Console.WriteLine($"  Гоблин:  HP {GoblinHp}, атака {GoblinAttack}, защита {GoblinDefense}, крит {GoblinCritChance}% (x{CritMultiplier})");
            Console.WriteLine($"  Скелет:  HP {SkeletonHp}, атака {SkeletonAttack}, защита {SkeletonDefense}, игнорирует защиту игрока");
            Console.WriteLine($"  Маг:     HP {MageHp}, атака {MageAttack}, защита {MageDefense}, заморозка {MageFreezeChance}%");
            Console.WriteLine($"  Сундук:  зелье {PotionChance}%, оружие {WeaponChance}%, доспех {100 - PotionChance - WeaponChance}%");
            Console.WriteLine($"  Оружие:  урон {WeaponMinDamage}-{WeaponMaxDamage}, доспех: защита {ArmorMinDefense}-{ArmorMaxDefense}");
        }
    }

    // все случайности игры проходят через один генератор
    static class Dice
    {
        private static Random random = new Random();

        // true с вероятностью percent процентов
        public static bool Chance(int percent)
        {
            return random.Next(100) < percent;
        }

        // случайное целое число от min до max включительно
        public static int Range(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }

    // ввод с проверкой, чтобы программа не вылетала
    static class ConsoleInput
    {
        public static int ReadNumber(string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                if (input == null)
                    Environment.Exit(0); // ввод закончился (Ctrl+Z) - закрываем программу

                int number;
                if (!int.TryParse(input.Trim(), out number))
                    Console.WriteLine("Ошибка: введите целое число");
                else if (number < min || number > max)
                    Console.WriteLine($"Ошибка: введите число от {min} до {max}");
                else
                    return number;
            }
        }
    }
}
