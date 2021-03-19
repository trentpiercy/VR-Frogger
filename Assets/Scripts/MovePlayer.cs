using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    EnvironmentSetup environmentSetup;

    public float moveForce = 600;
    public float forceCooldown;
    public bool fourD = true;

    public AudioClip jumpSound;
    public float jumpSoundPitch = 1;

    public delegate void CompletedLevelAction();
    public static event CompletedLevelAction OnLevelComplete;

    new Rigidbody rigidbody;
    Vector3 lastSnapped;

    [HideInInspector]
    public float forceCooldownTimer = 0f;

    Vector3 originalPosition;
    Quaternion originalRotation; 
    GameObject centerEyeAnchor;
    public GameObject playspace;
    Vector3 lastCenterEyePosition;

    private void Start()
    {
        if (playspace == null)
        {
            playspace = GameObject.Find("MixedRealityPlayspace");
        }

        DetectCollision.OnCollisionEvent += OnGameOver;
        OnLevelComplete += OnGameOver;

        //cameraRig = FindObjectOfType<OVRCameraRig>();
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");

        rigidbody = GetComponent<Rigidbody>();

        // copy originals
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        centerEyeAnchor.transform.position = new Vector3(centerEyeAnchor.transform.position.x, centerEyeAnchor.transform.position.y, centerEyeAnchor.transform.position.z + 0.2f);
        lastCenterEyePosition = centerEyeAnchor.transform.position;

        SetLastSnapped();
    }

    void OnGameOver()
    {
        rigidbody.velocity = new Vector3();
        rigidbody.angularVelocity = new Vector3();

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        SetLastSnapped();
    }

    private void Update()
    {
        SyncPlayerAndCamera();
    }

    private void SyncPlayerAndCamera()
    {
        // if camera moved, drag player with it if not under force
        if (centerEyeAnchor.transform.position != lastCenterEyePosition && forceCooldownTimer <= 0)
        {
            // get change in centerEye
            Vector3 change = centerEyeAnchor.transform.position - lastCenterEyePosition;

            // move player by change
            transform.position = new Vector3(transform.position.x + change.x, transform.position.y, transform.position.z + change.z);
            //transform.position = new Vector3(centerEyeAnchor.transform.position.x, transform.position.y, centerEyeAnchor.transform.position.z);
        }

        // compensate with MRTK play space for any difference btw player and centerEye due to force
        Vector3 diff = transform.position - centerEyeAnchor.transform.position;
        playspace.transform.position = new Vector3(playspace.transform.position.x + diff.x, playspace.transform.position.y, playspace.transform.position.z + diff.z);

        lastCenterEyePosition = centerEyeAnchor.transform.position;

        if (forceCooldownTimer > 0)
        {
            forceCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!PauseMenu.gameIsPaused)
        {
            SnapPlayer();
            DetectWin();
        }
    }

    private void SnapPlayer()
    {
        // Snap player to 2x2 grid
        Vector3 floorPosition = new Vector3(transform.position.x, 0, transform.position.z);
        float changeX = Mathf.Abs(floorPosition.x - lastSnapped.x);
        float changeZ = Mathf.Abs(floorPosition.z - lastSnapped.z);

        // if moved far enough: snap
        if (changeX >= 2 || changeZ >= 2)
        {
            print("Stopping cube");
            rigidbody.velocity = new Vector3();
            SetLastSnapped();
        }
    }

    private void DetectWin()
    {
        if (environmentSetup == null)
        {
            environmentSetup = FindObjectOfType<EnvironmentSetup>();
        }

        // detect win level
        if (environmentSetup != null && transform.position.z > environmentSetup.boardLength)
        {
            OnLevelComplete?.Invoke();
        }
    }

    public void MoveInput()
    {
        if (PauseMenu.gameIsPaused)
            return;

        print("Move input pressed");
        if (forceCooldownTimer <= 0)
        {
            print("Applying force");
            if (fourD)
            {
                Vector3 forward = centerEyeAnchor.transform.forward;
                forward.y = 0;
                float heading = Quaternion.LookRotation(forward).eulerAngles.y;
                print(heading);
                /*
                0: positive z
                > 315, < 45 

                90: positive x
                > 45, < 135

                180: negative z
                > 135, < 225

                270: negative x
                > 225, < 315
                */

                if (heading >= 315 || heading <= 45)
                {
                    rigidbody.AddForce(new Vector3(0, 0, moveForce)); // + z
                } else if (heading >= 45 && heading <= 135)
                {
                    rigidbody.AddForce(new Vector3(moveForce, 0, 0)); // + x
                } else if (heading >= 135 && heading <= 225)
                {
                    rigidbody.AddForce(new Vector3(0, 0, -moveForce)); // - z
                } else if (heading >= 225 && heading <= 315)
                {
                    rigidbody.AddForce(new Vector3(-moveForce, 0, 0)); // - x
                }
            } else
            {
                rigidbody.AddForce(new Vector3(0, 0, moveForce));
            }

            forceCooldownTimer = forceCooldown; // set to 2s cooldown

            // play move sound
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = jumpSound;
            audioSource.pitch = jumpSoundPitch;
            audioSource.Play();
        }
        else
        {
            print("Waiting on timer, no force");
        }
    }

    void SetLastSnapped()
    {
        float x = Mathf.Floor(transform.position.x);
        float z = Mathf.Floor(transform.position.z);

        if (x % 2 != 0)
        {
            if (x > lastSnapped.x)
            {
                x--;
            }
            else
            {
                x++;
            }
        }

        if (z % 2 != 0) {
            if (z > lastSnapped.z)
            {
                z--;
            }
            else
            {
                z++;
            }
        }

        lastSnapped = new Vector3(x, 0, z);
        print("Last snapped set to " + lastSnapped);

        // if snapped, can apply force again
        forceCooldownTimer = 0f;
    }

    public void ToggleJumpWithTrigger()
    {
        InputActionHandler jumpHandler = GetComponent<InputActionHandler>();
        if (jumpHandler.enabled)
        {
            // if active, turn off
            print("Toggling off trigger jumping");
            jumpHandler.enabled = false;
        } else
        {
            print("Toggling on trigger jumping");
            jumpHandler.enabled = true;
        }
    }

    public void Toggle4D()
    {
        fourD = !fourD;
    }
}
