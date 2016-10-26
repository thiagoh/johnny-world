using UnityEngine;
using System.Collections;

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
