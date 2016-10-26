using UnityEngine;
using System.Collections;


/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * RestartPositionOnPlayerDeath class: restarts the position of game items
 * Last Modified: 10/26/2016
 */
public class RestartPositionOnPlayerDeath : MonoBehaviour {

	private Transform transform;
    private Rigidbody2D rigidbody;
	private Vector2 initialPosition;

	// Use this for initialization
	void Start () {
		transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
	}
	
    public void restart() {

        if (rigidbody != null) {
            rigidbody.velocity = Vector2.zero;
        }
        
        transform.position = initialPosition;
    }
}
