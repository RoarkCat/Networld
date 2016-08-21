using UnityEngine;
using System.Collections;

public class BasicEnemy : BaseCharacterClass
{
    private bool moveHasExecuted;

    public BasicEnemy()
    {
        CharacterClassName = "Enemy";
        CharacterClassDescription = "This a bad guy.";
        Health = 250;
        MaxHealth = 250;
        TurnPriority = 4;
        isEnemy = true;
        Move01Damage = 20;
        Move02Damage = 30;
        UltimateDamage = 50;
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
