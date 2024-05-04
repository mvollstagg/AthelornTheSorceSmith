using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Scripts.Entities.Class;

[CreateAssetMenu(fileName = "UIStatDataSO", menuName = "Scriptable Objects/UI/StatDataSO", order = 1)]
public class UIStatDataSO : ScriptableObject
{
    public List<CharacterStatUI> statDataList;
}