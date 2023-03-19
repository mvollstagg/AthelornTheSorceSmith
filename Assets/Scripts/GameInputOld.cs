using System;
using Scripts.Core.Singletons;

public class GameInputOld : Singleton<GameInput>
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    private PlayerInputActions playerInputActions;
    // private void Awake() {
    //     playerInputActions = new();
    //     playerInputActions.Player.Enable();

    //     playerInputActions.Player.Interact.performed += Interact_Performed;
    //     playerInputActions.Player.InteractAlternate.performed += InteractAlternate_Performed;
    //     playerInputActions.Player.Pause.performed += Pause_Performed;
    // }
    
    // void OnDestroy()
    // {
    //     playerInputActions.Player.Interact.performed -= Interact_Performed;
    //     playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_Performed;
    //     playerInputActions.Player.Pause.performed -= Pause_Performed;
    //     playerInputActions.Dispose();
    // }

    // private void Pause_Performed(InputAction.CallbackContext context)
    // {
    //     OnPauseAction?.Invoke(this, EventArgs.Empty);
    // }

    // private void InteractAlternate_Performed(InputAction.CallbackContext obj)
    // {
    //     OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    // }

    // private void Interact_Performed(InputAction.CallbackContext obj)
    // {
    //     OnInteractAction?.Invoke(this, EventArgs.Empty);
    // }

    // public Vector2 GetMovementVectorNormalized(){
    //     Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    //     inputVector = inputVector.normalized;
    //     return inputVector;
    // }
}
