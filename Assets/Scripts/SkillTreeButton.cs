using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButton : MonoBehaviour
{
    [SerializeField] private SkillType skillType;
    private int skillLevel = 0;
    [SerializeField] private int maxSkillLevel = 3;
    [SerializeField] private TeammateNames teammateName;
    [SerializeField] private SkillPointType skillPointType;

    [SerializeField] Image image;
    [SerializeField] Image backgroundImage;

    public Image grid;

    [SerializeField] Material skillLockedMaterial;
    [SerializeField] Material SkillUnlockableMaterial;

    private int playerSlot = 0;

    private void Awake()
    {
        playerSlot = PlayerData.GetPartyMemberStats(teammateName);

        skillLockedMaterial = FindObjectOfType<SkillTreeScript>().skillLockedMaterial;
        SkillUnlockableMaterial = FindObjectOfType<SkillTreeScript>().skillUnlockedMaterial;

        if(maxSkillLevel == 1 || skillLevel == maxSkillLevel)
        {
            grid.enabled = false;
        }
    }

    public TeammateNames GetTeammate()
    {
        return teammateName;
    }

    public SkillType GetSkillType()
    {
        return skillType;
    }

    //public void SetMaterials(Material lockedMaterial, Material unlockedMaterial)
    //{
    //    skillLockedMaterial = lockedMaterial;
    //    SkillUnlockableMaterial = unlockedMaterial;
    //}

    public void UpdateVisual()
    {
        if (PlayerData.party[playerSlot].IsSkillUnlocked(skillType))
        {
            //image.material = null;
            //backgroundImage.material = null;
            image.color = Color.white;
            backgroundImage.color = Color.white;
        }
        else
        {
            if (PlayerData.party[playerSlot].CanUnlock(skillType))
            {
                image.color = new Color(.6f, .6f, .6f);
                backgroundImage.color = new Color(.6f, .6f, .6f);
                //image.material = SkillUnlockableMaterial;
                //backgroundImage.material = SkillUnlockableMaterial;
            }
            else
            {
                image.color = new Color(.3f, .3f, .3f);
                backgroundImage.color = new Color(.3f, .3f, .3f);
                //image.material = skillLockedMaterial;
                //backgroundImage.material = skillLockedMaterial;
            }
        }
    }

    public void PartyEnhanceStats()
    {
        if (skillLevel < maxSkillLevel)
        {

        }
        else
        {
            return;
        }

        PlayerData.PartyEnhanceStats(new SkillLevels(skillType, skillLevel + 1, maxSkillLevel, skillPointType), teammateName, IncreaseSkillCount);
        if (skillLevel == maxSkillLevel)
        {
            grid.enabled = false;
            ////////var ss = GetComponent<Button>().spriteState;
            ////////GetComponent<Image>().sprite = maxSkillImageHighlight;
            //////////ss.disabledSprite = _disabledSprite;
            ////////ss.highlightedSprite = maxSkillImageHighlight;
            ////////ss.pressedSprite = maxSkillImageHighlight;
            ////////GetComponent<Button>().spriteState = ss;
        }

        //new OnSkillUnlockedEventArgs { skillLevel = new SkillLevels(skillType, skillLevel), name = teammateName };
    }

    private void IncreaseSkillCount()
    {
        skillLevel++;
    }
}
