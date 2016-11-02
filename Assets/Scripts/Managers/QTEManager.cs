using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QTEManager : MonoBehaviour {

    public GameLoop gameLoop;
    public GameObject playerPrefab;

    private bool firstPassPosition;
    private bool firstPassOverall = true;
    private bool killPlayerFirstPass = true;
    private GameObject playerController;
    private Image timerBar;
    private GameObject timerBarHolder;
    private float playerStartTravelTime;
    private float playerTravelDistance;
    private float playerTravelSpeed = 1.0f;
    private float startTimeTrack;
    //private CheckForAnimationFinish checkForAnimFinish;

    public void QTEUpdate()
    {
        // move player to set position
        if (gameLoop.qteInstance.stopMovement)
        {
            if (!firstPassPosition)
            {
                playerStartTravelTime = Time.time;
                playerTravelDistance = Vector3.Distance(playerPrefab.transform.parent.position, gameLoop.qteInstance.playerPosition.position);
                firstPassPosition = true;
            }
            float distCovered = (Time.time - playerStartTravelTime) * playerTravelSpeed;
            float fracTraveled = distCovered / playerTravelDistance;
            playerPrefab.transform.parent.position = Vector3.Lerp(playerPrefab.transform.parent.position, gameLoop.qteInstance.playerPosition.position, fracTraveled);
        }

        // start timer
        if (firstPassOverall)
        {
            killPlayerFirstPass = true;
            playerController = GameObject.Find("PlayerController");
            GameObject timerBarGO = playerController.transform.Find("Canvas/TimerBarHolder/TimerBar").gameObject;
            timerBar = timerBarGO.GetComponent<Image>();
            timerBarHolder = playerController.transform.Find("Canvas/TimerBarHolder").gameObject;
            timerBarHolder.SetActive(true);
            startTimeTrack = Time.time;
            firstPassOverall = false;
            foreach (var singleChoice in gameLoop.qteInstance.choices)
            {
                singleChoice.buttonHolder.gameObject.SetActive(true);
            }
        }
        timerBar.fillAmount = ((gameLoop.qteInstance.timeForChoice - (Time.time - startTimeTrack)) / gameLoop.qteInstance.timeForChoice);

        // loop through cases for qtes
        foreach (var singleChoice in gameLoop.qteInstance.choices)
        {
            switch (singleChoice.resultingAction)
            {
                case (ChoiceOutcome.ActivateAnimator):
                    // checkForAnimFinish = singleChoice.animator.gameObject.GetComponent<CheckForAnimationFinish>();
                    if (Input.GetButtonDown(singleChoice.buttonToPress))
                    {
                        singleChoice.animator.SetTrigger(singleChoice.triggerNameIfAnimator);
                        timerBarHolder.SetActive(false);
                        foreach (var singleChoice2 in gameLoop.qteInstance.choices)
                        {
                            singleChoice2.buttonHolder.gameObject.SetActive(false);
                        }
                        firstPassPosition = false;
                        firstPassOverall = true;
                        gameLoop.isQTE = false;
                        gameLoop.isRunning = true;

                    }
                    else if ((Time.time - startTimeTrack) >= gameLoop.qteInstance.timeForChoice)
                    {
                        timerBarHolder.SetActive(false);
                        firstPassPosition = false;
                        firstPassOverall = true;
                        gameLoop.isQTE = false;
                        gameLoop.isRunning = true;
                        foreach (var singleChoice2 in gameLoop.qteInstance.choices)
                        {
                            singleChoice2.buttonHolder.gameObject.SetActive(false);
                        }
                    }
                    //if (checkForAnimFinish.moveIsFinished)
                    //{
                        // If you feel like implementing a wait... but platforms mess up super bad. Need to find a way to make the player controller copy movements of moving objects.

                        //firstPassPosition = false;
                        //firstPassOverall = true;
                        //gameLoop.isQTE = false;
                        //gameLoop.isRunning = true;
                    //}
                    break;
                case (ChoiceOutcome.KillPlayerIfFail):
                    if (killPlayerFirstPass)
                    {
                        singleChoice.animator.SetTrigger("Begin");
                        killPlayerFirstPass = false;
                    }
                    if (Input.GetButtonDown(singleChoice.buttonToPress))
                    {
                        singleChoice.animator.SetTrigger(singleChoice.triggerNameIfAnimator);
                        timerBarHolder.SetActive(false);
                        foreach (var singleChoice2 in gameLoop.qteInstance.choices)
                        {
                            singleChoice2.buttonHolder.gameObject.SetActive(false);
                        }
                        firstPassPosition = false;
                        firstPassOverall = true;
                        gameLoop.isQTE = false;
                        gameLoop.isRunning = true;

                    }
                    else if ((Time.time - startTimeTrack) >= gameLoop.qteInstance.timeForChoice)
                    {
                        timerBarHolder.SetActive(false);
                        firstPassPosition = false;
                        firstPassOverall = true;
                        gameLoop.isQTE = false;
                        gameLoop.isRunning = true;
                        foreach (var singleChoice2 in gameLoop.qteInstance.choices)
                        {
                            singleChoice2.buttonHolder.gameObject.SetActive(false);
                        }
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    break;
            }
        }
    }
}
