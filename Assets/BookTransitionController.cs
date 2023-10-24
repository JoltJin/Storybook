using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BookTransitionController : MonoBehaviour
{
    [SerializeField] private GameObject book, table, cloth, page, level;
    [SerializeField] private Animator bookAnim;
    [SerializeField] private Material screenshotMat;


    [SerializeField] private LayerMask basicMask;
    [SerializeField] private LayerMask screenshotMask;

    private RenderTexture screenshotRend;
    public static BookTransitionController Instance;

    private string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        screenshotRend = new RenderTexture(1920, 1080, 4);

        if (screenshotMat != null)
        {
            screenshotMat.SetTexture("_BaseMap", screenshotRend);
        }

        screenshotMat.color = new Color(1, 1, 1, 0);

        DontDestroyOnLoad(gameObject);
    }

    public void OpenRealBook()
    {
        FindObjectOfType<BattleController>()?.StartBattle();
        book.SetActive(false);
        table.SetActive(false);
        cloth.SetActive(false);
    }

    public void StartFading()
    {
        StartCoroutine(FadingController());
    }

    IEnumerator FadingController()
    {

        Time.timeScale = 0;
        float duration = 1;

        page.SetActive(true);

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float timeDuration = time / duration;
            screenshotMat.color = new Color(1, 1, 1, timeDuration);


            yield return new WaitForEndOfFrame();
        }

        time = 0;

        duration = 1;

        book.SetActive(true);


        while (time < duration)
        {

            time += Time.unscaledDeltaTime;
            float timeDuration = 1 - time/duration/2;
            screenshotMat.color = new Color(1, 1, 1, timeDuration); //= Color.Lerp(screenshotMat.color, new Color(1,1,1,.5f), Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();
        }
        level.SetActive(false);
        bookAnim.SetTrigger("CloseBook1");
        GetComponent<Animator>().SetTrigger("Pullback1");

        table.SetActive(true);
        cloth.SetActive(true);


        //SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action1"))
        {
            
        }
    }

    public void BattleTransition(string sceneToLoad)
    {
        //Camera.main.cullingMask = screenshotMask;
        Camera.main.targetTexture = screenshotRend;
        Camera.main.cullingMask = basicMask;
        Camera.main.Render();
        Camera.main.targetTexture = null;

        this.sceneToLoad = sceneToLoad;
        StartFading();
    }
}
