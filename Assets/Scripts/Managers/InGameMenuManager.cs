using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Entities.Class;

public class InGameMenuManager : Singleton<InGameMenuManager>
{
    [Header("In-Game Elements")]
    [SerializeField] private Transform _inGameMenu;
    [SerializeField] private Transform _menuPanel;
    [SerializeField] private Transform _tabPanel;
    [SerializeField] private RectTransform _menuOutline;
    [SerializeField] private Dictionary<string, Button> _menuButtons = new Dictionary<string, Button>();

    void Start()
    {
        // Collect the menu buttons and add them to the dictionary
        Button[] menuButtons = _menuPanel.GetComponentsInChildren<Button>();
        foreach (Button button in menuButtons)
        {
            _menuButtons[button.name] = button;
        }

        // Assign click listeners to the menu buttons based on the names of the tabs under the tab panel
        foreach (Transform tab in _tabPanel)
        {
            string tabName = tab.gameObject.name;
            if (_menuButtons.ContainsKey(tabName))
            {
                _menuButtons[tabName].onClick.AddListener(() => _ShowPanel(tabName));
            }
        }

        // Set the initial panel to the inventory panel
        if (InputManager.Instance.IsCurrentDeviceMouse)
        {
            TurnMenuInteractable(true);
        }
        else
        {
            TurnMenuInteractable(false);
        }
    }

    public void ToggleInGameMenu()
    {
        _inGameMenu.gameObject.SetActive(!_inGameMenu.gameObject.activeSelf);
            
        if (_inGameMenu.gameObject.activeSelf)
        {
            _ShowPanel(InGameMenus.INVENTORY);
            InventoryManager.Instance.OnInventoryEnabled();

            // Enable UI input and disable rest of the input
            InputManager.Instance.SwitchActionMap(ActionMaps.UI);
            InputManager.Instance.EnableActionMap(ActionMaps.INGAMEMENU);
        }
        else
        {
            InventoryManager.Instance.OnInventoryDisabled();

            // Enable Character inputs and disable UI input
            InputManager.Instance.EnableActionMap(ActionMaps.CHARACTER);
            InputManager.Instance.EnableActionMap(ActionMaps.CAMERA);
            InputManager.Instance.DisableActionMap(ActionMaps.UI);
        }
    }

    public void TurnMenuInteractable(bool interactable)
    {
        foreach (Button button in _menuButtons.Values)
        {
            button.interactable = interactable;
        }
    }

    private void _ShowPanel(string panelName)
    {
        // Hide all panels
        foreach (Transform panel in _tabPanel)
        {
            panel.gameObject.SetActive(false);
        }

        // Show the panel with the given name
        if (_tabPanel.Find(panelName) != null)
        {            
            _tabPanel.Find(panelName).gameObject.SetActive(true);

            // Chance OutlineFrame parent to the new panel
            _menuOutline.SetParent(_menuPanel.Find(panelName));
            _menuOutline.anchoredPosition = Vector2.zero;
        }
    }
}
