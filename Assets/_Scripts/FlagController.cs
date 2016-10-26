using UnityEngine;
using System.Collections;

/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * FlagController class: controls red flags
 * Last Modified: 10/26/2016
 */
public class FlagController : MonoBehaviour {

    private bool _checked;

    [Header("Sound Clips")]
    public AudioSource checkPointSound;

    // Use this for initialization
    void Start() {
        _checked = false;
    }

    public void check() {
        if (!_checked) {
            _checked = true;
            checkPointSound.Play();
        }
    }
}
