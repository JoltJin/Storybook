using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleIconRotation : MonoBehaviour
{
    [SerializeField] private Vector3[] positions = new Vector3[3];
    [SerializeField] private GameObject[] icons = new GameObject[3];

    private Vector3[] originalPositions;
    private Vector3[] currentScale;
    private Vector3[] originalScale;
    private Vector3 mainSlotPos;

    private List<float> depths = new List<float>();
    private int[] spriteOrder;
    public bool CanSelectOption { get; private set; } = true;

    public bool moving = false;

    private Animator anim;

    private Color highlightedColor;

    private void Start()
    {
        highlightedColor = new Color(.85f, .85f, .85f);

        anim = GetComponent<Animator>();
        mainSlotPos = icons[0].transform.localPosition;

        originalPositions  = new Vector3[positions.Length];

        originalScale = new Vector3[positions.Length];
        currentScale =  new Vector3[positions.Length];

        spriteOrder = new int[icons[0].transform.childCount];

        for (int i = 0; i < spriteOrder.Length; i++)
        {
            spriteOrder[i] = icons[0].transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder;
        }



        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = icons[i].transform.localPosition;
            originalPositions[i] = icons[i].transform.localPosition;
            originalScale[i] = currentScale[i] = icons[i].transform.localScale;

            if(depths.Count < 1 )
            {
                depths.Add(positions[i].z);
            }else
            {
                for (int j = 0; j < depths.Count; j++)
                {
                    if (depths[j] == positions[i].z)
                    {
                        break;
                    }
                    else if(j == depths.Count - 1 && depths[j] != positions[i].z)
                    {
                        depths.Add(positions[i].z);
                    }
                }
            }
        }

        depths.Sort();

        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = 0; j < depths.Count; j++)
            {
                if (positions[i].z >= depths[j])
                {
                    foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
                    {
                        sprite.sortingOrder -= 10;
                    }
                }
            }
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
                Debug.Log("Slot is " + type);
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
                    spriteColor.color = highlightedColor;
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
            icons[i].transform.localPosition = positions[i] = originalPositions[i];
            icons[i].transform.localScale = originalScale[i];
        }


        mainSlotPos = icons[0].transform.localPosition;
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
                currentScale[i] = icons[0].transform.localScale;
            }
            else
            {
                positions[i] = icons[i + 1].transform.localPosition;
                currentScale[i] = icons[i + 1].transform.localScale;
            }

            if (checker < positions[i].x)
            {
                checker = positions[i].x;
                infront = i;
            }

        }

        moving = true;

        //for (int i = 0; i < positions.Length; i++)
        //{
        //    if(i == infront)
        //    {
        //        foreach (SpriteRenderer sprite in icons[infront].GetComponentsInChildren<SpriteRenderer>())
        //        {
        //            sprite.sortingOrder += 10;
        //        }
        //    }
        //    else
        //    {
        //        foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
        //        {
        //            sprite.sortingOrder--;
        //        }
        //    }

        //}

        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = 0; j < spriteOrder.Length; j++)
            {
                icons[i].transform.GetChild(j).GetComponent<SpriteRenderer>().sortingOrder = spriteOrder[j];
            }

            for (int j = 0; j < depths.Count; j++)
            {
                if (positions[i].z >= depths[j])
                {
                    foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
                    {
                        sprite.sortingOrder -= 10;
                    }
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
                currentScale[i] = icons[positions.Length - 1].transform.localScale;
            }
            else
            {
                positions[i] = icons[i - 1].transform.localPosition;
                currentScale[i] = icons[i - 1].transform.localScale;
            }

            if (checker > positions[i].x)
            {
                checker = positions[i].x;
                behind = i;
            }
        }

        //for (int i = 0; i < positions.Length; i++)
        //{
        //    if (i == behind)
        //    {
        //        foreach (SpriteRenderer sprite in icons[behind].GetComponentsInChildren<SpriteRenderer>())
        //        {
        //            sprite.sortingOrder -= 10;
        //        }
        //    }
        //    else
        //    {
        //        foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
        //        {
        //            sprite.sortingOrder++;
        //        }
        //    }

        //}

        moving = true;

        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = 0; j < spriteOrder.Length; j++)
            {
                icons[i].transform.GetChild(j).GetComponent<SpriteRenderer>().sortingOrder = spriteOrder[j];
            }

            for (int j = 0; j < depths.Count; j++)
            {
                if (positions[i].z >= depths[j])
                {
                    foreach (SpriteRenderer sprite in icons[i].GetComponentsInChildren<SpriteRenderer>())
                    {
                        sprite.sortingOrder -= 10;
                    }
                }
            }
        }

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
                    spriteColor.color = highlightedColor;
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
                icons[i].transform.localScale = Vector3.Lerp(icons[i].transform.localScale, currentScale[i], time / duration);

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