using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    public class CharacterStat
    {
        public string name;
        public int maxValue;
        public int currentValue;

        public CharacterStat(string name, int currentValue, int maxValue)
        {
            this.name = name;
            this.currentValue = currentValue;
            this.maxValue = maxValue;
        }
    }
}