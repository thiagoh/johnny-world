using UnityEngine;
using System.Collections;
using System;

public class DamagePlayerScript : MonoBehaviour {

    private PlayerController controller;

    public void Start() {

        controller = GameObject.FindObjectOfType<PlayerController>();
    }

    private void damage(GameObject gameObject) {
        controller.damage();
    }

    public void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.CompareTag("Player")) {
            damage(other.gameObject);
        }
    }

    public void OnCollisionStay2D(Collision2D other) {

        if (other.gameObject.CompareTag("Player")) {
            damage(other.gameObject);
        }
    }

}
