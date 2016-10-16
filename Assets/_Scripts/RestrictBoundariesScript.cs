using UnityEngine;
using System.Collections;

public class RestrictBoundariesScript : MonoBehaviour {

    void LateUpdate() {

        transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, GameController.MIN_VISIBLE_GAME_ITEM_X, 999999f),
                    transform.position.y,
                    transform.position.z);
    }
}
