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

    // stuff for ultimate
    private int currentLetterName = 0;
    private int currentLetterPhrase = 0;
    private bool typingName = true;
    private bool typingPhrase = false;
    private bool firstCycle = true;
    private int randomInt = 0;
    private string chosenWord = "";
    private GameObject typableTextHolder;
    private GameObject newTypableHolder;
    private List<GameObject> lettersToType;
    private float timePerEnemy = 3f;
    private int numOfAliveEnemies = 0;

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
        UltimateDamage = 200;
        Move01Name = "Hidden Missile";
        Move02Name = "Arrow Shot";
        UltimateName = "Ultimate (30)";
        UltimateLimitRequirement = 30;
    }

    void Start()
    {
        playerController = GameObject.Find("PlayerController");
        gameLoop = playerController.GetComponent<GameLoop>();
        GameObject timerBarGO = playerController.transform.Find("Canvas/TimerBarHolder/TimerBar").gameObject;
        timerBar = timerBarGO.GetComponent<Image>();
        timerBarHolder = playerController.transform.Find("Canvas/TimerBarHolder").gameObject;
        concentricCirclesHolder = playerController.transform.Find("BattleMovesObjects/ConcentricCirclesHolder").gameObject;
        arrowContainer = playerController.transform.Find("BattleMovesObjects/ArrowHolder").gameObject;
        targetBarHolder = playerController.transform.Find("Canvas/TargetBarHolder").gameObject;
        targetBarTarget = playerController.transform.Find("Canvas/TargetBarHolder/TargetBarTarget").gameObject;
        beginPoint = playerController.transform.Find("Canvas/TargetBarHolder/BeginningPoint").gameObject;
        endPoint = playerController.transform.Find("Canvas/TargetBarHolder/EndingPoint").gameObject;
        targetBarBackfill = playerController.transform.Find("Canvas/TargetBarHolder/TargetBarBackfill").gameObject;
        typableTextHolder = playerController.transform.Find("BattleMovesObjects/TypableTextHolder").gameObject;
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
            positionSpawnList.Clear();
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
        if (moveFirstPass)
        {
            battleSystem = playerController.GetComponent<BattleSystemStateMachine>();
            startTimeTrack = Time.time;
            moveFirstPass = false;
            currentEnemy = 0;
            currentLetterName = 0;
            currentLetterPhrase = 0;
            numOfAliveEnemies = 0;
            damageDict = new Dictionary<BaseCharacterClass, int>();
            lettersToType = new List<GameObject>();
            timerBarHolder.SetActive(true);
            potentialButtons = new List<string>(new string[] { "n'ghaagl", "ph'shugg", "ph'nglui", "mglw'nafh", "wgah'nagl" });
            positionSpawnList.Clear();
            foreach (EnemyLayout enem in gameLoop.battleEncounterInstance.listOfEnemies)
            {
                positionSpawnList.Add(enem.enemyPosition + enem.enemyColliderPosition);
            }
            foreach (BaseCharacterClass enemy in battleSystem.enemyList)
            {
                if (enemy.isDead)
                {
                    continue;
                }
                else
                {
                    numOfAliveEnemies++;
                }
            }
        }
        if (battleSystem.enemyList[currentEnemy].isDead)
        {
            currentEnemy++;
        }
        else {
            // add to damage dict
            // damageDict add battlesystem.enemylist[currentenmy]
            if (firstCycle)
            {
                randomInt = UnityEngine.Random.Range(0, potentialButtons.Count);
                chosenWord = potentialButtons[randomInt];
                firstCycle = false;
                newTypableHolder = Instantiate(typableTextHolder, Vector3.zero, Quaternion.identity);
                newTypableHolder.transform.parent = gameLoop.battleEncounterInstance.transform;
                newTypableHolder.transform.localPosition = positionSpawnList[currentEnemy];
                newTypableHolder.transform.localPosition = new Vector3(newTypableHolder.transform.localPosition.x, newTypableHolder.transform.localPosition.y, -6);
                GameObject placeholderLetter = newTypableHolder.transform.Find("PlaceholderLetter").gameObject;
                float xTextPosition = -(battleSystem.enemyList[currentEnemy].CharacterClassName.Length / 4);
                foreach (char letter in battleSystem.enemyList[currentEnemy].CharacterClassName)
                {
                    GameObject newLetter = Instantiate(placeholderLetter, Vector3.zero, Quaternion.identity);
                    newLetter.transform.parent = newTypableHolder.transform;
                    newLetter.GetComponent<TextMesh>().text = letter.ToString();
                    newLetter.transform.localPosition = new Vector3(xTextPosition, 0, 0);
                    xTextPosition += 0.5f;
                    lettersToType.Add(newLetter);
                }
                foreach (Transform child in newTypableHolder.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            // account for spaces
            if (battleSystem.enemyList[currentEnemy].CharacterClassName[currentLetterName].ToString() == " " && typingName)
            {
                if (Input.GetKeyDown("space"))
                {
                    currentLetterName++;
                    if (currentLetterName >= battleSystem.enemyList[currentEnemy].CharacterClassName.Length)
                    {
                        currentLetterName = 0;
                        Destroy(newTypableHolder);
                        typingName = false;
                        typingPhrase = true;
                    }
                }
            }
            // end account for spaces
            else if (Input.GetKeyDown(battleSystem.enemyList[currentEnemy].CharacterClassName[currentLetterName].ToString().ToLower()) && typingName)
            {
                lettersToType[currentLetterName].GetComponent<TextMesh>().color = Color.red;
                currentLetterName++;
                if (currentLetterName >= battleSystem.enemyList[currentEnemy].CharacterClassName.Length)
                {
                    currentLetterName = 0;
                    Destroy(newTypableHolder);
                    lettersToType.Clear();
                    typingName = false;
                    typingPhrase = true;
                    newTypableHolder = Instantiate(typableTextHolder, Vector3.zero, Quaternion.identity);
                    newTypableHolder.transform.parent = gameLoop.battleEncounterInstance.transform;
                    newTypableHolder.transform.localPosition = positionSpawnList[currentEnemy];
                    newTypableHolder.transform.localPosition = new Vector3(newTypableHolder.transform.localPosition.x, newTypableHolder.transform.localPosition.y, -6);
                    GameObject placeholderLetter = newTypableHolder.transform.Find("PlaceholderLetter").gameObject;
                    float xTextPosition = -(chosenWord.Length / 4);
                    foreach (char letter in chosenWord)
                    {
                        GameObject newLetter = Instantiate(placeholderLetter, Vector3.zero, Quaternion.identity);
                        newLetter.transform.parent = newTypableHolder.transform;
                        newLetter.GetComponent<TextMesh>().text = letter.ToString();
                        newLetter.transform.localPosition = new Vector3(xTextPosition, 0, 0);
                        xTextPosition += 0.5f;
                        lettersToType.Add(newLetter);
                    }
                    foreach (Transform child in newTypableHolder.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
            else if (Input.GetKeyDown(chosenWord[currentLetterPhrase].ToString().ToLower()) && typingPhrase)
            {
                lettersToType[currentLetterPhrase].GetComponent<TextMesh>().color = Color.red;
                currentLetterPhrase++;
                if (currentLetterPhrase >= chosenWord.Length)
                {
                    damageDict.Add(battleSystem.enemyList[currentEnemy], UltimateDamage);
                    Destroy(newTypableHolder);
                    currentLetterPhrase = 0;
                    lettersToType.Clear();
                    typingPhrase = false;
                    typingName = true;
                    firstCycle = true;
                    currentEnemy++;
                }
            }
        }
        if (currentEnemy >= gameLoop.battleEncounterInstance.listOfEnemies.Count)
        {
            timerBarHolder.SetActive(false);
            timerBar.fillAmount = 1;
            typingName = true;
            typingPhrase = false;
            moveFirstPass = true;
            proceedNext = false;
        }
        else if (Time.time - startTimeTrack >= timePerEnemy * numOfAliveEnemies)
        {
            Debug.Log(currentLetterName + " : " + currentLetterPhrase);
            float percentageDamage = 0;
            if (typingName)
            {
                percentageDamage = (currentLetterName) / ((float)battleSystem.enemyList[currentEnemy].CharacterClassName.Length + (float)chosenWord.Length);
            }
            else if (typingPhrase)
            {
                percentageDamage = ((float)battleSystem.enemyList[currentEnemy].CharacterClassName.Length + currentLetterPhrase) / ((float)battleSystem.enemyList[currentEnemy].CharacterClassName.Length + (float)chosenWord.Length);
            }
            //float percentageDamage = (currentLetterName + currentLetterPhrase) / ((float)battleSystem.enemyList[currentEnemy].CharacterClassName.Length + (float)chosenWord.Length);
            float damageToDeal = Mathf.Round(UltimateDamage * percentageDamage);
            Debug.Log(percentageDamage + " : " + damageToDeal);
            damageDict.Add(battleSystem.enemyList[currentEnemy], (int)damageToDeal);
            Destroy(newTypableHolder);
            timerBarHolder.SetActive(false);
            timerBar.fillAmount = 1;
            typingName = true;
            typingPhrase = false;
            moveFirstPass = true;
            proceedNext = false;
        }
        timerBar.fillAmount = ((timePerEnemy * numOfAliveEnemies) - (Time.time - startTimeTrack)) / (timePerEnemy * numOfAliveEnemies);
    }
}
