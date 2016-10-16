using UnityEngine;
using System.Collections;
using System;

public class EnemyController : MonoBehaviour {

    private Transform sightStart;
    private Transform sightEnd;
    private Transform lineOfSight;

    private static float MAX_SPEED = 8;
    private static float INITIAL_SPEED = 5;
    private static int POINTS_EARNED_BY_KILLING_ME = 100;

    private float speed;
    private Transform transform;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private bool grounded;

    [Header("Sound Clips")]
    public AudioSource deathSound;

    // Use this for initialization
    void Start() {

        rigidbody = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        sightStart = transform.Find("SightStart");
        sightEnd = transform.Find("SightEnd");
        lineOfSight = transform.Find("LineOfSight");

        grounded = true;
        speed = INITIAL_SPEED;
    }

    void FixedUpdate() {

        // check if the object is grounded 
        if (grounded) {

            // move the object in the direction of his local scale
            rigidbody.velocity = new Vector2(transform.localScale.x, 0) * speed;
            animator.SetInteger("EnemyState", 1);

            bool isGroundAhead = Physics2D.Linecast(sightStart.position, sightEnd.position, 1 << LayerMask.NameToLayer("Solid"));
            bool isPlayerDetected = Physics2D.Linecast(sightStart.position, lineOfSight.position, 1 << LayerMask.NameToLayer("Player"));

            // for debugging purposes only
            Debug.DrawLine(sightStart.position, sightEnd.position);
            Debug.DrawLine(sightStart.position, lineOfSight.position);

            // check if there is ground ahead for the object to walk
            if (isGroundAhead == false) {
                // flip the direction
                flip();

            } else {

                bool isSpikeAhead = Physics2D.Linecast(sightStart.position, sightEnd.position, 1 << LayerMask.NameToLayer("Spike"));

                if (isSpikeAhead) {
                    // flip the direction
                    flip();
                }
            }

            // check if player is detected and then increase speed
            if (isPlayerDetected) {
                // increase speed to maximumSpeedkl
                speed = Mathf.Clamp(speed * 2, INITIAL_SPEED, MAX_SPEED);

            } else {
                speed = INITIAL_SPEED;
            }

        } else {
            animator.SetInteger("EnemyState", 0);
        }
    }

    // object is grounded if it stays on the platform
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.CompareTag("Platform")) {
            grounded = true;
        }
    }

    // object is not grounded if it leaves the platform
    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Platform")) {
            grounded = false;
        }
    }

    private void flip() {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            deathSound.Play();
            gameObject.transform.position = Vector2.one * 99999999f;

            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.incrementScore(POINTS_EARNED_BY_KILLING_ME);

            Destroy(gameObject, 3);
        }
    }
}
