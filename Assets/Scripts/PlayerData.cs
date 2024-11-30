using System;
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

public enum TeammateNames
{
    Agatha,
    Faylee,
    Kael,
    Willow
}

public enum SkillType
{
    None,
    HP1,//2 hp
    HP2,//2 hp
    HPPlus,//2 hp
    Health_Regen,
    MP1,//2 mp
    MP2,//2 mp
    Mana_Regen,
    Defense,//1 def
    DefensePlus,//1 def
    Attack,//1 attack
    Skill1,
    Skill1_1,
    Skill1_2,
    Skill2,
    Skill2_1,
    Skill2_2,
    Skill3,
    Skill4,
    Meditating_Guard,
    Opposing_Viewpoint
}

public enum SkillPointType
{
    Agatha,
    Kael,
    Passive
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

public class SkillLevels
{
    public SkillType Type;
    public int Level;
    public int MaxLevel;
    public SkillPointType skillPoint;

    public SkillLevels(SkillType type, int level, int maxLevel, SkillPointType skillPointType)
    {
        Type = type;
        Level = level;
        MaxLevel = maxLevel;
        skillPoint = skillPointType;
    }
}

public class PlayerData
{
    public static int currentMP;
    public static int MaxMP;
    public static int unmodifiedMaxMP;

    public static List<PartyStats> party = new List<PartyStats>();

    public static List<Enchantment> enchantments = new List<Enchantment>();

    public static bool inCutscene = false;

    public static void PartyEnhanceStats(SkillLevels skillLevels, TeammateNames charName, Action UpdateButtonVisual)
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (party[i].charName == charName)
            {
                for (int j = 0; j < party[i].skillLevels.Count; j++)
                {
                    if (party[i].skillLevels[j].Type == skillLevels.Type)
                    {
                        party[i].AdjustSkill(skillLevels, j, UpdateButtonVisual);
                        return;
                    }
                }
                party[i].TryUnlockSkill(skillLevels, UpdateButtonVisual);
                return;
            }
        }
        Debug.Log("Character " + charName + " isn't on the team??");
    }

    public static int GetPartyMemberStats(TeammateNames charName)
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (party[i].charName == charName)
            {
                return i;
            }
        }

        Debug.Log("Not found");
        return 0;
    }
}

public class PartyStats
{
    public TeammateNames charName;
    public int currentHealth;
    public int unmodifiedMaxHealth;
    public int maxHealth;
    public int unmodifiedBaseDamage;
    public int baseDamage;
    public int defense;
    public int unmodifiedDefense;
    public int bonusMana = 0;
    public Status status = Status.Normal;

    public int currentLevel = 1;
    public int currentExp = 0;

    public int MagicSkillPoints { get; private set; } = 0;
    public int PhysicalSkillPoints { get; private set; } = 0;

    public List<SkillLevels> skillLevels = new List<SkillLevels>();

    public event EventHandler OnSkillPointsChanged;
    public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;
    public class OnSkillUnlockedEventArgs : EventArgs
    {
        public SkillLevels skillLevel;
        public TeammateNames name;
    }

    public void AddMagicSkillPoints()
    {
        MagicSkillPoints++;
        OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void AddPhysicalSkillPoints()
    {
        PhysicalSkillPoints++;
        OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddMagicSkillPoints(int points)
    {
        MagicSkillPoints+= points;
        OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void AddPhysicalSkillPoints(int points)
    {
        PhysicalSkillPoints+= points;
        OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public PartyStats(TeammateNames name, int health, int attack, int defense)
    {
        charName = name;
        currentHealth = health;
        maxHealth = unmodifiedMaxHealth = health;
        baseDamage = unmodifiedBaseDamage = attack;
        this.defense = unmodifiedDefense = defense;
    }

    public PartyStats(TeammateNames name, int health, int totalHealth, int attack, int defense)
    {
        charName = name;
        currentHealth  = health;
        maxHealth = unmodifiedMaxHealth = totalHealth;
        baseDamage = unmodifiedBaseDamage = attack;
        this.defense = unmodifiedDefense = defense;
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        for (int i = 0; i < skillLevels.Count; i++)
        {
            if (skillLevels[i].Type == skillType)
            {
                return true;
            }
        }

        return false;
    }

    public bool TryUnlockSkill(SkillLevels skillLevel, Action UpdateButtonVisual)
    {
        if(CanUnlock(skillLevel.Type))
        {
            if (CheckSkillPoints(skillLevel.skillPoint))
            {
                UnlockSkill(skillLevel);
                UpdateButtonVisual();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public bool CanUnlock(SkillType type)
    {
        SkillType[] requirement = GetSkillRequirements(type);
        if (requirement[0] == SkillType.None)
        {
            return true;
        }    

        foreach (SkillType skill in requirement)
        {
            if (!IsSkillUnlocked(skill))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckSkillPoints(SkillPointType pointType)
    {
        switch (pointType)
        {
            case SkillPointType.Agatha:
                if(MagicSkillPoints > 0)
                {
                    MagicSkillPoints--;
                    OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                else
                {
                    return false;
                }
            case SkillPointType.Kael:
                if(PhysicalSkillPoints > 0)
                {
                    PhysicalSkillPoints--;
                    OnSkillPointsChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                else
                { 
                    return false; 
                }
            case SkillPointType.Passive:
                return false;
        }

        return false;
    }
    private void UnlockSkill(SkillLevels skill)
    {
        skillLevels.Add(skill);
        ChangeStats(skill);
    }

    public void AdjustSkill(SkillLevels skill, int placement, Action UpdateButtonVisual)
    {
        if (CheckSkillPoints(skill.skillPoint))
        {
            skillLevels[placement] = skill;
            ChangeStats(skill);

            UpdateButtonVisual();
        }
    }

    public SkillType[] GetSkillRequirements(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.HPPlus: return new SkillType[] { SkillType.HP1 };
            case SkillType.DefensePlus: return new SkillType[] { SkillType.Defense, SkillType.HP2 };
            case SkillType.Skill1_1: return new SkillType[] { SkillType.Skill1 };
            case SkillType.Skill1_2: return new SkillType[] { SkillType.Skill1, SkillType.Opposing_Viewpoint };
            case SkillType.Skill2_1: return new SkillType[] { SkillType.Skill2 };
            case SkillType.Meditating_Guard: return new SkillType[] { SkillType.DefensePlus, SkillType.Mana_Regen };
                //case SkillType.Skill2: return new SkillType[] { SkillType.Skill1 };
        }

        return new SkillType[] { SkillType.None };
    }


    private void ChangeStats(SkillLevels skill)
    {
        switch (skill.Type)
        {
            case SkillType.HP1:
            case SkillType.HP2:
                if (currentHealth == maxHealth)
                {
                    maxHealth += 2; //(2 * skill.Level);
                    currentHealth = maxHealth;
                }
                else
                {
                    maxHealth += 2;// (2 * skill.Level);
                }
                Debug.Log(charName + " now has " + maxHealth + " max health!");
                break;
            case SkillType.HPPlus:
                if (currentHealth == maxHealth)
                {
                    maxHealth = maxHealth + (4 * skill.Level);
                    currentHealth = maxHealth;
                }
                else
                {
                    maxHealth = maxHealth + (4 * skill.Level);
                }
                Debug.Log(charName + " now has " + maxHealth + " max health!");
                break;
            case SkillType.Defense:
                defense = defense + skill.Level;
                Debug.Log(charName + " now has gained defense!");
                break;
            case SkillType.DefensePlus:
                defense = defense + skill.Level;
                Debug.Log(charName + " now has gained extra defense!");
                break;
            case SkillType.MP1:
            case SkillType.MP2:
                bonusMana += 2;
                PlayerData.MaxMP = PlayerData.unmodifiedMaxMP + 2;
                Debug.Log(charName + " now has gained mp!");
                break;

            case SkillType.Attack:
                baseDamage = unmodifiedBaseDamage + skill.Level;

                Debug.Log(charName + " now has gained attack!");
                break;
            case SkillType.Meditating_Guard:
                Debug.Log(charName + " now has gained guard plus!");
                break;

        }


        OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { skillLevel = skill, name = charName });
    }
    public void addEnchantments()
    {

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