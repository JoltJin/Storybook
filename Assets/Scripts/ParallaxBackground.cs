using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Vector2 parallaxEffectMultiplier;
    [SerializeField]
    private bool infiniteHorizontal = false;
    [SerializeField]
    private bool infiniteVertical = false;
    [SerializeField]
    private bool autoIncreaseSize = false;

    private Transform cameraTransform;
    private Vector3 lastCameraPos;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    // Start is called before the first frame update
    void Start()
    {
        if(autoIncreaseSize)
        {
            GetComponent<SpriteRenderer>().size *=5;
        }

        cameraTransform = Camera.main.transform;
        lastCameraPos = cameraTransform.position;

        if(TryGetComponent(out SpriteRenderer render))
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        }
        //textureUnitSizeX *= (1f/3f);
        //textureUnitSizeY *= (1f/3f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltamove = cameraTransform.position - lastCameraPos;
        transform.position += new Vector3(deltamove.x * parallaxEffectMultiplier.x, deltamove.y * parallaxEffectMultiplier.y);
        lastCameraPos = cameraTransform.position;
        
        if(infiniteHorizontal)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                Debug.Log(transform.position.x);
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
                Debug.Log(transform.position.x);
            }
        }

        if(infiniteVertical)
        {
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
            }
        }
    }
}
