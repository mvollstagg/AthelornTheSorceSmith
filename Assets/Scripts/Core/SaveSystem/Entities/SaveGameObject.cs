using UnityEngine;
using System.Collections.Generic;
using Scripts.Core.SaveSystem.Entities;
using Scripts.Entities.Class;
using System.Reflection;
using System;
using Newtonsoft.Json;

namespace Scripts.Core.SaveSystem.Entities
{
    public class SaveGameObject : MonoBehaviour, ISaveGameObject
    {
        // This will hold the data about what components and fields/properties to save
        public List<ComponentData> componentsToSave = new List<ComponentData>();

        public Dictionary<string, object> CollectData()
        {
            var data = new Dictionary<string, object>();
            foreach (var compData in componentsToSave)
            {
                if (compData.saveComponent)
                {
                    Component comp = GetComponent(compData.componentName);
                    if (comp != null)
                    {
                        var compDict = new Dictionary<string, object>();
                        foreach (var field in compData.fieldsToSave)
                        {
                            var fieldValue = comp.GetType().GetField(field).GetValue(comp);
                            compDict.Add(field, fieldValue);
                        }
                        foreach (var property in compData.propertiesToSave)
                        {
                            var propertyValue = comp.GetType().GetProperty(property).GetValue(comp, null);
                            compDict.Add(property, propertyValue);
                        }
                        data.Add(compData.componentName, compDict);
                    }
                }
            }
            return data;
        }

        public void Register()
        {
            // Assuming EventManager and GameEvents are properly defined elsewhere
            EventManager.Instance.Trigger(GameEvents.ON_SAVEGAMEOBJECT_INITIALIZED, this, new OnSaveGameObjectEventArgs { GameObject = this.gameObject });
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("Collecting data...");
                var collectedData = CollectData();

                var settings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    Formatting = Newtonsoft.Json.Formatting.Indented
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(collectedData, settings);
                PlayerPrefs.SetString(gameObject.name + "_SaveData", json);
                PlayerPrefs.Save();
                Debug.Log("Data saved to PlayerPrefs.");
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                Debug.Log("Loading data...");
                string savedData = PlayerPrefs.GetString(gameObject.name + "_SaveData", null);
                if (!string.IsNullOrEmpty(savedData))
                {
                    // Use Json.NET to deserialize the data
                    var collectedData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(savedData);
                    ApplyData(collectedData);
                    Debug.Log("Data loaded from PlayerPrefs.");
                }
            }


            void ApplyData(Dictionary<string, Dictionary<string, object>> collectedData)
            {
                foreach (var compData in collectedData)
                {
                    // Use reflection to get the type of the component based on the string name
                    System.Type compType = GetTypeFromAllAssemblies(compData.Key);
                    if (compType != null)
                    {
                        Component comp = this.gameObject.GetComponent(compType);
                        if (comp != null)
                        {
                            foreach (var fieldData in compData.Value)
                            {
                                // Use reflection to get the field or property by name
                                MemberInfo memberInfo = compType.GetField(fieldData.Key) as MemberInfo ??
                                                        compType.GetProperty(fieldData.Key) as MemberInfo;

                                if (memberInfo != null)
                                {
                                    // Deserialize the JSON data back into the correct type
                                    object value = DeserializeJson(memberInfo, fieldData.Value.ToString());

                                    // Set the value on the component
                                    if (memberInfo.MemberType == MemberTypes.Field)
                                    {
                                        ((FieldInfo)memberInfo).SetValue(comp, value);
                                    }
                                    else if (memberInfo.MemberType == MemberTypes.Property)
                                    {
                                        ((PropertyInfo)memberInfo).SetValue(comp, value);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            object DeserializeJson(MemberInfo memberInfo, string json)
            {
                // Determine the type that we're deserializing
                System.Type type = null;
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    type = ((FieldInfo)memberInfo).FieldType;
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    type = ((PropertyInfo)memberInfo).PropertyType;
                }

                // Use Json.NET or another JSON library to deserialize the JSON string into the correct type
                return JsonConvert.DeserializeObject(json, type);
            }

            System.Type GetTypeFromAllAssemblies(string typeName)
            {
                System.Type type = System.Type.GetType(typeName);

                if (type != null)
                    return type;

                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(typeName);
                    if (type != null)
                        return type;
                }

                // Check for Unity built-in components
                type = System.Type.GetType("UnityEngine." + typeName + ", UnityEngine");
                return type;
            }

        }
    }

}