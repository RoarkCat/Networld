using UnityEngine;
using System.Collections;

public class LimitBreakItem : MonoBehaviour {

    public int limitBreakValue = 0;

    void OnTriggerEnter(Collider playerObject)
    {
        if (playerObject.tag == "Player")
        {
            playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent += limitBreakValue;
            Debug.Log(playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent);
            Destroy(this.gameObject);
        }
    }
}
