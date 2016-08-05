using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

    public BaseRunner runnerControl;
    public BattleSystemStateMachine battleControl;
    public bool isRunning = true;
    public EncounterScript battleEncounterInstance;
    public PartyManager partyManager;

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
            isRunning = false;
            battleEncounterInstance = other.gameObject.GetComponent<EncounterScript>();
        }
    }
}
