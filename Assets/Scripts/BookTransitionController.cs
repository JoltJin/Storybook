using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cinemachine;

public class BookTransitionController : MonoBehaviour
{
    [SerializeField] private GameObject book, table, cloth, page, level;
    //[SerializeField] private Animator bookAnim;
    [SerializeField] private Material screenshotMat;
    [SerializeField] private Transform battleLoadLocation;


    [SerializeField] private LayerMask basicMask;
    [SerializeField] private LayerMask screenshotMask;

    private RenderTexture screenshotRend;
    public static BookTransitionController Instance;

    private string sceneToLoad;

    private Vector3 oldCameraPos;
    private Quaternion oldCameraRotation;
    private float oldCameraView = 1;

    [SerializeField] private CinemachineVirtualCamera pauseCam;
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

        //DontDestroyOnLoad(gameObject);

        book.SetActive(false);
        table.SetActive(false);
        cloth.SetActive(false);
        page.SetActive(false);
    }

    public void OpenRealBook()
    {
        Time.timeScale = 1;
        Debug.Log("open real book");
        //FindObjectOfType<BattleController>()?.StartBattle();
        FindObjectOfType<BattleController>().UnfoldBattle();
        book.SetActive(false);
        table.SetActive(false);
        cloth.SetActive(false);
        page.SetActive(false);
    }

    public void BeginBattle()
    {
        //bookAnim.SetTrigger("StartBattle"); 
        GetComponent<Animator>().SetTrigger("StartBattle");
    }

    public void StartFading()
    {
        StartCoroutine(BattleEntranceController());
    }

    IEnumerator BattleEntranceController()
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

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        table.SetActive(true);
        cloth.SetActive(true);
        level.SetActive(false);
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            

            float timeDuration = 1 - time/duration/2;
            //float timeDuration = time / duration;

            screenshotMat.color = new Color(1, 1, 1, timeDuration); //= Color.Lerp(screenshotMat.color, new Color(1,1,1,.5f), Time.deltaTime * 2);

            yield return new WaitForEndOfFrame();
        }


        //bookAnim.SetTrigger("CloseBook1");
        GetComponent<Animator>().SetTrigger("Pullback1");

        oldCameraPos = Camera.main.transform.position;
        oldCameraRotation = Camera.main.transform.rotation;
        oldCameraView = Camera.main.fieldOfView;

        Debug.Log(Camera.main.transform.position + "position " + Camera.main.transform.rotation + " rotation");

        Camera.main.transform.rotation = Quaternion.Euler(7.273f, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z);
        
         time = 0;
         duration = 2;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float fov = Camera.main.fieldOfView;
            //////// Setting field of view for camera to lerp to (20.50212) 

            //float timeDuration = 1 - time / duration / 2;
            float timeDuration = time / duration;

            Camera.main.fieldOfView = Mathf.Lerp(fov, 20.50212f, timeDuration);

            yield return new WaitForEndOfFrame();
        }
        screenshotMat.color = new Color(1, 1, 1, 0);

        FindObjectOfType<BattleController>().HideScene(battleLoadLocation.position);
    }

    IEnumerator BattleExitController(string sceneToUnload)
    {

        Time.timeScale = 0;
        float duration = 1;

        book.SetActive(true);
        table.SetActive(true);
        cloth.SetActive(true);
        level.SetActive(false);
        SceneManager.UnloadSceneAsync(sceneToUnload);

        GetComponent<Animator>().SetTrigger("EndBattle");
        yield return new WaitForSecondsRealtime(duration);

        page.SetActive(true);

        screenshotMat.color = new Color(1, 1, 1, 0.5f);


        ////////1
        float time = 0.5f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float timeDuration = time / duration;
            screenshotMat.color = new Color(1, 1, 1, timeDuration);


            yield return new WaitForEndOfFrame();
        }


        ///////////
        ////////3

        time = 0;
        duration = 1;

        pauseCam.m_Lens.FieldOfView = Camera.main.fieldOfView;
        pauseCam.gameObject.SetActive(true);

        while (time < duration)
        {
            Debug.Log(pauseCam.m_Lens.FieldOfView);
            time += Time.unscaledDeltaTime;

            float fov = pauseCam.m_Lens.FieldOfView;
            //////// Setting field of view for camera to lerp to (20.50212) 

            //float timeDuration = 1 - time / duration / 2;
            float timeDuration = time / duration;

            pauseCam.m_Lens.FieldOfView = Mathf.Lerp(fov, oldCameraView, timeDuration);

            yield return null;
        }

        Camera.main.transform.rotation = oldCameraRotation;
        Camera.main.transform.position = oldCameraPos;

        Debug.Log(Camera.main.transform.position + "position " + Camera.main.transform.rotation + " rotation");
        book.SetActive(false);
        table.SetActive(false);
        cloth.SetActive(false);
        level.SetActive(true);
        ////////////
        //////////2
        time = 0;

        duration = 1.5f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            Debug.Log(Time.timeScale);
            float timeDuration = 1 - time / duration;
            //float timeDuration = time / duration;

            screenshotMat.color = new Color(1, 1, 1, timeDuration); //= Color.Lerp(screenshotMat.color, new Color(1,1,1,.5f), Time.deltaTime * 2);

            yield return new WaitForEndOfFrame();
        }
        /////////
        page.SetActive(false);
        pauseCam.gameObject.SetActive(false);
        Time.timeScale = 1;
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

    public void OverworldTransition(string sceneToUnload)
    {
        StartCoroutine(BattleExitController(sceneToUnload));
        //SceneManager.UnloadSceneAsync(sceneToUnload);

        //Camera.main.transform.position = oldCameraPos;
        //Camera.main.transform.rotation = oldCameraRotation;
        //Camera.main.fieldOfView = oldCameraView;

        //level.SetActive(true);


    }
}
