using UnityEngine;
using System.Collections;

/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * KillScript class: kills the objects that touches the death plane
 * Last Modified: 10/26/2016
 */
public class KillScript : MonoBehaviour {

    public void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.CompareTag("DeathPlane")) {
            Destroy(this.gameObject, 0f);
        }
    }
}
