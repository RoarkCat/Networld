using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Jackson : BaseCharacterClass
{

    public Jackson()
    {
        CharacterClassName = "Jackson";
        CharacterClassDescription = "A powerful gremlin.";
        Health = 90;
        MaxHealth = 90;
        TurnPriority = 2;
        isEnemy = false;
        Move01Damage = 50;
        Move02Damage = 10;
        UltimateDamage = 80;
    }

    public override void Move01()
    {
        proceedNext = true;
        HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
        damagedEnemies = new List<GameObject>();
        foreach (HitboxCollision hbox in checkHitbox)
        {
            if (hbox.isHit)
            {
                damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        proceedNext = false;
    }

    public override void Move02()
    {
        proceedNext = true;
        HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
        damagedEnemies = new List<GameObject>();
        foreach (HitboxCollision hbox in checkHitbox)
        {
            if (hbox.isHit)
            {
                damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Jackson move 2!");
        proceedNext = false;
    }

    public override void Ultimate()
    {
        proceedNext = true;
        HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
        damagedEnemies = new List<GameObject>();
        foreach (HitboxCollision hbox in checkHitbox)
        {
            if (hbox.isHit)
            {
                damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Jackson dank memes the enemy to oblivion!");
        proceedNext = false;
    }
}
