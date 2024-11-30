using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeScript : MonoBehaviour
{
    [SerializeField] private TeammateNames teamSkillTree;
    public Material skillLockedMaterial;
    public Material skillUnlockedMaterial;

    private List<SkillTreeButton> buttonList = new List<SkillTreeButton>();
    private int playerSlot;

    [SerializeField] private SkillUnlockPath[] skillUnlockPathArray;
    [SerializeField] private TextMeshProUGUI health, attack, defense, magicSkillPoints, physicalSkillPoints, additionalMana, passiveBonus;

    private void OnEnable()
    {
        playerSlot = PlayerData.GetPartyMemberStats(teamSkillTree);
        if (buttonList.Count == 0)
        {
            foreach (SkillTreeButton button in transform.GetComponentsInChildren<SkillTreeButton>())
            {
                buttonList.Add(button);
                //button.SetMaterials(skillLockedMaterial, skillUnlockedMaterial);
            }
        }

        ChangeSkills();
    }

    private void Start()
    {
        PlayerData.party[playerSlot].AddMagicSkillPoints(10);
        PlayerData.party[playerSlot].AddPhysicalSkillPoints(10);
    }

    private void OnDisable()
    {
        PlayerData.party[playerSlot].OnSkillUnlocked -= PartyStats_OnSkillUnlocked;
    }

   public void ChangeSkills()
    {
         
        PlayerData.party[playerSlot].OnSkillUnlocked += PartyStats_OnSkillUnlocked;
        PlayerData.party[playerSlot].OnSkillPointsChanged += PartyStats_OnSkillPointsChanged;
        
        UpdateVisuals();
        UpdateSkillPoints();
    }

    private void PartyStats_OnSkillPointsChanged(object sender, System.EventArgs e)
    {
        UpdateSkillPoints();
    }
    private void PartyStats_OnSkillUnlocked(object sender, PartyStats.OnSkillUnlockedEventArgs e)
    {
        UpdateVisuals();
    }
    private void UpdateSkillPoints()
    {
        magicSkillPoints.text = "x" + PlayerData.party[playerSlot].MagicSkillPoints;
        physicalSkillPoints.text = "x" + PlayerData.party[playerSlot].PhysicalSkillPoints;
        Debug.Log(PlayerData.party[playerSlot].charName);
    }
    private void UpdateVisuals()
    {
        health.text = PlayerData.party[playerSlot].currentHealth + "/" + PlayerData.party[playerSlot].maxHealth;
        attack.text = PlayerData.party[playerSlot].baseDamage.ToString();
        defense.text = PlayerData.party[playerSlot].defense.ToString();
        
        additionalMana.text =  PlayerData.party[playerSlot].bonusMana.ToString();

        passiveBonus.text = "";
        for (int i = 0; i < PlayerData.party[playerSlot].skillLevels.Count; i++)
        {
            if (PlayerData.party[playerSlot].skillLevels[i].Type == SkillType.Mana_Regen)
            {
                passiveBonus.text = PlayerData.party[playerSlot].skillLevels[i].Type.ToString() + "/n";

            }
        }
        passiveBonus.text = passiveBonus.text.Replace("_", " ");
        

        foreach (SkillUnlockPath path in skillUnlockPathArray)
        {
            foreach (Image line in path.linkImageArray)
            {
                line.color = new Color(0, 0, 0, .3f);
            }
        }

        foreach (SkillUnlockPath path in skillUnlockPathArray)
        {
            if (PlayerData.party[playerSlot].IsSkillUnlocked(path.skillType))
            {
                foreach (Image line in path.linkImageArray)
                {
                    line.color = Color.white;
                }
            }
            
            else if (PlayerData.party[playerSlot].CanUnlock(path.skillType))
            {
                foreach (Image line in path.linkImageArray)
                {
                    line.color = new Color(0.5f, 0.5f, 0.5f, .6f);
                }
            }

        }
    }

    [System.Serializable]
    public class SkillUnlockPath
    {
        public SkillType skillType;
        public Image[] linkImageArray  = new Image[1];
    }
}
