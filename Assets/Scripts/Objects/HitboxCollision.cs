using UnityEngine;
using System.Collections;

public class HitboxCollision : MonoBehaviour {

    public bool isHit = false;
    public GameObject hitboxGameObject;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "AllyBattleCollider")
        {
            isHit = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "AllyBattleCollider")
        {
            isHit = false;
        }
    }
    
}
