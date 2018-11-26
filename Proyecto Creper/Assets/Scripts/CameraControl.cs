using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Target")]
    public Transform lookAt;                        // Reference to the look at position.
    public Transform player;                        // Reference to the player position.
    private Vector3 target;                         // Position for the camera to follow.
    private Vector3 inputPos;                       // Position for the input to be centered around the player.
    private Vector3 refVel;                         // Velocity for the smooth damp to manage.
    public float cameraDist = 7.5f;                 // Max distance of the camera.
    public float followTime = 0.5f;                 // Travel speed of the camera.

    // Input
    public bool center;                             // To know if camera should center in player.

    // Shake
    private bool shaking;                           // To know whether it's shaking.
    private Vector3 shakeDirection;                 // Direction to shake towards.
    private float shakeMagnitude;                   // How far in the direction.
    private float shakeTime;                        // How long to shake.
    private Vector3 shakeOffset;                    // To manage shake.

    void FixedUpdate()
    {
        // Get the input position centered around the world.
        inputPos = CaptureInputPos();

        // Account for screen shake.
        shakeOffset = UpdateShake();

        // Get the target position moved and centered around the lookAt or player.
        target = UpdateTargetPos();

        // Smoothly move the camera closer to it's target location.
        UpdateCameraPosition();
    }

    Vector3 CaptureInputPos()
    {
        Vector2 ret = Vector2.zero;

        // Controller input.
        if (InputHandler.instance.controller)
        {
            // Get the raw input vector, It is always centered and with smooth edges.
            ret.x = Input.GetAxis("XAim");
            ret.y = Input.GetAxis("YAim");
        }
        // Mouse input.
        else if (!InputHandler.instance.controller)
        {
            // Get the raw mouse position.
            ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            // Center it around the world, (0, 0).
            ret *= 2;
            ret -= Vector2.one;

            // Smooth edges
            if (ret.magnitude > 1f)
                ret = ret.normalized;
        }

        return ret;
    }

    Vector3 UpdateShake()
    {
        // Set shaking to false when the shake time is up.
        if (!shaking || Time.time > shakeTime)
        {
            shaking = false;
            // Return zero so that it won't effect the target.
            return Vector3.zero;
        }

        // Find out how far to shake, in what direction.
        Vector3 tempOffset = shakeDirection;
        tempOffset *= shakeMagnitude;

        return tempOffset;
    }

    Vector3 UpdateTargetPos()
    {
        // Multiply input vector by distance scalar.
        Vector3 inputOffset = inputPos * cameraDist;

        // Center the position around the lookAt or player
        Vector3 ret = Vector3.zero;
        if (!center)
        {
            ret = lookAt.position + inputOffset;
            // Add the screen shake vector to the target.
            ret += shakeOffset;
        }
        else if (center)
            ret = player.position;        

        // Make sure camera stays at same Z coord.
        ret.z = transform.position.z;

        return ret;
    }

    void UpdateCameraPosition()
    {
        // Smoothly move towards the target.
        Vector3 tempPos = Vector3.SmoothDamp(transform.position, target, ref refVel, followTime);
        // Update the position.
        transform.position = tempPos;
    }

    public void Shake(Vector3 direction, float magnitude, float time)
    {        
        // Set up the shake values.
        shaking = true;        
        shakeDirection = direction;        
        shakeMagnitude = magnitude;        
        shakeTime = Time.time + time;
    }
}
