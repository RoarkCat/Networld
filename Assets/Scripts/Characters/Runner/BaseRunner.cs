using UnityEngine;
using System.Collections;
using System.Collections.Generic;


    [RequireComponent(typeof(Rigidbody))]

    public class BaseRunner : MonoBehaviour
    {
        public Transform playerObjectTransform;
        public Transform topRaycastTransform;
        public Transform bottomRaycastTransform;
        public static float distanceTraveled;
        public float acceleration;
        public int numberOfJumps = 2;
        public float teleDistance = 5;

        private bool grounded;
        private bool canJump;
        private bool triggerJump = false;
        private bool triggerTele = false;
        private bool letGoOfSpace = false;
        private bool isFalling = false;
        private bool isHoldingBack = false;
        private int trackJump = 0;
        private Rigidbody rb;
        public Animator runner;

        public float initialJumpImpulse = 5f;
        public float incrementingJumpImpulse = 0.01f;
        public float allowedTimeToHoldJump = 0.25f;
        public float currentTimeToHoldJump = 0;
        public float fallVelocityDecay = 0;
        public float fallVelocityDecayRate = 1f;
        private float dynamicJumpImpulse;

        public Animator runnerStart()
        {
            rb = GetComponent<Rigidbody>();
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
            //if (Input.GetButtonDown("Tele") && !grounded)
            //{
            //    runner.SetBool("Grounded Running", false);
            //    triggerTele = true;
            //    runner.SetTrigger("Tele");
            //}

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
            if (Input.GetKeyDown(KeyCode.RightArrow))
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

            transform.localPosition = new Vector3(this.transform.localPosition.x + 4, this.transform.localPosition.y, 0f);
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
            rb.velocity = new Vector3(acceleration / 1.5f, rb.velocity.y, 0f);
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

        void OnCollisionEnter()
        {
            grounded = true;
        }

        void OnCollisionExit()
        {
            grounded = false;
            runner.SetBool("Grounded Running", false);
        }

        void checkHitsForDashing(RaycastHit[] checkHits)
        {
            for (int i = 0; i < checkHits.Length; i++)
            {
                RaycastHit hit = checkHits[i];
                if (hit.collider.tag == "LimitCollection")
                {
                    hit.transform.GetComponent<LimitBreakItem>().CheckForRaycastHit(GetComponent<Collider>());
                    Debug.Log("Hit limit collection");
                }
                else if (hit.collider.tag == "HealthCollection")
                {
                    hit.transform.GetComponent<HealthItem>().CheckForRaycastHit(GetComponent<Collider>());
                    Debug.Log("Hit health collection");
                }
            }
        }
    }
