using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    

    [SerializeField] private BattleSceneTypes battleToLoad;

    public EnemyAIBase body;

    [SerializeField] EnemyPoolScript[] enemyPool = new EnemyPoolScript[1];

    private void Start()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && PlayerController.canBattle)
        {
            instance = this;
            Time.timeScale = 0;

            //Camera.main.targetTexture = screenshot;
            //Camera.main.Render();
            //Camera.main.targetTexture = null;

            BattleController.AddParticipant(PlayerData.party[0].charName, BattleController.CurrentTurn.Main, PlayerData.party[0].maxHealth, PlayerData.party[0].currentHealth, PlayerData.party[0].baseDamage, PlayerData.party[0].defense);

            if(PlayerData.party.Count > 1 )
            {
                BattleController.AddParticipant(PlayerData.party[1].charName, BattleController.CurrentTurn.Partner, PlayerData.party[1].maxHealth, PlayerData.party[1].currentHealth, PlayerData.party[1].baseDamage, PlayerData.party[1].defense);
            }

            int poolNum = 0;
            if (enemyPool.Length > 1)
            {
                poolNum = Random.Range(0, enemyPool.Length);
            }

            for (int i = 0; i <= enemyPool[poolNum].enemy.Length - 1; i++)
            {
                BattleController.AddParticipant(enemyPool[poolNum].enemy[i].enemyName, BattleController.CurrentTurn.Enemy1 + i, enemyPool[poolNum].enemy[i].maxHealth, enemyPool[poolNum].enemy[i].maxHealth, enemyPool[poolNum].enemy[i].baseAttack, enemyPool[poolNum].enemy[i].baseDefense);
            }
            
            //BattleController.AddParticipant("Agatha", BattleController.CurrentTurn.Main, 10, 10, 1, 0);
            //BattleController.AddParticipant("Faylee", BattleController.CurrentTurn.Partner, 10, 10, 1, 0);
            //BattleController.AddParticipant("Jackalope", BattleController.CurrentTurn.Enemy1, 5, 1, 0);
            Time.timeScale = 1;
            //sceneBody = FindObjectOfType<BoxCollider2D>().gameObject;
            //sceneBody.SetActive(false);
            //SceneManager.LoadScene(battleToLoad.ToString(), LoadSceneMode.Additive);

            BookTransitionController.Instance.BattleTransition(battleToLoad.ToString());
        }
    }

    public void EndBattle()
    {
        BookTransitionController.Instance.OverworldTransition(battleToLoad.ToString());
        //SceneManager.UnloadSceneAsync(battleToLoad.ToString());
        //sceneBody.SetActive(true);
        //Time.timeScale = 1;

        BattleController.RemoveParticipants();

        Destroy(instance.body.gameObject.transform.parent.gameObject);

    }

    public void FleeBattle()
    {
        BookTransitionController.Instance.OverworldTransition(battleToLoad.ToString());

        BattleController.RemoveParticipants();

        
    }

    IEnumerator DelayRefight()
    {
        PlayerController.canBattle = false;
        yield return new WaitForSeconds(1.5f);
        PlayerController.canBattle = true;
    }

    private void OnEnable()
    {
        StartCoroutine(DelayRefight());
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
