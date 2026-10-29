using System;

namespace Rogalik
{
    // Singleton - все шансы и случайные величины игры хранятся в одном месте
    class Settings
    {
        private static readonly Settings _instance = new Settings();
        private Settings() { }
        public static Settings Instance { get { return _instance; } }
        private Random rnd = new Random();
        public readonly int ChestChance = 50, BossEvery = 10;
        public readonly int PlayerHp = 100, DodgeChance = 40, BlockMin = 70, BlockMax = 100;
        public readonly int DmgMin = 80, DmgMax = 120; // разброс урона в %
        public readonly int GoblinHp = 25, GoblinAtk = 7, GoblinDef = 1, GoblinCrit = 20;
        public readonly int SkeletonHp = 30, SkeletonAtk = 6, SkeletonDef = 2;
        public readonly int MageHp = 20, MageAtk = 9, MageDef = 0, MageFreeze = 25;
        public readonly int PotionChance = 30, WeaponChance = 35, WeaponMin = 8, WeaponMax = 20, ArmorMin = 2, ArmorMax = 10;
        public bool Chance(int percent) => rnd.Next(100) < percent;
        public int Range(int min, int max) => rnd.Next(min, max + 1);
        public int Spread(int value) => value * Range(DmgMin, DmgMax) / 100;
    }
    static class Input
    {
        public static int Number(string text, int min, int max)
        {
            while (true)
            {
                Console.Write(text);
                string line = Console.ReadLine();
                if (line == null) Environment.Exit(0);
                if (int.TryParse(line, out int n) && n >= min && n <= max) return n;
                Console.WriteLine($"Ошибка! Введите число от {min} до {max}");
            }
        }
    }

    // ---------- Предметы ----------
    enum ItemType { Potion, Weapon, Armor }
    abstract class Item
    {
        public string Name { get; }
        protected Item(string name) { Name = name; }
        public abstract void Use(Player player);
        public override string ToString() => Name;
    }
    class Potion : Item
    {
        public Potion() : base("Лечебное зелье") { }
        public override void Use(Player player) { player.Heal(); Console.WriteLine($"Здоровье восстановлено: {player.Hp}"); }
    }
    class Weapon : Item
    {
        public int Damage { get; }
        public Weapon(string name, int damage) : base(name) { Damage = damage; }
        public override string ToString() => $"{Name} (урон {Damage})";
        public override void Use(Player player)
        {
            Console.WriteLine($"Новое оружие: {this}\nТекущее:      {player.Weapon}");
            if (Input.Number("1 - взять, 2 - выбросить: ", 1, 2) == 1) player.Equip(this);
        }
    }
    class Armor : Item
    {
        public int Defense { get; }
        public Armor(string name, int defense) : base(name) { Defense = defense; }
        public override string ToString() => $"{Name} (защита {Defense})";
        public override void Use(Player player)
        {
            Console.WriteLine($"Новый доспех: {this}\nТекущий:      {player.Armor}");
            if (Input.Number("1 - надеть, 2 - выбросить: ", 1, 2) == 1) player.Equip(this);
        }
    }
    // простая фабрика предметов
    class ItemFactory
    {
        static string[] weapons = { "Меч", "Топор", "Кинжал", "Булава", "Копье" };
        static string[] armors = { "Кожанка", "Кольчуга", "Латы", "Стеганка" };
        public static Item Create(ItemType type)
        {
            Settings s = Settings.Instance;
            switch (type)
            {
                case ItemType.Potion: return new Potion();
                case ItemType.Weapon: return new Weapon(weapons[s.Range(0, weapons.Length - 1)], s.Range(s.WeaponMin, s.WeaponMax));
                case ItemType.Armor: return new Armor(armors[s.Range(0, armors.Length - 1)], s.Range(s.ArmorMin, s.ArmorMax));
                default: return null;
            }
        }
        public static Item CreateRandom()
        {
            Settings s = Settings.Instance;
            int roll = s.Range(1, 100);
            if (roll <= s.PotionChance) return Create(ItemType.Potion);
            if (roll <= s.PotionChance + s.WeaponChance) return Create(ItemType.Weapon);
            return Create(ItemType.Armor);
        }
    }

    // ---------- Игрок ----------
    class Player
    {
        public int MaxHp { get; }
        public int Hp { get; private set; }
        public Weapon Weapon { get; private set; } = new Weapon("Палка", 10);
        public Armor Armor { get; private set; } = new Armor("Рубаха", 3);
        public bool Defending { get; private set; }
        public bool Frozen { get; private set; }
        public bool IsAlive => Hp > 0;
        public Player() { MaxHp = Settings.Instance.PlayerHp; Hp = MaxHp; }
        public void Heal() => Hp = MaxHp;
        public void Equip(Weapon weapon) => Weapon = weapon;
        public void Equip(Armor armor) => Armor = armor;
        public void Defend() => Defending = true;
        public void Freeze() => Frozen = true;
        public void Unfreeze() => Frozen = false;
        // false - если игрок уклонился
        public bool TakeHit(int damage, bool ignoreArmor)
        {
            Settings s = Settings.Instance;
            if (Defending)
            {
                Defending = false;
                if (s.Chance(s.DodgeChance)) { Console.WriteLine("Вы уклонились от атаки!"); return false; }
                int block = ignoreArmor ? 0 : Armor.Defense * s.Range(s.BlockMin, s.BlockMax) / 100;
                damage = Math.Max(0, damage - block);
                Console.WriteLine(ignoreArmor ? "Враг игнорирует вашу защиту!" : $"Блок поглотил {block} урона");
            }
            Hp = Math.Max(0, Hp - damage);
            Console.WriteLine($"Вы получили {damage} урона. Здоровье: {Hp}/{MaxHp}");
            return true;
        }
    }

    // ---------- Враги ----------
    abstract class Enemy
    {
        public string Name { get; }
        public int MaxHp { get; }
        public int Hp { get; private set; }
        public int Attack { get; }
        public int Defense { get; }
        public bool IsBoss { get; }
        public bool IsAlive => Hp > 0;
        protected Enemy(string name, int hp, int attack, int defense, bool isBoss)
        { Name = name; MaxHp = hp; Hp = hp; Attack = attack; Defense = defense; IsBoss = isBoss; }
        protected static int Scale(int value, double k) => (int)Math.Round(value * k);
        public abstract string Ability();
        protected virtual int Crit(int damage) => damage;
        protected virtual bool IgnoreArmor => false;
        protected virtual void AfterHit(Player player) { }
        public int TakeDamage(int damage)
        {
            int real = Math.Max(1, damage - Defense); // хотя бы 1 урон проходит
            Hp = Math.Max(0, Hp - real);
            return real;
        }
        // порядок атаки общий, особенности у каждого врага свои
        public void AttackPlayer(Player player)
        {
            Console.WriteLine($"{Name} атакует!");
            int damage = Crit(Math.Max(1, Settings.Instance.Spread(Attack)));
            if (player.TakeHit(damage, IgnoreArmor) && player.IsAlive) AfterHit(player);
        }
        protected void TryFreeze(Player player, int chance)
        {
            if (!Settings.Instance.Chance(chance)) return;
            player.Freeze();
            Console.WriteLine($"{Name} заморозил вас! Следующий ход пропускается");
        }
        public override string ToString() => $"{(IsBoss ? "БОСС " : "")}{Name} - HP {Hp}, атака {Attack}, защита {Defense}, {Ability()}";
    }
    class Goblin : Enemy
    {
        int critChance;
        public Goblin() : this("Гоблин", 1, 1, 1, 0, false) { }
        protected Goblin(string name, double hp, double atk, double def, int critBonus, bool boss)
            : base(name, Scale(Settings.Instance.GoblinHp, hp), Scale(Settings.Instance.GoblinAtk, atk), Scale(Settings.Instance.GoblinDef, def), boss)
        { critChance = Settings.Instance.GoblinCrit + critBonus; }
        public override string Ability() => $"шанс крита {critChance}%";
        protected override int Crit(int damage)
        {
            if (!Settings.Instance.Chance(critChance)) return damage;
            Console.WriteLine("Критический удар!");
            return damage * 2;
        }
    }
    class Skeleton : Enemy
    {
        public Skeleton() : this("Скелет", 1, 1, 1, false) { }
        protected Skeleton(string name, double hp, double atk, double def, bool boss)
            : base(name, Scale(Settings.Instance.SkeletonHp, hp), Scale(Settings.Instance.SkeletonAtk, atk), Scale(Settings.Instance.SkeletonDef, def), boss) { }
        public override string Ability() => "игнорирует защиту";
        protected override bool IgnoreArmor => true;
    }
    class Mage : Enemy
    {
        int freezeChance;
        public Mage() : this("Маг", 1, 1, 1, 0, false) { }
        protected Mage(string name, double hp, double atk, double def, int freezeBonus, bool boss)
            : base(name, Scale(Settings.Instance.MageHp, hp), Scale(Settings.Instance.MageAtk, atk), Scale(Settings.Instance.MageDef, def), boss)
        { freezeChance = Settings.Instance.MageFreeze + freezeBonus; }
        public override string Ability() => $"шанс заморозки {freezeChance}%";
        protected override void AfterHit(Player player) => TryFreeze(player, freezeChance);
    }

    // ---------- Боссы ----------
    class Vvg : Goblin { public Vvg() : base("ВВГ", 2.0, 1.5, 1.2, 10, true) { } }
    class Kowalski : Skeleton { public Kowalski() : base("Ковальский", 2.5, 1.3, 1.4, true) { } }
    class ArchMage : Mage { public ArchMage() : base("Архимаг C++", 1.8, 1.6, 1.1, 10, true) { } }
    class Pestov : Skeleton
    {
        int freezeChance = Settings.Instance.MageFreeze + 15;
        public Pestov() : base("Пестов С--", 1.3, 1.8, 0.6, true) { }
        public override string Ability() => $"игнорирует защиту, шанс заморозки {freezeChance}%";
        protected override void AfterHit(Player player) => TryFreeze(player, freezeChance);
    }
    // первые три - обычные враги, остальные - боссы
    enum EnemyType { Goblin, Skeleton, Mage, Vvg, Kowalski, ArchMage, Pestov }
    // простая фабрика врагов
    class EnemyFactory
    {
        public static Enemy Create(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Goblin: return new Goblin();
                case EnemyType.Skeleton: return new Skeleton();
                case EnemyType.Mage: return new Mage();
                case EnemyType.Vvg: return new Vvg();
                case EnemyType.Kowalski: return new Kowalski();
                case EnemyType.ArchMage: return new ArchMage();
                case EnemyType.Pestov: return new Pestov();
                default: return null;
            }
        }
        public static Enemy CreateRandom() => Create((EnemyType)Settings.Instance.Range(0, 2));
        public static Enemy CreateRandomBoss() => Create((EnemyType)Settings.Instance.Range(3, 6));
    }

    // ---------- Игра ----------
    class Game
    {
        Player player = new Player();
        int turn, kills, bosses;
        public void Run()
        {
            while (player.IsAlive)
            {
                turn++;
                Console.WriteLine($"\n===== Ход {turn} | HP {player.Hp}/{player.MaxHp} | {player.Weapon} | {player.Armor} =====");
                if (turn % Settings.Instance.BossEvery == 0) Fight(EnemyFactory.CreateRandomBoss());
                else if (Settings.Instance.Chance(Settings.Instance.ChestChance))
                {
                    Item item = ItemFactory.CreateRandom();
                    Console.WriteLine($"Вы нашли сундук! Внутри: {item}");
                    item.Use(player); // у каждого предмета своя реализация Use
                }
                else Fight(EnemyFactory.CreateRandom());
                if (player.IsAlive && Input.Number("1 - дальше, 0 - выйти: ", 0, 1) == 0) break;
            }
            Console.WriteLine($"\nИгра окончена. Ходов: {turn}, побеждено врагов: {kills}, боссов: {bosses}");
        }
        void Fight(Enemy enemy)
        {
            Console.WriteLine(enemy);
            player.Unfreeze();
            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine($"\nВы: {player.Hp}/{player.MaxHp}   {enemy.Name}: {enemy.Hp}/{enemy.MaxHp}");
                if (player.Frozen)
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход");
                    player.Unfreeze();
                }
                else if (Input.Number("1 - атака, 2 - защита: ", 1, 2) == 1)
                    Console.WriteLine($"Вы нанесли {enemy.TakeDamage(Settings.Instance.Spread(player.Weapon.Damage))} урона");
                else
                    player.Defend();
                if (enemy.IsAlive) enemy.AttackPlayer(player);
            }
            if (!player.IsAlive) { Console.WriteLine($"Вы погибли. Вас победил {enemy.Name}"); return; }
            kills++;
            if (enemy.IsBoss) bosses++;
            Console.WriteLine($"{enemy.Name} повержен!");
        }
    }
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n=== ТЕКСТОВЫЙ РОГАЛИК ===\n1 - Новая игра\n0 - Выход");
                if (Input.Number("Выбор: ", 0, 1) == 0) return;
                new Game().Run();
            }
        }
    }
}
