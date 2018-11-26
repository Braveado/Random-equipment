using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Components references
    private Rigidbody2D playerRb;                   // Reference to the player rigidbody2D component.
    private Transform standingCheck;                // Reference to the position marking where to check if the player is standing. 
    private Transform dodgeCheck;                   // Reference to the position marking from where to check against slopes on dodges.
    private Transform climbCheck;                   // Reference to the position marking from where to check for platform grabs.
    private Animator playerRig;                     // Reference to the player animator component.
    private CameraControl playerCam;                // Reference to the camera control that follows the player.
    private PlayerInventory inventory;              // Reference to the player inventory component.

    // Inputs
    private float xMov;                             // Input for horizontal movement.
    private float yMov;                             // Input for vertical movement.
    private float xBuffer;                          // For linear multiplication with xMov.
    private bool jump;                              // Input for jump action.
    private bool dodge;                             // Input for dodge action.
    private bool clamber;                           // Input for clamber action.
    private bool inventoryToggle;                   // Input for toggling the inventory;
    private bool mainBasic;                         // Input for main hand basic action.

    // Actions
    Actions action;                                 // The actual action state of the player.
    enum Actions                                    // Valid action states for the player.   
    { Movement, Jump, Dodge, Clamber, Basic };

    [Header("Movement")]
    public LayerMask floor;                         // A mask determining what is floor to the player. 
    private float standingLength = 1f;              // Length of the ray cast to determine if standing.
    private bool standing;                          // Whether or not the player is standing.
    public float maxSpeed = 18f;                    // The speed of the player in the x axis while running.
    public float jumpForce = 2400f;                 // Amount of force added when the player jumps.
    public bool airControl = true;                  // Whether or not the player can move while jumping.
    public float dodgeForce = 2400f;                // Amount of force added when the player dodges.
    private float dodgeLength = 10f;                // Length of the ray cast to manage slopes in a dodge.
    private bool facingRight = true;                // For determining which way the player is currently facing. 
    public LayerMask climbThrough;                  // A mask determining what the player can clamber.
    private float climbRadius = 0.5f;               // Radius of the circle cast to determine if can climb.
    private bool climb;                             // Whether or not the player can climb.
    private Vector2 climbPos;                       // The center of the platform to climb.

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        // Get references here.
        playerRb = GetComponent<Rigidbody2D>();
        standingCheck = transform.Find("PositionChecks").Find("StandingCheck");
        dodgeCheck = transform.Find("PositionChecks").Find("DodgeCheck");
        climbCheck = transform.Find("PositionChecks").Find("ClimbCheck");
        playerRig = GetComponent<Animator>();
        playerCam = FindObjectOfType<CameraControl>();
        inventory = GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        // Initialize variables here.
        inventory.SetupInventary();
    }

    private void Update()
    {
        InputHandler.instance.CheckInputMethod();
        ToggleInventory();
        ManageActions();        
    }

    private void ToggleInventory()
    {
        if (InputHandler.instance.controller ? Input.GetButtonDown("InventoryC") : Input.GetButtonDown("Inventory"))
        {
            if (!inventoryToggle)
            {
                // Show the menu.
                inventoryToggle = true;
                playerCam.center = true;
                inventory.Enable();
            }
            else if (inventoryToggle)
            {
                // Hide the menu.
                inventoryToggle = false;
                playerCam.center = false;
                inventory.Disable();
            }
        }
    }

    private void ManageActions()
    {
        if (action.Equals(Actions.Movement))
        {
            // Get movement inputs.
            if (InputHandler.instance.controller)
            {
                yMov = Input.GetAxis("YMovC");
                xMov = Input.GetAxis("XMovC");
            }
            else
            {
                yMov = Input.GetAxis("YMov");
                xMov = Input.GetAxis("XMov");
            }

            // Get jump input
            if (standing && yMov >= 0f)
            {
                jump = InputHandler.instance.controller ? Input.GetButtonDown("JumpC") : Input.GetButtonDown("Jump");
                if (jump)
                {
                    action = Actions.Jump;
                    return;
                }
            }

            // Get clamber input.
            if (climb && !standing)
            {
                clamber = (yMov > 0);
                if (clamber)
                {
                    action = Actions.Clamber;
                    return;
                }
            }

            if (!inventoryToggle)
            {
                // Get use input.                
                if (InputHandler.instance.controller ? Input.GetButtonDown("UseC") : Input.GetButtonDown("Use"))                
                    inventory.EquipPreview();                

                // Get dodge input.
                if (standing)
                {
                    dodge = InputHandler.instance.controller ? Input.GetButtonDown("DodgeC") : Input.GetButtonDown("Dodge");
                    if (dodge)
                    {
                        action = Actions.Dodge;
                        return;
                    }
                }

                // Get main hand basic input.
                if (standing)
                {
                    mainBasic = InputHandler.instance.controller ? Input.GetButtonDown("MHBasicC") : Input.GetButtonDown("MHBasic");
                    if (mainBasic)
                    {
                        action = Actions.Basic;
                        return;
                    }
                }
            }
            else if(inventoryToggle)
            {
                // Get equipment input.
                if (InputHandler.instance.controller ? Input.GetButtonDown("MHBasicC") : Input.GetButtonDown("MHSpecial"))                
                    inventory.ToggleEquipment();                            
            }
        }
        else if (action.Equals(Actions.Jump))
        {
            // Get the player movement if it has air control.
            if (airControl)
            {
                // Get horizontal movement input.
                if (InputHandler.instance.controller)
                    xMov = Input.GetAxis("XMovC");
                else
                    xMov = Input.GetAxis("XMov");
            }

            // Get vertical movement input.
            if (InputHandler.instance.controller)
                yMov = Input.GetAxis("YMovC");
            else
                yMov = Input.GetAxis("YMov");

            // Get clamber input.
            if (climb)
            {
                clamber = (yMov > 0);
                if (clamber)
                {
                    action = Actions.Clamber;
                    return;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        FloorCheck();
        ClimbCheck();

        switch (action)
        {
            case Actions.Movement:
                Movement();
                break;
            case Actions.Jump:
                Jump();
                break;
            case Actions.Dodge:
                Dodge();
                break;
            case Actions.Clamber:
                Clamber();
                break;
            case Actions.Basic:
                Basic();
                break;
        }
    }

    private void FloorCheck()
    {
        // Reset the standing variable.
        standing = false;

        // The player is standing if a ray cast from the standingcheck position hits anything designated as floor.
        RaycastHit2D hit = Physics2D.Raycast(standingCheck.position, Vector2.down, standingLength, floor);
        if (hit.collider != null)
            standing = true;

        // Set the standing of the animator.
        playerRig.SetBool("standing", standing);

        // Set the yspeed of the animator to the rigidbody y velocity.        
        playerRig.SetFloat("ySpeed", playerRb.velocity.y);
    }    

    private void ClimbCheck()
    {
        // Reset the climb variable.
        climb = false;

        // The player can climb if a circle cast to the climbcheck position hits anything designated as climbthrough.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(climbCheck.position, climbRadius, climbThrough);
        if (colliders.Length > 0)
        {
            climb = true;
            climbPos = colliders[0].transform.position;
        }
    }

    private void Movement()
    {
        // Move the player.
        playerRb.velocity = new Vector2(xMov * maxSpeed, playerRb.velocity.y);

        // Set the xspeed of the animator to the absolute value of the xmov.
        playerRig.SetFloat("xSpeed", Mathf.Abs(xMov));

        // If the input is moving the player right and the player is facing left...
        if (xMov > 0 && !facingRight)
        {
            // ... flip the player.
            facingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (xMov < 0 && facingRight)
        {
            // ... flip the player.
            facingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Constrain the rigidbody if standing still.
        if (xMov == 0 && standing)
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        // Or reset the constraints of the rigidbody.
        else
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Jump()
    {
        // If the player jumps...
        if (jump)
        {
            // ... add a vertical force to the player.
            playerRb.velocity = new Vector2(xMov * maxSpeed, 0);
            playerRb.AddForce(new Vector2(0f, jumpForce));

            // Turn off standing to prevent the end of the state in the first frame.
            standing = false;

            // Turn off jump to prevent repeats.
            jump = false;            
        }

        // Let the player move if it has air control.
        if (airControl)
            Movement();        

        // Reset the player action state when landing on a floor. Managing max ySpeed in slopes.
        if (standing && Mathf.Abs(playerRb.velocity.y) < 15f)        
            action = Actions.Movement;                    
    }

    private void Dodge()
    {
        // If the player dodges...
        if (dodge)
        {
            // ... set the dodge variable in the animator.
            playerRig.SetBool("dodge", true);

            // Turn off dodge to prevent repeats.
            dodge = false;
        }

        // Linear slow down.
        playerRb.velocity *= 0.95f;

        // Manage incoming slopes y changing the gravity of the player.
        RaycastHit2D hit = Physics2D.Raycast(dodgeCheck.position, (facingRight ? Vector2.right : Vector2.left), dodgeLength, floor);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)            
            playerRb.gravityScale = 0;
        else
            playerRb.gravityScale = 10;
    }

    private void Clamber()
    {
        // If the player clambers...
        if (clamber)
        {
            // ... set the clamber variable in the animator.
            playerRig.SetBool("clamber", true);

            // Stop the player movement.
            playerRb.velocity = Vector2.zero;

            // Constraint the player rigidbody.
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

            // Set the player y position based on the climbY value.
            Vector3 pos = transform.position;
            pos.y = climbPos.y - climbCheck.localPosition.y;
            transform.position = pos;

            // Reset the climb variable to prevent it being true at the start of the next jump.
            climb = false;

            // Turn off clamber to prevent repeats.
            clamber = false;
        }
    }

    private void Basic()
    {
        // If the player uses the main hand basic...
        if (mainBasic)
        {
            // ... set the basic variable in the animator.
            playerRig.SetBool("basic", true);

            // Turn off mainBasic to prevent repeats.
            mainBasic = false;
        }
    }

    // Methods called by animations.

    private void StartDodge()
    {
        // Stop the movement of the player.
        playerRb.velocity = new Vector2(0f, playerRb.velocity.y);

        // Reset the xmov input.
        xMov = 0f;

        // Reset the xspeed of the animator.
        playerRig.SetFloat("xSpeed", 0f);

        // Reset the constraints of the rigidbody.            
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Add a horizontal force to the player.            
        if (facingRight)
            playerRb.AddForce(new Vector2(dodgeForce, 0f));
        else if (!facingRight)
            playerRb.AddForce(new Vector2(-dodgeForce, 0f));        
    }

    private void EndDodge()
    {
        // Reset the dodge variable in the animator.
        playerRig.SetBool("dodge", false);

        // Stop the player movement.
        playerRb.velocity = new Vector2(0, playerRb.velocity.y);

        // Reset the player gravity.
        playerRb.gravityScale = 10;
        
        // Reset the player action state.
        action = Actions.Movement;
    }

    private void AddClamberForce()
    {
        // Constraint the player rigidbody.
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Add a force to the player. Managing the least force to pass its collider through and stay there.
        playerRb.AddForce(new Vector2((climbPos.x >= transform.position.x ? 200 : -200), 2300f));
    }

    private void EndClamber()
    {
        // Reset the clamber variable in the animator.
        playerRig.SetBool("clamber", false);

        // Reset the player action state.
        action = Actions.Movement;
    }

    private void StartBasic()
    {
        // Stop the movement of the player.
        playerRb.velocity = new Vector2(0f, playerRb.velocity.y);

        // Reset the xmov input.
        xMov = 0f;

        // Reset the xspeed of the animator.
        playerRig.SetFloat("xSpeed", 0f);

        // Reset the constraints of the rigidbody.            
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;        
    }

    private void AddBasicForce()
    {
        // Add a horizontal force to the player.            
        if (facingRight)
            playerRb.AddForce(new Vector2(900, 0f));
        else if (!facingRight)
            playerRb.AddForce(new Vector2(-900, 0f));
    }

    private void StopBasicForce()
    {
        // Stop the player movement.
        playerRb.velocity = new Vector2(0, playerRb.velocity.y);
    }

    private void EndBasic()
    {
        // Reset the basic variable in the animator.
        playerRig.SetBool("basic", false);        

        // Reset the player action state.
        action = Actions.Movement;
    }

    // Methods called by collisions.
}
