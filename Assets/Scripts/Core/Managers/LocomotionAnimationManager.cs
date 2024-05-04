using UnityEngine;
using Scripts.Entities.Enum;
using Scripts.Core;
public class LocomotionAnimationManager : Singleton<LocomotionAnimationManager>
{
    private Camera mainCamera;
    [SerializeField]
    private MousePositionAreaType mouseArea;
    [SerializeField]
    private LocomotionAnimationType animationType;
    public Vector2 LocomotionMoveInput => DetermineLocomotionMoveInput();

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 fromCenterToMouse = mousePosition - new Vector2(Screen.width / 2, Screen.height / 2);
        float angle = Vector2.SignedAngle(Vector2.up, fromCenterToMouse);

        // Normalize angle to be in the range of 0 to 360
        angle = (angle + 360) % 360;

        mouseArea = DetermineArea(angle);
        animationType = DetermineAnimationType();
    }

    private MousePositionAreaType DetermineArea(float angle)
    {
        if (angle <= 22.5f || angle > 337.5f)
            return MousePositionAreaType.Forward;
        if (angle <= 67.5f)
            return MousePositionAreaType.ForwardLeft;
        if (angle <= 112.5f)
            return MousePositionAreaType.Left;
        if (angle <= 157.5f)
            return MousePositionAreaType.BackwardLeft;
        if (angle <= 202.5f)
            return MousePositionAreaType.Backward;
        if (angle <= 247.5f)
            return MousePositionAreaType.BackwardRight;
        if (angle <= 292.5f)
            return MousePositionAreaType.Right;
        if (angle <= 337.5f)
            return MousePositionAreaType.ForwardRight;

        return MousePositionAreaType.Unknown; // Fallback, should not happen
    }

    private LocomotionAnimationType DetermineAnimationType()
    {
        var input = new Vector2(InputManager.Instance.move.x, InputManager.Instance.move.y);
        // Straight Directions
        if (input.x == 0 && input.y == 1) // Moving up
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.BackwardLeft;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.ForwardLeft;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }
        else if (input.x == 0 && input.y == -1) // Down
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.ForwardLeft;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.BackwardLeft;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }
        else if (input.x == -1 && input.y == 0) // Moving Left
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.BackwardLeft;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.ForwardLeft;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }
        else if (input.x == 1 && input.y == 0) // Moving Right
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.ForwardLeft;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.BackwardLeft;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.BackwardRight;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }

        // Diagonal Directions
        else if (input.x == -1 && input.y == 1) // Moving Up-Left (Forward Left)
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.ForwardLeft;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.BackwardLeft;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.Left;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }
        else if (input.x == 1 && input.y == 1) // Moving Up-Right (Forward Right)
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.BackwardLeft;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.ForwardLeft;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.Forward;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }
        else if (input.x == -1 && input.y == -1) // Moving Down-Left (Backward Left)
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.ForwardLeft;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.ForwardRight;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.BackwardLeft;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }
        else if (input.x == 1 && input.y == -1) // Moving Down-Right (Backward Right)
        {
            switch (mouseArea)
            {
                case MousePositionAreaType.Forward:
                    return LocomotionAnimationType.Right;
                case MousePositionAreaType.ForwardRight:
                    return LocomotionAnimationType.BackwardRight;
                case MousePositionAreaType.Right:
                    return LocomotionAnimationType.Backward;
                case MousePositionAreaType.BackwardRight:
                    return LocomotionAnimationType.BackwardLeft;
                case MousePositionAreaType.Backward:
                    return LocomotionAnimationType.Left;
                case MousePositionAreaType.BackwardLeft:
                    return LocomotionAnimationType.ForwardLeft;
                case MousePositionAreaType.Left:
                    return LocomotionAnimationType.Forward;
                case MousePositionAreaType.ForwardLeft:
                    return LocomotionAnimationType.ForwardRight;
                default:
                    return LocomotionAnimationType.Idle;
            }
        }

        return LocomotionAnimationType.Idle;
    }

    private Vector2 DetermineLocomotionMoveInput()
    {
        switch (animationType)
        {
            case LocomotionAnimationType.Forward:
                return new Vector2(0, 1);
            case LocomotionAnimationType.ForwardRight:
                return new Vector2(1, 1);
            case LocomotionAnimationType.Right:
                return new Vector2(1, 0);
            case LocomotionAnimationType.BackwardRight:
                return new Vector2(1, -1);
            case LocomotionAnimationType.Backward:
                return new Vector2(0, -1);
            case LocomotionAnimationType.BackwardLeft:
                return new Vector2(-1, -1);
            case LocomotionAnimationType.Left:
                return new Vector2(-1, 0);
            case LocomotionAnimationType.ForwardLeft:
                return new Vector2(-1, 1);
            default:
                return new Vector2(0, 0);
        }
    }
}
