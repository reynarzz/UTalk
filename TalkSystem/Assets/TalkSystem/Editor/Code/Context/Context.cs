﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace uTalk.Editor
{
    public class Context : EditorWindow
    {
        private GUIStyle _createButton;
        private GUIStyle _cancelButton;
        private GUIStyle _centeredLabel;

        private static string _title;

        private static Action<string> _onGroupCreated;
        private static Action<string, string> _onTalkCreated;
        private static Action _onDeleted;
        private static ContextMenuType _type;

        private string _subGroupName;
        private int _subGroupIndex;

        private static string[] _subGroups = { "Default", "Custom" };

        private static Rect _constantPos;
        private string _name;
        private static string _thingToDeleteName;
        private static string _typeNameToDelete;
        private bool _itsNameDuplicated;

        public enum ContextMenuType
        {
            CreateGroup, CreateTalk, Delete
        }


        public static void ShowCreateGroup(Rect position, string title, Action<string> onGroupCreated)
        {
            _title = title;

            _onGroupCreated = onGroupCreated;
            _type = ContextMenuType.CreateGroup;

            Init(position, title);
        }

        public static void ShowCreateTalk(Rect position, string title, List<string> groups, Action<string, string> onTalkCreated)
        {
            _title = title;

            _onTalkCreated = onTalkCreated;
            _type = ContextMenuType.CreateTalk;
            _subGroups = groups.ToArray();

            Init(position, title);
        }

        public static void Delete(Rect position, string title, string thingToDeleteName, string typeName, Action onDeleted)
        {
            _onDeleted = onDeleted;
            _thingToDeleteName = thingToDeleteName;
            _typeNameToDelete = typeName;
            _type = ContextMenuType.Delete;

            Init(position, title);
        }

        private static void Init(Rect position, string title)
        {
            var window = GetWindow<Context>();

            if (_type == ContextMenuType.CreateGroup || _type == ContextMenuType.Delete)
            {
                window.maxSize = new Vector2(250, 100);
                window.minSize = new Vector2(250, 100);
            }
            else if (_type == ContextMenuType.CreateTalk)
            {
                window.maxSize = new Vector2(250, 150);
                window.minSize = new Vector2(250, 150);
            }

            position.x -= window.minSize.x / 2;
            position.y -= (window.minSize.y / 2) + 50;

            window.position = position;
            window.titleContent = new GUIContent(title);

            _constantPos = position;
            window.ShowModalUtility();
        }

        private void OnGUI()
        {
            switch (_type)
            {
                case ContextMenuType.CreateGroup:
                    CreateGroup();
                    break;
                case ContextMenuType.CreateTalk:
                    CreateTalk();
                    break;
                case ContextMenuType.Delete:
                    Delete();
                    break;
            }
        }

        private void CreateGroup()
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

            GUI.SetNextControlName("createName");
            _name = GUILayout.TextField(_name);
            GUILayout.EndVertical();

            GUI.FocusControl("createName");

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            var current = Event.current;

            var enterPressed = current.keyCode == KeyCode.Return;

            if (!string.IsNullOrEmpty(_name) && (GUILayout.Button("Create", _createButton) || enterPressed))
            {
                Close();

                _onGroupCreated(_name);
            }

            if (GUILayout.Button("Cancel", _cancelButton))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void CreateTalk()
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

            GUI.SetNextControlName("createName");
            _name = GUILayout.TextField(_name);
            GUILayout.EndVertical();

            //GUI.FocusControl("createName");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sub Group");

            _subGroupIndex = EditorGUILayout.Popup(_subGroupIndex, _subGroups);
            GUILayout.EndHorizontal();

            if (_subGroupIndex == 1)
            {
                _subGroupName = GUILayout.TextField(_subGroupName);
            }
            else if (_subGroupIndex > 1)
            {
                _subGroupName = _subGroups[_subGroupIndex];
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            var current = Event.current;
            var enterPressed = current.keyCode == KeyCode.Return;

            if (!string.IsNullOrEmpty(_name) && (GUILayout.Button("Create", _createButton) || enterPressed))
            {
                if (_subGroupIndex == 0)
                {
                    _subGroupName = _subGroups[0];
                }
                _onTalkCreated(_subGroupName, _name);
                Close();
            }

            if (GUILayout.Button("Cancel", _cancelButton))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void Delete()
        {
            if (_createButton == null)
            {
                _createButton = new GUIStyle(GUI.skin.button);
                _cancelButton = new GUIStyle(GUI.skin.button);

                _createButton.margin.left = 20;
                _createButton.margin.right = 20;

                _cancelButton.margin.left = 20;
                _cancelButton.margin.right = 20;

                _centeredLabel = new GUIStyle(GUI.skin.label);
                _centeredLabel.alignment = TextAnchor.MiddleCenter;
                _centeredLabel.wordWrap = true;
            }

            position = _constantPos;

            GUILayout.Space(13);

            GUILayout.Label("Delete \"" + _thingToDeleteName + "\" " + _typeNameToDelete + "?", _centeredLabel);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete", _createButton))
            {
                _onDeleted();
                Close();
            }

            if (GUILayout.Button("Cancel", _cancelButton))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}
