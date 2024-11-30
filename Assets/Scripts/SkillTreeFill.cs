using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeFill : MonoBehaviour
{
    private Image fill;
    [SerializeField] SkillType skillType;
    void Start()
    {
        fill = GetComponent<Image>();
        int playerSlot = PlayerData.GetPartyMemberStats(GetComponentInParent<SkillTreeButton>().GetTeammate());
        PlayerData.party[playerSlot].OnSkillUnlocked += UpdateSkillFill;

        fill.fillAmount = 0;
    }

    private void UpdateSkillFill(object sender, PartyStats.OnSkillUnlockedEventArgs e)
    {
        if (skillType == e.skillLevel.Type)
        {
            float fillAmount = 0;

            if (e.skillLevel.MaxLevel > 1)
            {
                if (e.skillLevel.Level == 1)
                {
                    fillAmount = .3f;
                }
                else if (e.skillLevel.Level == 2)
                {
                    fillAmount = .7f;
                }
                else if(e.skillLevel.Level == 3)
                {
                    fillAmount = 1;
                }
                else
                {
                    fillAmount = 0;
                }
            }
            else
            {
                fillAmount = 1;
            }


            fill.fillAmount = fillAmount;
        }
    }

}
