using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    public GameLoop gameLoop;
    public BaseRunner baseRunner;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BattleZone")
        {
            gameLoop.initiateBattle(other);
        }
        else if (other.tag == "QTEZone")
        {
            gameLoop.initiateQTE(other);
        }
        else if (other.tag == "KillPlayer")
        {
            gameLoop.gameManager.ReloadCheckpoint();
        }
    }

    void OnCollisionEnter()
    {
        baseRunner.grounded = true;
    }

    void OnCollisionExit()
    {
        baseRunner.grounded = false;
        baseRunner.runner.SetBool("Grounded Running", false);
    }
}
