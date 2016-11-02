using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

    public BaseRunner runnerControl;
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
            cameraAnimator.SetBool("BattleState", true);
            cameraAnimator.SetBool("RunnerState", false);
            isRunning = false;
            isBattle = true;
            battleEncounterInstance = other.gameObject.GetComponent<EncounterScript>();
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
    }
}
