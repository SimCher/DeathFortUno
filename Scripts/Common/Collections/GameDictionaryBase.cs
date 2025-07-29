using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Collections
{
    [System.Serializable]
    public class GameDictionaryBase<TValue> : SerializableDictionary<string, TValue> where TValue : Object
    {
        private TValue GetValue(System.Type type) => this[type.Name];

        public T GetValue<T>() where T : TValue => GetValue(typeof(T)) as T;
    }
}