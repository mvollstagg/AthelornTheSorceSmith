using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    [System.Serializable]
    public class ExperienceLevel
    {
        [SerializeField] private int level;
        public int Level => level;

        public int requiredExperience;

        public ExperienceLevel(int level, int requiredExperience)
        {
            this.level = Mathf.Max(level, 1); // Ensure level is always 1 or greater
            this.requiredExperience = requiredExperience;
        }
    }
}