using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public EnvironmentSetup environmentSetup;
    public Vector3 moveForce = new Vector3();
    public float forceCooldown;

    new Rigidbody rigidbody;

    float lastSnappedZ = 0f;

    [HideInInspector]
    public float forceCooldownTimer = 0f;

    Vector3 originalPosition;
    Quaternion originalRotation;

    public delegate void CompletedLevelAction();
    public static event CompletedLevelAction OnLevelComplete;

    GameObject centerEyeAnchor;
    public GameObject playspace;

    Vector3 lastCenterEyePosition;

    private void Start()
    {
        if (environmentSetup == null)
        {
            environmentSetup = FindObjectOfType<EnvironmentSetup>();
        }

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

        lastSnappedZ = Mathf.Floor(originalPosition.z);

        centerEyeAnchor.transform.position = new Vector3(centerEyeAnchor.transform.position.x, centerEyeAnchor.transform.position.y, centerEyeAnchor.transform.position.z + 0.2f);
        //cameraOriginalPosition = centerEyeAnchor.transform.position;
        lastCenterEyePosition = centerEyeAnchor.transform.position;
    }

    private void Update()
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
        // if moved far enough: snap
        if (transform.position.z >= lastSnappedZ + 2)
        {
            print("Stopping cube");
            rigidbody.velocity = new Vector3();
            SetLastSnappedZ();
        }

        // detect win level
        if (transform.position.z > environmentSetup.boardLength) {
            OnLevelComplete?.Invoke();
        }
    }

    void OnGameOver()
    {
        rigidbody.velocity = new Vector3();
        rigidbody.angularVelocity = new Vector3();

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        //centerEyeAnchor.transform.position = new Vector3(cameraOriginalPosition.x, centerEyeAnchor.transform.position.y, cameraOriginalPosition.z);

        lastSnappedZ = Mathf.Floor(originalPosition.z);
    }

    public void MoveInput()
    {
        if (PauseMenu.gameIsPaused)
            return;

        print("Move input pressed");
        if (forceCooldownTimer <= 0)
        {
            print("Applied force");
            rigidbody.AddForce(moveForce);
            forceCooldownTimer = forceCooldown; // set to 2s cooldown
        }
        else
        {
            print("Waiting on timer, no force");
        }
    }

    void SetLastSnappedZ()
    {
        lastSnappedZ = Mathf.Floor(transform.position.z);
        if (lastSnappedZ % 2 != 0)
        {
            lastSnappedZ -= 1;
        }

        print("Last snapped set to " + lastSnappedZ.ToString());

        // if snapped, can apply force again
        forceCooldownTimer = 0f;
    }
}
