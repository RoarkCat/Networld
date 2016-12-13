using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

    public class BaseRunner : MonoBehaviour
    {
        public Transform playerObjectTransform;
        public Transform topRaycastTransform;
        public Transform bottomRaycastTransform;
        public static float distanceTraveled;
        public float acceleration;
        public int numberOfJumps = 2;
        public float teleDistance = 5;
        public GameLoop gameLoop;

        public bool grounded;
        private bool canJump;
        private bool triggerJump = false;
        private bool triggerTele = false;
        private bool letGoOfSpace = false;
        private bool isFalling = false;
        private bool isHoldingBack = false;
        private int trackJump = 0;
        public Rigidbody rb;
        public Image dashBar01;
        public Image dashBar02;
        private int dashTrack = 2;
        private float dashCooldown = 1;
        private float dashTimeTrack = 0;
        private float slowdownCutAmount = 2f;
        private float downfallTimeTrack = 0;
        public Animator runner;

        public float initialJumpImpulse = 5f;
        public float incrementingJumpImpulse = 0.01f;
        public float allowedTimeToHoldJump = 0.25f;
        public float currentTimeToHoldJump = 0;
        public float fallVelocityDecay = 0;
        public float fallVelocityDecayRate = 1f;
        private float dynamicJumpImpulse;
        public GameObject playerController;

        public Animator runnerStart()
        {
            playerController = transform.Find("PlayerContainer/PlayerController").gameObject;
            if (playerController != null)
            {
                rb = playerController.GetComponent<Rigidbody>();
            }
            dashBar01 = playerController.transform.Find("Player/AnimationsContainer/Canvas/DashBar01").GetComponent<Image>();
            dashBar02 = playerController.transform.Find("Player/AnimationsContainer/Canvas/DashBar02").GetComponent<Image>();
            return runner;
        }

        public void runnerUpdate()
        {
        // Jump
        if (canJump && Input.GetButtonDown("Jump"))
            {
                dynamicJumpImpulse = initialJumpImpulse;
                currentTimeToHoldJump = Time.time;
                triggerJump = true;
                letGoOfSpace = false;
                trackJump++;
                if (trackJump == 1)
                {
                    runner.SetBool("Grounded Running", false);
                    runner.SetTrigger("Jump");
                }
                else if (trackJump == 2) runner.SetTrigger("DoubleJump");
            }
        if (Input.GetKeyUp("space"))
        {
            letGoOfSpace = true;
            fallVelocityDecay = 0;
            dynamicJumpImpulse = 0;
            triggerJump = false;
        }

        // Double jump.
        if (trackJump == numberOfJumps)
            {
                canJump = false;
                trackJump = 0;
            }
        // Tele down.
        if (Input.GetButtonDown("Tele"))
        {
            rb.transform.gameObject.layer = 11;
            downfallTimeTrack = Time.time;
        }
        if (Time.time - downfallTimeTrack >= 0.35f) { rb.transform.gameObject.layer = 8; }

        if (grounded)
            {
                if (runner.GetBool("Grounded Running") == false)
                {
                    runner.SetTrigger("FireEvent");
                    runner.SetBool("Grounded Running", true);
                }
                isFalling = false;
                fallVelocityDecay = 0;
            }

            // If falling
            if (!grounded && letGoOfSpace)
            {
                isFalling = true;
            }

            // Hold back
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                isHoldingBack = true;
            }
            else
            {
                isHoldingBack = false;
            }

            // Forward dash
            if (Input.GetKeyDown(KeyCode.RightArrow) && dashTrack > 0)
            {
                Ray checkForCollectionRay01 = new Ray(playerObjectTransform.position, Vector3.right);
                Ray checkForCollectionRay02 = new Ray(topRaycastTransform.position, Vector3.right);
                Ray checkForCollectionRay03 = new Ray(bottomRaycastTransform.position, Vector3.right);
                RaycastHit[] checkHitCollection01;
                checkHitCollection01 = Physics.RaycastAll(checkForCollectionRay01, 4);
                RaycastHit[] checkHitCollection02;
                checkHitCollection02 = Physics.RaycastAll(checkForCollectionRay02, 4);
                RaycastHit[] checkHitCollection03;
                checkHitCollection03 = Physics.RaycastAll(checkForCollectionRay03, 4);
                checkHitsForDashing(checkHitCollection01);
                checkHitsForDashing(checkHitCollection02);
                checkHitsForDashing(checkHitCollection03);

                if (dashTrack != 1)
                {
                    dashTimeTrack = Time.time;
                }
                dashTrack--;
                playerController.transform.localPosition = new Vector3(playerController.transform.localPosition.x + 4, playerController.transform.localPosition.y, 0f);
            }
            manageDashing();
        }

        void manageDashing()
        {
            float timeCountdown = (Time.time - dashTimeTrack) / dashCooldown;

            if (dashTrack == 1)
            {
                dashBar02.fillAmount = timeCountdown;
                dashBar02.color = Color.yellow;
                if (timeCountdown >= 1)
                {
                    dashTrack++;
                    dashBar02.color = Color.magenta;
                }
            }
            else if (dashTrack == 0)
            {
                dashBar02.fillAmount = 0;
                dashBar01.fillAmount = timeCountdown;
                dashBar01.color = Color.yellow;
                if (timeCountdown >= 1)
                {
                    dashTrack++;
                    dashBar01.color = Color.magenta;
                    dashTimeTrack = Time.time;
                }
            }
        }

        public void runnerFixedUpdate()
        {
        if (triggerJump)
        {
            if (Input.GetKey("space"))
            {
                dynamicJumpImpulse += incrementingJumpImpulse;
                if (currentTimeToHoldJump >= Time.time - allowedTimeToHoldJump)
                {
                    triggerJump = false;
                }
            }

            if (trackJump < numberOfJumps)
            {
                rb.velocity = new Vector3(rb.velocity.x, dynamicJumpImpulse, 0f);
            }
            else if (trackJump == numberOfJumps)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + dynamicJumpImpulse, 0f);
            }
            //triggerJump = false;
        }

        if (letGoOfSpace)
        {
            fallVelocityDecay += fallVelocityDecayRate;
            if (rb.velocity.y <= -20)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - fallVelocityDecay, 0f);
                // using an easing function here actually just feels really bad, womp. maybe for jumping up? definitely not for down
                //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - Mathf.Pow(fallVelocityDecay / 1, 3), 0f);
            }
        }

        // Constant move.
        if (isHoldingBack)
        {
            rb.velocity = new Vector3(acceleration / slowdownCutAmount, rb.velocity.y, 0f);
        }
        else
        {
            rb.velocity = new Vector3(acceleration, rb.velocity.y, 0f);
        }

        if (triggerTele)
            {
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - teleDistance, 0f);
                triggerTele = false;
            }

            distanceTraveled = transform.localPosition.x;
            if (grounded && rb.velocity.y <= 0.05f)
            {
                canJump = true;
                trackJump = 0;
            }
        }

        public void stopFixedUpdate()
        {
        rb.velocity = new Vector3(0, 0, 0);
        }

        // add checks for hitting objects while dashing here
        void checkHitsForDashing(RaycastHit[] checkHits)
        {
            for (int i = 0; i < checkHits.Length; i++)
            {
                RaycastHit hit = checkHits[i];
                if (hit.collider.tag == "LimitCollection")
                {
                    hit.transform.GetComponent<LimitBreakItem>().CheckForRaycastHit();
                    Debug.Log("Hit limit collection");
                }
                else if (hit.collider.tag == "HealthCollection")
                {
                    hit.transform.GetComponent<HealthItem>().CheckForRaycastHit();
                    Debug.Log("Hit health collection");
                }
                else if (hit.collider.tag == "BattleZone")
                {
                    gameLoop.initiateBattle(hit.collider);
                }
                else if (hit.collider.tag == "QTEZone")
                {
                    gameLoop.initiateQTE(hit.collider);
                }
                else if (hit.collider.tag == "Checkpoint")
                {
                    hit.transform.GetComponent<CheckpointCheck>().feedDataToGameManager(playerObjectTransform.transform.parent.GetComponent<Collider>());
                }
            }
        }
    }
