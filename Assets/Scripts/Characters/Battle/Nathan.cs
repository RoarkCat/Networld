using UnityEngine;
using System.Collections;

public class Nathan : BaseCharacterClass
{

    public Nathan()
    {
        CharacterClassName = "Nathan";
        CharacterClassDescription = "Just some nerd.";
        Health = 150;
        TurnPriority = 3;
        isEnemy = false;
        Move01Damage = 50;
        Move02Damage = 10;
        UltimateDamage = 80;
    }

    public override void Move01()
    {
        proceedNext = true;
        Debug.Log("Nathan move 1!");
        proceedNext = false;
    }

    public override void Move02()
    {
        proceedNext = true;
        Debug.Log("Nathan move 2!");
        proceedNext = false;
    }

    public override void Ultimate()
    {
        proceedNext = true;
        Debug.Log("Nathan ultimate!!");
        proceedNext = false;
    }
}
