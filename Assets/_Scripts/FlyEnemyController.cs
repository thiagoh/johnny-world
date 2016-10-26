using UnityEngine;
using System.Collections;

/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * FlyEnemyController class: controls the fly enemy 
 * Last Modified: 10/26/2016
 */
public class FlyEnemyController : MonoBehaviour {

    private static float MAX_SPEED = 5f;
    private static float INITIAL_SPEED = 2.5f;
    private static float WALK_FASTER_WHEN_DETECT_PLAYER_DURATION = 3f;
    public static int POINTS_EARNED_BY_KILLING_ME = 100;

    private Transform _transform;
    private Rigidbody2D _rigidbody;

    private Transform sightStart;
    private Transform lineOfSight;

    // current speed
    private float speed;

    // Use this for initialization
    void Start() {

        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();

        sightStart = _transform.Find("SightStart");
        lineOfSight = _transform.Find("LineOfSight");

        speed = INITIAL_SPEED;
    }

    void FixedUpdate() {

        // move the object in the direction of his local scale
        _rigidbody.velocity = new Vector2(_transform.localScale.x * speed, _rigidbody.velocity.y);

        bool isObstacleAhead = Physics2D.Linecast(sightStart.position, lineOfSight.position, 1 << LayerMask.NameToLayer("Solid"));

        // for debugging purposes only
        Debug.DrawLine(sightStart.position, lineOfSight.position);

        // check if there is ground ahead for the object to fly
        if ( isObstacleAhead) {
            // flip the direction
            flip();
        }
    }

    private void flip() {
        _transform.localScale = new Vector2(-_transform.localScale.x, _transform.localScale.y);
    }
}
