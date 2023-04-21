using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Entities.Class
{
    [System.Serializable]
    public class ItemTrait
    {
        public TraitType Type;
        public TraitStatus Status;
        public int Value;
    }
}