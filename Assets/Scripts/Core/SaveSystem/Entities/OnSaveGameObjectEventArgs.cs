using System;
using Scripts.Entities.Enum;
using UnityEngine;

namespace Scripts.Core.SaveSystem.Entities
{
    public class OnSaveGameObjectEventArgs : EventArgs 
    {
        public GameObject GameObject { get; set; }
    }
}