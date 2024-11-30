using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlingHud : MonoBehaviour
{
    private RenderTexture screenshotRend;
    [SerializeField] private Material screenshotMat;
    [SerializeField] private LayerMask screenshotMask;
    [SerializeField] private LayerMask basicMask;
    [SerializeField] private Animator corners;

    // Start is called before the first frame update
    void Start()
    {
        screenshotRend = new RenderTexture(1920, 1080, 4);

        if (screenshotMat != null)
        {
        }


        corners.transform.gameObject.SetActive(false);

        StartCoroutine(WaitTime());
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(2);
        corners.SetTrigger("Idle");
        screenshotMat.color = new Color(1, 1, 1, 0);

        screenshotMat.SetTexture("_BaseMap", screenshotRend);
        screenshotMat.color = new Color(1, 1, 1, 1);
        Camera.main.targetTexture = screenshotRend;
        Camera.main.cullingMask = basicMask;
        Camera.main.Render();
        Camera.main.targetTexture = null;
        corners.transform.gameObject.SetActive(true);

        float duration = .5f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float timeDuration = time / duration;
            screenshotMat.color = new Color(1, 1, 1, timeDuration);


            yield return new WaitForEndOfFrame();
        }
        corners.SetTrigger("Fold");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
