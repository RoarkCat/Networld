using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCollisionDamage : MonoBehaviour {

    public int damageToDo = 0;
    public BattleSystemStateMachine battleSystemRef;

    private BaseCharacterClass hitCharacter;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<BaseCharacterClass>() == null)
            {
                hitCharacter = other.GetComponentInChildren<BaseCharacterClass>();
            }
            else
            {
                hitCharacter = other.GetComponent<BaseCharacterClass>();
            }

            hitCharacter.Health = hitCharacter.Health - damageToDo;
            battleSystemRef.healthManager[hitCharacter.CharacterClassName] = hitCharacter.Health;

            Transform healthBarHolder = hitCharacter.transform.Find("Player/AnimationsContainer/Canvas/HealthBar");
            if (healthBarHolder == null)
            {
                healthBarHolder = hitCharacter.gameObject.transform.Find("AnimationsContainer/Canvas/HealthBar");
            }
            Image healthBar = healthBarHolder.gameObject.GetComponent<Image>();
            healthBar.fillAmount = (float)hitCharacter.Health / (float)hitCharacter.MaxHealth;
            Debug.Log(hitCharacter.CharacterClassName + " now has " + hitCharacter.Health);
        }
    }
}
