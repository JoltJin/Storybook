using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleIconRotation : MonoBehaviour
{
    [SerializeField] private Vector3[] positions = new Vector3[3];
    [SerializeField] private GameObject[] icons = new GameObject[3];
    private Vector3[] originalPositions;
    private Vector3 mainSlotPos;
    public bool CanSelectOption { get; private set; } = true;

    public bool moving = false;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        mainSlotPos = icons[0].transform.localPosition;

        originalPositions = new Vector3[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = icons[i].transform.localPosition;
            originalPositions[i] = icons[i].transform.localPosition;
        }

        ResetColors();
    }

    public IconType GetSlot()
    {
        IconType type = IconType.Main_Attack;

        for (int i = 0; i < icons.Length; i++)
        {
            if (mainSlotPos == positions[i])
            {
                type = icons[i].GetComponent<BattleIconType>().GetIcon();
                break;
            }
        }
        return type;
    }

    private void Update()
    {

    }

    private void ResetColors()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (icons[i].transform.localPosition == mainSlotPos)
            {
                foreach (SpriteRenderer spriteColor in icons[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteColor.color = Color.white;
                }
            }
            else
            {
                foreach (SpriteRenderer spriteColor in icons[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteColor.color = Color.grey;
                    //spriteColor.color = new Color(.35f, .35f, .35f);
                }
            }
        }
    }

    public void ResetPositions()
    {
        for (int i = 0; i < originalPositions.Length; i++)
        {
            icons[i].transform.localPosition = originalPositions[i];
        }

        ResetColors();
    }

    public void SetAnimTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void RotateCounterclockwise()
    {
        if (moving)
            return;

        int infront = 0;
        float checker = 5;// number above length to always be set first

        for (int i = 0; i < positions.Length; i++)
        {

            if (i == positions.Length - 1)
            {
                positions[i] = icons[0].transform.localPosition;
            }
            else
            {
                positions[i] = icons[i + 1].transform.localPosition;
            }

            if(checker < positions[i].x)
            {
                checker = positions[i].x;
                infront = i;
            }

        }

        moving = true;

        for (int i = 0; i < positions.Length; i++)
        {
            if(i == infront)
            {
                foreach (SpriteRenderer sprite in icons[infront].GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.sortingOrder += 2;
                }
            }
            else
            {
                foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.sortingOrder--;
                }
            }

        }

        StartCoroutine(MoveIcons(0));
    }

    public void RotateClockwise()
    {
        if (moving)
            return;

        int behind = 0;
        float checker = 5;

        for (int i = 0; i < positions.Length; i++)
        {

            if (i == 0)
            {
                positions[i] = icons[positions.Length - 1].transform.localPosition;
            }
            else
            {
                positions[i] = icons[i - 1].transform.localPosition;
            }

            if (checker > positions[i].x)
            {
                checker = positions[i].x;
                behind = i;
            }
        }

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == behind)
            {
                foreach (SpriteRenderer sprite in icons[behind].GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.sortingOrder -= 2;
                }
            }
            else
            {
                foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    sprite.sortingOrder++;
                }
            }

        }

        moving = true;

        //foreach (SpriteRenderer sprite in icons[icons.Length - 1].GetComponentsInChildren<SpriteRenderer>())
        //{
        //    sprite.sortingOrder -= 2;
        //}
        StartCoroutine(MoveIcons(icons.Length - 1));
    }

    IEnumerator MoveIcons(int sortingIcon)
    {
        CanSelectOption = false;

        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] == mainSlotPos)
            {
                foreach (SpriteRenderer spriteColor in icons[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteColor.color = Color.white;
                }
            }
            else
            {
                foreach (SpriteRenderer spriteColor in icons[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteColor.color = Color.grey;
                }
            }
        }
        
        yield return new WaitForSeconds(0.01f);
        float time = 0;
        float duration = 1;
        while (time < duration / 2)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                icons[i].transform.localPosition = Vector3.Lerp(icons[i].transform.localPosition, positions[i], time / duration);
            }
            time += Time.deltaTime;

            if (time / duration >= .4f)
            {
                CanSelectOption = true;
            }
            yield return null;
        }

        for (int i = 0; i < positions.Length; i++)
        {
            icons[i].transform.localPosition = positions[i];
        }

        moving = false;
    }

    public void EnableSelection()
    {
        CanSelectOption = true;
    }

    public void DisableSelection()
    {
        CanSelectOption = false;
    }
}