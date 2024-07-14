using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Status
{
    Sleep,
    Poisoned,
    Muted,
    Burned,
    Cursed,
    Dead,
    Frozen,

    Normal
}

public enum EnchantmentType
{
    Attack,
    Defense,
}

public enum EnchantmentTarget
{
    Main,
    Partner,
    Both
}

public class PlayerData
{
    public static int currentMP;
    public static int MaxMP;

    public static List<PartyStats> party = new List<PartyStats>();

    public static List<Enchantment> enchantments = new List<Enchantment>();
}
public class PartyStats
{
    public string charName;
    public int currentHealth;
    public int maxHealth;
    public int baseDamage;
    public int defense;
    public Status status = Status.Normal;

    public int currentLevel = 1;
    public int currentExp = 0;

    public PartyStats(string name, int health, int attack, int defense)
    {
        charName = name;
        currentHealth = health;
        maxHealth = health;
        baseDamage = attack;
        this.defense = defense;
    }

    public PartyStats(string name, int health, int totalHealth, int attack, int defense)
    {
        charName = name;
        currentHealth = health;
        maxHealth = totalHealth;
        baseDamage = attack;
        this.defense = defense;
    }
}

public class Enchantment
{
    public string name;
    public string description;
    public int cost;
    public EnchantmentType type;
    public int effectAmount;
    public EnchantmentTarget target;

    public Enchantment(string name, string description, int cost, EnchantmentType type, int effectAmount, EnchantmentTarget target)
    {
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.type = type;
        this.effectAmount = effectAmount;
        this.target = target;
    }
}