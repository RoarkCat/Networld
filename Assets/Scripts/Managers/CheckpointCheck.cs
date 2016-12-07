using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCheck : MonoBehaviour {

    public Transform spawnLocation;
    public GameManager gameManager;

    void OnTriggerEnter (Collider player)
    {
        if (player.tag == "Player")
        {
            feedDataToGameManager(player);
        }
    }

    public void feedDataToGameManager(Collider player)
    {
        gameManager.currentCheckpointSpawn = spawnLocation;
        gameManager.currentPlayerInstance = player.transform.parent.gameObject;
    }
}
