using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    private static float MIN_VISIBLE_CAMERA_X = -1.2f;
    private static float MAX_VELOCITY = 3f;

    public float velocity;
    public float jumpForce;

    public int rings { get; set; }
    public int lives { get; set; }
    public int score { get; set; }

    public Camera camera;
    public Transform spawnPoint;

    private Transform sightStart;
    private Transform sightEnd1;
    private Transform sightEnd2;
    private Transform sightEnd3;
    private Transform transform;
    private Rigidbody2D rigidbody;
    private float move;
    private float jump;
    private bool springJump;
    private bool facingRight;
    private bool grounded;
    private Animator animator;

    [Header("Sound Clips")]
    public AudioSource jumpSound;
    public AudioSource deathSound;
    public AudioSource coinSound;
    public AudioSource loseCoinSound;

    // Use this for initialization
    void Start() {
        initialize();
    }

    private void initialize() {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        sightStart = transform.Find("SightStart");
        sightEnd1 = transform.Find("SightEnd1");
        sightEnd2 = transform.Find("SightEnd2");
        sightEnd3 = transform.Find("SightEnd3");

        springJump = false;
        move = 0;
        jump = 0;
        rings = 0;
        lives = 5;
        score = 0;
        facingRight = true;
        grounded = false;
        transform.position = spawnPoint.position;
    }

    public void Update() {
        grounded = Physics2D.Linecast(sightStart.position, sightEnd1.position, 1 << LayerMask.NameToLayer("Solid"))
            || Physics2D.Linecast(sightStart.position, sightEnd2.position, 1 << LayerMask.NameToLayer("Solid"))
            || Physics2D.Linecast(sightStart.position, sightEnd3.position, 1 << LayerMask.NameToLayer("Solid"));
    }

    public void FixedUpdate() {

        movePlayer();
        moveCamera();
    }

    private void movePlayer() {
        float move = Input.GetAxisRaw("Horizontal");

        if (move > 0f) {
            animator.SetInteger("PlayerState", 1);
            move = 1f;
            facingRight = true;
            flip();
        } else if (move < 0f) {
            animator.SetInteger("PlayerState", 1);
            move = -1f;
            facingRight = false;
            flip();
        } else {
            animator.SetInteger("PlayerState", 0);
            move = 0f;
        }

        if (grounded) {

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
                jump = 1f;
                jumpSound.Play();
            }

            if (springJump) {
                jump = 1f;
                jumpSound.Play();
            }

        } else {
            //move = 0f;
            jump = 0f;
        }

        if (!grounded || jump > 0f) {

            float _jumpForce = springJump ? jumpForce * 2 : jumpForce;
            springJump = false;

            rigidbody.AddForce(new Vector2(move * velocity, jump * _jumpForce), ForceMode2D.Force);

        } else {
            rigidbody.velocity = new Vector2(move * velocity, rigidbody.velocity.y);
        }
    }

    public void LateUpdate() {

        rigidbody.position = new Vector2(
                Mathf.Clamp(rigidbody.position.x, GameController.MIN_VISIBLE_GAME_ITEM_X, 999999f),
                rigidbody.position.y);
    }

    public static bool isBetween(float v, float from, float to) {

        if (to < from) {
            float lower = to;
            to = from;
            from = lower;
        }

        return v >= from && v <= to;
    }

    private void flip() {
        if (facingRight) {
            transform.localScale = new Vector2(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        } else {
            transform.localScale = new Vector2(-Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        }
    }

    internal void incrementScore(int score) {
        this.score += score;
    }

    private void OnCollisionStay2D(Collision2D other) {
        //if (other.gameObject.CompareTag("Platform")) {
        //    grounded = true;
        //}

        if (other.gameObject.CompareTag("Spring")) {
            springJump = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("DeathPlane")) {
            die();
        }
        if (other.gameObject.CompareTag("Enemy")) {
            damage();
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Coin")) {
            catchCoin(other.gameObject);
        }
    }

    private void catchCoin(GameObject coin) {

        CoinController coinController = coin.GetComponent<CoinController>();

        coinSound.Play();
        rings += coinController.getCoinValue();
        Destroy(coin);
    }

    public void OnCollisionExit2D(Collision2D other) {
        animator.SetInteger("PlayerState", 2);
        //grounded = false;
    }

    public void damage() {

        if (rings > 0) {
            loseCoinSound.Play();
            rings = 0;
            transform.position = spawnPoint.position;
        } else if (rings <= 0) {
            rings = 0;
            die();
        }
    }

    public void die() {
        lives -= 1;
        rings = 0;
        deathSound.Play();
        transform.position = spawnPoint.position;

        RestartPositionOnPlayerDeath[] routines = GameObject.FindObjectsOfType<RestartPositionOnPlayerDeath>();

        foreach (var item in routines) {
            item.restart();
        }
    }

    private void moveCamera() {

        camera.transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, MIN_VISIBLE_CAMERA_X, 999999f),
                    Mathf.Clamp(transform.position.y, 0f, 999999f),
                    -10f);
    }

}