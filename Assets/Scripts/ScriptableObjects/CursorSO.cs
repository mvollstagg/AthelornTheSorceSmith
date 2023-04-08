using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

[CreateAssetMenu()]
public class CursorSO : ScriptableObject
{
    public CursorType CursorType;
    public Transform CursorTransform;
}