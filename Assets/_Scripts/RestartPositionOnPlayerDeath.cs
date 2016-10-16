using UnityEngine;
using System.Collections;

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
