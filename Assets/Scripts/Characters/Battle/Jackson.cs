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

    // stuff for move 2
    private GameObject arrowContainer;
    private GameObject targetBarHolder;
    private GameObject targetBarTarget;
    private GameObject targetBarBackfill;
    private GameObject beginPoint;
    private GameObject endPoint;
    private int boltsLanded = 0;
    private float move02Time = 3.5f;
    private bool rotateUp = true;
    private bool rotateDown = false;
    private List<GameObject> clonedBolts;
    private float randomRotationValue = 0;
    private float scaleGrowthValue = 0.025f;
    private Vector3 arrowScale;
    private float randomTimeBetweenRotations;
    private float trackRotationTime;
    private float distanceFromBegToEnd;
    private float chosenTargetPoint;

    public Jackson()
    {
        CharacterClassName = "Jackson";
        CharacterClassDescription = "A powerful gremlin.";
        Health = 90;
        MaxHealth = 90;
        TurnPriority = 2;
        isEnemy = false;
        Move01Damage = 80;
        Move02Damage = 75;
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
        arrowContainer = playerController.transform.Find("Main Camera/BattleMovesObjects/ArrowHolder").gameObject;
        targetBarHolder = playerController.transform.Find("OrthoCamera/Canvas/TargetBarHolder").gameObject;
        targetBarTarget = playerController.transform.Find("OrthoCamera/Canvas/TargetBarHolder/TargetBarTarget").gameObject;
        beginPoint = playerController.transform.Find("OrthoCamera/Canvas/TargetBarHolder/BeginningPoint").gameObject;
        endPoint = playerController.transform.Find("OrthoCamera/Canvas/TargetBarHolder/EndingPoint").gameObject;
        targetBarBackfill = playerController.transform.Find("OrthoCamera/Canvas/TargetBarHolder/TargetBarBackfill").gameObject;
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
        if (moveFirstPass)
        {
            startTimeTrack = Time.time;
            moveFirstPass = false;
            arrowContainer.transform.localPosition = Vector3.zero;
            randomRotationValue = UnityEngine.Random.Range(10f, 20f)/10000;
            timerBarHolder.SetActive(true);
            clonedBolts = new List<GameObject>();
            damageDict = new Dictionary<BaseCharacterClass, int>();
            arrowScale = arrowContainer.transform.localScale;
            randomTimeBetweenRotations = UnityEngine.Random.Range(25f, 150f) / 100f;
            trackRotationTime = Time.time;
            distanceFromBegToEnd = Mathf.Abs(beginPoint.transform.localPosition.x) + Mathf.Abs(endPoint.transform.localPosition.x);
            chosenTargetPoint = distanceFromBegToEnd * (UnityEngine.Random.Range(70f, 95f) / 100f);
            Debug.Log(distanceFromBegToEnd);
            Vector3 newTargetLocation = new Vector3(beginPoint.transform.localPosition.x + chosenTargetPoint, targetBarTarget.transform.localPosition.y, targetBarTarget.transform.localPosition.z);
            targetBarTarget.transform.localPosition = newTargetLocation;
            foreach (Transform child in arrowContainer.transform)
            {
                child.gameObject.SetActive(true);
            }
            foreach (Transform child in targetBarHolder.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        if (Input.GetKey("space"))
        {
            arrowScale.x += scaleGrowthValue;
            arrowContainer.transform.localScale = arrowScale;
            if (arrowContainer.transform.localScale.x >= 3.2f)
            {
                arrowScale.x = 3.2f;
                arrowContainer.transform.localScale = arrowScale;
            }
        }
        if (Input.GetKey("up"))
        {
            Quaternion arrowRotation = arrowContainer.transform.localRotation;
            arrowRotation.z += 50f / 10000;
            if (arrowRotation.z >= .4) { arrowRotation.z = .4f; }
            arrowContainer.transform.localRotation = arrowRotation;
        }
        else if (Input.GetKey("down"))
        {
            Quaternion arrowRotation = arrowContainer.transform.localRotation;
            arrowRotation.z -= 50f / 10000;
            if (arrowRotation.z <= -.4) { arrowRotation.z = .4f; }
            arrowContainer.transform.localRotation = arrowRotation;
        }
        if (Time.time - startTimeTrack >= move02Time)
        {
            foreach (GameObject damagedEnemy in arrowContainer.GetComponentInChildren<HitboxCollision>().hitboxGameObject)
            {
                float targetPositionAsPercentage = chosenTargetPoint / distanceFromBegToEnd;
                float barPositionAsPercentage = (arrowContainer.transform.localScale.x - 1) / 2.2f;
                float percentageDifference = Mathf.Abs(targetPositionAsPercentage - barPositionAsPercentage);
                int move02AdjustedDamage = 0;
                if (barPositionAsPercentage >= .99f) { move02AdjustedDamage = 0; }
                else
                {
                    move02AdjustedDamage = (int)(Move02Damage * (1 - percentageDifference));
                }
                if (damageDict.ContainsKey(damagedEnemy.GetComponent<BaseCharacterClass>()))
                {
                    damageDict[damagedEnemy.GetComponent<BaseCharacterClass>()] = damageDict[damagedEnemy.GetComponent<BaseCharacterClass>()] + move02AdjustedDamage;
                }
                else
                {
                    damageDict.Add(damagedEnemy.GetComponent<BaseCharacterClass>(), move02AdjustedDamage);
                }
            }
            arrowContainer.GetComponentInChildren<HitboxCollision>().hitboxGameObject.Clear();
            startTimeTrack = 0;
            moveFirstPass = true;
            proceedNext = false;
            timerBar.fillAmount = 1;
            timerBarHolder.SetActive(false);
            arrowContainer.transform.localScale = new Vector3 (1, 1, 1);
            foreach(Transform child in arrowContainer.transform)
            {
                child.gameObject.SetActive(false);
            }
            foreach(Transform child in targetBarHolder.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            if (rotateUp)
            {
                Quaternion arrowRotation = arrowContainer.transform.localRotation;
                arrowRotation.z += randomRotationValue;
                arrowContainer.transform.localRotation = arrowRotation;
                arrowScale.x -= scaleGrowthValue / 2;
                if (arrowScale.x <= 1) { arrowScale.x = 1; }
                arrowContainer.transform.localScale = arrowScale;
                if (arrowContainer.transform.localRotation.z >= .4 || Time.time - trackRotationTime >= randomTimeBetweenRotations)
                {
                    rotateUp = false;
                    rotateDown = true;
                    trackRotationTime = Time.time;
                    randomTimeBetweenRotations = UnityEngine.Random.Range(25f, 150f) / 100f;
                }
            }
            else if (rotateDown)
            {
                Quaternion arrowRotation = arrowContainer.transform.localRotation;
                arrowRotation.z -= randomRotationValue;
                arrowContainer.transform.localRotation = arrowRotation;
                arrowScale.x -= scaleGrowthValue / 2;
                if (arrowScale.x <= 1) { arrowScale.x = 1; }
                arrowContainer.transform.localScale = arrowScale;
                if (arrowContainer.transform.localRotation.z <= -.4 || Time.time - trackRotationTime >= randomTimeBetweenRotations)
                {
                    rotateDown = false;
                    rotateUp = true;
                    trackRotationTime = Time.time;
                    randomTimeBetweenRotations = UnityEngine.Random.Range(25f, 150f) / 100f;
                }
            }
        }
        targetBarBackfill.GetComponent<Image>().fillAmount = (arrowContainer.transform.localScale.x - 1) / 2.2f;
        timerBar.fillAmount = (move02Time - (Time.time - startTimeTrack)) / move02Time;
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
