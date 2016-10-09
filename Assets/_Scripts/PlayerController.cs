using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    public float velocity = 10f;
    public float jumpForce = 100f;
    public Camera camera;

    private Transform transform;
    private Rigidbody2D rigidbody;
    private float move;
    private float jump;
    private bool facingRight;

    [Header("Sound Clips")]
    public AudioSource jumpSound;
    public AudioSource deathSound;

    // Use this for initialization
    void Start() {
        initialize();
    }

    private void initialize() {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody2D>();
        move = 0;
        jump = 0;
        facingRight = true;
    }

    public void FixedUpdate() {

        move = normalize(Input.GetAxis("Horizontal"));



        if (move > 0f) {
            facingRight = true;
            flip();
        } else if (move < 0f) {
            facingRight = false;
            flip();
        } else {
            move = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0f) {
            jump = 1f;
            //jumpSound.Play();
        }

        rigidbody.AddForce(new Vector2(
                   move * velocity,
                   jump * jumpForce),
                   ForceMode2D.Force);

        if (jump > 0f) {
            jump = 0f;
        }

        moveCamera();
        Input.ResetInputAxes();
    }

    private void flip() {
        if (facingRight) {
            transform.localScale = new Vector2(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        } else {
            transform.localScale = new Vector2(-Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        }
    }

    private void OnCollisionStay2D(Collision2D other) {

    }

    private void OnCollisionEnter2D(Collision2D other) {
    }

    public void OnCollisionExit2D(Collision2D other) {

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