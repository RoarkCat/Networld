using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueClass : MonoBehaviour
{
    public bool stopMovement;
    public bool isDuringBattle;
    public Transform playerPosition;
    public DialogueBoxComponents[] allDialogueForEvent;
}

public enum DialogueAnimatorChoice
{
    AnimateInFromLeft,
    AnimateInFromRight
};

[System.Serializable]
public class DialogueBoxComponents
{
    public DialogueAnimatorChoice animationIntroTrigger;
    public GameObject characterImage;
    public string speakerName;
    public string timestamp;
    [TextArea(3,10)]
    public string textToSay;
    public float timeToSay;
}
