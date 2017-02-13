using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBattleClass : MonoBehaviour
{
    public DialogueBattleConditions condition;
    public int turnForDialogue;
    public DialogueBattleBoxComponents[] allDialogueForEvent;
    [HideInInspector]
    public bool thisEventIsDone = false;
}

public enum DialogueBattleConditions
{
    BattleBeginning,
    OnTurnX,
    EnemyHealthIs,
    PlayerHealthIs,
    EndBattle
};

[System.Serializable]
public class DialogueBattleBoxComponents
{
    public DialogueAnimatorChoice animationIntroTrigger;
    public GameObject characterImage;
    public string speakerName;
    public string timestamp;
    [TextArea(3, 10)]
    public string textToSay;
    public float timeToSay;
}
