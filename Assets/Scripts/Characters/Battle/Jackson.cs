using UnityEngine;
using System.Collections;

public class Jackson : BaseCharacterClass
{

    public Jackson()
    {
        CharacterClassName = "Jackson";
        CharacterClassDescription = "A powerful gremlin.";
        Health = 90;
        TurnPriority = 2;
        isEnemy = false;
        Move01Damage = 50;
        Move02Damage = 10;
        UltimateDamage = 80;
    }

    public override void Move01()
    {
        proceedNext = true;
        Debug.Log("Jackson move 1!");
        proceedNext = false;
    }

    public override void Move02()
    {
        proceedNext = true;
        Debug.Log("Jackson move 2!");
        proceedNext = false;
    }

    public override void Ultimate()
    {
        proceedNext = true;
        Debug.Log("Jackson dank memes the enemy to oblivion!");
        proceedNext = false;
    }
}
