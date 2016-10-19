using UnityEngine;
using System.Collections;

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
