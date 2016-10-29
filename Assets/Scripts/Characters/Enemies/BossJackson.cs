using UnityEngine;
using System.Collections;

public class BossJackson : BaseCharacterClass
{
    private bool moveHasExecuted;

    public BossJackson()
    {
        CharacterClassName = "BigBadJackson";
        CharacterClassDescription = "This a boss.";
        Health = 600;
        MaxHealth = 600;
        TurnPriority = 10;
        isEnemy = true;
        Move01Damage = 50;
        Move02Damage = 80;
        UltimateDamage = 100;
        proceedNext = true;
    }

    public override void Move01()
    {
        proceedNext = true;
        if (!moveHasExecuted)
        {
            moveIsFinished = false;
            Debug.Log("Enemy move 1!");
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
            moveIsFinished = false;
            Debug.Log("Enemy move 2!");
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
            moveIsFinished = false;
            Debug.Log("Enemy ultimate!!");
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
