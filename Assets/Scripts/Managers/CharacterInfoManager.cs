using System;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterInfoManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI titleText;

    private void Awake()
    {
        // Find the child object with the specified name and get its TextMeshProUGUI component
        titleText = FindChildObject<TextMeshProUGUI>("Title");
        if (titleText == null)
        {
            titleText = FindChildObject<TextMeshProUGUI>("Status");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_STAT_INFO_CHANGED, this, EventArgs.Empty);
        // Change the text color of the titleText to the desired color
        if (titleText != null)
        {
            titleText.color = Color.yellow; // Replace with your desired color
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Change the text color of the titleText back to its original color
        if (titleText != null)
        {
            titleText.color = Color.white; // Replace with the original color
        }
    }

    private T FindChildObject<T>(string childObjectName) where T : Component
    {
        Transform childTransform = transform.Find(childObjectName);
        if (childTransform != null)
        {
            return childTransform.GetComponent<T>();
        }
        else
        {
            Debug.LogWarning($"Child object '{childObjectName}' not found.");
            return null;
        }
    }
}
