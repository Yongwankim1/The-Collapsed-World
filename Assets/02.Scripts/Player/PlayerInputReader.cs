using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput)),DefaultExecutionOrder(-1000)]
public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    #region 액션
    InputAction moveAction;
    InputAction interactAction;
    InputAction toggleLightAction;
    InputAction attackAction;
    InputAction inventoryAction;
    #endregion

    #region 액션이름
    [SerializeField] string moveActionName = "Move";
    [SerializeField] string interactActionName = "Interact";
    [SerializeField] string toggleLightActionName = "LightToggle";
    [SerializeField] string attackActionName = "Attack";
    [SerializeField] string inventoryActionName = "InventoryToggle";
    #endregion

    #region 액션 변수
    public Vector2 MoveDir { get; private set; }
    public bool IsInteractPerformedThisFrame { get; private set; }
    public bool IsLightingToggle { get; private set; }
    public bool IsAttackPerformedThisFrame { get; private set; }
    public bool IsInventoryPerformedThisFrame {  get; private set; }

    public Stack<bool> DontAction = new Stack<bool>();
    #endregion


    private void Awake()
    {
        if(playerInput == null)
            playerInput = GetComponent<PlayerInput>();
        ResolveActions();
    }

    private void Update()
    {
        IsInventoryPerformedThisFrame = inventoryAction != null && inventoryAction.WasPerformedThisFrame();
        if (EscManager.Instance.Count > 0)
        {
            IsAttackPerformedThisFrame = false;
            IsInteractPerformedThisFrame = false;
            MoveDir = Vector2.zero;
            return;
        }
        MoveDir = moveAction.ReadValue<Vector2>().normalized;
        if (toggleLightAction.WasPerformedThisFrame()) IsLightingToggle = !IsLightingToggle;
        IsInteractPerformedThisFrame = interactAction != null && interactAction.WasPerformedThisFrame();
        IsAttackPerformedThisFrame = attackAction != null && attackAction.WasPerformedThisFrame();
    }

    private void ResolveActions()
    {
        moveAction = FindAction(moveActionName);
        interactAction = FindAction(interactActionName);
        toggleLightAction = FindAction(toggleLightActionName);
        attackAction = FindAction(attackActionName);
        inventoryAction = FindAction(inventoryActionName);
    }

    private InputAction FindAction(string actionName)
    {
        if (string.IsNullOrEmpty(actionName))
        {
            Debug.LogWarning("변수명이 비어있습니다.");
            return null;
        }
        InputAction action = playerInput.actions.FindAction(actionName, false);
        if (action == null)
        {
            Debug.LogWarning("액션을 찾을 수 없습니다");
        }
        return action;
    }

    public bool MoveAction()
    {
        if(MoveDir.sqrMagnitude > 0.01f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RunAction()
    {
        if (MoveDir.sqrMagnitude > 0.01f && Keyboard.current.shiftKey.IsPressed())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
