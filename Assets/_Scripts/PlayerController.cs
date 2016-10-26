using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    // max velocity permitted
    private static float MAX_VELOCITY = 3f;
    // how long is the player going to be super in case of catch a red diamond
    private static float SUPER_PLAYER_BONUS_TIME = 8f;

    // current velocity
    public float velocity;
    // force of jump
    public float jumpForce;

    // rigs property
    public int rings { get; set; }
    // lives property
    public int lives { get; set; }
    // score property
    public int score { get; set; }
    // camera reference
    private Camera _camera;
    // spawn point of player
    private Transform spawnPoint;
    // boss phase spawn point of player
    private Transform bossPhaseSpawnPoint;
    private Transform gameEndPlane;
    // reference to game controller
    private GameController gameController;
    // min X is permitted to player move
    private float minXPermittedForPlayer;
    // min X is permitted to camera move
    private float minXPermittedForCamera;

    // player's tranform
    private Transform _transform;
    // player's rigidbody
    private Rigidbody2D _rigidbody;
    // how long is a super player
    private float superPlayerTime;
    // is currently a super player (special speed!)
    private bool superPlayer;
    // is moving right or left control
    private float move;
    // is jumping control
    private float jump;
    // last jump timestamp
    private float lastJump;
    // last death timestamp
    private float lastDeath;
    // is there a spring below?
    private bool springBelow;
    // facing right?
    private bool facingRight;
    // is grounded?
    private bool grounded;
    // player animator
    private Animator _animator;
    // can jump again control
    private bool canJumpAgain;
    // is in boss phase
    private bool bossPhase;
    private float lastDamage;
    // player's sight start
    private Transform sightStart;
    // player's sight end 1
    private Transform sightEnd1;
    // player's sight end 2
    private Transform sightEnd2;
    // player's sight end 3
    private Transform sightEnd3;

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
        bossPhaseSpawnPoint = GameObject.Find("BossPhaseSpawnPoint").transform;
        gameEndPlane = GameObject.Find("GameEndPlane").transform;

        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        sightStart = _transform.Find("SightStart");
        sightEnd1 = _transform.Find("SightEnd1");
        sightEnd2 = _transform.Find("SightEnd2");
        sightEnd3 = _transform.Find("SightEnd3");

        bossPhase = false;
        springBelow = false;
        move = 0;
        jump = 0;
        rings = 0;
        lives = 10;
        score = 0;
        facingRight = true;
        grounded = false;
        superPlayerTime = 0f;
        superPlayer = false;
        canJumpAgain = true;

        lastJump = 0f;
        lastDamage = 0f;
        lastDeath = 0f;
        minXPermittedForPlayer = GameController.MIN_VISIBLE_GAME_ITEM_X;
        minXPermittedForCamera = GameController.MIN_VISIBLE_CAMERA_X;
        _transform.position = spawnPoint.position;
    }

    public void Update() {
        handleSuperItems();
        lastDamage += Time.deltaTime;
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

            float _jumpForce = springBelow ? jumpForce * 2 : jumpForce;
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
                Mathf.Clamp(_rigidbody.position.x, minXPermittedForPlayer, gameEndPlane.position.x),
                _rigidbody.position.y);
    }

    private void moveCamera() {

        _camera.transform.position = new Vector3(
                    Mathf.Clamp(_transform.position.x, minXPermittedForCamera,
                    gameEndPlane.position.x - _camera.orthographicSize),
                    Mathf.Clamp(_transform.position.y, 0f, 999999f),
                    -10f);
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

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("DeathPlane")) {
            doDie();

        } else if (other.gameObject.CompareTag("Enemy")) {
            print("OnCollisionEnter2D Enemy");

            if (superPlayer) {

                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null) {
                    enemyController.damage();
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
        } else if (other.gameObject.CompareTag("BossPhase")) {
            enterBossPhase(other.gameObject);
        } else if (other.gameObject.CompareTag("Enemy")) {
            print("OnCollisionEnter2D Enemy");

            if (_rigidbody.velocity.y < 0f) {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null) {
                    enemyController.damage();
                }
            }

        }
    }

    private void enterBossPhase(GameObject bossPhasePlane) {

        if (bossPhase) {
            return;
        }

        bossPhase = true;
        minXPermittedForPlayer = bossPhasePlane.transform.position.x;
        minXPermittedForCamera = bossPhasePlane.transform.position.x;
        spawnPoint = bossPhaseSpawnPoint.transform;
        if (gameController.backgroundSound.isPlaying) {
            gameController.backgroundSound.Stop();
        }
        if (gameController.superPlayerSound.isPlaying) {
            gameController.superPlayerSound.Stop();
        }
        gameController.bossPhaseSound.Play();
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
    }

    public void damage() {

        if (superPlayer) {
            return;
        }

        if (lastDamage < 1f) {
            return;
        }

        doDamage();
    }

    private void doDamage() {

        lastDamage = 0f;

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
        lastDamage = 0f;

        if (lives <= 0) {
            gameController.gameOver();
        }
    }
}