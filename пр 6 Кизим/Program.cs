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

    // ===== Предметы =====

    enum ItemType
    {
        Potion,
        Weapon,
        Armor
    }

    // общий класс для всего, что лежит в сундуке
    abstract class Item
    {
        public string Name { get; private set; }

        protected Item(string name)
        {
            Name = name;
        }

        public abstract string GetInfo();

        // Полиморфизм: каждый предмет сам знает, что с ним делать
        public abstract void Use(Player player);
    }

    class HealthPotion : Item
    {
        public HealthPotion() : base("Лечебное зелье") { }

        public override string GetInfo()
        {
            return $"{Name} (полностью восстанавливает здоровье)";
        }

        public override void Use(Player player)
        {
            player.Heal();
            Console.WriteLine($"Вы выпили зелье. Здоровье восстановлено: {player.Hp}/{player.MaxHp}");
        }
    }

    class Weapon : Item
    {
        public int Damage { get; private set; }

        public Weapon(string name, int damage) : base(name)
        {
            Damage = damage;
        }

        public override string GetInfo()
        {
            return $"{Name} (урон {Damage})";
        }

        // показать новое и текущее оружие и дать выбор
        public override void Use(Player player)
        {
            Console.WriteLine($"  Новое оружие:   {GetInfo()}");
            Console.WriteLine($"  Текущее оружие: {player.Weapon.GetInfo()}");
            if (ConsoleInput.ReadNumber("1 - взять новое, 2 - выбросить: ", 1, 2) == 1)
            {
                player.Equip(this);
                Console.WriteLine($"Теперь у вас в руках: {GetInfo()}");
            }
            else
                Console.WriteLine("Вы выбросили находку");
        }
    }

    class Armor : Item
    {
        public int Defense { get; private set; }

        public Armor(string name, int defense) : base(name)
        {
            Defense = defense;
        }

        public override string GetInfo()
        {
            return $"{Name} (защита {Defense})";
        }

        public override void Use(Player player)
        {
            Console.WriteLine($"  Новый доспех:   {GetInfo()}");
            Console.WriteLine($"  Текущий доспех: {player.Armor.GetInfo()}");
            if (ConsoleInput.ReadNumber("1 - надеть новый, 2 - выбросить: ", 1, 2) == 1)
            {
                player.Equip(this);
                Console.WriteLine($"Теперь на вас: {GetInfo()}");
            }
            else
                Console.WriteLine("Вы выбросили находку");
        }
    }

    // Простая фабрика: только она решает, какой именно предмет создать
    class ItemFactory
    {
        private static string[] weaponNames = { "Меч", "Топор", "Кинжал", "Булава", "Копье", "Боевой молот" };
        private static string[] armorNames = { "Кожаная куртка", "Стеганка", "Кольчуга", "Чешуйчатый доспех", "Латы" };

        public static Item CreateItem(ItemType type)
        {
            GameSettings settings = GameSettings.Instance;
            switch (type)
            {
                case ItemType.Potion:
                    return new HealthPotion();
                case ItemType.Weapon:
                    return new Weapon(RandomName(weaponNames), Dice.Range(settings.WeaponMinDamage, settings.WeaponMaxDamage));
                case ItemType.Armor:
                    return new Armor(RandomName(armorNames), Dice.Range(settings.ArmorMinDefense, settings.ArmorMaxDefense));
                default:
                    return null;
            }
        }

        // что лежит в сундуке - решает случай
        public static Item CreateRandomItem()
        {
            GameSettings settings = GameSettings.Instance;
            int roll = Dice.Range(1, 100);
            if (roll <= settings.PotionChance)
                return CreateItem(ItemType.Potion);
            if (roll <= settings.PotionChance + settings.WeaponChance)
                return CreateItem(ItemType.Weapon);
            return CreateItem(ItemType.Armor);
        }

        private static string RandomName(string[] names)
        {
            return names[Dice.Range(0, names.Length - 1)];
        }
    }

    // ===== Игрок =====

    class Player
    {
        public int MaxHp { get; private set; }
        public int Hp { get; private set; }
        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }
        public bool IsDefending { get; private set; }
        public bool IsFrozen { get; private set; }

        public Player()
        {
            GameSettings settings = GameSettings.Instance;
            MaxHp = settings.PlayerMaxHp;
            Hp = MaxHp;
            Weapon = new Weapon("Деревянная палка", settings.StartWeaponDamage);
            Armor = new Armor("Рваная рубаха", settings.StartArmorDefense);
        }

        public bool IsAlive
        {
            get { return Hp > 0; }
        }

        public void Heal()
        {
            Hp = MaxHp;
        }

        // одно оружие и один доспех одновременно: новый предмет заменяет старый
        public void Equip(Weapon weapon)
        {
            Weapon = weapon;
        }

        public void Equip(Armor armor)
        {
            Armor = armor;
        }

        // урон игрока: урон оружия со случайным разбросом
        public int RollDamage()
        {
            GameSettings settings = GameSettings.Instance;
            return Weapon.Damage * Dice.Range(settings.DamageMinPercent, settings.DamageMaxPercent) / 100;
        }

        public void Defend()
        {
            IsDefending = true;
        }

        public void Freeze()
        {
            IsFrozen = true;
        }

        public void Unfreeze()
        {
            IsFrozen = false;
        }

        // удар врага по игроку; false - если игрок уклонился
        public bool TakeHit(int damage, bool ignoreArmor)
        {
            GameSettings settings = GameSettings.Instance;
            if (IsDefending)
            {
                IsDefending = false; // защита действует только на одну атаку

                if (Dice.Chance(settings.DodgeChance))
                {
                    Console.WriteLine("  Вы уклонились от атаки!");
                    return false;
                }

                if (ignoreArmor)
                {
                    Console.WriteLine("  Враг игнорирует вашу защиту - блок не сработал!");
                }
                else
                {
                    int block = Armor.Defense * Dice.Range(settings.BlockMinPercent, settings.BlockMaxPercent) / 100;
                    damage = Math.Max(0, damage - block);
                    Console.WriteLine($"  Уклониться не вышло, но блок поглотил {block} урона");
                }
            }

            Hp = Math.Max(0, Hp - damage);
            Console.WriteLine($"  Вы получили {damage} урона. Здоровье: {Hp}/{MaxHp}");
            return true;
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Здоровье: {Hp}/{MaxHp}");
            Console.WriteLine($"Оружие:   {Weapon.GetInfo()}");
            Console.WriteLine($"Доспех:   {Armor.GetInfo()}");
        }
    }

    // ===== Враги =====

    // Абстракция: общие характеристики всех врагов - здоровье, атака, защита
    abstract class Enemy
    {
        public string Name { get; private set; }
        public int MaxHp { get; private set; }
        public int Hp { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public bool IsBoss { get; private set; }

        protected Enemy(string name, int hp, int attack, int defense, bool isBoss)
        {
            Name = name;
            MaxHp = hp;
            Hp = hp;
            Attack = attack;
            Defense = defense;
            IsBoss = isBoss;
        }

        public bool IsAlive
        {
            get { return Hp > 0; }
        }

        public abstract string GetRace();
        public abstract string GetAbility();

        // для боссов: базовая характеристика, умноженная на множитель
        protected static int Scale(int value, double multiplier)
        {
            return (int)Math.Round(value * multiplier);
        }

        // игрок бьет врага: защита врага уменьшает урон, но хотя бы 1 урон проходит
        public int TakeDamage(int damage)
        {
            int realDamage = Math.Max(1, damage - Defense);
            Hp = Math.Max(0, Hp - realDamage);
            return realDamage;
        }

        // атака врага: порядок у всех одинаковый,
        // а особенности (крит, игнор брони, заморозка) у каждого свои - полиморфизм
        public void AttackPlayer(Player player)
        {
            GameSettings settings = GameSettings.Instance;
            int damage = Math.Max(1, Attack * Dice.Range(settings.DamageMinPercent, settings.DamageMaxPercent) / 100);

            Console.WriteLine($"{Name} атакует!");
            damage = ApplyCrit(damage);
            bool hit = player.TakeHit(damage, IgnoresArmor());
            if (hit && player.IsAlive)
                AfterHit(player);
        }

        protected virtual int ApplyCrit(int damage)
        {
            return damage;
        }

        protected virtual bool IgnoresArmor()
        {
            return false;
        }

        protected virtual void AfterHit(Player player)
        {
        }

        // заморозка - общая для магов и Пестова
        protected void TryFreeze(Player player, int chance)
        {
            if (Dice.Chance(chance))
            {
                player.Freeze();
                Console.WriteLine($"  {Name} наложил заморозку! Вы пропустите следующий ход");
            }
        }

        public void ShowInfo()
        {
            string title = IsBoss ? "БОСС" : "Враг";
            Console.WriteLine($"{title}: {Name} (раса: {GetRace()})");
            Console.WriteLine($"  Здоровье: {Hp}   Атака: {Attack}   Защита: {Defense}");
            Console.WriteLine($"  Особенность: {GetAbility()}");
        }
    }

    // Гоблин - шанс критического урона
    class Goblin : Enemy
    {
        public int CritChance { get; private set; }

        public Goblin() : this("Гоблин", 1.0, 1.0, 1.0, 0, false) { }

        // protected - этот конструктор нужен боссам-гоблинам: множители к базовым характеристикам
        protected Goblin(string name, double hpMultiplier, double attackMultiplier, double defenseMultiplier, int critBonus, bool isBoss)
            : base(name,
                   Scale(GameSettings.Instance.GoblinHp, hpMultiplier),
                   Scale(GameSettings.Instance.GoblinAttack, attackMultiplier),
                   Scale(GameSettings.Instance.GoblinDefense, defenseMultiplier),
                   isBoss)
        {
            CritChance = GameSettings.Instance.GoblinCritChance + critBonus;
        }

        public override string GetRace()
        {
            return "гоблин";
        }

        public override string GetAbility()
        {
            return $"шанс критического удара {CritChance}% (урон x{GameSettings.Instance.CritMultiplier})";
        }

        protected override int ApplyCrit(int damage)
        {
            if (!Dice.Chance(CritChance))
                return damage;

            Console.WriteLine("  Критический удар!");
            return (int)(damage * GameSettings.Instance.CritMultiplier);
        }
    }

    // Скелет - игнорирует защиту игрока
    class Skeleton : Enemy
    {
        public Skeleton() : this("Скелет", 1.0, 1.0, 1.0, false) { }

        protected Skeleton(string name, double hpMultiplier, double attackMultiplier, double defenseMultiplier, bool isBoss)
            : base(name,
                   Scale(GameSettings.Instance.SkeletonHp, hpMultiplier),
                   Scale(GameSettings.Instance.SkeletonAttack, attackMultiplier),
                   Scale(GameSettings.Instance.SkeletonDefense, defenseMultiplier),
                   isBoss)
        {
        }

        public override string GetRace()
        {
            return "скелет";
        }

        public override string GetAbility()
        {
            return "игнорирует защиту игрока";
        }

        protected override bool IgnoresArmor()
        {
            return true;
        }
    }

    // Маг - шанс наложить заморозку (игрок пропускает ход)
    class Mage : Enemy
    {
        public int FreezeChance { get; private set; }

        public Mage() : this("Маг", 1.0, 1.0, 1.0, 0, false) { }

        protected Mage(string name, double hpMultiplier, double attackMultiplier, double defenseMultiplier, int freezeBonus, bool isBoss)
            : base(name,
                   Scale(GameSettings.Instance.MageHp, hpMultiplier),
                   Scale(GameSettings.Instance.MageAttack, attackMultiplier),
                   Scale(GameSettings.Instance.MageDefense, defenseMultiplier),
                   isBoss)
        {
            FreezeChance = GameSettings.Instance.MageFreezeChance + freezeBonus;
        }

        public override string GetRace()
        {
            return "маг";
        }

        public override string GetAbility()
        {
            return $"шанс заморозки {FreezeChance}% (вы пропускаете ход)";
        }

        protected override void AfterHit(Player player)
        {
            TryFreeze(player, FreezeChance);
        }
    }

    // ===== Боссы =====
    // Наследование: каждый босс - это враг своей расы с усиленными характеристиками

    // ВВГ (гоблин): здоровье x2.0, атака x1.5, защита x1.2, крит +10%
    class Vvg : Goblin
    {
        public Vvg() : base("ВВГ", 2.0, 1.5, 1.2, 10, true) { }
    }

    // Ковальский (скелет): здоровье x2.5, атака x1.3, защита x1.4
    class Kowalski : Skeleton
    {
        public Kowalski() : base("Ковальский", 2.5, 1.3, 1.4, true) { }
    }

    // Архимаг C++ (маг): здоровье x1.8, атака x1.6, защита x1.1, заморозка +10%
    class ArchMage : Mage
    {
        public ArchMage() : base("Архимаг C++", 1.8, 1.6, 1.1, 10, true) { }
    }

    // Пестов С-- (скелет): здоровье x1.3, атака x1.8, защита x0.6,
    // игнорирует защиту, как скелет, и может заморозить: шанс мага +15%
    class Pestov : Skeleton
    {
        public int FreezeChance { get; private set; }

        public Pestov() : base("Пестов С--", 1.3, 1.8, 0.6, true)
        {
            FreezeChance = GameSettings.Instance.MageFreezeChance + 15;
        }

        public override string GetAbility()
        {
            return base.GetAbility() + $", шанс заморозки {FreezeChance}%";
        }

        protected override void AfterHit(Player player)
        {
            TryFreeze(player, FreezeChance);
        }
    }

    enum EnemyType
    {
        Goblin,
        Skeleton,
        Mage
    }

    enum BossType
    {
        Vvg,
        Kowalski,
        ArchMage,
        Pestov
    }

    // Простая фабрика: создание врагов собрано в одном месте,
    // поэтому новый тип врага добавляется одной строкой case
    class EnemyFactory
    {
        public static Enemy CreateEnemy(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Goblin:
                    return new Goblin();
                case EnemyType.Skeleton:
                    return new Skeleton();
                case EnemyType.Mage:
                    return new Mage();
                default:
                    return null;
            }
        }

        public static Enemy CreateBoss(BossType type)
        {
            switch (type)
            {
                case BossType.Vvg:
                    return new Vvg();
                case BossType.Kowalski:
                    return new Kowalski();
                case BossType.ArchMage:
                    return new ArchMage();
                case BossType.Pestov:
                    return new Pestov();
                default:
                    return null;
            }
        }

        // тип врага выбирается случайно из всех значений перечисления
        public static Enemy CreateRandomEnemy()
        {
            Array types = Enum.GetValues(typeof(EnemyType));
            return CreateEnemy((EnemyType)types.GetValue(Dice.Range(0, types.Length - 1)));
        }

        public static Enemy CreateRandomBoss()
        {
            Array types = Enum.GetValues(typeof(BossType));
            return CreateBoss((BossType)types.GetValue(Dice.Range(0, types.Length - 1)));
        }
    }
}
