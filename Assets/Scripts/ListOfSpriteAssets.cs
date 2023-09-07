using System.Collections.Generic;
using UnityEngine;

namespace TextSprites
{
    [CreateAssetMenu(fileName = "List of Sprite Assets", menuName = "Databases/List of Sprite Assets", order = 0)]
    public class ListOfSpriteAssets : ScriptableObject
    {
        public List<SpriteAnimationSpeed> SpriteAssets;
    }
}