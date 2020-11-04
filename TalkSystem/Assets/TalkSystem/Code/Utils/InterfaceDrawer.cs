using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace TalkSystem
{
    [CustomPropertyDrawer(typeof(ISerialize))]
    public class InterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = (attribute as ISerialize).InterfaceType;

            var inst = property.FindPropertyRelative("_inst");
        
            EditorGUI.ObjectField(position, inst, type);
        }
    }
}
