using UnityEngine;
using System.Collections;

/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * EnemyKillTrigger class: damages the enemy on touch
 * Last Modified: 10/26/2016
 */
public class EnemyKillTrigger : MonoBehaviour {

    public void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            gameObject.GetComponentInParent<EnemyController>().damage();
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        //if (other.gameObject.CompareTag("Player")) {
        //    gameObject.GetComponentInParent<EnemyController>().damage();
        //}
    }
}
