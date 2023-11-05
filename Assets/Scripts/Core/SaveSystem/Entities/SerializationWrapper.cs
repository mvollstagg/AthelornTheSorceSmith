//using System.Collections.Generic;

//namespace Scripts.Core.SaveSystem.Entities
//{
//    [System.Serializable]
//    private class SerializationWrapper<TK, TV>
//    {
//        public List<TK> keys;
//        public List<TV> values;

//        public SerializationWrapper(Dictionary<TK, TV> dict)
//        {
//            keys = new List<TK>(dict.Keys);
//            values = new List<TV>(dict.Values);
//        }

//        public Dictionary<TK, TV> ToDictionary()
//        {
//            var dict = new Dictionary<TK, TV>();
//            for (int i = 0; i < keys.Count; i++)
//            {
//                dict.Add(keys[i], values[i]);
//            }
//            return dict;
//        }
//    }
//}
