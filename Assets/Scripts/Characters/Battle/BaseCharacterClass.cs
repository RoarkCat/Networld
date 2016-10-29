using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseCharacterClass : MonoBehaviour {

    public string CharacterClassName { get; set; }
    public string CharacterClassDescription { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int TurnPriority { get; set; }
    public bool proceedNext { get; set; }
    public bool isEnemy { get; set; }
    public bool isDead { get; set; }

    public abstract void Move01();
    public int Move01Damage { get; set; }
    public string Move01Name { get; set; }

    public abstract void Move02();
    public int Move02Damage { get; set; }
    public string Move02Name { get; set; }

    public abstract void Ultimate();
    public int UltimateDamage { get; set; }
    public string UltimateName { get; set; }
    public int UltimateLimitRequirement { get; set; }

    public List<GameObject> damagedEnemies { get; set; }
    public Dictionary<BaseCharacterClass, int> damageDict { get; set; }
    public bool moveIsFinished { get; set; }

    public float rightEdgeOfScreen = 13.36f;
    public float leftEdgeOfScreen = -10f;

}
