using UnityEngine;
using System.Collections;

public class EnemyKillTrigger : MonoBehaviour {

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            gameObject.GetComponentInParent<EnemyController>().die();
        }
    }
}
