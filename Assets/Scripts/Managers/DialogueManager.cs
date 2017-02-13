using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public GameLoop gameLoop;
    public GameObject dialogueContainer;
    public TextMesh speakerName, timeStamp, bodyText;
    public BattleSystemStateMachine battleSystem;

    private Animator dialogueAnimator;
    private int trackDialogue = 0;
    private bool dialogueInitialization = false;
    private float startTime = 0;
    private CheckForAnimationFinish endAnimCheck;

    public void DialogueUpdate()
    {
        if (!gameLoop.isBattle)
        {
            DialogueEventOutsideBattle();
        }
    }

    // Controls dialogue that happens outside of battle.
    public void DialogueEventOutsideBattle()
    {
        dialogueAnimator = dialogueContainer.GetComponent<Animator>();
        dialogueContainer.SetActive(true);

        if (!dialogueInitialization)
        {
            startTime = Time.time;
            endAnimCheck = dialogueContainer.GetComponent<CheckForAnimationFinish>();
            dialogueInitialization = true;
        }

        if (trackDialogue < gameLoop.dialogueInstance.allDialogueForEvent.Length)
        {
            switch (gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].animationIntroTrigger)
            {
                case (DialogueAnimatorChoice.AnimateInFromLeft):
                    if (trackDialogue == 0)
                    {
                        dialogueAnimator.SetBool("EnterLeft", true);
                    }
                    else if (gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue - 1].animationIntroTrigger == DialogueAnimatorChoice.AnimateInFromRight)
                    {
                        dialogueAnimator.SetBool("EnterRight", false);
                        dialogueAnimator.SetBool("ExitLeft", false);
                        dialogueAnimator.SetBool("ExitRight", true);
                        dialogueAnimator.SetBool("EnterLeft", true);
                    }
                    break;
                case (DialogueAnimatorChoice.AnimateInFromRight):
                    if (trackDialogue == 0)
                    {
                        dialogueAnimator.SetBool("EnterRight", true);
                    }
                    else if (gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue - 1].animationIntroTrigger == DialogueAnimatorChoice.AnimateInFromLeft)
                    {
                        dialogueAnimator.SetBool("EnterRight", true);
                        dialogueAnimator.SetBool("ExitLeft", true);
                        dialogueAnimator.SetBool("ExitRight", false);
                        dialogueAnimator.SetBool("EnterLeft", false);
                    }
                    break;
            }

            if (trackDialogue == 0)
            {
                speakerName.text = gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].speakerName;
                timeStamp.text = gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].timestamp;
                bodyText.text = gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].textToSay;
                gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].characterImage.SetActive(true);
                endAnimCheck.moveIsFinished = false;
            }
            else
            {
                if (endAnimCheck.moveIsFinished)
                {
                    speakerName.text = gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].speakerName;
                    timeStamp.text = gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].timestamp;
                    bodyText.text = gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].textToSay;
                    gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue - 1].characterImage.SetActive(false);
                    gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].characterImage.SetActive(true);
                    endAnimCheck.moveIsFinished = false;
                }
            }

            if (Input.GetKeyDown("space") && gameLoop.dialogueInstance.stopMovement)
            {
                trackDialogue++;
            }
            else if (Time.time - startTime >= gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue].timeToSay && !gameLoop.dialogueInstance.stopMovement)
            {
                startTime = Time.time;
                trackDialogue++;
            }
        }
        else
        {
            switch (gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue - 1].animationIntroTrigger)
            {
                case (DialogueAnimatorChoice.AnimateInFromLeft):
                    dialogueAnimator.SetBool("ExitLeft", true);
                    break;
                case (DialogueAnimatorChoice.AnimateInFromRight):
                    dialogueAnimator.SetBool("ExitRight", true);
                    break;
            }
            if (endAnimCheck.moveIsFinished)
            {
                endAnimCheck.moveIsFinished = false;
                dialogueAnimator.SetBool("EnterRight", false);
                dialogueAnimator.SetBool("ExitLeft", false);
                dialogueAnimator.SetBool("ExitRight", false);
                dialogueAnimator.SetBool("EnterLeft", false);
                gameLoop.dialogueInstance.allDialogueForEvent[trackDialogue - 1].characterImage.SetActive(false);
                dialogueContainer.SetActive(false);
                trackDialogue = 0;
                dialogueInitialization = false;
                gameLoop.dialogueInstance = null;
                gameLoop.isDialogue = false;
                gameLoop.isRunning = true;
            }
        }
    }

    public void DialogueEventInsideBattle(DialogueBattleClass dialogueData)
    {
        dialogueAnimator = dialogueContainer.GetComponent<Animator>();
        dialogueContainer.SetActive(true);

        if (!dialogueInitialization)
        {
            startTime = Time.time;
            endAnimCheck = dialogueContainer.GetComponent<CheckForAnimationFinish>();
            dialogueInitialization = true;
        }

        if (trackDialogue < dialogueData.allDialogueForEvent.Length)
        {
            switch (dialogueData.allDialogueForEvent[trackDialogue].animationIntroTrigger)
            {
                case (DialogueAnimatorChoice.AnimateInFromLeft):
                    if (trackDialogue == 0)
                    {
                        dialogueAnimator.SetBool("EnterLeft", true);
                    }
                    else if (dialogueData.allDialogueForEvent[trackDialogue - 1].animationIntroTrigger == DialogueAnimatorChoice.AnimateInFromRight)
                    {
                        dialogueAnimator.SetBool("EnterRight", false);
                        dialogueAnimator.SetBool("ExitLeft", false);
                        dialogueAnimator.SetBool("ExitRight", true);
                        dialogueAnimator.SetBool("EnterLeft", true);
                    }
                    break;
                case (DialogueAnimatorChoice.AnimateInFromRight):
                    if (trackDialogue == 0)
                    {
                        dialogueAnimator.SetBool("EnterRight", true);
                    }
                    else if (dialogueData.allDialogueForEvent[trackDialogue - 1].animationIntroTrigger == DialogueAnimatorChoice.AnimateInFromLeft)
                    {
                        dialogueAnimator.SetBool("EnterRight", true);
                        dialogueAnimator.SetBool("ExitLeft", true);
                        dialogueAnimator.SetBool("ExitRight", false);
                        dialogueAnimator.SetBool("EnterLeft", false);
                    }
                    break;
            }

            if (trackDialogue == 0)
            {
                speakerName.text = dialogueData.allDialogueForEvent[trackDialogue].speakerName;
                timeStamp.text = dialogueData.allDialogueForEvent[trackDialogue].timestamp;
                bodyText.text = dialogueData.allDialogueForEvent[trackDialogue].textToSay;
                dialogueData.allDialogueForEvent[trackDialogue].characterImage.SetActive(true);
                endAnimCheck.moveIsFinished = false;
            }
            else
            {
                if (endAnimCheck.moveIsFinished)
                {
                    speakerName.text = dialogueData.allDialogueForEvent[trackDialogue].speakerName;
                    timeStamp.text = dialogueData.allDialogueForEvent[trackDialogue].timestamp;
                    bodyText.text = dialogueData.allDialogueForEvent[trackDialogue].textToSay;
                    dialogueData.allDialogueForEvent[trackDialogue - 1].characterImage.SetActive(false);
                    dialogueData.allDialogueForEvent[trackDialogue].characterImage.SetActive(true);
                    endAnimCheck.moveIsFinished = false;
                }
            }

            if (Input.GetKeyDown("space"))
            {
                trackDialogue++;
            }
            /*else if (Time.time - startTime >= dialogueData.allDialogueForEvent[trackDialogue].timeToSay)
            {
                startTime = Time.time;
                trackDialogue++;
            }*/
        }
        else
        {
            switch (dialogueData.allDialogueForEvent[trackDialogue - 1].animationIntroTrigger)
            {
                case (DialogueAnimatorChoice.AnimateInFromLeft):
                    dialogueAnimator.SetBool("ExitLeft", true);
                    break;
                case (DialogueAnimatorChoice.AnimateInFromRight):
                    dialogueAnimator.SetBool("ExitRight", true);
                    break;
            }
            if (endAnimCheck.moveIsFinished)
            {
                endAnimCheck.moveIsFinished = false;
                dialogueAnimator.SetBool("EnterRight", false);
                dialogueAnimator.SetBool("ExitLeft", false);
                dialogueAnimator.SetBool("ExitRight", false);
                dialogueAnimator.SetBool("EnterLeft", false);
                dialogueData.allDialogueForEvent[trackDialogue - 1].characterImage.SetActive(false);
                dialogueContainer.SetActive(false);
                trackDialogue = 0;
                dialogueInitialization = false;
                battleSystem.dialogueConditionIsMet = false;
                dialogueData.thisEventIsDone = true;
                //gameLoop.dialogueInstance = null;
                //gameLoop.isDialogue = false;
                //gameLoop.isRunning = true;
            }
        }
    }
}
