using UnityEngine;
using System.Collections.Generic;
using Scripts.Core.SaveSystem.Entities;
using Scripts.Entities.Class;

public class SaveGameObject : MonoBehaviour, ISaveGameObject
{
    // This will hold the data about what components and fields/properties to save
    public List<ComponentData> componentsToSave = new List<ComponentData>();

    public Dictionary<string, object> CollectData()
    {
        // implement this method
        return null;
    }

    public void Register()
    {
        EventManager.Instance.Trigger(GameEvents.ON_SAVEGAMEOBJECT_INITIALIZED, this, new OnSaveGameObjectEventArgs { GameObject = this.gameObject });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("Saving...");
        }
    }
}