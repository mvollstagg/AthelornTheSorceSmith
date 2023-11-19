using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Scripts.Entities.Class;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using Scripts.Core;

public class CharacterUIManager : Singleton<CharacterUIManager>
{
    public Transform stats;
    public Transform statsInventoryMenu;
    public Transform statsCharacterMenu;
    public Transform baseStatsCharacterMenu;
    public Transform derrivedStatsCharacterMenu;
    public Transform expBar;
    public TMP_Text statDetailsText;
    public TMP_Text statPointsDetailsText;
    public UIStatDataSO statDataSO;
    [SerializeField]
    private Dictionary<string, Button> increaseButtons = new Dictionary<string, Button>();
    [SerializeField]
    private Dictionary<string, Button> decreaseButtons = new Dictionary<string, Button>();
    public Transform applyButton;
    public Transform revertButton;

    void Start()
    {
        if (CharacterManager.Instance.characterDataSO != null)
        {
            UpdateInGameCharacterStatUI(CharacterManager.Instance.characterDataSO.GetStats());
        }
        EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_STAT_INFO_CHANGED, CharacterUIManager_OnCharacterStatInfoChanged);

        // Populate the dictionaries for increase and decrease buttons
        foreach (Transform statTransform in CharacterUIManager.Instance.baseStatsCharacterMenu)
        {
            string statName = statTransform.name;
            Button increaseButton = statTransform.Find("Increase").GetComponent<Button>();
            Button decreaseButton = statTransform.Find("Decrease").GetComponent<Button>();

            increaseButtons.TryAdd(statName, increaseButton);
            decreaseButtons.TryAdd(statName, decreaseButton);

            // Add listeners to the buttons
            increaseButton.onClick.AddListener(() => OnIncreaseButtonClicked(statName));
            decreaseButton.onClick.AddListener(() => OnDecreaseButtonClicked(statName));
        }

        // Add listeners to the apply and revert buttons
        applyButton.gameObject.GetComponent<Button>().onClick.AddListener(() => OnApplyButtonClicked());
        revertButton.gameObject.GetComponent<Button>().onClick.AddListener(() => OnRevertButtonClicked());

        UpdateIncreaseDecreaseButtonsVisibility();
    }

    void LateUpdate()
    {
        var currentStats = Character.Instance.GetCharacterData();
        UpdateInGameStatUI(currentStats.GetStats());
        UpdateInGameInventoryStatUI(currentStats.GetStats());

        // UpdateIncreaseDecreaseButtonsVisibility();
    }

    public void UpdateIncreaseDecreaseButtonsVisibility()
    {
        var characterData = Character.Instance.GetCharacterData();
        var characterManagerData = CharacterManager.Instance.characterDataSO;

        var statPoints = characterManagerData.statPoints;

        foreach (var stat in characterManagerData.GetBaseStats())
        {
            string statName = stat.Key;
            int currentStatValue = stat.Value.currentValue;

            // Check if there are available stat points or if the stat has been increased
            if (statPoints > 0)
            {
                // Show the Increase button if there are available points or if the stat has been increased
                increaseButtons[statName].gameObject.SetActive(true);

                // Show the Decrease button if the stat value is greater than the base value
                if (currentStatValue > characterData.GetBaseStats()[statName].currentValue)
                {
                    decreaseButtons[statName].gameObject.SetActive(true);
                }
                else
                {
                    // Hide the Decrease button if the stat value is equal to or less than the base value
                    decreaseButtons[statName].gameObject.SetActive(false);
                }
            }
            else
            {
                // Hide the Increase button if there are no available points and the stat has not been increased
                increaseButtons[statName].gameObject.SetActive(false);

                // Show the Decrease button if the stat has a value greater than the base value
                if (currentStatValue > characterData.GetBaseStats()[statName].currentValue)
                {
                    decreaseButtons[statName].gameObject.SetActive(true);
                }
                else
                {
                    // Hide the Decrease button if the stat value is equal to or less than the base value
                    decreaseButtons[statName].gameObject.SetActive(false);
                }
            }
        }

        UpdateApplyRevertButtonsVisibility();
    }

    public void UpdateApplyRevertButtonsVisibility()
    {
        var statPoints = Character.Instance.GetCharacterData().statPoints;
        if (statPoints > 0)
        {
            // Show the Apply and Revert buttons
            applyButton.gameObject.SetActive(true);
            revertButton.gameObject.SetActive(true);
        }
        else
        {
            // Hide the Apply and Revert buttons
            applyButton.gameObject.SetActive(false);
            revertButton.gameObject.SetActive(false);
        }
    }

    void OnIncreaseButtonClicked(string statName)
    {
        var characterData = CharacterManager.Instance.characterDataSO;
        var statPoints = characterData.statPoints;

        // Check if there are available stat points
        if (statPoints > 0)
        {
            // Increase the stat value and deduct one stat point
            characterData.IncreaseStat(statName);
            characterData.statPoints--;

            // Update the UI and buttons visibility
            UpdateInGameCharacterStatUI(characterData.GetStats());
            UpdateIncreaseDecreaseButtonsVisibility();

            // Fire an event to inform other systems about the change in character stats
            // EventManager.Instance.DispatchEvent(GameEvents.ON_CHARACTER_STAT_INFO_CHANGED);
        }
    }

    void OnDecreaseButtonClicked(string statName)
    {
        var characterData = CharacterManager.Instance.characterDataSO;

        // Decrease the stat value and refund one stat point
        characterData.DecreaseStat(statName);
        characterData.statPoints++;

        // Update the UI and buttons visibility
        UpdateInGameCharacterStatUI(characterData.GetStats());
        UpdateIncreaseDecreaseButtonsVisibility();

        // Fire an event to inform other systems about the change in character stats
        // EventManager.Instance.DispatchEvent(GameEvents.ON_CHARACTER_STAT_INFO_CHANGED);
    }

    private void OnRevertButtonClicked()
    {
        CharacterManager.Instance.RevertChanges();
    }

    private void OnApplyButtonClicked()
    {
        CharacterManager.Instance.ApplyChanges();
    }

    private void CharacterUIManager_OnCharacterStatInfoChanged(object sender, EventArgs e)
    {
        // Check if the sender object is not null
        if (sender != null)
        {
            // Extract the sender object's name
            string senderName = sender.ToString().Replace(" (CharacterInfoManager)", "");

            // Check if statDataSO is not null
            if (statDataSO != null)
            {
                // Find the corresponding StatData based on the sender's name
                var statData = statDataSO.statDataList.Find(x => x.name == senderName);

                // Check if the StatData is not null
                if (statData != null)
                {
                    // Access the description
                    string description = statData.description;

                    // Update the UI with the description
                    statDetailsText.text = description;
                }
            }
        }
    }

    public void UpdateInGameStatUI(Dictionary<string, CharacterStat> statsData)
    {
        // In-Game stats
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

    public void UpdateInGameInventoryStatUI(Dictionary<string, CharacterStat> statsData)
    {
        // InGameMenu Inventory tab stats
        foreach (Transform statTransform in statsInventoryMenu)
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

        // Adjust ExpBar
        Image expBarImage = expBar.transform.Find("Line").GetComponent<Image>();
        float expFillAmount = (float)statsData["Experience"].currentValue / statsData["Experience"].maxValue;
        expBarImage.fillAmount = Mathf.Clamp01(expFillAmount);
        // Adjust ExpBar text for example 1200/2400 Lv. 24
        TextMeshProUGUI expText = expBar.transform.Find("Details").GetComponent<TextMeshProUGUI>();
        expText.text = $"{statsData["Experience"].currentValue}/{statsData["Experience"].maxValue} Lv. {statsData["Level"].currentValue}";
        
    }

    public void UpdateInGameCharacterStatUI(Dictionary<string, CharacterStat> statsData)
    {
        // InGameMenu Character tab stats
        foreach (Transform statTransform in statsCharacterMenu)
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

        // Base stats
        foreach (Transform statTransform in baseStatsCharacterMenu)
        {
            string statName = statTransform.name;
            FieldInfo field = typeof(CharacterDataSO).GetField(statName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (field != null)
            {
                int statValue = (int)field.GetValue(CharacterManager.Instance.characterDataSO);

                // Update child object title
                Transform titleTransform = statTransform.Find("Title");
                if (titleTransform != null)
                {
                    TextMeshProUGUI titleText = titleTransform.GetComponent<TextMeshProUGUI>();
                    string currentTitle = titleText.text;
                    int colonIndex = currentTitle.IndexOf(':');
                    if (colonIndex >= 0)
                    {
                        string newTitle = currentTitle.Substring(0, colonIndex + 1) + " " + statValue.ToString();
                        titleText.text = newTitle;
                    }
                }
            }
        }

        // Avaliable stat points
        var statPoints = Character.Instance.GetCharacterData().statPoints;
        statPointsDetailsText.text = "Avaliable Points: " + statPoints.ToString();

        // Derived stats
        foreach (Transform statTransform in derrivedStatsCharacterMenu)
        {
            string statName = statTransform.name;
            PropertyInfo property = typeof(CharacterDataSO).GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant().Contains(statName.ToLowerInvariant()));

            if (property != null)
            {
                double statValue = (double)property.GetValue(CharacterManager.Instance.characterDataSO);
                // Update child object title
                Transform titleTransform = statTransform.Find("Title");
                if (titleTransform != null)
                {
                    TextMeshProUGUI titleText = titleTransform.GetComponent<TextMeshProUGUI>();
                    string currentTitle = titleText.text;
                    int colonIndex = currentTitle.IndexOf(':');
                    if (colonIndex >= 0)
                    {
                        string newTitle = currentTitle.Substring(0, colonIndex + 1) + " " + statValue.ToString();
                        titleText.text = newTitle;
                    }

                }
            }
        }
    }
}
