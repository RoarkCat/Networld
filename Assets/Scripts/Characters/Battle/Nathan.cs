using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Nathan : BaseCharacterClass
{

    public Nathan()
    {
        CharacterClassName = "Nathan";
        CharacterClassDescription = "Just some nerd.";
        Health = 150;
        MaxHealth = 150;
        TurnPriority = 3;
        isEnemy = false;
        Move01Damage = 50;
        Move02Damage = 10;
        UltimateDamage = 80;
        Move01Name = "Nathan Move 01";
        Move02Name = "Nathan Move 02";
        UltimateName = "Nathan Ultimate";
        UltimateLimitRequirement = 50;
        damageDict = new Dictionary<BaseCharacterClass, int>();
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
                //damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Nathan move 1!");
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
                //damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Nathan move 2!");
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
                //damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Nathan ultimate!!");
        proceedNext = false;
    }
}
