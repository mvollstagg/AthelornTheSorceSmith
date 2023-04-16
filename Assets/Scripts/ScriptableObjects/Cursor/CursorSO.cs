using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Entities.Enum;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "CursorSO",
    menuName = "Scriptable Objects/Cursor"
)]
public class CursorSO : ScriptableObject
{
    public CursorType CursorType;
    public Transform CursorTransform;
}