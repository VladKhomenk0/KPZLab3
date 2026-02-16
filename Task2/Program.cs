using System;

namespace Task2
{
    public abstract class Hero
    {
        public string Name { get; set; }
        public Hero(string name) => Name = name;

        public abstract int GetPower();
        public abstract string GetDescription();
    }
    
    public class Warrior : Hero
    {
        public Warrior(string name) : base(name) { }
        public override int GetPower() => 10;
        public override string GetDescription() => $"Воїн {Name}";
    }

    public class Mage : Hero
    {
        public Mage(string name) : base(name) { }
        public override int GetPower() => 8;
        public override string GetDescription() => $"Маг {Name}";
    }

    public class Palladin : Hero
    {
        public Palladin(string name) : base(name) { }
        public override int GetPower() => 12;
        public override string GetDescription() => $"Паладин {Name}";
    }
    
    public abstract class InventoryDecorator : Hero
    {
        protected Hero _hero;

        public InventoryDecorator(Hero hero) : base(hero.Name)
        {
            _hero = hero;
        }

        public override int GetPower() => _hero.GetPower();
        public override string GetDescription() => _hero.GetDescription();
    }
    
    public class Clothing : InventoryDecorator
    {
        public Clothing(Hero hero) : base(hero) { }
        public override int GetPower() => base.GetPower() + 2;
        public override string GetDescription() => base.GetDescription() + " у броні";
    }

    public class Weapon : InventoryDecorator
    {
        public Weapon(Hero hero) : base(hero) { }
        public override int GetPower() => base.GetPower() + 15;
        public override string GetDescription() => base.GetDescription() + " з мечем";
    }

    public class Artifact : InventoryDecorator
    {
        public Artifact(Hero hero) : base(hero) { }
        public override int GetPower() => base.GetPower() + 5;
        public override string GetDescription() => base.GetDescription() + " з артефактом";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Декоратор\n");
            
            Hero myHero = new Warrior("Борислав");
            Console.WriteLine($"Створено: {myHero.GetDescription()} (Сила: {myHero.GetPower()})");

            myHero = new Clothing(myHero);
            Console.WriteLine($"Одягнули: {myHero.GetDescription()} (Сила: {myHero.GetPower()})");

            myHero = new Weapon(myHero);
            Console.WriteLine($"Озброїли: {myHero.GetDescription()} (Сила: {myHero.GetPower()})");

            myHero = new Weapon(myHero); 
            Console.WriteLine($"Ще один меч: {myHero.GetDescription()} (Сила: {myHero.GetPower()})");

            Console.ReadKey();
        }
    }
}