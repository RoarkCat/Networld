using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainCharacter : BaseCharacterClass {

    private GameObject playerController;

    private float keyPress = 0;
    private float timeToPress = 2.5f;
    private float startTimeTrack = 0;
    private bool moveFirstPass = true;
    public GameObject movingBar;
    private GameObject movingBarParent;
    public Image timerBar;
    public GameObject timerBarHolder;
    public GameObject catchAllCollider;

    // stuff for move 1
    private int boltsLanded = 0;
    private float move01Time = 2.5f;
    private bool moveRight = true;
    private bool moveLeft = false;
    private List<GameObject> clonedBolts;
    private float randomMoveSpeed = 0;
    private int boltsCalled = 0;

    // stuff for move 2
    private float damageBuildUp = 0;
    private float maxDamage = 100;
    public float move02Time = 1.5f;
    public Image damageBar;
    public GameObject damageBarHolder;
    public BattleSystemStateMachine battleSystem;

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
    private float timePerEnemy = 3.5f;
    private int numOfAliveEnemies = 0;
    private int currentEnemy = 0;
    private List<string> potentialButtons;
    private List<Vector3> positionSpawnList = new List<Vector3>();
    public GameLoop gameLoop;

    public MainCharacter()
    {
        CharacterClassName = "Your Name";
        CharacterClassDescription = "You do you, man.";
        Health = 100;
        MaxHealth = 100;
        TurnPriority = 1;
        isEnemy = false;
        Move01Damage = 60;
        // Move02Damage changed by move02 method
        UltimateDamage = 200;
        Move01Name = "Lightning Strike [space]";
        Move02Name = "Hammer Time [space]";
        UltimateName = "Ultimate (30)[type]";
        UltimateLimitRequirement = 30;
    }

    void Start()
    {
        playerController = GameObject.Find("GameManager");
        gameLoop = GameObject.Find("GameManager").GetComponent<GameLoop>();
        typableTextHolder = playerController.transform.Find("PlayerContainer/PlayerController/BattleMovesObjects/TypableTextHolder").gameObject;
    }

    public override void Move01()
    {
        proceedNext = true;
        if (moveFirstPass)
        {
            startTimeTrack = Time.time;
            moveFirstPass = false;
            movingBarParent = movingBar.transform.parent.gameObject;
            movingBar.transform.localPosition = Vector3.zero;
            randomMoveSpeed = UnityEngine.Random.Range(0.3f, 0.7f);
            timerBarHolder.SetActive(true);
            clonedBolts = new List<GameObject>();
            damageDict = new Dictionary<BaseCharacterClass, int>();
            foreach (Transform child in movingBar.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        if (Input.GetKeyDown("space"))
        {
            Vector3 currentPos = movingBar.transform.localPosition;
            GameObject newBolt = MonoBehaviour.Instantiate(movingBar, currentPos, Quaternion.identity) as GameObject;
            newBolt.transform.parent = movingBarParent.transform;
            newBolt.transform.localPosition = currentPos;
            newBolt.transform.localScale = Vector3.one;
            clonedBolts.Add(newBolt);
            boltsCalled++;
            foreach (GameObject damagedEnemy in newBolt.GetComponentInChildren<HitboxCollision>().hitboxGameObject)
            {
                if (damageDict.ContainsKey(damagedEnemy.GetComponent<BaseCharacterClass>()))
                {
                    damageDict[damagedEnemy.GetComponent<BaseCharacterClass>()] = damageDict[damagedEnemy.GetComponent<BaseCharacterClass>()] + Move01Damage;
                }
                else
                {
                    damageDict.Add(damagedEnemy.GetComponent<BaseCharacterClass>(), Move01Damage);
                }
            }
        }
        if (Time.time - startTimeTrack >= move01Time || boltsCalled >= 3)
        {
            foreach (GameObject bolts in clonedBolts)
            {
                movingBar.GetComponentInChildren<HitboxCollision>().hitboxGameObject.Clear();
                bolts.GetComponentInChildren<HitboxCollision>().hitboxGameObject.Clear();
                MonoBehaviour.Destroy(bolts);
            }
            movingBar.transform.localPosition = Vector3.zero;
            foreach (Transform child in movingBar.transform)
            {
                child.gameObject.SetActive(false);
            }
            boltsCalled = 0;
            startTimeTrack = 0;
            moveFirstPass = true;
            proceedNext = false;
            timerBar.fillAmount = 1;
            timerBarHolder.SetActive(false);
        }
        else
        {
            if (moveRight)
            {
                Vector3 barPosition = movingBar.transform.localPosition;
                barPosition = new Vector3(barPosition.x + randomMoveSpeed, barPosition.y, barPosition.z);
                movingBar.transform.localPosition = barPosition;
                if (movingBar.transform.localPosition.x >= 13.36f)
                {
                    moveRight = false;
                    moveLeft = true;
                }
            }
            else if (moveLeft)
            {
                Vector3 barPosition = movingBar.transform.localPosition;
                barPosition = new Vector3(barPosition.x - randomMoveSpeed, barPosition.y, barPosition.z);
                movingBar.transform.localPosition = barPosition;
                if (movingBar.transform.localPosition.x <= -4f)
                {
                    moveLeft = false;
                    moveRight = true;
                }
            }
        }
        timerBar.fillAmount = (move01Time - (Time.time - startTimeTrack)) / move01Time;
    }

    public override void Move02()
    {
        proceedNext = true;
        if (moveFirstPass)
        {
            startTimeTrack = Time.time;
            moveFirstPass = false;
            damageBarHolder.SetActive(true);
            timerBarHolder.SetActive(true);
            damageDict = new Dictionary<BaseCharacterClass, int>();
            // Move02Damage = 200;
        }
        if (Input.GetKeyDown("space"))
        {
            damageBuildUp += 14;
            if (damageBuildUp >= maxDamage)
            {
                damageBuildUp = maxDamage;
            }
        }
        if (Time.time - startTimeTrack >= move02Time)
        {
            Vector3 currentClosest = new Vector3(0,0,9001);
            foreach (BaseCharacterClass enemy in battleSystem.enemyList)
            {
                foreach (KeyValuePair<BaseCharacterClass,GameObject> pair in battleSystem.participantDictionary)
                {
                    if (enemy == pair.Key)
                    {
                        Debug.Log(currentClosest);
                        if (currentClosest.x <= pair.Value.transform.position.x && currentClosest.z != 9001 || enemy.isDead)
                        {
                            continue;
                        }
                        else
                        {
                            currentClosest = pair.Value.transform.position;
                            Move02Damage = (int)((damageBuildUp / maxDamage) * 200);
                            damageDict.Clear();
                            damageDict.Add(pair.Key, Move02Damage);
                        }
                    }
                }
            }
            damageBuildUp = 0;
            damageBar.fillAmount = 0;
            damageBarHolder.SetActive(false);
            timerBar.fillAmount = 1;
            timerBarHolder.SetActive(false);
            moveFirstPass = true;
            proceedNext = false;
        }
        damageBuildUp -= ((UnityEngine.Random.Range(5f, 10f)) / 10);
        if (damageBuildUp <= 0)
        {
            damageBuildUp = 0;
        }
        damageBar.fillAmount = damageBuildUp / maxDamage;
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
            firstCycle = true;
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
            firstCycle = true;
            typingPhrase = false;
            moveFirstPass = true;
            proceedNext = false;
        }
        timerBar.fillAmount = ((timePerEnemy * numOfAliveEnemies) - (Time.time - startTimeTrack)) / (timePerEnemy * numOfAliveEnemies);
    }
}
