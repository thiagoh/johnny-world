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
            print("OnCollisionEnter2D Player");
            damage(other.gameObject);
        }
    }

    public void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            print("OnCollisionStay2D Player");
            damage(other.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            print("OnTriggerEnter2D Player");
            damage(other.gameObject);
        }
    }
}
