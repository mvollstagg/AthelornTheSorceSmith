using System.Collections;
using System.Collections.Generic;
using Scripts.Entities.Class;
using UnityEngine;

namespace Scripts.Core.SaveSystem.Entities
{
    public class SaveGameObject : MonoBehaviour, ISaveGameObject
    {
        void Start()
        {
            Register();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Register()
        {
            EventManager.Instance.Trigger(GameEvents.ON_SAVEGAMEOBJECT_INITIALIZED, this, new OnSaveGameObjectEventArgs { GameObject = this.gameObject });
        }
        
        public Dictionary<string, object> CollectData()
        {
            Dictionary<string, object> objectData = new Dictionary<string, object>();

            Component[] components = GetComponents<Component>();
            foreach (var component in components)
            {
                Dictionary<string, object> componentData = new Dictionary<string, object>();
                var fields = component.GetType().GetFields(); // Get all public fields of the component

                foreach (var field in fields)
                {
                    componentData[field.Name] = field.GetValue(component);
                }

                objectData[component.GetType().Name] = componentData;
            }

            return objectData;
        }
    }
}