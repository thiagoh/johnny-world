﻿using UnityEngine;
using System.Collections;

// reference to the UI namespace
using UnityEngine.UI;

// reference to manage my scenes
using UnityEngine.SceneManagement;
using System;

/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * GameController class: controls the game
 * Last Modified: 10/26/2016
 */
public class GameController : MonoBehaviour {

    // player reference
    private PlayerController playerController;
    // min visible x of any item in the game
    public static float MIN_VISIBLE_GAME_ITEM_X = -7.2f;
    // min visible x of camera in the game
    public static float MIN_VISIBLE_CAMERA_X = -0.8f;

    [Header("UI Objects")]
    public Text LivesLabel;
    public Text ScoreLabel;
    public Text RingsLabel;
    public Text FinalScoreLabel;
    public Text GameOverLabel;

    [Header("Sound clips")]
    // background sound track
    public AudioSource backgroundSound;
    // finish game sound track
    public AudioSource finishGameSound;
    // game over sound track
    public AudioSource gameOverSound;
    // boss phase sound track
    public AudioSource bossPhaseSound;
    // super player sound track
    public AudioSource superPlayerSound;

    // Use this for initialization
    void Start() {
        playerController = GameObject.FindObjectOfType<PlayerController>();

        FinalScoreLabel.gameObject.SetActive(false);
        GameOverLabel.gameObject.SetActive(false);

        LivesLabel.gameObject.SetActive(true);
        ScoreLabel.gameObject.SetActive(true);
        RingsLabel.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        updateUI();
    }

    public void updateUI() {
        LivesLabel.text = "Lives: " + playerController.lives;
        ScoreLabel.text = "Score: " + playerController.score;
        RingsLabel.text = "Rings: " + playerController.rings;
        FinalScoreLabel.text = "Final Score: " + playerController.score;
    }

    public void stopSounds() {
        if (backgroundSound.isPlaying) {
            backgroundSound.Stop();
        }
        if (bossPhaseSound.isPlaying) {
            bossPhaseSound.Stop();
        }
        if (superPlayerSound.isPlaying) {
            superPlayerSound.Stop();
        }
        if (finishGameSound.isPlaying) {
            finishGameSound.Stop();
        }
    }

    public void finishGame() {

        FinalScoreLabel.gameObject.SetActive(true);
        GameOverLabel.gameObject.SetActive(true);

        LivesLabel.gameObject.SetActive(false);
        ScoreLabel.gameObject.SetActive(false);
        RingsLabel.gameObject.SetActive(false);

        playerController.gameObject.SetActive(false);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies) {
            item.SetActive(false);
        }

        stopSounds();
        finishGameSound.Play();
    }

    public void gameOver() {

        FinalScoreLabel.gameObject.SetActive(true);
        GameOverLabel.gameObject.SetActive(true);

        LivesLabel.gameObject.SetActive(false);
        ScoreLabel.gameObject.SetActive(false);
        RingsLabel.gameObject.SetActive(false);

        playerController.gameObject.SetActive(false);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies) {
            item.SetActive(false);
        }

        stopSounds();
        gameOverSound.Play();


    }
}
