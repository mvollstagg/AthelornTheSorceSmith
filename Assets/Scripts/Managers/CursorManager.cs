using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JohnTheBlacksmith.Assets.Scripts.Core;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Transform _cursors;
    [SerializeField] List<CursorSO> _cursorSOList;
    Transform _currentCursor;

    

    void Start()
    {
        // Instantiate pointer cursor from _cursorSOs under _cursors
        _currentCursor = Instantiate(_cursorSOList.FirstOrDefault(x => x.CursorType == CursorType.Pointer).CursorTransform, _cursors);

        CursorInteractor.OnMouseEnterInteractable += CursorInteractor_OnMouseEnterInteractable;
    }

    private void CursorInteractor_OnMouseEnterInteractable(object sender, CursorInteractor.OnMouseEnterInteractableInteractableEventArgs e)
    {
        // Destroy current cursor
        if(_currentCursor != null)
            Destroy(_currentCursor.gameObject);
        _currentCursor = Instantiate(_cursorSOList.FirstOrDefault(x => x.CursorType == e.CursorType).CursorTransform, _cursors);
    }

    void Update()
    {
        if(!CameraController.Instance.cursorLocked)
            _cursors.gameObject.SetActive(true);
        else
            _cursors.gameObject.SetActive(false);
        _cursors.position = Input.mousePosition;
    }    
}
