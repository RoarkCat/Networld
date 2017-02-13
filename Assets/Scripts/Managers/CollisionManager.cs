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
            if (other.GetComponent<DialogueClass>() != null)
            {
                gameLoop.initiateDialogueInsideBattle(other);
            }
        }
        else if (other.tag == "QTEZone")
        {
            gameLoop.initiateQTE(other);
        }
        else if (other.tag == "DialogueZone")
        {
            gameLoop.initiateDialogue(other);
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
