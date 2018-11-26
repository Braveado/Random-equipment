using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public static InputHandler instance;                            // Reference to the handler for global use.

    private void Awake()
    {
        // Assign the reference to this instance;
        instance = this;
    }

    public bool controller;

    public void CheckInputMethod()
    {
        // Only check for changes if there is at least one controller.
        if (Input.GetJoystickNames().Length > 0)
        {
            // Check for controller input if using mouse and keyboard.
            if (!controller)
            {
                if (Input.GetAxis("XMovC") != 0 || Input.GetAxis("YMovC") != 0 ||
                   Input.GetAxis("XAim") != 0 || Input.GetAxis("YAim") != 0 ||
                   Input.GetButtonDown("JumpC") || Input.GetButtonDown("DodgeC") ||
                   Input.GetButtonDown("UseC") || Input.GetButtonDown("InventoryC") ||
                   Input.GetButtonDown("MHBasicC") ||
                   Input.GetAxis("XDPad") != 0 || Input.GetAxis("YDPad") != 0)
                {
                    controller = true;
                    Cursor.visible = false;
                }
            }
            // Check for mouse and keyboard input if using controller.
            else if (controller)
            {
                if (Input.GetAxis("XMov") != 0 || Input.GetAxis("YMov") != 0 ||
                    Input.GetAxis("XMouse") != 0 || Input.GetAxis("YMouse") != 0 ||
                    Input.GetButtonDown("Jump") || Input.GetButtonDown("Dodge") ||
                    Input.GetButtonDown("Use") || Input.GetButtonDown("Inventory") ||
                    Input.GetButtonDown("MHBasic"))
                {
                    controller = false;
                    Cursor.visible = true;
                }
            }
        }
        // Default to mouse and keyboard when there are no controllers.
        else if (controller)
        {
            controller = false;
            Cursor.visible = true;
        }
    }
}
