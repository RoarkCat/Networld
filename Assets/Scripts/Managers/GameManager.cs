using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public Transform currentCheckpointSpawn;
    [HideInInspector]
    public GameObject currentPlayerInstance;

    public GameLoop gameLoop;
    public BaseRunner baseRunner;
    public BattleSystemStateMachine battleSystem;
    public PartyManager partyManager;

	public void ReloadCheckpoint()
    {
        if (currentCheckpointSpawn != null && currentPlayerInstance != null)
        {
            GameObject newPlayerInstance = Instantiate(currentPlayerInstance, currentCheckpointSpawn.position, Quaternion.identity, GameObject.Find("GameManager").transform);

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

            Destroy(currentPlayerInstance);
            Debug.Log("reloading checkpoint");
        }
    }
}
