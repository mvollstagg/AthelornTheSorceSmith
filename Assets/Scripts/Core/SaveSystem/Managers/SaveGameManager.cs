using System.Collections.Generic;
using UnityEngine;
using Scripts.Core.SaveSystem.Entities;
using Scripts.Entities.Class;
using Scripts.Core;

namespace AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem
{
    public class SaveGameManager : Singleton<SaveGameManager>
    {
        private List<KeyValue<string, GameObject>> _saveGameObjects = new List<KeyValue<string, GameObject>>();

        void Awake()
        {
            Debug.Log("SaveGameManager initialized");
            EventManager.Instance.AddListener<OnSaveGameObjectEventArgs>(GameEvents.ON_SAVEGAMEOBJECT_INITIALIZED, OnSaveGameObjectInitialized);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                foreach (var saveGameObject in _saveGameObjects)
                {
                    var data = saveGameObject.Value.GetComponent<ISaveGameObject>().CollectData();
                    Debug.Log("#---------Collected Data----------#");
                    Debug.Log(data.Values);
                }
            }
        }

        private void OnSaveGameObjectInitialized(object sender, OnSaveGameObjectEventArgs e)
        {
            _saveGameObjects.Add(new KeyValue<string, GameObject>
            {
                Key = e.GameObject.name,
                Value = e.GameObject
            });

            Debug.Log(e.GameObject.name + " added to SaveGameManager", e.GameObject);
        }
    }
}