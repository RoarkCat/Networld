using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BattleSystemStateMachine : MonoBehaviour
{

    public enum BattleStates
    {
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

    public GameObject moveNamesHolder;
    public TextMesh Move01Text, Move02Text, UltimateText;
    public GameLoop gameLoop;
    public GameObject gameManager;
    public GameObject playerPrefab;
    public int currentHero;
    public int turnTracker = 0;
    public bool dialogueConditionIsMet = false;
    public CameraController camMovementScript;
    public GameObject playerContainerObject;
    private bool useMove01;
    private bool useMove02;
    private bool useUltimate;
    private bool enemyMoveDecision;
    private bool listCreationFinished = false;
    private bool moveIsFinished;
    private bool containerBackInOriginalPosition = false;
    private float randomMove;
    private int randomHero;
    private Animator animator;
    private bool firstPassPosition;
    private float playerTravelDistance;
    private float playerStartTravelTime;
    private float playerTravelSpeed = 1.0f;
    private bool beginningInitIsOver = false;
    private int dashPositionTracker = 0;

    public BattleStates currentState;
    public Dictionary<BaseCharacterClass, GameObject> participantDictionary = new Dictionary<BaseCharacterClass, GameObject>();
    public Dictionary<string, int> healthManager = new Dictionary<string, int>();
    private MainCharacter mc = new MainCharacter();
    private DetermineEnemies determineEnems = new DetermineEnemies();
    private LimitBreakCollection limitBreakCollection;
    private BaseRunner baseRunner;
    private Image healthBar;
    private DialogueBattleClass[] battleConditions;
    private DialogueBattleClass chosenDialogueEvent;
    private DialogueManager dialogueManager;
    public List<BaseCharacterClass> participantList = new List<BaseCharacterClass>();
    public List<BaseCharacterClass> enemyList = new List<BaseCharacterClass>();
    public List<BaseCharacterClass> heroList = new List<BaseCharacterClass>();

    public void battleStart()
    {
        currentState = BattleStates.INTRO;
        limitBreakCollection = this.GetComponent<LimitBreakCollection>();
        baseRunner = this.GetComponent<BaseRunner>();
        dialogueManager = this.GetComponent<DialogueManager>();

    }

    public void battleUpdate()
    {
        battleConditions = gameLoop.battleEncounterInstance.gameObject.GetComponents<DialogueBattleClass>();

        foreach (DialogueBattleClass eventParams in battleConditions)
        {
            switch (eventParams.condition)
            {
                case (DialogueBattleConditions.BattleBeginning):
                    if (beginningInitIsOver && !eventParams.thisEventIsDone)
                    {
                        dialogueConditionIsMet = true;
                        chosenDialogueEvent = eventParams;
                    }
                    break;

                case (DialogueBattleConditions.OnTurnX):
                    if (eventParams.turnForDialogue == turnTracker && !eventParams.thisEventIsDone)
                    {
                        dialogueConditionIsMet = true;
                        chosenDialogueEvent = eventParams;
                    }
                    break;

                case (DialogueBattleConditions.EnemyHealthIs):
                    break;

                case (DialogueBattleConditions.PlayerHealthIs):
                    break;

                case (DialogueBattleConditions.EndBattle):
                    break;
            }
        }


        if (!dialogueConditionIsMet)
        {
            if (currentState == BattleStates.INTRO)
            {
                if (!firstPassPosition)
                {
                    playerStartTravelTime = Time.time;
                    playerTravelDistance = Vector3.Distance(playerPrefab.transform.parent.position, gameLoop.battleEncounterInstance.playerPosition.position);
                    firstPassPosition = true;
                    containerBackInOriginalPosition = false;
                }
                float distCovered = (Time.time - playerStartTravelTime) * playerTravelSpeed;
                float fracTraveled = distCovered / playerTravelDistance;
                playerPrefab.transform.parent.position = Vector3.Lerp(playerPrefab.transform.parent.position, gameLoop.battleEncounterInstance.playerPosition.position, fracTraveled);
                if (playerPrefab.transform.parent.position == gameLoop.battleEncounterInstance.playerPosition.position)
                {
                    currentState = BattleStates.START;
                    beginningInitIsOver = true;
                    camMovementScript.enabled = !camMovementScript.enabled;
                    firstPassPosition = false;
                }
            }
            else if (currentState == BattleStates.START)
            {
                moveNamesHolder.SetActive(true);
                Move01Text.text = participantList[currentHero].Move01Name;
                Move02Text.text = participantList[currentHero].Move02Name;
                UltimateText.text = participantList[currentHero].UltimateName;
                if (Input.GetButtonDown("a") && participantList[currentHero].proceedNext == false)
                {
                    useMove01 = true;
                }
                else if (Input.GetButtonDown("s") && participantList[currentHero].proceedNext == false)
                {
                    useMove02 = true;
                }
                else if (Input.GetButtonDown("d") && participantList[currentHero].proceedNext == false)
                {
                    if (participantList[currentHero].UltimateLimitRequirement <= limitBreakCollection.limitBreakCurrent)
                    {
                        limitBreakCollection.limitBreakCurrent -= participantList[currentHero].UltimateLimitRequirement;
                        Transform limitBarHolder = playerPrefab.transform.parent.Find("Canvas/LimitBreakBar");
                        Image limitBar = limitBarHolder.gameObject.GetComponent<Image>();
                        limitBar.fillAmount = (float)gameManager.GetComponent<LimitBreakCollection>().limitBreakCurrent / (float)gameManager.GetComponent<LimitBreakCollection>().limitBreakMax;
                        useUltimate = true;
                    }
                    else
                    {
                        Debug.Log(participantList[currentHero].UltimateLimitRequirement + " ::: " + limitBreakCollection.limitBreakCurrent);
                        moveNamesHolder.GetComponent<Animator>().SetTrigger("fire");
                    }
                }
            }

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

            // Actions allowable to dodge abilities. IE: Jumping and position shifting
            //if (participantList[currentHero].isEnemy)
            // Jump
            if (baseRunner.canJump && Input.GetButtonDown("Jump") && participantList[currentHero].isEnemy && currentState != BattleStates.WIN)
            {
                baseRunner.dynamicJumpImpulse = baseRunner.initialJumpImpulse;
                baseRunner.currentTimeToHoldJump = Time.time;
                baseRunner.triggerJump = true;
                baseRunner.letGoOfSpace = false;
                baseRunner.trackJump++;
                if (baseRunner.trackJump == 1)
                {
                    baseRunner.runner.SetBool("Grounded Running", false);
                    baseRunner.runner.SetTrigger("Jump");
                }
                else if (baseRunner.trackJump == 2) baseRunner.runner.SetTrigger("DoubleJump");
            }
            if (Input.GetKeyUp("space") && participantList[currentHero].isEnemy)
            {
                baseRunner.letGoOfSpace = true;
                baseRunner.fallVelocityDecay = 0;
                baseRunner.dynamicJumpImpulse = 0;
                baseRunner.triggerJump = false;
            }
            // Double jump.
            if (baseRunner.trackJump == baseRunner.numberOfJumps)
            {
                baseRunner.canJump = false;
                baseRunner.trackJump = 0;
            }
            // Stuff when returning to ground.
            if (baseRunner.grounded)
            {
                if (baseRunner.runner.GetBool("Grounded Running") == false)
                {
                    baseRunner.runner.SetTrigger("FireEvent");
                    baseRunner.runner.SetBool("Grounded Running", true);
                }
                baseRunner.isFalling = false;
                baseRunner.fallVelocityDecay = 0;
            }
            // Right dash
            if (participantList[currentHero].isEnemy && currentState != BattleStates.WIN && currentState != BattleStates.LOSE)
            {
                if (Input.GetKeyDown("right") && dashPositionTracker < 1)
                {
                    dashPositionTracker++;
                    playerContainerObject.transform.position = new Vector3(playerContainerObject.transform.position.x + 2.0f,
                                                                           playerContainerObject.transform.position.y,
                                                                           playerContainerObject.transform.position.z);
                }
                if (Input.GetKeyDown("left") && dashPositionTracker > -1)
                {
                    dashPositionTracker--;
                    playerContainerObject.transform.position = new Vector3(playerContainerObject.transform.position.x - 2.0f,
                                                                           playerContainerObject.transform.position.y,
                                                                           playerContainerObject.transform.position.z);
                }
            }
        }
        // dialogue condition is met here vvv
        else
        {
            dialogueManager.DialogueEventInsideBattle(chosenDialogueEvent);
        }
    }

    public void battleFixedUpdate()
    {
        if (baseRunner.triggerJump)
        {
            if (Input.GetKey("space"))
            {
                baseRunner.dynamicJumpImpulse += baseRunner.incrementingJumpImpulse;
                if (baseRunner.currentTimeToHoldJump >= Time.time - baseRunner.allowedTimeToHoldJump)
                {
                    baseRunner.triggerJump = false;
                }
            }

            if (baseRunner.trackJump < baseRunner.numberOfJumps)
            {
                baseRunner.rb.velocity = new Vector3(0, baseRunner.dynamicJumpImpulse, 0f);
            }
            else if (baseRunner.trackJump == baseRunner.numberOfJumps)
            {
                baseRunner.rb.velocity = new Vector3(0, baseRunner.rb.velocity.y + baseRunner.dynamicJumpImpulse, 0f);
            }
            //triggerJump = false;
        }

        if (baseRunner.letGoOfSpace)
        {
            baseRunner.fallVelocityDecay += baseRunner.fallVelocityDecayRate;
            if (baseRunner.rb.velocity.y <= -20)
            {
                baseRunner.rb.velocity = new Vector3(0, baseRunner.rb.velocity.y, 0f);
            }
            else
            {
                baseRunner.rb.velocity = new Vector3(0, baseRunner.rb.velocity.y - baseRunner.fallVelocityDecay, 0f);
                // using an easing function here actually just feels really bad, womp. maybe for jumping up? definitely not for down
                //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - Mathf.Pow(fallVelocityDecay / 1, 3), 0f);
            }
        }
        // Grounded stuff. Reset jump to 0.
        if (baseRunner.grounded && baseRunner.rb.velocity.y <= 0.05f)
        {
            baseRunner.canJump = true;
            baseRunner.trackJump = 0;
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
            participantList = participantList.OrderBy(o => o.TurnPriority).ToList();
            foreach (BaseCharacterClass hero in participantList)
            {
                Debug.Log(hero.CharacterClassName + ":" + hero.TurnPriority);
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
                    executeHeroMove(participantList[currentHero].Move01, ref useMove01, "Move01");
                }
                else if (useMove02 == true)
                {
                    executeHeroMove(participantList[currentHero].Move02, ref useMove02, "Move02");
                }
                else if (useUltimate == true)
                {
                    executeHeroMove(participantList[currentHero].Ultimate, ref useUltimate, "Ultimate");
                }
            }
        }
    }

    public void winActions()
    {
        //Debug.Log("You won!");
        moveNamesHolder.SetActive(false);
        gameLoop.battleEncounterInstance.gameObject.SetActive(false);
        returnPlayerContainerToZero();
        if (containerBackInOriginalPosition)
        {
            Debug.Log("container back to 0");
            gameLoop.isRunning = true;
            gameLoop.isBattle = false;
            gameLoop.cameraAnimator.SetBool("BattleState", false);
            gameLoop.cameraAnimator.SetBool("RunnerState", true);
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
            turnTracker = 0;
            beginningInitIsOver = false;
            camMovementScript.enabled = !camMovementScript.enabled;
            participantDictionary.Clear();
            participantList.Clear();
            heroList.Clear();
            enemyList.Clear();
            currentHero = 0;
            listCreationFinished = false;
            currentState = BattleStates.INTRO;
        }
    }

    public void loseActions()
    {
        moveNamesHolder.SetActive(false);
        gameLoop.isRunning = true;
        gameLoop.isBattle = false;
        gameLoop.cameraAnimator.SetBool("BattleState", false);
        gameLoop.cameraAnimator.SetBool("RunnerState", true);
        foreach (KeyValuePair<BaseCharacterClass, GameObject> character in participantDictionary)
        {
            if (character.Value.gameObject == playerPrefab)
            {
                continue;
            }
            else
            {
                Debug.Log("Destroying " + character.Value.name);
                Destroy(character.Value.gameObject);
            }
        }
        turnTracker = 0;
        beginningInitIsOver = false;
        camMovementScript.enabled = !camMovementScript.enabled;
        participantDictionary.Clear();
        participantList.Clear();
        heroList.Clear();
        enemyList.Clear();
        currentHero = 0;
        listCreationFinished = false;
        currentState = BattleStates.INTRO;
        gameLoop.gameManager.ReloadCheckpoint();
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

    public void executeHeroMove(Action moveMethod, ref bool moveBeingUsed, string moveName)
    {
        // moveName is for the animator trigger.
        moveMethod();
        if (participantList[currentHero].proceedNext == false)
        {
            moveBeingUsed = false;
            foreach (KeyValuePair<BaseCharacterClass, int> pair in participantList[currentHero].damageDict)
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
            turnTracker++;
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
                turnTracker++;
                checkForDeath();
                checkForEnd();
                resetTrackerCount();
            }
        }
    }

    public void returnPlayerContainerToZero()
    {
        Debug.Log(gameLoop.isRunning);
        if (!firstPassPosition)
        {
            playerStartTravelTime = Time.time;
            playerTravelDistance = Vector3.Distance(playerContainerObject.transform.localPosition, Vector3.zero);
            firstPassPosition = true;
        }
        float distCovered = (Time.time - playerStartTravelTime) * playerTravelSpeed;
        float fracTraveled = distCovered / playerTravelDistance;
        playerContainerObject.transform.localPosition = Vector3.Lerp(playerContainerObject.transform.localPosition, Vector3.zero, fracTraveled);
        if (playerContainerObject.transform.localPosition == Vector3.zero)
        {
            firstPassPosition = false;
            containerBackInOriginalPosition = true;
        }
    }
}
