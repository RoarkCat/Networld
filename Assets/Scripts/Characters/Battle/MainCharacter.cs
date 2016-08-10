using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MainCharacter : BaseCharacterClass {

    BattleSystemStateMachine battleSystem;
    public float keyPress = 0;
    public float timeToPress = 2.5f;
    public float startTimeTrack = 0;
    public bool moveFirstPass = true;
    public GameObject movingBar;
    public GameObject movingBarParent;
    public GameObject catchAllCollider;

    // stuff for move 1
    public int boltsLanded = 0;
    public float move01Time = 10f;
    public bool moveRight = true;
    public bool moveLeft = false;
    public List<GameObject> clonedBolts;
    public float randomMoveSpeed = 0;
    public int boltsCalled = 0;

    public MainCharacter()
    {
        CharacterClassName = "Your Name";
        CharacterClassDescription = "You do you, man.";
        Health = 100;
        MaxHealth = 100;
        TurnPriority = 1;
        isEnemy = false;
        Move01Damage = 50;
        Move02Damage = 10;
        UltimateDamage = 80;
    }

    public override void Move01()
    {
        proceedNext = true;
        if (moveFirstPass)
        {
            startTimeTrack = Time.time;
            moveFirstPass = false;
            movingBar = GameObject.Find("VerticalLineHolder");
            movingBarParent = movingBar.transform.parent.gameObject;
            movingBar.transform.localPosition = Vector3.zero;
            randomMoveSpeed = UnityEngine.Random.Range(0.3f, 0.7f);
            clonedBolts = new List<GameObject>();
            damagedEnemies = new List<GameObject>();
            foreach (Transform child in movingBar.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        if (Input.GetKeyDown("space"))
        {
            HitboxCollision[] checkHitbox = MonoBehaviour.FindObjectsOfType(typeof(HitboxCollision)) as HitboxCollision[];
            Vector3 currentPos = movingBar.transform.localPosition;
            GameObject newBolt = MonoBehaviour.Instantiate(movingBar, currentPos, Quaternion.identity) as GameObject;
            newBolt.transform.parent = movingBarParent.transform;
            newBolt.transform.localPosition = currentPos;
            newBolt.transform.localScale = Vector3.one;
            foreach (HitboxCollision hbox in checkHitbox)
            {
                if (hbox.isHit)
                {
                    damagedEnemies.Add(hbox.hitboxGameObject);
                }
            }
            clonedBolts.Add(newBolt);
            boltsCalled++;
        }
        if (Time.time - startTimeTrack >= move01Time || boltsCalled >= 3)
        {
            foreach (GameObject bolts in clonedBolts)
            {
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
                damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        Debug.Log("MC move 2!");
        proceedNext = false;
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
                damagedEnemies.Add(hbox.hitboxGameObject);
            }
        }
        damagedEnemies.Clear();
    }
}
