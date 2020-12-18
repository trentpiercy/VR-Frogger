using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    public Vector3 velocity = new Vector3();
    public EnvironmentSetup environmentSetup;

    int spawnOffset;
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        if (environmentSetup == null)
        {
            environmentSetup = FindObjectOfType<EnvironmentSetup>();
        }

        spawnOffset = environmentSetup.boardWidth / 2 - 1;

        originalPosition = gameObject.transform.position;
        originalRotation = gameObject.transform.rotation;

        // subscribe to collision detection
        DetectCollision.OnCollisionEvent += OnGameOver;

        // subscribe to game end
        MovePlayer.OnLevelComplete += OnGameOver;

        // subscribe to pause/resume
        PauseMenu.OnPauseEvent += OnPause;
        PauseMenu.OnResumeEvent += OnResume;
    }

    void OnGameOver()
    {
        gameObject.transform.position = originalPosition;
        gameObject.transform.rotation = originalRotation;
        GetComponent<Rigidbody>().velocity = velocity;
        GetComponent<Rigidbody>().angularVelocity = new Vector3();
    }

    void OnPause()
    {
        GetComponent<Rigidbody>().velocity = new Vector3();
    }

    void OnResume()
    {
        GetComponent<Rigidbody>().velocity = velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float y = gameObject.transform.position.y;
        float z = gameObject.transform.position.z;

        if (gameObject.transform.position.x > spawnOffset)
        {
            gameObject.transform.position = new Vector3(-spawnOffset, y, z);

        } else if (gameObject.transform.position.x < -spawnOffset)
        {
            gameObject.transform.position = new Vector3(spawnOffset, y, z);
        }
    }
}
