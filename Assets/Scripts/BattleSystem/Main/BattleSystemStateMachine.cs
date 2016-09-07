using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BattleSystemStateMachine : MonoBehaviour {

    public enum BattleStates {
        INTRO,
        START,
        PLAYERCHOICE,
        PLAYERANIMATE,
        PLAYERACTIONSUMMARY,
        ENEMYCHOICE,
        ENEMYANIMATE,
        ENEMYACTIONSUMMARY,
        LOSE,
        WIN
    }

    public GameLoop gameLoop;
    public GameObject playerPrefab;
    public int currentHero;
    private bool useMove01;
    private bool useMove02;
    private bool useUltimate;
    private bool enemyMoveDecision;
    private bool listCreationFinished = false;
    private bool moveIsFinished;
    private float randomMove;
    private int randomHero;
    private Animator animator;
    private bool firstPassPosition;
    private float playerTravelDistance;
    private float playerStartTravelTime;
    private float playerTravelSpeed = 1.0f;

    public BattleStates currentState;
    public Dictionary<BaseCharacterClass, GameObject> participantDictionary = new Dictionary<BaseCharacterClass, GameObject>();
    public Dictionary<string, int> healthManager = new Dictionary<string, int>();
    private MainCharacter mc = new MainCharacter();
    private DetermineEnemies determineEnems = new DetermineEnemies();
    private LimitBreakCollection limitBreakCollection;
    private Image healthBar;
    public List<BaseCharacterClass> participantList = new List<BaseCharacterClass>();
    public List<BaseCharacterClass> enemyList = new List<BaseCharacterClass>();
    public List<BaseCharacterClass> heroList = new List<BaseCharacterClass>();

    public void battleStart () {
        currentState = BattleStates.INTRO;
        limitBreakCollection = this.GetComponent<LimitBreakCollection>();
	}

    public void battleUpdate () {
	    switch (currentState)
        {
            case (BattleStates.INTRO):
                // Handle taking hero from running state to battle state.
                // Intro main character animation.
                introActions();
                break;
            case (BattleStates.START):
                // Load in party members.
                // Party member intro animations.
                // Load in enemies.
                // Enemy intro animations.
                // Load in UI.
                // Order turn priorities.
                // Check who has turn priority and proceed to proper state.
                startActions();
                break;
            case (BattleStates.PLAYERCHOICE):
                // Prompt player to choose a move within time limit.
                // Move should handle 'mini game.' Call move in player class.
                // Move to PLAYERANIMATE.
                break;
            case (BattleStates.PLAYERANIMATE):
                // Call player animation for move.
                // Call actual mini game for move from player.
                // Move to action summary.
                break;
            case (BattleStates.PLAYERACTIONSUMMARY):
                // Calculate damage done.
                // Apply damage done.
                // Play damage taken animations.
                // Check if enemy has taken lethal damage.
                // Move to appropriate state (win/player choice/enemy choice).
                break;
            case (BattleStates.ENEMYCHOICE):
                // Select ability to use. Maybe some abilities can be dodged / negated?
                // Move to enemy animate.
                break;
            case (BattleStates.ENEMYANIMATE):
                // Animate enemy.
                // Player mini game.
                // Move to action summary.
                break;
            case (BattleStates.ENEMYACTIONSUMMARY):
                // Calculate damage done.
                // Apply damage done.
                // Play damage taken animations.
                // Check if any player has taken lethal damage.
                // Move to appropriate state. (lose/player choice/enemy choice)
                break;
            case (BattleStates.LOSE):
                // Tell player they've lost. Animation? YOU DIED
                // Revert back to last checkpoint.
                loseActions();
                break;
            case (BattleStates.WIN):
                // Winning animations.
                // Deload party animations. Move player back to center at same time.
                // Change from battle state controllers to running controller.
                winActions();
                break;
        }
	}

    public void battleOnGUI()
    {
        if (currentState == BattleStates.INTRO)
        {
            if (!firstPassPosition)
            {
                playerStartTravelTime = Time.time;
                playerTravelDistance = Vector3.Distance(playerPrefab.transform.parent.position, gameLoop.battleEncounterInstance.playerPosition.position);
                firstPassPosition = true;
            }
            float distCovered = (Time.time - playerStartTravelTime) * playerTravelSpeed;
            float fracTraveled = distCovered / playerTravelDistance;
            playerPrefab.transform.parent.position = Vector3.Lerp(playerPrefab.transform.parent.position, gameLoop.battleEncounterInstance.playerPosition.position, fracTraveled);
            if (playerPrefab.transform.parent.position == gameLoop.battleEncounterInstance.playerPosition.position)
            {
                currentState = BattleStates.START;
                firstPassPosition = false;
            }
        }
        else if (currentState == BattleStates.START)
        {
            if (GUILayout.Button(participantList[currentHero].Move01Name) && participantList[currentHero].proceedNext == false)
            {
                useMove01 = true;
            }
            else if (GUILayout.Button(participantList[currentHero].Move02Name) && participantList[currentHero].proceedNext == false)
            {
                useMove02 = true;
            }
            else if (GUILayout.Button(participantList[currentHero].UltimateName) && participantList[currentHero].proceedNext == false && participantList[currentHero].UltimateLimitRequirement <= limitBreakCollection.limitBreakCurrent)
            {
                limitBreakCollection.limitBreakCurrent -= participantList[currentHero].UltimateLimitRequirement;
                Transform limitBarHolder = playerPrefab.transform.parent.Find("OrthoCamera/Canvas/LimitBreakBar");
                Image limitBar = limitBarHolder.gameObject.GetComponent<Image>();
                limitBar.fillAmount = (float)playerPrefab.transform.parent.GetComponent<LimitBreakCollection>().limitBreakCurrent / (float)playerPrefab.transform.parent.GetComponent<LimitBreakCollection>().limitBreakMax;
                useUltimate = true;
            }
        }
    }

    public void introActions()
    {
        if (!listCreationFinished)
        {
            BaseCharacterClass mcScript = playerPrefab.GetComponent<BaseCharacterClass>();
            participantList.Add(mcScript);
            heroList.Add(mcScript);
            participantDictionary.Add(mcScript, playerPrefab);
            playerPrefab.GetComponent<Animator>().SetTrigger("Idle");
            foreach (KeyValuePair<string, int> heroHealth in healthManager)
            {
                if (heroHealth.Key == mcScript.CharacterClassName)
                {
                    mcScript.Health = heroHealth.Value;
                }
            }
            foreach (PartyLayout ally in gameLoop.partyManager.listOfAllies)
            {
                GameObject go = Instantiate(ally.friendlyPrefab, ally.friendlyPosition, Quaternion.identity) as GameObject;
                go.transform.parent = gameLoop.partyManager.allyTransform;
                go.transform.localPosition = ally.friendlyPosition;
                BaseCharacterClass characterScript = go.GetComponent<BaseCharacterClass>();
                participantList.Add(characterScript);
                heroList.Add(characterScript);
                participantDictionary.Add(characterScript, go);
                foreach (KeyValuePair<string, int> heroHealth in healthManager)
                {
                    Debug.Log(heroHealth + " " + characterScript);
                    if (heroHealth.Key == characterScript.CharacterClassName)
                    {
                        characterScript.Health = heroHealth.Value;
                    }
                }
                go.GetComponent<Animator>().SetTrigger("Idle");
                Transform healthBarHolder = go.transform.Find("AnimationsContainer/Canvas/HealthBar");
                healthBar = healthBarHolder.gameObject.GetComponent<Image>();
                healthBar.fillAmount = (float)characterScript.Health / (float)characterScript.MaxHealth;
            }
            checkForDeath();
            foreach (EnemyLayout enem in gameLoop.battleEncounterInstance.listOfEnemies)
            {
                GameObject go = Instantiate(enem.enemyPrefab, enem.enemyPosition, Quaternion.identity) as GameObject;
                go.transform.parent = gameLoop.battleEncounterInstance.battleEncounterTransform;
                go.transform.localPosition = enem.enemyPosition;
                Transform colliderTransform = go.transform.Find("AnimationsContainer/collider");
                colliderTransform.localPosition = enem.enemyColliderPosition;
                colliderTransform.localScale = enem.enemyColliderScale;
                BaseCharacterClass characterScript = go.GetComponent<BaseCharacterClass>();
                participantList.Add(characterScript);
                enemyList.Add(characterScript);
                participantDictionary.Add(characterScript, go);
            }

            foreach (BaseCharacterClass hero in participantList)
            {
                Debug.Log(hero.CharacterClassName + ":" + hero.Health.ToString());
            }
            listCreationFinished = true;
        }
    }

    public void startActions()
    {
        // Enemy move.
        if (participantList[currentHero].isEnemy == true)
        {
            if (participantList[currentHero].isDead)
            {
                currentHero++;
                resetTrackerCount();
            }
            else {
                if (!enemyMoveDecision)
                {
                    randomMove = UnityEngine.Random.Range(0, 3);
                    randomHero = UnityEngine.Random.Range(0, heroList.Count);
                    enemyMoveDecision = true;
                }
                if (randomMove == 0)
                {
                    executeEnemyMove(participantList[currentHero].Move01, participantList[currentHero].Move01Damage, "Move01");
                }
                else if (randomMove == 1)
                {
                    executeEnemyMove(participantList[currentHero].Move02, participantList[currentHero].Move02Damage, "Move02");
                }
                else if (randomMove == 2)
                {
                    executeEnemyMove(participantList[currentHero].Ultimate, participantList[currentHero].UltimateDamage, "Move03");
                }
            }
        }
        //Hero move.
        else {
            if (participantList[currentHero].isDead)
            {
                currentHero++;
                resetTrackerCount();
            }
            else {
                int randomEnemy = UnityEngine.Random.Range(0, enemyList.Count);
                if (useMove01 == true)
                {
                    executeHeroMove(participantList[currentHero].Move01, participantList[currentHero].Move01Damage, ref useMove01, null);
                }
                else if (useMove02 == true)
                {
                    executeHeroMove(participantList[currentHero].Move02, participantList[currentHero].Move02Damage, ref useMove02, null);
                }
                else if (useUltimate == true)
                {
                    participantList[currentHero].Ultimate();
                    if (participantList[currentHero].proceedNext == false)
                    {
                        useUltimate = false;
                        enemyList[randomEnemy].Health = enemyList[randomEnemy].Health - participantList[currentHero].UltimateDamage;
                        Debug.Log(participantList[currentHero].CharacterClassName + " deals " + participantList[currentHero].UltimateDamage.ToString() + " damage to " + enemyList[randomEnemy].CharacterClassName + ". " + enemyList[randomEnemy].Health.ToString() + " health left.");
                        currentHero++;
                        checkForDeath();
                        checkForEnd();
                        resetTrackerCount();
                    }
                }
            }
        }
    }

    public void winActions()
    {
        Debug.Log("You won!");
        gameLoop.isRunning = true;
        BaseCharacterClass mcScript = playerPrefab.GetComponent<BaseCharacterClass>();
        if (mcScript.Health <= 0)
        {
            mcScript.Health = 1;
            mcScript.isDead = false;
            playerPrefab.GetComponent<Animator>().SetTrigger("Idle");
        }
        foreach (BaseCharacterClass hero in heroList)
        {
            if (healthManager.ContainsKey(hero.CharacterClassName))
            {
                healthManager[hero.CharacterClassName] = hero.Health;
            }
            else
            {
                healthManager.Add(hero.CharacterClassName, hero.Health);
            }
        }
        foreach (KeyValuePair<BaseCharacterClass, GameObject> character in participantDictionary)
        {
            if (character.Value.gameObject == playerPrefab)
            {
                continue;
            }
            else
            {
                Destroy(character.Value.gameObject);
            }
        }
        participantDictionary.Clear();
        participantList.Clear();
        heroList.Clear();
        enemyList.Clear();
        currentHero = 0;
        listCreationFinished = false;
        currentState = BattleStates.INTRO;
    }

    public void loseActions()
    {
        listCreationFinished = false;
        currentState = BattleStates.INTRO;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void checkForDeath()
    {
        foreach (KeyValuePair<BaseCharacterClass, GameObject> character in participantDictionary)
        {
            if (character.Key.Health <= 0 && !character.Key.isDead)
            {
                character.Key.isDead = true;
                character.Key.Health = 0;
                Debug.Log(character.Key.CharacterClassName + " has died.");
                character.Value.gameObject.GetComponent<Animator>().SetTrigger("Death");
            }
        }
        //foreach (BaseCharacterClass character in participantList)
        //{
        //    if (character.Health <= 0 && !character.isDead)
        //    {
        //        character.isDead = true;
        //        character.Health = 0;
        //        if (character.CharacterClassName == "Your Name")
        //        {
        //            Animator playerAnim = playerPrefab.GetComponent<Animator>();
        //            playerAnim.SetTrigger("Dead");
        //        }
        //        Debug.Log(character.CharacterClassName + " has died.");
        //    }
        //}
    }

    public void checkForEnd()
    {
        bool allEnemyDead = enemyList.All(living => living.isDead == true);
        bool allPlayerDead = heroList.All(living => living.isDead == true);
        if (allEnemyDead)
        {
            currentState = BattleStates.WIN;
        }
        if (allPlayerDead)
        {
            currentState = BattleStates.LOSE;
        }
    }

    public void resetTrackerCount()
    {
        if (currentHero + 1 > participantList.Count)
        {
            currentHero = 0;
        }
    }

    public void executeHeroMove(Action moveMethod, int moveDamage, ref bool moveBeingUsed, string moveName)
    {
        // moveName is for the animator trigger.
        moveMethod();
        if (participantList[currentHero].proceedNext == false)
        {
            moveBeingUsed = false;
            Dictionary<BaseCharacterClass, int> damageDict = new Dictionary<BaseCharacterClass, int>();
            foreach (GameObject enemy in participantList[currentHero].damagedEnemies)
            {
                foreach (KeyValuePair<BaseCharacterClass, GameObject> pair in participantDictionary)
                {
                    if (pair.Value == enemy)
                    {
                        if (damageDict.ContainsKey(pair.Key))
                        {
                            damageDict[pair.Key] = damageDict[pair.Key] + moveDamage;
                        }
                        else
                        {
                            damageDict.Add(pair.Key, moveDamage);
                        }
                    }
                }
            }
            foreach (KeyValuePair<BaseCharacterClass, int> pair in damageDict)
            {
                pair.Key.Health = pair.Key.Health - pair.Value;
                Animator animator = participantDictionary[pair.Key].GetComponent<Animator>();
                animator.SetTrigger("TakeDamage");
                TextMesh textMesh = participantDictionary[pair.Key].GetComponentInChildren<TextMesh>(true);
                textMesh.text = pair.Value.ToString();
                Transform healthBarHolder = participantDictionary[pair.Key].transform.Find("AnimationsContainer/Canvas/HealthBar");
                healthBar = healthBarHolder.gameObject.GetComponent<Image>();
                healthBar.fillAmount = (float)pair.Key.Health / (float)pair.Key.MaxHealth;
                Debug.Log(participantList[currentHero].CharacterClassName + " deals " + pair.Value.ToString() + " damage to " + pair.Key.CharacterClassName);
            }
            currentHero++;
            checkForDeath();
            checkForEnd();
            resetTrackerCount();
        }
    }

    public void executeEnemyMove(Action moveMethod, int moveDamage, string moveName)
    {
        if (heroList[randomHero].isDead)
        {
            randomHero = UnityEngine.Random.Range(0, heroList.Count);
        }
        else {
            moveMethod();
            if (!moveIsFinished)
            {
                foreach (KeyValuePair<BaseCharacterClass, GameObject> pair in participantDictionary)
                {
                    if (participantList[currentHero] == pair.Key)
                    {
                        animator = pair.Value.GetComponent<Animator>();
                        animator.SetTrigger(moveName);
                        moveIsFinished = true;
                    }
                }
            }
            if (participantList[currentHero].proceedNext == false)
            {
                enemyMoveDecision = false;
                moveIsFinished = false;
                foreach (BaseCharacterClass participant in participantList)
                {
                    participant.moveIsFinished = false;
                }
                animator.SetTrigger("Reset");
                heroList[randomHero].Health = heroList[randomHero].Health - moveDamage;
                foreach (KeyValuePair<BaseCharacterClass, GameObject> pair in participantDictionary)
                {
                    if (heroList[randomHero] == pair.Key)
                    {
                        Animator heroAnimator = participantDictionary[pair.Key].GetComponent<Animator>();
                        heroAnimator.SetTrigger("TakeDamage");
                        TextMesh textMesh = participantDictionary[pair.Key].GetComponentInChildren<TextMesh>(true);
                        textMesh.text = moveDamage.ToString();
                    }
                }
                Transform healthBarHolder = participantDictionary[heroList[randomHero]].transform.Find("AnimationsContainer/Canvas/HealthBar");
                healthBar = healthBarHolder.gameObject.GetComponent<Image>();
                healthBar.fillAmount = (float)heroList[randomHero].Health / (float)heroList[randomHero].MaxHealth;
                Debug.Log(participantList[currentHero].CharacterClassName + " deals " + moveDamage.ToString() + " damage to " + heroList[randomHero].CharacterClassName + ". " + heroList[randomHero].Health.ToString() + " health left.");
                currentHero++;
                checkForDeath();
                checkForEnd();
                resetTrackerCount();
            }
        }
    }
}
