using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class ISerialize : PropertyAttribute
    {
        private readonly Type _interfaceType;
        public Type InterfaceType => _interfaceType;

        public ISerialize(Type interfaceType)
        {
            _interfaceType = interfaceType;
        }
    }
}
