using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private Transform sightStart;
    private Transform sightMiddle;
    private Transform sightEnd;
    private Transform lineOfSight;

    private static float MAX_SPEED = 6;
    private static float INITIAL_SPEED = 3;
    public static int POINTS_EARNED_BY_KILLING_ME = 100;

    private float speed;
    private Transform transform;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private bool grounded;

    public bool jumper;

    [Header("Sound Clips")]
    public AudioSource deathSound;

    // Use this for initialization
    void Start() {

        rigidbody = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        sightStart = transform.Find("SightStart");
        sightMiddle = transform.Find("SightMiddle");
        sightEnd = transform.Find("SightEnd");
        lineOfSight = transform.Find("LineOfSight");

        grounded = true;
        speed = INITIAL_SPEED;
        lastJump = 0f;
        checkLastJumpPeriod = 1f;
        chanceOfJump = 0.4f;
    }

    private float lastJump;
    private float checkLastJumpPeriod;
    private float chanceOfJump;

    void FixedUpdate() {

        // check if the object is grounded 
        if (grounded) {

            // move the object in the direction of his local scale
            rigidbody.velocity = new Vector2(transform.localScale.x * speed, rigidbody.velocity.y);
            animator.SetInteger("EnemyState", 1);

            bool isGroundAhead = Physics2D.Linecast(sightStart.position, sightEnd.position, 1 << LayerMask.NameToLayer("Solid"));
            bool isObstacleAhead = Physics2D.Linecast(sightStart.position, sightMiddle.position, 1 << LayerMask.NameToLayer("Solid"));
            bool isPlayerDetected = Physics2D.Linecast(sightStart.position, lineOfSight.position, 1 << LayerMask.NameToLayer("Player"));

            // for debugging purposes only
            Debug.DrawLine(sightStart.position, sightEnd.position);
            Debug.DrawLine(sightStart.position, lineOfSight.position);

            // check if there is ground ahead for the object to walk
            if (isGroundAhead == false || isObstacleAhead) {
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

            if (jumper && lastJump > checkLastJumpPeriod) {

                lastJump = 0;
                checkLastJumpPeriod = Random.Range(0.6f, 2f);

                if (Random.Range(0f, 1f) <= chanceOfJump) {
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.AddForce(Vector2.up * 400f, ForceMode2D.Force);
                    animator.SetInteger("EnemyState", 0);
                }
            }

        } else {
            rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);
            animator.SetInteger("EnemyState", 0);
        }

        lastJump += Time.fixedDeltaTime;
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
}
