using UnityEngine;
using System.Collections;

public class MageEnemy : BaseCharacterClass
{

    private bool moveHasExecuted;

    public MageEnemy()
    {
        CharacterClassName = "Mage Enemy";
        CharacterClassDescription = "This a bad guy.";
        Health = 200;
        MaxHealth = 200;
        TurnPriority = 4;
        isEnemy = true;
        Move01Damage = 30;
        Move02Damage = 40;
        UltimateDamage = 45;
        proceedNext = true;
    }

    public override void Move01()
    {
        proceedNext = true;
        if (!moveHasExecuted)
        {
            Debug.Log("Mage enemy move 1!");
            moveHasExecuted = true;
        }
        if (moveIsFinished)
        {
            moveHasExecuted = false;
            moveIsFinished = false;
            proceedNext = false;
        }
    }

    public override void Move02()
    {
        proceedNext = true;
        if (!moveHasExecuted)
        {
            Debug.Log("Mage enemy move 2!");
            moveHasExecuted = true;
        }
        if (moveIsFinished)
        {
            moveHasExecuted = false;
            moveIsFinished = false;
            proceedNext = false;
        }
    }

    public override void Ultimate()
    {
        proceedNext = true;
        if (!moveHasExecuted)
        {
            Debug.Log("Mage enemy ultimate!");
            moveHasExecuted = true;
        }
        if (moveIsFinished)
        {
            moveHasExecuted = false;
            moveIsFinished = false;
            proceedNext = false;
        }
    }
}
