using System.Collections;
using System.Collections.Generic;
using TextSprites;
using TMPro;
using UnityEngine;

public class TextMeshSpriteAnimator : MonoBehaviour
{
    [SerializeField] private ListOfSpriteAssets spriteAssets;
    [SerializeField] private string message;
    [SerializeField] private string keyword;

    private TMP_Text _textbox;

    private void Awake()
    {
        _textbox = GetComponent<TMP_Text>();
    }

    void Start()
    {
        _textbox.text = ReplaceWithTag(message);
    }

    private string ReplaceWithTag(string message)
    {
        if(!message.Contains(keyword))
        {
            return message;
        }

        TMP_SpriteAsset asset = null;
        float speed = 0;

        foreach(var assetToAnimate in spriteAssets.SpriteAssets)
        {
            if(assetToAnimate.Asset.name == keyword)
            {
                asset = assetToAnimate.Asset;
                speed = assetToAnimate.Speed;
                break;
            }
        }

        if(spriteAssets == null)
        {
            Debug.Log(message: $"Sprite sheet for {keyword} not found!");
            return message;
        }

        int framesInSequence = asset.spriteCharacterTable.Count - 1; //System.Math.Max(0, asset.spriteCharacterTable.Count - 1);
        message = message.Replace(oldValue: keyword,
            newValue: $"<sprite+\"{asset.name}\" anim=\"{0}, {framesInSequence}, {speed}\">");
        
        return message;
    }
}
