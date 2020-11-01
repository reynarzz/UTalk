using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TalkSystem.Editor
{
    public class Context : EditorWindow
    {
        private GUIStyle _createButton;
        private GUIStyle _cancelButton;
        private static string _title;

        private static Action<string> _onCreated;

        public Context()
        {
           
        }

        private static Rect _constantPos;
        private string _text;

        public static void ShowContext(Rect position, string title, Action<string> onCreated)
        {
            _title = title;
            _onCreated = onCreated;

            var window = GetWindow<Context>();
            
            window.maxSize = new Vector2(250, 100);
            window.minSize = new Vector2(250, 100);

            position.x -= window.minSize.x / 2;
            position.y -= (window.minSize.y / 2) + 20;

            window.position = position;
            window.titleContent = new GUIContent(title);
            //window.Show();
            _constantPos = position;
            window.ShowModal();
            //window.ShowModalUtility();
        }

        private void OnGUI()
        {
            if (_createButton == null)
            {
                _createButton = new GUIStyle(GUI.skin.button);
                _cancelButton = new GUIStyle(GUI.skin.button);

                _createButton.margin.left = 20;
                _createButton.margin.right = 20;

                _cancelButton.margin.left = 20;
                _cancelButton.margin.right = 20;

            }

            position = _constantPos;

            GUILayout.Space(13);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Create " + _title);
            GUILayout.Space(4);

            _text = GUILayout.TextField(_text);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if(!string.IsNullOrEmpty(_text) && GUILayout.Button("Create", _createButton))
            {
                {
                    _onCreated(_text);
                    Close();
                }
                

            }

            if (GUILayout.Button("Cancel", _cancelButton))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}
