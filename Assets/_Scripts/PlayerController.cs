using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    public float velocity = 30f;
    public float jumpForce = 100f;
    public int rings = 0;
    public Camera camera;
    public Transform spawnPoint;

    private Transform transform;
    private Rigidbody2D rigidbody;
    private float move;
    private float jump;
    private bool facingRight;
    private bool grounded;
    private int movingTo;

    private static float MAX_VELOCITY = 5f;

    [Header("Sound Clips")]
    public AudioSource jumpSound;
    public AudioSource deathSound;

    // Use this for initialization
    void Start() {
        initialize();
    }

    public void damage() {

        if (rings > 0) {
            rings = 0;
        } else if (rings <= 0) {
            rings = 0;
            die();
        }
    }

    private void initialize() {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody2D>();
        move = 0;
        jump = 0;
        facingRight = true;
        grounded = false;
        transform.position = spawnPoint.position;
    }

    public void FixedUpdate() {

        float axis = Input.GetAxisRaw("Horizontal");
        move = normalize(axis);

        //Debug.Log("Move: " + axis + " || " + move + " Grounded: " + grounded);

        if (move > 0f) {
            facingRight = true;
            flip();
        } else if (move < 0f) {
            facingRight = false;
            flip();
        } else {
            move = 0f;
        }

        if (grounded) {

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) {
                jump = 1f;
                //jumpSound.Play();
                movingTo = rigidbody.velocity.x == 0f ? 0 : (int)(rigidbody.velocity.x / Math.Abs(rigidbody.velocity.x));
            }

        } else {
            //move = 0f;
        }

        //print("velocity: " + rigidbody.velocity + " axis: " + axis);
        int currentMovingTo = rigidbody.velocity.x == 0f ? 0 : (int)(rigidbody.velocity.x / Math.Abs(rigidbody.velocity.x));
        if (grounded || currentMovingTo == movingTo) {
            if ((rigidbody.velocity.x >= 1f && axis < 0f)
                ||
                (rigidbody.velocity.x <= -1f && axis > 0f)) {
                // moving to one direction but trying to break (move to another direction)
                move = Mathf.Clamp(-rigidbody.velocity.x * 0.8f, -MAX_VELOCITY, MAX_VELOCITY);
                print("BREAK! Move to " + (move > 0f ? "right" : "left") + " " + move);
            }
        }

        if (move != 0f) {
            print("Vel: " + rigidbody.velocity.x + " move: " + move);
        }

        //print("is between: " + vel: 3.788259 move: -3.030607)
        //print("is between: " + (isBetween(3.788259f, -MAX_VELOCITY, 1f) ? "yes" : "no"));

        if ((move > 0f && rigidbody.velocity.x < MAX_VELOCITY)
            ||
            (move < 0f && rigidbody.velocity.x > -MAX_VELOCITY)) {
            print("Apply!");
            rigidbody.AddForce(new Vector2(move * velocity, 0f), ForceMode2D.Force);
        }

        rigidbody.AddForce(new Vector2(0f, jump * jumpForce), ForceMode2D.Force);

        if (jump > 0f) {
            jump = 0f;
        }

        moveCamera();
        Input.ResetInputAxes();
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

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.CompareTag("Platform")) {
            grounded = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("DeathPlane")) {
            die();
        }
    }

    public void OnCollisionExit2D(Collision2D other) {
        grounded = false;
    }

    public void die() {
        //deathSound.Play();
        transform.position = spawnPoint.position;
    }

    private void moveCamera() {

        camera.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    -10f);
    }

    float normalize(float move) {

        if (move > 0f) {
            return 1f;
        } else if (move < 0f) {
            return -1f;
        } else {
            return 0f;
        }
    }
}