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
    public class TalkGroups : IPage
    {
        private readonly TalkDataContainerScriptable _data;
        private readonly PageNavigator _navigator;

        private string _searchText;
        private GUIStyle _groupButtonStyle;
        private GUIStyle _navigationButtons;
        private GUIStyle _groupGridButtons;

        private int _currentGroup = -1;
        public string NavigationName => "Groups";
        private Language _language;

        private struct TalkGroup
        {
            public string Name { get; set; }
            public int TalkCount { get; set; }
        }

        private List<TalkGroup> _group;
        private GUIContent[] _groupsTextGrid;
        private bool _deleteGroup;

        public TalkGroups(TalkDataContainerScriptable data, PageNavigator navigator)
        {
            _data = data;
            _navigator = navigator;

            _groupGridButtons = new GUIStyle(GUI.skin.button);
            _groupGridButtons.margin.top = 10;
            _groupGridButtons.margin.bottom = 10;
            _groupGridButtons.margin.right = 10;
            _groupGridButtons.margin.left = 10;

            _groupGridButtons.padding.bottom = 25;
            _groupGridButtons.padding.top = 25;

            _group = new List<TalkGroup>();
            _groupsTextGrid = new GUIContent[] { new GUIContent("Default") };
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            _language = (Language)EditorGUILayout.EnumPopup(_language, GUILayout.Width(_language.ToString().Length * 10));

            var search = EditorStyles.toolbarSearchField;

            search.margin.left = 5;
            search.margin.right = 5;
            search.margin.top = 5;
            search.padding.left = 20;

            _searchText = EditorGUILayout.TextField(_searchText, search);

            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.MaxWidth(30)))
            {
                AddGroup();
                return;
            }

            if (GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.MaxWidth(30)))
            {
                _deleteGroup = true;
            }

            GUILayout.EndHorizontal();

            if (_currentGroup < 0)
            {

                Groups();
            }

            TalksPage();
        }


        private void AddGroup()
        {
            Context.ShowCreateGroup(TalkEditorWindow.Position, "Group", x =>
            {
                _group.Add(new TalkGroup() { Name = x });

                var text = _groupsTextGrid.ToList();
                text.Add(new GUIContent(x));

                _groupsTextGrid = text.ToArray();
            });
        }

        private Vector2 _gridScroll;


        private void Groups()
        {
            GUILayout.Space(4);

            _gridScroll = GUILayout.BeginScrollView(_gridScroll);

            _currentGroup = GUILayout.SelectionGrid(_currentGroup, _groupsTextGrid, 2, _groupGridButtons);

            if (_deleteGroup)
            {
                var selectedGroup = 0;

                for (int i = 0; i < Mathf.CeilToInt((float)_groupsTextGrid.Length / 2); i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (_groupsTextGrid.Length > selectedGroup)
                        {
                            if (GUI.Button(new Rect(j * Screen.width / 2, i * 80, 20, 20), "X"))
                            {
                                Debug.Log("Delete: " + selectedGroup);
                            }
                        }

                        selectedGroup++;
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void TalksPage()
        {
            if (_currentGroup > -1)
            {
                _navigator.PushPage<TalksPage>().NavigationName = _groupsTextGrid[_currentGroup].text;

                _currentGroup = -1;
            }
        }
    }
}
