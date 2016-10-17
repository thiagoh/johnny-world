using UnityEngine;
using System.Collections;

// reference to the UI namespace
using UnityEngine.UI;

// reference to manage my scenes
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    private PlayerController playerController;

    public static float MIN_VISIBLE_GAME_ITEM_X = -7.131f;
    public static float MIN_VISIBLE_CAMERA_X = -1.2f;

    [Header("UI Objects")]
    public Text LivesLabel;
    public Text ScoreLabel;
    public Text RingsLabel;

    [Header("Sound clips")]
    // background song
    public AudioSource backgroundSound;
    // super player sound
    public AudioSource superPlayerSound;

    // Use this for initialization
    void Start() {
        playerController = GameObject.FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
        updateUI();
    }

    public void updateUI() {
        LivesLabel.text = "Lives: " + playerController.lives;
        ScoreLabel.text = "Score: " + playerController.score;
        RingsLabel.text = "Rings: " + playerController.rings;
    }
}
