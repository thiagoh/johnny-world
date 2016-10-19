using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    private static float MIN_VISIBLE_CAMERA_X = -1.2f;
    private static float MAX_VELOCITY = 3f;
    private static float SUPER_PLAYER_BONUS_TIME = 8f;

    public float velocity;
    public float jumpForce;

    public int rings { get; set; }
    public int lives { get; set; }
    public int score { get; set; }

    private Camera _camera;
    private Transform spawnPoint;
    private GameController gameController;

    private Transform sightStart;
    private Transform sightEnd1;
    private Transform sightEnd2;
    private Transform sightEnd3;
    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private float superPlayerTime;
    private bool superPlayer;
    private float move;
    private float jump;
    private float lastJump;
    private float lastDeath;
    private bool springBelow;
    private bool facingRight;
    private bool grounded;
    private Animator _animator;
    private bool canJumpAgain;

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

        gameController = GameObject.FindObjectOfType<GameController>();
        _camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        spawnPoint = GameObject.Find("SpawnPoint").transform;

        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        sightStart = _transform.Find("SightStart");
        sightEnd1 = _transform.Find("SightEnd1");
        sightEnd2 = _transform.Find("SightEnd2");
        sightEnd3 = _transform.Find("SightEnd3");

        springBelow = false;
        move = 0;
        jump = 0;
        rings = 0;
        lives = 5;
        score = 0;
        facingRight = true;
        grounded = false;
        superPlayerTime = 0f;
        superPlayer = false;
        canJumpAgain = true;

        lastJump = 0f;
        lastDeath = 0f;

        _transform.position = spawnPoint.position;
    }

    public void Update() {
        handleSuperItems();
    }

    public void FixedUpdate() {

        movePlayer();
        moveCamera();
    }

    private void handleSuperItems() {

        if (superPlayer) {
            superPlayerTime += Time.deltaTime;

            if (superPlayerTime >= SUPER_PLAYER_BONUS_TIME) {
                superPlayer = false;
                gameController.superPlayerSound.Stop();
                gameController.backgroundSound.Play();
            }
        }
    }

    private void movePlayer() {

        lastDeath += Time.fixedDeltaTime;

        grounded = Physics2D.Linecast(sightStart.position, sightEnd1.position, 1 << LayerMask.NameToLayer("Solid"))
           || Physics2D.Linecast(sightStart.position, sightEnd2.position, 1 << LayerMask.NameToLayer("Solid"))
           || Physics2D.Linecast(sightStart.position, sightEnd3.position, 1 << LayerMask.NameToLayer("Solid"));

        springBelow = Physics2D.Linecast(sightStart.position, sightEnd2.position, 1 << LayerMask.NameToLayer("Spring"));

        float move = 0f;

        if (lastDeath > 2f) {

            move = Input.GetAxisRaw("Horizontal");

            if (move > 0f) {
                _animator.SetInteger("PlayerState", 1);
                move = 1f;
                facingRight = true;
                flip();
            } else if (move < 0f) {
                _animator.SetInteger("PlayerState", 1);
                move = -1f;
                facingRight = false;
                flip();
            } else {
                _animator.SetInteger("PlayerState", 0);
                move = 0f;
            }
        }

        if (springBelow) {
            jump = 1f;
            lastJump = 0f;
            jumpSound.Play();

        } else {

            if (grounded) {

                if (canJumpAgain && Input.GetKey(KeyCode.W)) {
                    canJumpAgain = false;
                    jump = 1f;
                    lastJump = 0f;
                    jumpSound.Play();
                }

            } else {
                jump = 0f;
            }
        }

        if (!grounded || jump > 0f) {

            float _jumpForce = springBelow ? jumpForce * 3 : jumpForce;
            springBelow = false;
            _rigidbody.AddForce(new Vector2(move * velocity, jump * _jumpForce), ForceMode2D.Force);

        } else {
            _rigidbody.velocity = new Vector2(move * velocity, _rigidbody.velocity.y);
        }

        if (!canJumpAgain) {
            canJumpAgain = (lastJump > 0.6f && Input.GetKeyUp(KeyCode.W)) || lastJump > 0.8f;
        }

        lastJump += Time.fixedDeltaTime;
    }

    public void LateUpdate() {

        _rigidbody.position = new Vector2(
                Mathf.Clamp(_rigidbody.position.x, GameController.MIN_VISIBLE_GAME_ITEM_X, 999999f),
                _rigidbody.position.y);
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
            _transform.localScale = new Vector2(Math.Abs(_transform.localScale.x), Math.Abs(_transform.localScale.y));
        } else {
            _transform.localScale = new Vector2(-Math.Abs(_transform.localScale.x), Math.Abs(_transform.localScale.y));
        }
    }

    internal void incrementScore(int score) {
        this.score += score;
    }

    private void OnCollisionStay2D(Collision2D other) {
        //if (other.gameObject.CompareTag("Platform")) {
        //    grounded = true;
        //}

        //if (other.gameObject.CompareTag("Spring")) {
        //    springJump = true;
        //}
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("DeathPlane")) {
            doDie();

        } else if (other.gameObject.CompareTag("Enemy")) {

            if (superPlayer) {

                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null) {
                    enemyController.die();
                }
            } else {
                damage();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Coin")) {
            catchCoin(other.gameObject);

        } else if (other.gameObject.CompareTag("RedFlag")) {
            spawnPoint.position = other.gameObject.transform.position;
            FlagController flagController = other.gameObject.GetComponent<FlagController>();

            if (flagController != null) {
                flagController.check();
            }

        } else if (other.gameObject.CompareTag("RedDiamond")) {
            catchRedDiamond(other.gameObject);
        } else if (other.gameObject.CompareTag("Enemy")) {

            if (superPlayer) {

                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null) {
                    enemyController.die();
                }
            } else {
                damage();
            }
        }
    }

    private void catchRedDiamond(GameObject gameObject) {
        superPlayerTime = 0;
        superPlayer = true;
        gameObject.transform.position = Vector2.right * 99999f;
        gameController.backgroundSound.Stop();
        gameController.superPlayerSound.Play();
    }

    private void catchCoin(GameObject coin) {

        CoinController coinController = coin.GetComponent<CoinController>();

        coinSound.Play();
        rings += coinController.getCoinValue();
        Destroy(coin);
    }

    public void OnCollisionExit2D(Collision2D other) {
        _animator.SetInteger("PlayerState", 2);
        //grounded = false;
    }

    public void damage() {

        if (superPlayer) {
            return;
        }

        doDamage();
    }

    private void doDamage() {

        if (rings > 0) {
            loseCoinSound.Play();
            rings = 0;
            _rigidbody.velocity = Vector2.zero;
            _transform.position = spawnPoint.position;
            _animator.SetInteger("PlayerState", 0);
            lastDeath = 0f;

        } else if (rings <= 0) {
            rings = 0;
            die();
        }
    }

    public void die() {
        if (superPlayer) {
            return;
        }

        doDie();
    }

    public void doDie() {

        lives -= 1;
        rings = 0;
        deathSound.Play();
        _rigidbody.velocity = Vector2.zero;
        _transform.position = spawnPoint.position;
        _animator.SetInteger("PlayerState", 0);

        if (superPlayer) {
            gameController.superPlayerSound.Stop();
            gameController.backgroundSound.Play();
            superPlayer = false;
        }

        RestartPositionOnPlayerDeath[] routines = GameObject.FindObjectsOfType<RestartPositionOnPlayerDeath>();

        foreach (var item in routines) {
            item.restart();
        }

        lastDeath = 0f;
    }

    private void moveCamera() {

        _camera.transform.position = new Vector3(
                    Mathf.Clamp(_transform.position.x, MIN_VISIBLE_CAMERA_X, 999999f),
                    Mathf.Clamp(_transform.position.y, 0f, 999999f),
                    -10f);
    }

}