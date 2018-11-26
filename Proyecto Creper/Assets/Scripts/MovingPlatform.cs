using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class MovingPlatform : MonoBehaviour
{
    [Header("Positions")]
    public Transform startPosition;                             // References to the start position of the movement.
    public Transform endPosition;                               // References to the end position of the movement.

    [Header("Settings")]
    public Types type;                                          // The actual type of the platform.
    public enum Types { Timed, Hold, Triggered }                // The possible platform types.    
    private bool backwards;                                     // Whether or not the platform is moving backwards.
    private Vector3 velocity;                                   // Velocity reference for the smooth movement.
    public float smoothSpeed = 1f;                              // Speed of the smooth movement.
    public float stopSeconds = 1f;                              // Seconds to wait between movements.
    private float waitTimer;                                    // Timer to manage the start movement.
    public LightSprite lightSource;                             // The light to be used on the frame.
    public float lightSeconds = 1f;                             // The seconds to fill up or down the light alpha.
    private Color color;                                        // Color used to make the animations.

    [Header("Timed")]
    public float startSeconds = 1f;                             // Second to start moving. 

    // Hold
    private bool holding;                                       // To know if the player is touching the platform.


    private void Start()
    {
        // Initialize variables here.
        transform.position = startPosition.position;
        if(type.Equals(Types.Timed))
            waitTimer = Time.time + startSeconds;

        color = lightSource.Color;
        color.a = 0;
        lightSource.Color = color;
    }

    private void Update()
    {
        switch(type)
        {
            case Types.Timed:
                if(waitTimer < Time.time)
                {
                    if (lightSource.Color.a < 0.5f)
                    {
                        color = lightSource.Color;
                        color.a += Time.deltaTime * lightSeconds * 0.5f;
                        lightSource.Color = color;
                    }                    
                }
                else if(waitTimer > Time.time)
                {
                    if (lightSource.Color.a > 0)
                    {
                        color = lightSource.Color;
                        color.a -= Time.deltaTime * lightSeconds * 0.5f;
                        lightSource.Color = color;
                    }
                }
                break;
            case Types.Hold:
                if(holding && lightSource.Color.a < 0.5f)
                {
                    color = lightSource.Color;
                    color.a += Time.deltaTime * lightSeconds * 0.5f;
                    lightSource.Color = color;
                }
                else if(!holding && lightSource.Color.a > 0)
                {
                    color = lightSource.Color;
                    color.a -= Time.deltaTime * lightSeconds * 0.5f;
                    lightSource.Color = color;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        // Move the platform.
        switch (type)
        {
            case Types.Timed:
                // If its wait time is over.
                if (waitTimer < Time.time)
                {
                    if (!backwards)
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, endPosition.position, ref velocity, smoothSpeed);
                        // Determine if the platform should move backwards.
                        if (Mathf.Abs((endPosition.position - transform.position).magnitude) <= 0.01f)
                        {
                            backwards = true;
                            waitTimer = Time.time + stopSeconds;
                        }
                    }
                    else if (backwards)
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, startPosition.position, ref velocity, smoothSpeed);
                        // Determine if the platform should move forward.
                        if (Mathf.Abs((startPosition.position - transform.position).magnitude) <= 0.01f)
                        {
                            backwards = false;
                            waitTimer = Time.time + stopSeconds;
                        }
                    }
                }
                break;
            case Types.Hold:
                // If its wait time is over.
                if (waitTimer < Time.time)
                {
                    // If the player is touching it, move forward.
                    if (holding)
                        transform.position = Vector3.SmoothDamp(transform.position, endPosition.position, ref velocity, smoothSpeed);
                    // If the player isnt touching it, move backwards.
                    else if (!holding)
                        transform.position = Vector3.SmoothDamp(transform.position, startPosition.position, ref velocity, smoothSpeed);
                }
                break;
        }
    }

    // Methods called by collisions.

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If the player touches the platform, make it a child.
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(transform);
            if (type.Equals(Types.Hold))
            {
                holding = true;
                waitTimer = Time.time + stopSeconds;
                velocity = Vector3.zero;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // If the player leaves the platform, unparent it.
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
            if (type.Equals(Types.Hold))
            {
                holding = false;
                waitTimer = Time.time + stopSeconds;
                velocity = Vector3.zero;
            }
        }
    }
}
