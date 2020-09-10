using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    [Serializable]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys;
        [SerializeField] private List<TValue> _values;

        public SDictionary()
        {
            _keys = new List<TKey>();
            _values = new List<TValue>();
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < _keys.Count; i++)
            {
                Add(_keys.ElementAtOrDefault(i), _values.ElementAtOrDefault(i));
            }
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            for (int i = 0; i < Keys.Count; i++)
            {
                _keys.Add(Keys.ElementAt(i));
                _values.Add(Values.ElementAt(i));
            }
        }
    }
}