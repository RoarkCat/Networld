using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitboxCollision : MonoBehaviour {

    public bool isHit = false;
    public List<GameObject> hitboxGameObject;

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.tag == "AllyBattleCollider")
    //    {
    //        isHit = true;
    //    }
    //}

    //void OnTriggerExit(Collider col)
    //{
    //    if (col.tag == "AllyBattleCollider")
    //    {
    //        isHit = false;
    //    }
    //}

    void OnTriggerEnter (Collider col)
    {
        if (col.tag == "EnemyHitbox")
        {
            isHit = true;
            hitboxGameObject.Add(col.gameObject.transform.parent.transform.parent.gameObject);
        }
    }

    void OnTriggerExit (Collider col)
    {
        if (col.tag == "EnemyHitbox")
        {
            isHit = false;
            hitboxGameObject.Remove(col.gameObject.transform.parent.transform.parent.gameObject);
        }
    }
    
}
