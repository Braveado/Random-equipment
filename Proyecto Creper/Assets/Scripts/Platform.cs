using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Collider2D pCollider;                       // Reference to the first collider in the platform.
    private bool touching;                              // Whether or not the player is touching the platform.

    private void Awake()
    {
        // Get references here.
        pCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Disable the collider if the player wants to drop off.
        if (touching)
        {
            if (InputHandler.instance.controller)
            {
                if (Input.GetAxis("YMovC") < 0f && Input.GetButtonDown("JumpC"))
                    pCollider.enabled = false;
            }        
            else if (!InputHandler.instance.controller)
            {
                if(Input.GetAxis("YMov") < 0f && Input.GetButtonDown("Jump"))
                    pCollider.enabled = false;
            }
        }            
    }

    // Methods called by collisions.

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If the player touched the platform.
        if(other.gameObject.CompareTag("Player"))
        {
            touching = true;               
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // If the player leaves the platform.
        if (other.gameObject.CompareTag("Player"))
        {
            touching = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If the player droped off the platform...
        if (other.gameObject.CompareTag("Player"))
        {
            // enable the collider.
            pCollider.enabled = true;
        }
    }
}
