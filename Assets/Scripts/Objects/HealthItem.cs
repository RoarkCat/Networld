using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour {

    public int healthRestore = 0;
    public BattleSystemStateMachine battleSystemRef;
    public GameLoop gameLoop;
    private bool isHitThroughCollision = false;

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
        Debug.Log("Restoring " + healthRestore + " health.");
        go.GetComponentInChildren<BaseCharacterClass>().Health += healthRestore;
        if (go.GetComponentInChildren<BaseCharacterClass>().Health > go.GetComponentInChildren<BaseCharacterClass>().MaxHealth)
        {
            go.GetComponentInChildren<BaseCharacterClass>().Health = go.GetComponentInChildren<BaseCharacterClass>().MaxHealth;
        }
        battleSystemRef.healthManager[go.GetComponentInChildren<BaseCharacterClass>().CharacterClassName] = go.GetComponentInChildren<BaseCharacterClass>().Health;
        Transform healthBarHolder = go.transform.Find("PlayerContainer/PlayerController/Player/AnimationsContainer/Canvas/HealthBar");
        if (healthBarHolder == null)
        {
            healthBarHolder = go.transform.Find("PlayerContainer(Clone)/PlayerController/Player/AnimationsContainer/Canvas/HealthBar");
        }
        Image healthBar = healthBarHolder.gameObject.GetComponent<Image>();
        healthBar.fillAmount = (float)go.GetComponentInChildren<BaseCharacterClass>().Health / (float)go.GetComponentInChildren<BaseCharacterClass>().MaxHealth;

        incrementAllyHealth();

        Destroy(gameObject);
    }

    void incrementAllyHealth()
    {

        foreach (PartyLayout ally in gameLoop.partyManager.listOfAllies)
        {
            BaseCharacterClass currentChar = ally.friendlyPrefab.GetComponent<BaseCharacterClass>();
            List<string> keys = new List<string>(battleSystemRef.healthManager.Keys);
            foreach (string key in keys)
            {
                if (currentChar.CharacterClassName == key)
                {
                    battleSystemRef.healthManager[key] = battleSystemRef.healthManager[key] + healthRestore;
                    if (battleSystemRef.healthManager[key] >= currentChar.MaxHealth)
                    {
                        battleSystemRef.healthManager[key] = currentChar.MaxHealth;
                    }
                }
            }
        }
    }
}
