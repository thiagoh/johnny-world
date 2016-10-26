using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private static float MAX_SPEED = 3.5f;
    private static float INITIAL_SPEED = 2f;
    private static float MAX_BOSS_SPEED = MAX_SPEED * 2f;
    private static float INITIAL_BOSS_SPEED = INITIAL_SPEED * 2f;
    private static float WALK_FASTER_WHEN_DETECT_PLAYER_DURATION = 3f;
    public static int POINTS_EARNED_BY_KILLING_BOSS = 1000;
    public static int POINTS_EARNED_BY_KILLING_REGULAR_ENEMY = 100;

    private PlayerController playerController;
    private GameController gameController;
    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private Transform sightStart;
    private Transform sightMiddle;
    private Transform sightEnd;
    private Transform sightFeet;
    private Transform lineOfSight;

    // current speed
    private float speed;

    // is the enemy grounded?
    private bool grounded;

    // last time the enemy jumped
    private float lastJump;

    // randomize the jump of enemy
    private float checkLastJumpPeriod;

    // the change of enemy jump
    private float chanceOfJump;

    // walk faster when detect player
    private float walkFasterForPlayerDetected = 0;

    // enemy lives
    public int lives;
    // some enemies are jumpers and other are not
    public bool jumper;
    // is this enemy the boss
    public bool boss;

    [Header("Sound Clips")]
    public AudioSource deathSound;

    // Use this for initialization
    void Start() {

        playerController = GameObject.FindObjectOfType<PlayerController>();
        gameController = GameObject.FindObjectOfType<GameController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();

        sightStart = _transform.Find("SightStart");
        sightMiddle = _transform.Find("SightMiddle");
        sightFeet = _transform.Find("SightFeet");
        sightEnd = _transform.Find("SightEnd");
        lineOfSight = _transform.Find("LineOfSight");

        lives = 1;
        grounded = true;
        lastJump = 0f;
        checkLastJumpPeriod = 1f;
        walkFasterForPlayerDetected = 0f;

        if (boss) {
            lives = 3;
            jumper = true;
            _transform.localScale = new Vector3(_transform.localScale.x * 1.25f, _transform.localScale.y * 1.25f, _transform.localScale.z);
            GetComponent<SpriteRenderer>().color = new Color(255, 47, 0);
        }

        speed = getInitialSpeed();
        chanceOfJump = getChangeOfJump();
    }

    private float getChangeOfJump() {
        return boss ? 0.2f : 0.4f;
    }
    private float getInitialSpeed() {
        if (boss) {
            return INITIAL_BOSS_SPEED;
        } else {
            return INITIAL_SPEED;
        }
    }

    private float getMaxSpeed() {
        if (boss) {
            return MAX_BOSS_SPEED;
        } else {
            return MAX_SPEED;
        }
    }

    void FixedUpdate() {

        // check if the object is grounded 
        if (grounded) {

            // move the object in the direction of his local scale
            _rigidbody.velocity = new Vector2(_transform.localScale.x * speed, _rigidbody.velocity.y);
            _animator.SetInteger("EnemyState", 1);

            bool isGroundAhead = Physics2D.Linecast(sightStart.position, sightEnd.position, 1 << LayerMask.NameToLayer("Solid"))
                                    || Physics2D.Linecast(sightStart.position, sightFeet.position, 1 << LayerMask.NameToLayer("Solid"));
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
                walkFasterForPlayerDetected = WALK_FASTER_WHEN_DETECT_PLAYER_DURATION;
            }

            if (walkFasterForPlayerDetected > 0f) {
                // increase speed to maximumSpeedkl
                speed = Mathf.Clamp(speed * 2, getInitialSpeed(), getMaxSpeed());
            } else {
                speed = getInitialSpeed();
            }

            if (jumper && lastJump > checkLastJumpPeriod) {

                lastJump = 0;
                checkLastJumpPeriod = Random.Range(0.6f, 2f);

                if (Random.Range(0f, 1f) <= chanceOfJump) {
                    _rigidbody.velocity = Vector2.zero;
                    _rigidbody.AddForce(Vector2.up * 400f, ForceMode2D.Force);
                    _animator.SetInteger("EnemyState", 0);
                }
            }

        } else {
            _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
            _animator.SetInteger("EnemyState", 0);
        }

        lastJump += Time.fixedDeltaTime;
        if (walkFasterForPlayerDetected > 0f) {
            walkFasterForPlayerDetected -= Time.fixedDeltaTime;
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

    public void damage() {

        lives -= 1;
        deathSound.Play();

        if (lives <= 0) {
            die();
        }
    }

    public void die() {

        _transform.position = Vector2.one * 99999999f;

        if (boss) {
            playerController.incrementScore(POINTS_EARNED_BY_KILLING_BOSS);
        } else {
            playerController.incrementScore(POINTS_EARNED_BY_KILLING_REGULAR_ENEMY);
        }

        Destroy(gameObject, 3);

        if (boss) {
            gameController.finishGame();
        }
    }

    private void flip() {
        _transform.localScale = new Vector2(-_transform.localScale.x, _transform.localScale.y);
    }
}
