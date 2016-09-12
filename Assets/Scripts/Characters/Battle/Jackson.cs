using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Jackson : BaseCharacterClass
{
    private bool moveFirstPass = true;
    private float startTimeTrack;

    private GameObject playerController;
    public GameLoop gameLoop;
    public BattleSystemStateMachine battleSystem;
    public Image timerBar;
    public GameObject timerBarHolder;

    // move 01 stuff
    public GameObject concentricCirclesHolder;
    private float move01Time = 3.5f;
    private List<GameObject> clonedTargets;
    private List<string> potentialButtons;
    private bool beginShrink = true;
    private int currentEnemy = 0;
    private List<Vector3> positionSpawnList = new List<Vector3>();
    private List<string> keyPressList = new List<string>();
    private ConcentricCircleTarget targetScript;
    private GameObject buttonIndicator;

    public Jackson()
    {
        CharacterClassName = "Jackson";
        CharacterClassDescription = "A powerful gremlin.";
        Health = 90;
        MaxHealth = 90;
        TurnPriority = 2;
        isEnemy = false;
        Move01Damage = 75;
        Move02Damage = 10;
        UltimateDamage = 80;
        Move01Name = "Hidden Missile";
        Move02Name = "Jackson Move 02";
        UltimateName = "Jackson Ultimate";
        UltimateLimitRequirement = 100;
    }

    void Start()
    {
        playerController = GameObject.Find("PlayerController");
        gameLoop = playerController.GetComponent<GameLoop>();
        GameObject timerBarGO = playerController.transform.Find("OrthoCamera/Canvas/TimerBarHolder/TimerBar").gameObject;
        timerBar = timerBarGO.GetComponent<Image>();
        timerBarHolder = playerController.transform.Find("OrthoCamera/Canvas/TimerBarHolder").gameObject;
        concentricCirclesHolder = playerController.transform.Find("Main Camera/BattleMovesObjects/ConcentricCirclesHolder").gameObject;
    }

    public override void Move01()
    {
        proceedNext = true;
        if (moveFirstPass)
        {
            battleSystem = playerController.GetComponent<BattleSystemStateMachine>();
            startTimeTrack = Time.time;
            clonedTargets = new List<GameObject>();
            damagedEnemies = new List<GameObject>();
            moveFirstPass = false;
            currentEnemy = 0;
            damageDict = new Dictionary<BaseCharacterClass, int>();
            potentialButtons = new List<string>(new string[] { "a", "s", "d", "f", "space" });
            foreach (EnemyLayout enem in gameLoop.battleEncounterInstance.listOfEnemies)
            {
                float randomInt = UnityEngine.Random.Range(0, potentialButtons.Count);
                positionSpawnList.Add(enem.enemyPosition + enem.enemyColliderPosition);
                Debug.Log("Adding " + potentialButtons[(int)randomInt] + " to potential buttons.");
                keyPressList.Add(potentialButtons[(int)randomInt]);
                potentialButtons.Remove(potentialButtons[(int)randomInt]);
            }
        }
        if (battleSystem.enemyList[currentEnemy].isDead)
        {
            Debug.Log("adding " + currentEnemy);
            currentEnemy++;
        }
        else {
            if (beginShrink)
            {
                GameObject newCircleTarget = Instantiate(concentricCirclesHolder, Vector3.zero, Quaternion.identity);
                newCircleTarget.transform.parent = gameLoop.battleEncounterInstance.transform;
                newCircleTarget.transform.localPosition = positionSpawnList[currentEnemy];
                targetScript = newCircleTarget.GetComponent<ConcentricCircleTarget>();
                targetScript.stopShrinking = false;
                targetScript.keyPressed = keyPressList[currentEnemy];
                buttonIndicator = newCircleTarget.transform.Find("ButtonPressHolder/" + keyPressList[currentEnemy]).gameObject;
                buttonIndicator.SetActive(true);
                newCircleTarget.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                foreach (Transform child in newCircleTarget.transform)
                {
                    child.gameObject.SetActive(true);
                }
                clonedTargets.Add(newCircleTarget);
                beginShrink = false;
            }
            else if (targetScript.stopShrinking)
            {
                float targetX = targetScript.targetRing.transform.localScale.x;
                float currentX = targetScript.scalingRing.transform.localScale.x;
                float subtractFromDamage = Mathf.Abs(targetX - currentX);
                int move01Minus = (int)(Move01Damage - (subtractFromDamage * 30));
                if (move01Minus < 0) { move01Minus = 0; }
                damageDict.Add(battleSystem.enemyList[currentEnemy], move01Minus);
                currentEnemy++;
                buttonIndicator.SetActive(false);
                beginShrink = true;
            }
            else if (targetScript.fracScale >= 1 && targetScript.fracScale <= 2)
            {
                damageDict.Add(battleSystem.enemyList[currentEnemy], 0);
                currentEnemy++;
                beginShrink = true;
                buttonIndicator.SetActive(false);
            }
        }
        if (currentEnemy >= gameLoop.battleEncounterInstance.listOfEnemies.Count)
        {
            foreach (GameObject circleTarget in clonedTargets)
            {
                Destroy(circleTarget);
            }
            moveFirstPass = true;
            proceedNext = false;
        }
    }

    public override void Move02()
    {
        proceedNext = true;
        HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
        damagedEnemies = new List<GameObject>();
        foreach (HitboxCollision hbox in checkHitbox)
        {
            if (hbox.isHit)
            {
                //damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Jackson move 2!");
        proceedNext = false;
    }

    public override void Ultimate()
    {
        proceedNext = true;
        HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
        damagedEnemies = new List<GameObject>();
        foreach (HitboxCollision hbox in checkHitbox)
        {
            if (hbox.isHit)
            {
                //damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("Jackson dank memes the enemy to oblivion!");
        proceedNext = false;
    }
}
