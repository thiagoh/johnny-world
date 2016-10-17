using UnityEngine;
using System.Collections;

public class EnemyKillTrigger : MonoBehaviour {

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {

            EnemyController enemyController = gameObject.GetComponentInParent<EnemyController>();
            GameObject parent = gameObject.transform.parent.gameObject;

            enemyController.deathSound.Play();
            parent.transform.position = Vector2.one * 99999999f;

            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.incrementScore(EnemyController.POINTS_EARNED_BY_KILLING_ME);

            Destroy(parent, 3);
        }
    }
}
