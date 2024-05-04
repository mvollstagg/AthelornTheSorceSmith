using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    [System.Serializable]
    public class CharacterStatUI
    {
        public string name;
        [TextArea(3, 10)]
        public string description;
    }
}