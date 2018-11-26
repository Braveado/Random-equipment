using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class FadingPlatform : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer sprite;                           // Reference to the sprite thats going to fade.
    public LightSprite lightSource;                         // Reference to the light thats going to be used.
    public GameObject collisions;                           // Reference to the game object that holds the collisions.

    [Header("Settings")]
    public Types type;                                      // The actual type of the platform.
    public enum Types { Timed, Triggered }                  // The possible platform types.
    private bool fade;                                      // Whether the platform should fade in or out.
    public float fadeInSpeed = 1;                           // Speed multiplier for a full fade in.
    public float fadeOutSpeed = 1;                          // Speed multiplier for a full fade out.    
    public float stateSeconds = 1;                          // Seconds to wait between transitions.
    private float waitTimer;                                // Timer to manage transitions.    
    private Color color;                                    // Color used to make the animations.

    [Header("Timed")]
    public float startSeconds = 1;                          // Seconds to wait before fading in the first time.

    // Triggered
    private bool trigger;                                   // To know if the player touched the platform.


    private void Start()
    {
        // Initialize variables here.
        switch(type)
        {
            case Types.Timed:
                // Start the platform inactive.
                color = lightSource.Color;
                color.a = 0;
                lightSource.Color = color;
                collisions.SetActive(false);
                Color temp = sprite.color;
                temp.a = 0f;
                sprite.color = temp;
                waitTimer = Time.time + startSeconds;
                break;
            case Types.Triggered:
                // Start the platform active.
                color = lightSource.Color;
                color.a = 0;
                lightSource.Color = color;
                collisions.SetActive(true);
                color = sprite.color;
                color.a = 0.5f;
                sprite.color = color;
                startSeconds = 0;
                break;
        }
    }

    private void Update()
    {
        switch(type)
        {
            case Types.Timed:
                // Wait for the timer to fade.
                if (waitTimer < Time.time)
                {
                    // If the platform should fade in...
                    if (!fade)
                    {
                        // ... enable the collisions if they are disabled.
                        if (!collisions.activeInHierarchy)
                            collisions.SetActive(true);

                        // Rise the sprite alpha.
                        color = sprite.color;
                        color.a += Time.deltaTime * fadeInSpeed * 0.5f;
                        sprite.color = color;

                        // Rise the light alpha.
                        color = lightSource.Color;
                        color.a += Time.deltaTime * fadeInSpeed * 0.5f;
                        lightSource.Color = color;

                        // If it is half opaque...
                        if (sprite.color.a >= 0.5f && lightSource.Color.a >= 0.5f)
                        {
                            // ... prepare for the fade out.
                            fade = true;
                            waitTimer = Time.time + stateSeconds;
                        }
                    }
                    // If the plaform should fade out...
                    else if (fade)
                    {
                        // ... lower the sprite alpha.
                        color = sprite.color;
                        color.a -= Time.deltaTime * fadeOutSpeed * 0.5f;
                        sprite.color = color;

                        // Lower the light alpha.
                        color = lightSource.Color;
                        color.a -= Time.deltaTime * fadeOutSpeed * 0.5f;
                        lightSource.Color = color;

                        // If it is fully faded...
                        if (sprite.color.a <= 0f && lightSource.Color.a <= 0f)
                        {
                            // ... prepare for the fade in and disable the collisions.
                            fade = false;
                            waitTimer = Time.time + stateSeconds;
                            collisions.SetActive(false);
                        }
                    }
                }
                break;
            case Types.Triggered:
                // Wait for the trigger and timer to fade.
                if(trigger && waitTimer < Time.time)
                {
                    // If the plaform should fade out...
                    if (fade)
                    {
                        // ... lower the sprite alpha.
                        color = sprite.color;
                        color.a -= Time.deltaTime * fadeOutSpeed * 0.5f;
                        sprite.color = color;

                        // Lower the light alpha.
                        color = lightSource.Color;
                        color.a -= Time.deltaTime * fadeOutSpeed * 0.5f;
                        lightSource.Color = color;

                        // If it is fully faded...
                        if (sprite.color.a <= 0f && lightSource.Color.a <= 0f)
                        {
                            // ... prepare for the fade in and disable the collisions.
                            fade = false;
                            waitTimer = Time.time + stateSeconds;
                            collisions.SetActive(false);
                        }
                    }
                    // If the platform should fade in...
                    else if (!fade)
                    {
                        // ... enable the collisions if they are disabled.
                        if (!collisions.activeInHierarchy)
                            collisions.SetActive(true);

                        // Rise its alpha.
                        color = sprite.color;
                        color.a += Time.deltaTime * fadeInSpeed;
                        sprite.color = color;

                        // If it is half opaque...
                        if (sprite.color.a >= 0.5f)
                        {
                            // ... reset the trigger.  
                            trigger = false;
                        }
                    }
                }
                break;
        }        
    }

    // Methods called by collisions.

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Set up the fade out when the player triggers the platform.
        if(type.Equals(Types.Triggered) && other.gameObject.CompareTag("Player") && !trigger)
        {
            trigger = true;
            waitTimer = Time.time + stateSeconds;
            fade = true;

            //color = sprite.color;
            //color.a = 1f;
            //sprite.color = color;

            color = lightSource.Color;
            color.a = 0.5f;
            lightSource.Color = color;
        }

    }
}
