using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleSceneTypes
{
    Battle_Scene,
}

public class BattleStartTrigger : MonoBehaviour
{
    private static Scene battleScene;

    private static BattleStartTrigger instance;

    private static GameObject sceneBody;

    //private RenderTexture screenshot;

    //[SerializeField] private Material screenshotMaterial;

    [SerializeField] private BattleSceneTypes battleToLoad;

    public EnemyAIBase body;

    private void Start()
    {
        //screenshot = new RenderTexture(1920, 1080, 4);

        //screenshotMaterial.SetTexture("_BaseMap", screenshot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            instance = this;
            Time.timeScale = 0;

            //Camera.main.targetTexture = screenshot;
            //Camera.main.Render();
            //Camera.main.targetTexture = null;
            
            BattleController.AddParticipant("Agatha", 999, 999, 1, 0);
            BattleController.AddParticipant("Faylee", 10, 10, 1, 0);
            BattleController.AddParticipant("Jackalope", 999, 1, 0);
            Time.timeScale = 1;
            sceneBody = FindObjectOfType<BoxCollider2D>().gameObject;
            sceneBody.SetActive(false);
            SceneManager.LoadScene(battleToLoad.ToString(), LoadSceneMode.Additive);
        }
    }

    public void EndBattle()
    {
        SceneManager.UnloadSceneAsync(battleToLoad.ToString());
        sceneBody.SetActive(true);
        Time.timeScale = 1;

        BattleController.RemoveParticipants();

        Destroy(instance.body.gameObject);
    }

    private void Update()
    {
        //if(Input.GetAxisRaw("Horizontal") < 0)
        //{
        //    Debug.Log("");
        //    EndBattle();
        //}
    }
}
