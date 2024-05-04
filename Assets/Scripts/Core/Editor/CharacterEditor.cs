using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CustomEditor(typeof(Character))]
    [CanEditMultipleObjects]
    public class CharacterEditor : Editor
    {
        private CharacterDataSO _characterData;
        private LevelDataSO _levelData;
        private GUIStyle labelStyle;

        private void InitializeGUIStyles()
        {
            labelStyle = new GUIStyle(EditorStyles.label);
            _characterData = Character.Instance.GetCharacterData();
            _levelData = Character.Instance.GetLevelData();
            // TODO: Add custom styles here
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            DrawStatsGUI();
        }
        
        private void DrawStatsGUI()
        {
            InitializeGUIStyles();
            EditorGUILayout.LabelField("Derrived Stats", EditorStyles.boldLabel);
            DrawProgressBar("Health", (int)_characterData.currentHealth, (int)_characterData.maxHealth, Color.red);
            DrawProgressBar("Mana", (int)_characterData.currentMana, (int)_characterData.maxMana, Color.blue);
            DrawProgressBar("Stamina", (int)_characterData.currentStamina, (int)_characterData.maxStamina,
                new Color(1f, 0.7f, 0f));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Physical Damage: " + _characterData.physicalDamage, labelStyle);
            EditorGUILayout.LabelField("Physical Defence: " + _characterData.physicalDefence, labelStyle);
            EditorGUILayout.LabelField("Magical Damage: " + _characterData.magicalDamage, labelStyle);
            EditorGUILayout.LabelField("Magical Defence: " + _characterData.magicalDefence, labelStyle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Critical Chance: " + _characterData.criticalStrikeChance + "%", labelStyle);
            EditorGUILayout.LabelField("Critical Damage: " + _characterData.criticalStrikeDamage + "%", labelStyle);
            EditorGUILayout.LabelField("Accuracy: " + _characterData.accuracy, labelStyle);
            EditorGUILayout.LabelField("Dodge Chance: " + _characterData.dodgeChance + "%", labelStyle);
            EditorGUILayout.LabelField("Block Chance: " + _characterData.blockChance + "%", labelStyle);
            EditorGUILayout.LabelField("Influence Bonus: " + _characterData.influenceBonus + "%", labelStyle);
            EditorGUILayout.LabelField("Negotiation Bonus: " + _characterData.negotiationBonus + "%", labelStyle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Fire Resistance: " + _characterData.fireResistance + "%", labelStyle);
            EditorGUILayout.LabelField("Water Resistance: " + _characterData.waterResistance + "%", labelStyle);
            EditorGUILayout.LabelField("Earth Resistance: " + _characterData.earthResistance + "%", labelStyle);
            EditorGUILayout.LabelField("Air Resistance: " + _characterData.airResistance + "%", labelStyle);
            EditorGUILayout.LabelField("Poison Resistance: " + _characterData.poisonResistance + "%", labelStyle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Strength: " + _characterData.strength, labelStyle);
            EditorGUILayout.LabelField("Dexterity: " + _characterData.dexterity, labelStyle);
            EditorGUILayout.LabelField("Intelligence: " + _characterData.intelligence, labelStyle);
            EditorGUILayout.LabelField("Vitality: " + _characterData.vitality, labelStyle);
            EditorGUILayout.LabelField("Focus: " + _characterData.focus, labelStyle);
            EditorGUILayout.LabelField("Charisma: " + _characterData.charisma, labelStyle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Stat Points: " + _characterData.statPoints, labelStyle);
            EditorGUILayout.LabelField("Level: " + _characterData.level);
            DrawProgressBar("Experience", _characterData.experience, _levelData.GetRequiredExperience(_characterData.level),
                Color.yellow);
        }
        private void DrawProgressBar(string label, int value, int maxValue, Color backgroundColor)
        {
            var progress = (float)value / maxValue;
            var rect = GUILayoutUtility.GetRect(18, 18, "TextField");

            // Set the background color
            EditorGUI.DrawRect(rect, backgroundColor);

            EditorGUI.ProgressBar(rect, progress, label + ": " + value + " / " + maxValue);
        }
        
        private Texture2D MakeTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (var i = 0; i < pixels.Length; ++i) pixels[i] = color;
            var texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}