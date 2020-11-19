using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace uTalk.Editor
{
    public static class EditorUndoRedo
    {
        private const int _maxUndo = 30;
        private readonly static TalkDataContainerScriptable _scriptable;

        static EditorUndoRedo()
        {
            _scriptable = Utils.GetTalkScriptable();
        }
         
        public static void GUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if (!e.shift && e.control && e.keyCode == KeyCode.Z)
                {
                    Undo();
                }
                else
                {
                    if ((e.shift && e.keyCode == KeyCode.Z) || (e.control && e.keyCode == KeyCode.Y))
                    {
                        Redo();
                    }
                }
            }
        }

        private static void Undo()
        {
            //Debug.Log("undo");
            //UnityEditor.Undo.PerformUndo();
        } 
          
        private static void Redo()
        {
            //Debug.Log("redo");
            //UnityEditor.Undo.PerformRedo();
        }

        public static void Record()
        {
           // Debug.Log("record");
            var copy = _scriptable.Container.GetDeepCopy();


        }

        private struct UnduableObj
        {
            public void RedoAction(Action redo)
            {
                redo.Invoke();
            }
        }
    }
}
