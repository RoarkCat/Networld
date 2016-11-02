using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChoiceOutcome {
    ActivateAnimator,
    KillPlayerIfFail
};

[System.Serializable]
public class SingleChoice
{
    public string buttonToPress;
    public GameObject buttonHolder;
    public ChoiceOutcome resultingAction;
    public string triggerNameIfAnimator;
    public Animator animator;
}

public class QTEChoiceClass : MonoBehaviour {
    public bool stopMovement;
    public Transform playerPosition;
    public float timeForChoice;
    public SingleChoice[] choices;
}
