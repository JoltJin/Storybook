using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransparencyScript : MonoBehaviour
{
    private bool fadingOut = false, 
                 fadingIn = false;
    [SerializeField] float fadeTimer = .2f;
    [SerializeField] float fadeGoal = .4f;

    private SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        if(!GetComponent<SpriteRenderer>())
        {
            Debug.Log("There is no renderer");
        }
        else
        {
            rend = GetComponent<SpriteRenderer>();
        }

        if(!GetComponent<Collider>())
        {
            Debug.Log("There isn't a collider on this object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut()
    {
        if(!fadingOut)
        {
            fadingIn = false;

            StopAllCoroutines();

            StartCoroutine(FadeOutController());
        }
        
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FadeOutController()
    {
        fadingOut = true;
        float time = 0;
        float originalColor = rend.color.a;
        while (rend.color.a > fadeGoal)
        {
            time += Time.deltaTime;
            float timeDuration = time / fadeTimer;
            rend.color = new Color(1, 1, 1, Mathf.Lerp(originalColor, fadeGoal, timeDuration));
            yield return null;
        }
        //float time = fadeTimer;
        //float progress = (rend.color.a - fadeGoal) / fadeGoal;
        //while (fadeGoal < rend.color.a)
        //{
        //    time -= Time.unscaledDeltaTime;
        //    float timeDuration = (time * progress) / fadeTimer;
        //    rend.color = new Color(1, 1, 1, timeDuration);
        //    Debug.Log(timeDuration);
        //    yield return null;
        //}
        fadingOut = false;

        StartCoroutine(FadeInController());
    }

    IEnumerator FadeInController()
    {
        fadingIn = true;
        float time = 0;
        float originalColor = 1;
        while (rend.color.a < 1)
        {
            time += Time.deltaTime;
            float timeDuration = time / fadeTimer;
            rend.color = new Color(1, 1, 1, Mathf.Lerp(fadeGoal, originalColor, timeDuration));

            yield return null;
        }

        fadingIn = false;
    }
}
