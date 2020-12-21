using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public AudioClip collisionSound;
    public float collisionSoundPitch = 0.75f;

    public delegate void CollisionAction();
    public static event CollisionAction OnCollisionEvent;

    bool collisionActive = false;

    IEnumerator OnCollisionEnter(Collision collision)
    {
        print("Collision detected");

        if (collisionActive || PauseMenu.gameIsPaused)
            yield break;

        if (collision.gameObject.tag == "Obstacle")
        {
            collisionActive = true;
            Debug.Log("Collided with obstacle!");

            // play collision
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = collisionSound;
            audioSource.pitch = collisionSoundPitch;
            audioSource.Play();

            // seeing stuff collide is fun so wait for a second
            yield return new WaitForSeconds(.5f);

            // fire collision event to obstacles
            OnCollisionEvent?.Invoke();
            collisionActive = false;
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
