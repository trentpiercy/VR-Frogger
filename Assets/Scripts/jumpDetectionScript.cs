using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpDetectionScript : MonoBehaviour
{
    public static double baseline_position;
    public static bool jumping;
    public static bool calibrated = false;
    public static double sensitivity = .1;
    public static double time_threshold = 1;
    public static double jump_time = 0;
    private Vector3 previous_position;
    private Vector3 current_velocity;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Script started");
    }

    IEnumerator setBaseline()
    {

        baseline_position = transform.position.y;
        calibrated = true;
        yield return null;
    }
    void Update()
    {

    }
    void detectJumpV()
    {
        if (calibrated == false || Input.GetKeyDown("c"))
        {
            StartCoroutine("setBaseline");
            Debug.Log("Resetting y baseline");
        }
        Vector3 current_position = transform.position;
        current_velocity = (transform.position - previous_position) / Time.deltaTime;
        //Debug.Log("Velocity:" + current_velocity);
        if (current_velocity.y >= sensitivity && !jumping)
        {
            Debug.Log("Jump detected");
            jumping = true;
        }
        else { if ((current_velocity.y == 0.0 || jump_time > time_threshold) && jumping) { jumping = false; } }
        previous_position = transform.position;
        if (jumping)
        {
            jump_time += Time.deltaTime;
        }
        else
        {
            jump_time = 0;
        }
    }
    void detectJumpP()
    {
    }
    void FixedUpdate()
    {
        detectJumpV();
    }
}
