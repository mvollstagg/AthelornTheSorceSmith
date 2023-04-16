using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    public Canvas TargetCanvas;
    public Vector2 _newPosition;
    public Vector2 _mousePosition;
    [SerializeField] Transform _cursors;
    [SerializeField] List<CursorSO> _cursorSOList;
    Transform _currentCursor;

    void Start()
    {
        // Instantiate pointer cursor from _cursorSOs under _cursors
        // _currentCursor = Instantiate(_cursorSOList.FirstOrDefault(x => x.CursorType == CursorType.Pointer).CursorTransform, _cursors);
        // EventManager.Instance.AddListener<OnMouseInteractableEventArgs>("OnMouseEnterInteractable", CursorInteractor_OnMouseEnterInteractable);
    }

    private void CursorInteractor_OnMouseEnterInteractable(object sender, OnMouseInteractableEventArgs e)
    {
        // Destroy current cursor
        if(_currentCursor != null)
            Destroy(_currentCursor.gameObject);
        _currentCursor = Instantiate(_cursorSOList.FirstOrDefault(x => x.CursorType == e.CursorType).CursorTransform, _cursors);
    }

    void Update()
    {
        if(!CharacterRotateCamera.Instance.cursorLocked && Character.Instance.IsCurrentDeviceMouse)
            _cursors.gameObject.SetActive(true);
        else
            _cursors.gameObject.SetActive(false);
    }
    
    
    
    protected virtual void LateUpdate()
    {
        _mousePosition = Input.mousePosition;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(TargetCanvas.transform as RectTransform, _mousePosition, TargetCanvas.worldCamera, out _newPosition);
        // _cursors.position = TargetCanvas.transform.TransformPoint(_newPosition);
    }
}
