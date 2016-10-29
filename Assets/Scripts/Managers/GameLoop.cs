using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

    public BaseRunner runnerControl;
    public BattleSystemStateMachine battleControl;
    public bool isRunning = true;
    public EncounterScript battleEncounterInstance;
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
        if (isRunning)
        {
            runnerControl.runnerUpdate();
        }
        else
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
        if (isRunning == false)
        {
            battleControl.battleOnGUI();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BattleZone")
        {
            cameraAnimator.SetBool("BattleState", true);
            cameraAnimator.SetBool("RunnerState", false);
            isRunning = false;
            battleEncounterInstance = other.gameObject.GetComponent<EncounterScript>();
        }
    }
}
