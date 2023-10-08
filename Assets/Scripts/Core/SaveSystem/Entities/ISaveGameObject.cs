using System.Collections.Generic;

namespace Scripts.Core.SaveSystem.Entities
{
    public interface ISaveGameObject
    {
        void Register();
        Dictionary<string, object> CollectData();
    }
}