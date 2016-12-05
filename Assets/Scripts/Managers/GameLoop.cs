using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour {

    public BaseRunner runnerControl;
    public TextMesh timerText;
    public BattleSystemStateMachine battleControl;
    public QTEManager qteControl;
    public bool isRunning = true;
    public bool isBattle = false;
    public bool isQTE = false;
    public EncounterScript battleEncounterInstance;
    public QTEChoiceClass qteInstance;
    public PartyManager partyManager;
    public Animator cameraAnimator;
    private LimitBreakCollection limitBreakCollection = new LimitBreakCollection();

    void Start()
    {
        runnerControl.runnerStart();
        battleControl.battleStart();
    }

    void Update()
    {
        if (isQTE)
        {
            qteControl.QTEUpdate();
        }

        if (isRunning)
        {
            runnerControl.runnerUpdate();
        }
        else if (isBattle)
        {
            battleControl.battleUpdate();
        }
        timerText.text = ((int)Time.time).ToString();
    }

    void FixedUpdate()
    {
        if (isRunning)
        {
            runnerControl.runnerFixedUpdate();
        }
        else
        {
            runnerControl.stopFixedUpdate();
        }
    }

    void OnGUI()
    {
        if (isBattle == true)
        {
            //battleControl.battleOnGUI();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BattleZone")
        {
            initiateBattle(other);
        }
        else if (other.tag == "QTEZone")
        {
            qteInstance = other.gameObject.GetComponent<QTEChoiceClass>();
            isQTE = true;
            if (qteInstance.stopMovement)
            {
                isRunning = false;
            }
        }
        else if (other.tag == "KillPlayer")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void initiateBattle(Collider other)
    {
        cameraAnimator.SetBool("BattleState", true);
        cameraAnimator.SetBool("RunnerState", false);
        isRunning = false;
        isBattle = true;
        battleEncounterInstance = other.gameObject.GetComponent<EncounterScript>();
    }
}
