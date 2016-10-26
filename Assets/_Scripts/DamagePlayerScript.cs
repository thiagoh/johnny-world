using UnityEngine;
using System.Collections;
using System;

public class DamagePlayerScript : MonoBehaviour {

    private PlayerController controller;
    private float lastDamage;

    public void Start() {

        controller = GameObject.FindObjectOfType<PlayerController>();
        lastDamage = 0f;
    }

    public void Update() {
        lastDamage += Time.deltaTime;
    }

    private void damage(GameObject gameObject) {
        if (lastDamage < 1f) {
            return;
        }
        controller.damage();
        lastDamage = 0f;
    }

    public void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            damage(other.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            damage(other.gameObject);
        }
    }
}
