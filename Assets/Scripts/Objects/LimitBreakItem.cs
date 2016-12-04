using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LimitBreakItem : MonoBehaviour {

    public int limitBreakValue = 0;

    void OnTriggerEnter(Collider playerObject)
    {
        if (playerObject.tag == "Player")
        {
            playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent += limitBreakValue;
            if (playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent > playerObject.GetComponent<LimitBreakCollection>().limitBreakMax)
            {
                playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent = playerObject.GetComponent<LimitBreakCollection>().limitBreakMax;
            }
            Transform limitBarHolder = playerObject.transform.Find("Canvas/LimitBreakBar");
            Image limitBar = limitBarHolder.gameObject.GetComponent<Image>();
            limitBar.fillAmount = (float)playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent / (float)playerObject.GetComponent<LimitBreakCollection>().limitBreakMax;
            Debug.Log(playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent);
            Destroy(this.gameObject);
        }
    }

    public void CheckForRaycastHit(Collider playerObject)
    {
        playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent += limitBreakValue;
        if (playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent > playerObject.GetComponent<LimitBreakCollection>().limitBreakMax)
        {
            playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent = playerObject.GetComponent<LimitBreakCollection>().limitBreakMax;
        }
        Transform limitBarHolder = playerObject.transform.Find("Canvas/LimitBreakBar");
        Image limitBar = limitBarHolder.gameObject.GetComponent<Image>();
        limitBar.fillAmount = (float)playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent / (float)playerObject.GetComponent<LimitBreakCollection>().limitBreakMax;
        Debug.Log(playerObject.GetComponent<LimitBreakCollection>().limitBreakCurrent);
        Destroy(this.gameObject);
    }
}
