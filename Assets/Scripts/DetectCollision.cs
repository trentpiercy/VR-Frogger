using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public delegate void CollisionAction();
    public static event CollisionAction OnCollisionEvent;

    IEnumerator OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle" && !PauseMenu.gameIsPaused)
        {
            Debug.Log("Collided with obstacle!");

            // seeing stuff collide is fun so wait for a second
            yield return new WaitForSeconds(0f);

            // fire collision event to obstacles
            OnCollisionEvent?.Invoke();
        }
    }

    void Update()
    {
        // detect fall off world as collision
        if (transform.position.y < -10)
        {
            OnCollisionEvent?.Invoke();
        }
    }

    public static void InvokeCollisionEvent()
    {
        OnCollisionEvent?.Invoke();
    }
}
