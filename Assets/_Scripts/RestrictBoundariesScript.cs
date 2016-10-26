using UnityEngine;
using System.Collections;

/**
 * Author: Thiago de Andrade Souza
 * SudentID: 300886181
 * RestrictBoundariesScript class: controlls boundaries of objects
 * Last Modified: 10/26/2016
 */
public class RestrictBoundariesScript : MonoBehaviour {

    void LateUpdate() {

        transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, GameController.MIN_VISIBLE_GAME_ITEM_X, 999999f),
                    transform.position.y,
                    transform.position.z);
    }
}
