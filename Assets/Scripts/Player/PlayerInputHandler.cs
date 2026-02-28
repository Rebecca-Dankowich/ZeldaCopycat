using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private Vector2 moveInput;
    private bool jumpTriggered;
    private bool jumpHeld;

    public Vector2 MoveInput => moveInput;
    public bool JumpTriggered => jumpTriggered;
    public bool JumpHeld => jumpHeld;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            jumpTriggered = true;
            jumpHeld = true;
        }
        else if(context.canceled)
        {
            jumpHeld= false;
        }
    }

    private void LateUpdate()
    {
        // Consume the trigger after one frame so it doesn't fire repeatedly
        jumpTriggered = false;
    }
}
