using System;

namespace Scripts.Entities.Class
{
    public class KeyValue<TKey,TValue>
    {
       public TKey Key {get;set;}
       public TValue Value {get;set;}
    }
}