using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    GameObject centerEyeAnchor;
    MovePlayer movePlayer;
    public float jumpCooldown = 2f;

    // velocity detection
    double velocitySensitivity = 0.5;
    Vector3 lastPosition;
    Vector3 current_velocity;

    // moving avg detection
    int numberOfFramesForAverage = 72;
    Queue<float> yQueue = new Queue<float>();
    float averageY;
    float ySensitivity = 0.2f;
    float jumpTimer = 0f;

    // height detection
    public float heightChangeTrigger = 0.2f;
    public float headHeight = 100f;
    bool jumping = false;

    void Start()
    {
        if (movePlayer == null)
        {
            movePlayer = FindObjectOfType<MovePlayer>();
        }

        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");

        lastPosition = centerEyeAnchor.transform.position;

        averageY = centerEyeAnchor.transform.position.y;
    }
      
    void FixedUpdate()
    {
        if (!PauseMenu.gameIsPaused) {
            //TranslationDetection();
            HeightDetection();
        }
    }

    void VelocityDetection()
    {
        current_velocity = (centerEyeAnchor.transform.position - lastPosition) / Time.deltaTime;

        if (current_velocity.y >= velocitySensitivity && movePlayer.forceCooldownTimer <= 0)
        {
            TriggerJump();
        }

        lastPosition = centerEyeAnchor.transform.position;
    }

    void MovingAverageDetection()
    {
        // average y over the last 72 physics frames = 1 sec
        // if it changes by over amount -> fire jump

        
        // if full, remove and adjust
        if (yQueue.Count > numberOfFramesForAverage)
        {
            float rem = yQueue.Dequeue();
            averageY -= rem / numberOfFramesForAverage;
        }

        float currentY = centerEyeAnchor.transform.position.y;
        float changeFromAvg = currentY - averageY;
        // end timer if moving down
        if (changeFromAvg < 0)
        {
            jumpTimer = 0;
        }
        // jump if positive change & no current force & no timer
        else if (changeFromAvg > ySensitivity && movePlayer.forceCooldownTimer <= 0 && jumpTimer <= 0)
        {
            TriggerJump();
        }

        // add new y and adjust average
        yQueue.Enqueue(currentY);
        averageY += currentY / numberOfFramesForAverage;
        print("Average Y: " + averageY);

        // timer
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    public void CalibrateHeight()
    {
        headHeight = centerEyeAnchor.transform.position.y;
    }

    void HeightDetection()
    {
        if (!jumping)
        {
            // passed threshhold
            if (centerEyeAnchor.transform.position.y > headHeight + heightChangeTrigger)
            {
                TriggerJump();
                jumping = true;
            }

        } else if (jumping)
        {
            // fell back down
            if (centerEyeAnchor.transform.position.y <= headHeight + heightChangeTrigger)
            {
                print("Can jump again");
                jumping = false;
            }
        }

        // backup state change with timer
        if (jumping && jumpTimer <= 0)
        {
            jumping = false;
        }

        // decrement timer
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    void TriggerJump()
    {
        Debug.Log("Jump detected! Moving.");
        movePlayer.MoveInput();
        jumpTimer = jumpCooldown;
    }
}
