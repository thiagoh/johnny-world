﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    private static float MIN_VISIBLE_GAME_ITEM_X = -7.131f;
    private static float MIN_VISIBLE_CAMERA_X = -1.2f;
    private static float MAX_VELOCITY = 3f;

    public float velocity;
    public float jumpForce;
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
    private Animator animator;

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
        animator = GetComponent<Animator>();
        move = 0;
        jump = 0;
        facingRight = true;
        grounded = false;
        transform.position = spawnPoint.position;
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
                movingTo = rigidbody.velocity.x == 0f ? 0 : (int)(rigidbody.velocity.x / Math.Abs(rigidbody.velocity.x));
            }

        } else {
            //move = 0f;
            jump = 0f;
        }

        if (!grounded || jump > 0f) {
            rigidbody.AddForce(new Vector2(move * velocity, jump * jumpForce), ForceMode2D.Force);

        } else {
            rigidbody.velocity = new Vector2(move * velocity, rigidbody.velocity.y);
        }
    }

    public void LateUpdate() {

        rigidbody.position = new Vector2(Mathf.Clamp(rigidbody.position.x, MIN_VISIBLE_GAME_ITEM_X, 999999f), rigidbody.position.y);
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
        animator.SetInteger("PlayerState", 2);
        grounded = false;
    }

    public void die() {
        deathSound.Play();
        transform.position = spawnPoint.position;
    }

    private void moveCamera() {

        camera.transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, MIN_VISIBLE_CAMERA_X, 999999f),
                    transform.position.y,
                    -10f);
    }

}