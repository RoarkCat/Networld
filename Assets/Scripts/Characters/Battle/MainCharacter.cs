using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainCharacter : BaseCharacterClass {

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

    public MainCharacter()
    {
        CharacterClassName = "Your Name";
        CharacterClassDescription = "You do you, man.";
        Health = 100;
        MaxHealth = 100;
        TurnPriority = 1;
        isEnemy = false;
        Move01Damage = 50;
        // Move02Damage changed by move02 method
        UltimateDamage = 80;
        Move01Name = "Lightning Strike";
        Move02Name = "Hammer Time";
        UltimateName = "Ultimate";
        UltimateLimitRequirement = 50;
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
                if (movingBar.transform.localPosition.x >= rightEdgeOfScreen)
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
                if (movingBar.transform.localPosition.x <= leftEdgeOfScreen)
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
            damageBuildUp += 13;
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
            startTimeTrack = Time.time;
            moveFirstPass = false;
        }
        if (Input.GetKeyDown("space"))
        {
            keyPress++;
        }
        if (Time.time - startTimeTrack >= timeToPress)
        {
            Debug.Log(keyPress);
            keyPress = 0;
            moveFirstPass = true;
            proceedNext = false;
        }
        HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
        foreach (HitboxCollision hbox in checkHitbox)
        {
            if (hbox.isHit)
            {
                //damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        damagedEnemies.Clear();
    }
}
