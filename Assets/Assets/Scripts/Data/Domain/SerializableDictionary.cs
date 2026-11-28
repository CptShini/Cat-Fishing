using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Data.Domain
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new();
        [SerializeField] private List<TValue> values = new();

        public SerializableDictionary() { }
        
        public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }
        
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
                throw new DataMisalignedException($"Tried to deserialize a SerializableDictionary with {keys.Count} keys and {values.Count} values.");
            
            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }
    }

    public static class SerializableDictionaryExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this SerializableDictionary<TKey, TValue> dictionary) =>
            dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static SerializableDictionary<TKey, TValue> ToSerializableDictionary<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary) =>
            new(dictionary);

        public static Dictionary<Guid, TResult> ToDictionary<TSource, TResult>(
            this SerializableDictionary<string, TSource> dic,
            Func<TSource, TResult> converter
        )
        {
            var dictionary = new Dictionary<Guid, TResult>();

            foreach (string id in dic.Keys)
            {
                Guid key = id.TryParseGuid();
                TResult value = converter(dic[id]);
                dictionary.Add(key, value);
            }
            
            return dictionary;
        }

        public static SerializableDictionary<string, TResult> ToSerializableDictionary<TSource, TResult>(
            this Dictionary<Guid, TSource> guidDic,
            Func<TSource,TResult> converter
        )
        {
            var serializableDictionary = new SerializableDictionary<string, TResult>();
            
            foreach (Guid id in guidDic.Keys)
            {
                string key = id.ToString();
                TResult value = converter(guidDic[id]);
                serializableDictionary.Add(key, value);
            }
            
            return serializableDictionary;
        }
    }
}