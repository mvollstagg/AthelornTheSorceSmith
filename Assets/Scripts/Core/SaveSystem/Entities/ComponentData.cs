// You can define a serializable class to hold the data about the components
using System.Collections.Generic;

[System.Serializable]
public class ComponentData
{
    public string componentName;
    public List<string> fieldsToSave = new List<string>();
    public List<string> propertiesToSave = new List<string>(); // Add this line
    public bool saveComponent = false; // This will be set from the editor
}