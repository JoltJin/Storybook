using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeSelector : MonoBehaviour
{
    private TeammateNames buttonPressed = TeammateNames.Agatha;

    [SerializeField] private SkillTreeCharacterButton[] characterTreeArray = new SkillTreeCharacterButton[2];
    [SerializeField] private Animator animatedCharacter;

    public void ChangeSkillTree(Button button)
    {
        if(button.GetComponent<SkillTreeSelectorButton>())
        {
            TeammateNames character = button.GetComponent<SkillTreeSelectorButton>().CharacterSkillTree;

            if(character != buttonPressed)
            {
                buttonPressed = character;
                for (int i = 0; i < characterTreeArray.Length; i++)
                {
                    if (characterTreeArray[i].TeammateName == character)
                    {
                        characterTreeArray[i].characterTree.SetActive(true);

                        animatedCharacter.SetTrigger(character.ToString());
                    }
                    else
                    {
                        characterTreeArray[i].characterTree.SetActive(false);
                    }
                }
            }
        }
    }
}
[System.Serializable]
public class SkillTreeCharacterButton
{
    public TeammateNames TeammateName;
    public GameObject characterTree;
}
