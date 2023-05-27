using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharacterUIManager : MonoBehaviour
{
    public Transform stats;
    private Dictionary<string, CharacterStat> characterStats;

    void Update()
    {
        characterStats = Character.Instance.GetStats();
        UpdateStatUI(characterStats);
    }
    public void UpdateStatUI(Dictionary<string, CharacterStat> statsData)
    {
        foreach (Transform statTransform in stats)
        {
            string statName = statTransform.name;
            if (statsData.TryGetValue(statName, out CharacterStat stat))
            {
                TextMeshProUGUI statusText = statTransform.Find("Status").GetComponent<TextMeshProUGUI>();
                statusText.text = $"{stat.currentValue}/{stat.maxValue}";

                Image barImage = statTransform.Find("Bar").GetComponent<Image>();
                float fillAmount = (float)stat.currentValue / stat.maxValue;
                barImage.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
    }

}
