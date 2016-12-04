using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour {

    public GameObject endLevelObject;
    public GameLoop gameLoop;

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameLoop.isRunning = false;
            endLevelObject.SetActive(true);
        }
    }
}
