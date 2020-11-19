using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace uTalk
{
    /// <summary>Wraps an interface to serialize a field with the "ISerializeField" attribute.</summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ISerialize<T> where T : class
    {
        [SerializeField] private UnityEngine.Object _instance;
        [SerializeField] private UnityEngine.Object[] _instances;

        public ISerialize()
        {
            //it will not print a caution message in unity.
            _instance = default;
            _instances = default;
        }
         
        private T _implementationInst;
        public T Type
        {
            get
            {
                if (_implementationInst == null)
                {
                    if (!typeof(T).IsArray)
                    {
                        return _implementationInst = _instance as T;
                    }
                    else
                    {
                        Debug.Log(_instances.Length);
                        //Doesn't work
                        return _implementationInst = _instances as T;
                    }
                }
                else
                {
                    return _implementationInst;
                }
            }
        }
    }
}
