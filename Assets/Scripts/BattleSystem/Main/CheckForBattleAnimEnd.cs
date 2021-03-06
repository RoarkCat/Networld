﻿using UnityEngine;
using System.Collections;

public class CheckForBattleAnimEnd : MonoBehaviour {

    BattleSystemStateMachine battleSystem;

	public void fireMoveFinish ()
    {
        GameObject go = GameObject.Find("GameManager");
        battleSystem = go.GetComponent<BattleSystemStateMachine>();
        foreach (BaseCharacterClass participant in battleSystem.participantList) {
            participant.moveIsFinished = true;
        }
    } 
}
