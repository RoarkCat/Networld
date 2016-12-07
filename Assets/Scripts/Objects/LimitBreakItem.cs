using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LimitBreakItem : MonoBehaviour {

    public int limitBreakValue = 0;

    void OnTriggerEnter(Collider playerObject)
    {
        if (playerObject.tag == "Player")
        {
            CheckForRaycastHit();
        }
    }

    public void CheckForRaycastHit()
    {
        GameObject go = GameObject.Find("GameManager");
        go.GetComponent<LimitBreakCollection>().limitBreakCurrent += limitBreakValue;
        if (go.GetComponent<LimitBreakCollection>().limitBreakCurrent > go.GetComponent<LimitBreakCollection>().limitBreakMax)
        {
            go.GetComponent<LimitBreakCollection>().limitBreakCurrent = go.GetComponent<LimitBreakCollection>().limitBreakMax;
        }
        Transform limitBarHolder = go.transform.Find("PlayerContainer/PlayerController/Canvas/LimitBreakBar");
        if (limitBarHolder == null)
        {
            limitBarHolder = go.transform.Find("PlayerContainer(Clone)/PlayerController/Canvas/LimitBreakBar");
        }
        Image limitBar = limitBarHolder.gameObject.GetComponent<Image>();
        limitBar.fillAmount = (float)go.GetComponent<LimitBreakCollection>().limitBreakCurrent / (float)go.GetComponent<LimitBreakCollection>().limitBreakMax;
        Debug.Log(go.GetComponent<LimitBreakCollection>().limitBreakCurrent);
        Destroy(this.gameObject);
    }
}
