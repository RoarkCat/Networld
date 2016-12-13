using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public Transform currentCheckpointSpawn;
    [HideInInspector]
    public GameObject currentPlayerInstance;

    public GameLoop gameLoop;
    public BaseRunner baseRunner;
    public BattleSystemStateMachine battleSystem;
    public PartyManager partyManager;

    private float healthLostOnDeath = 10f;

	public void ReloadCheckpoint()
    {
        if (currentCheckpointSpawn != null && currentPlayerInstance != null)
        {
            // Check players health. Game over / reload the level if health is too low.
            float currentPlayerHealth = currentPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().Health - healthLostOnDeath;
            Transform healthBarHolder = currentPlayerInstance.transform.Find("PlayerController/Player/AnimationsContainer/Canvas/HealthBar");
            Image healthBar = healthBarHolder.gameObject.GetComponent<Image>();
            healthBar.fillAmount = (float)currentPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().Health / (float)currentPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().MaxHealth;

            if (currentPlayerHealth <= 0f)
            {
                // This is where we do game over / reload stuff.
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            battleSystem.healthManager[currentPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().CharacterClassName] = currentPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().Health;

            // Respawn a copy of the player and change the names so that the new one has the old ones name.
            GameObject newPlayerInstance = Instantiate(currentPlayerInstance, Vector3.zero, Quaternion.identity, GameObject.Find("GameManager").transform);
            newPlayerInstance.transform.Find("PlayerController").transform.position = currentCheckpointSpawn.position;
            currentPlayerInstance.name = currentPlayerInstance.name + "(destroying)";
            int indexOfParenthesis = newPlayerInstance.name.IndexOf("(");
            newPlayerInstance.name = newPlayerInstance.name.Substring(0, indexOfParenthesis);

            // Manually relink a bunch of garbage that gets decoupled when destroying the player. Definitely a better way to do this.
            baseRunner.rb = newPlayerInstance.transform.Find("PlayerController").GetComponent<Rigidbody>();
            baseRunner.runner = newPlayerInstance.transform.Find("PlayerController/Player").GetComponent<Animator>();
            baseRunner.playerObjectTransform = newPlayerInstance.transform.Find("PlayerController/Player").transform;
            baseRunner.topRaycastTransform = newPlayerInstance.transform.Find("PlayerController/Player/TopRayCastOrigin").transform;
            baseRunner.bottomRaycastTransform = newPlayerInstance.transform.Find("PlayerController/Player/BottomRayCastOrigin").transform;
            baseRunner.dashBar01 = newPlayerInstance.transform.Find("PlayerController/Player/AnimationsContainer/Canvas/DashBar01").GetComponent<Image>();
            baseRunner.dashBar02 = newPlayerInstance.transform.Find("PlayerController/Player/AnimationsContainer/Canvas/DashBar02").GetComponent<Image>();
            baseRunner.playerController = newPlayerInstance.transform.Find("PlayerController").gameObject;

            battleSystem.moveNamesHolder = newPlayerInstance.transform.Find("PlayerController/MovesContainer").gameObject;
            battleSystem.Move01Text = newPlayerInstance.transform.Find("PlayerController/MovesContainer/Move01/MoveNameText").GetComponent<TextMesh>();
            battleSystem.Move02Text = newPlayerInstance.transform.Find("PlayerController/MovesContainer/Move02/MoveNameText").GetComponent<TextMesh>();
            battleSystem.UltimateText = newPlayerInstance.transform.Find("PlayerController/MovesContainer/Ultimate/MoveNameText").GetComponent<TextMesh>();
            battleSystem.playerPrefab = newPlayerInstance.transform.Find("PlayerController/Player").gameObject;

            partyManager.allyTransform = newPlayerInstance.transform.Find("PlayerController").transform;

            foreach (PartyLayout ally in partyManager.listOfAllies)
            {
                if (newPlayerInstance.transform.Find("PlayerController/" + ally.friendlyPrefab.name + "(Clone)"))
                {
                    Destroy(newPlayerInstance.transform.Find("PlayerController/" + ally.friendlyPrefab.name + "(Clone)").gameObject);
                }
            }

            newPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().Health = (int)currentPlayerHealth;
            healthBarHolder = newPlayerInstance.transform.Find("PlayerController/Player/AnimationsContainer/Canvas/HealthBar");
            healthBar = healthBarHolder.gameObject.GetComponent<Image>();
            healthBar.fillAmount = (float)newPlayerInstance.GetComponentInChildren<BaseCharacterClass>().Health / (float)newPlayerInstance.GetComponentInChildren<BaseCharacterClass>().MaxHealth;

            Destroy(currentPlayerInstance);
            newPlayerInstance.transform.Find("PlayerController/Player").transform.GetComponent<Animator>().SetBool("Grounded Running", true);
            newPlayerInstance.transform.Find("PlayerController/Player").transform.GetComponent<Animator>().SetTrigger("FireEvent");
            Debug.Log(newPlayerInstance.transform.Find("PlayerController/Player").GetComponent<BaseCharacterClass>().Health);
        }
    }
}
