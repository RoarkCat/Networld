using UnityEngine;
using System.Collections;


    [RequireComponent(typeof(Rigidbody))]

    public class BaseRunner : MonoBehaviour
    {

        public static float distanceTraveled;
        public float acceleration;
        public float jumpImpulse = 6;
        public int numberOfJumps = 2;
        public float teleDistance = 5;

        private bool grounded;
        private bool canJump;
        private bool triggerJump = false;
        private bool triggerTele = false;
        private int trackJump = 0;
        private Rigidbody rb;
        public Animator runner;

        public Animator runnerStart()
        {
            rb = GetComponent<Rigidbody>();
            //runner = GetComponentInChildren<Animator>();
            return runner;
        }

        public void runnerUpdate()
        {
            // Jump
            if (canJump && Input.GetButtonDown("Jump"))
            {
                triggerJump = true;
                trackJump++;
                if (trackJump == 1)
                {
                    runner.SetBool("Grounded Running", false);
                    runner.SetTrigger("Jump");
                }
                else if (trackJump == 2) runner.SetTrigger("DoubleJump");
            }
            // Double jump.
            if (trackJump == numberOfJumps)
            {
                canJump = false;
                trackJump = 0;
            }
            // Tele down.
            if (Input.GetButtonDown("Tele") && !grounded)
            {
                runner.SetBool("Grounded Running", false);
                triggerTele = true;
                runner.SetTrigger("Tele");
            }

            if (grounded)
            {
                if (runner.GetBool("Grounded Running") == false)
                {
                    runner.SetTrigger("FireEvent");
                    runner.SetBool("Grounded Running", true);
                }
            }
        }

        public void runnerFixedUpdate()
        {
            if (triggerJump)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpImpulse, 0f);
                triggerJump = false;
            }
            if (triggerTele)
            {
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - teleDistance, 0f);
                triggerTele = false;
            }
            // Constant move.
            rb.velocity = new Vector3(acceleration, rb.velocity.y, 0f);

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
    }
