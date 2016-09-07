using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour {

    public int healthRestore = 0;
    public BattleSystemStateMachine battleSystemRef;
    public GameLoop gameLoop;

    void OnTriggerEnter(Collider playerObject)
    {
        if (playerObject.tag == "Player")
        {
            playerObject.GetComponentInChildren<BaseCharacterClass>().Health += healthRestore;
            if (playerObject.GetComponentInChildren<BaseCharacterClass>().Health > playerObject.GetComponentInChildren<BaseCharacterClass>().MaxHealth)
            {
                playerObject.GetComponentInChildren<BaseCharacterClass>().Health = playerObject.GetComponentInChildren<BaseCharacterClass>().MaxHealth;
            }
            Transform healthBarHolder = playerObject.transform.Find("Player/AnimationsContainer/Canvas/HealthBar");
            Image healthBar = healthBarHolder.gameObject.GetComponent<Image>();
            healthBar.fillAmount = (float)playerObject.GetComponentInChildren<BaseCharacterClass>().Health / (float)playerObject.GetComponentInChildren<BaseCharacterClass>().MaxHealth;

            incrementAllyHealth();

            Debug.Log("Destroying health item");
            Destroy(gameObject);
        }
    }

    void incrementAllyHealth()
    {

        foreach (PartyLayout ally in gameLoop.partyManager.listOfAllies)
        {
            BaseCharacterClass currentChar = ally.friendlyPrefab.GetComponent<BaseCharacterClass>();
            Debug.Log(gameLoop.partyManager.listOfAllies);
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
