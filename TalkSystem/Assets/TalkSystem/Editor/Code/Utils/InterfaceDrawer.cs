using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace TalkSystem
{
    [CustomPropertyDrawer(typeof(ISerializeField))]
    public class InterfaceDrawer : PropertyDrawer
    {
        private bool _foldOut = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetObj = property.serializedObject.targetObject.GetType();

            var field = targetObj.GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                //Debug.Log(property.name);
                field = targetObj.GetField(property.name);
            }
            //Debug.Log(field == null);
            var type = field.FieldType.GetGenericArguments()[0];

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (type.IsArray)
            {
                var inst = property.FindPropertyRelative("_instances");
                if (GUI.Button(position, "Add"))
                {
                    inst.arraySize++;
                }

                indent = 1;
                if (_foldOut = EditorGUI.Foldout(position, _foldOut, label))
                {
                    for (int i = 0; i < inst.arraySize; i++)
                    {
                        var element = inst.GetArrayElementAtIndex(i);
                        position.y += 10 * i;
                     
                        label.text = "Element " + i;

                         element.objectReferenceValue = EditorGUI.ObjectField(position, label, element.objectReferenceValue, type.GetElementType(), true);
                    }
                }
               
                //Debug.Log();
            }
            else
            {
                var inst = property.FindPropertyRelative("_instance");

                inst.objectReferenceValue = EditorGUI.ObjectField(position, label, inst.objectReferenceValue, type, true);
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();



            //var targetObj = property.serializedObject.targetObject.GetType();

            //var field = targetObj.GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic);

            //if(field == null)
            //{
            //    Debug.Log(property.name);
            //    field = targetObj.GetField(property.name);
            //}
            //Debug.Log(field == null);
            //var type = field.FieldType.GetGenericArguments()[0];

            //var inst = property.FindPropertyRelative("_inst");

            //if(_white == null)
            //{
            //    _white = Texture2D.whiteTexture;
            //}

            //label.image = _white;

            //for (int i = 0; i < length; i++)
            //{

            //}
        }
    }
}
