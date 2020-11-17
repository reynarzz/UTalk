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
    public class TalkGroupsPage : IPage
    {
        private readonly TalkDataContainer _dataContainer;
        private readonly PageNavigator _navigator;

        private string _searchText;
        private GUIStyle _groupButtonStyle;
        private GUIStyle _navigationButtons;
        private GUIStyle _groupGridButtons;

        private int _currentGroup = -1;
        public string NavigationName => "Groups";

        private GUIContent[] _groupsTextGrid;
        private bool _deleteGroup;

        private List<SDictionary<string, TalksGroupData>> _groups;
        private List<GUIContent> _groupsTextGridList;

        private Vector2 _gridScroll;

        public TalkGroupsPage(TalkDataContainer dataContainer, PageNavigator navigator)
        {
            _dataContainer = dataContainer;
            _navigator = navigator;

            _groupGridButtons = new GUIStyle(GUI.skin.button);
            _groupGridButtons.margin.top = 10;
            _groupGridButtons.margin.bottom = 10;
            _groupGridButtons.margin.right = 10;
            _groupGridButtons.margin.left = 10;

            _groupGridButtons.padding.bottom = 25;
            _groupGridButtons.padding.top = 25;

            _groups = new List<SDictionary<string, TalksGroupData>>();
            _groupsTextGridList = new List<GUIContent>();

            SetGroups();
        }

        private void SetGroups()
        {
            _groups.Clear();
            _groupsTextGridList.Clear();

            var groups = _dataContainer.GetGroupsByLanguage(_dataContainer.Language).Groups;
            _groups.Add(groups);

            for (int i = 0; i < groups.Count; i++)
            {
                //Maybe i will add icons or something.
                _groupsTextGridList.Add(new GUIContent(groups.Keys.ElementAt(i)));
            }

            _groupsTextGrid = _groupsTextGridList.ToArray();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            
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

            _deleteGroup = GUILayout.Toggle(_deleteGroup, "x", EditorStyles.toolbarButton, GUILayout.MaxWidth(30));

            var prevLang = _dataContainer.Language;

            _dataContainer.Language = (Language)EditorGUILayout.EnumPopup(_dataContainer.Language, GUILayout.Width(_dataContainer.Language.ToString().Length * 10));

            LanguageSwitchedUpdate(_dataContainer.Language, prevLang);


            GUILayout.EndHorizontal();

            if (_groups.Count > 0)
            {
                if (_currentGroup < 0)
                {

                    Groups();
                }

                TalksPage();
            }
        }

        private void LanguageSwitchedUpdate(Language current, Language compare)
        {
            if (current != compare)
            {
                SetGroups();
            }
        }


        private void AddGroup()
        {
            Context.ShowCreateGroup(TalkEditorWindow.Position, "Group", x =>
            {
                _dataContainer.CreateGroup(x, _dataContainer.Language);

                SetGroups();
            });
        }



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
                                Context.Delete(TalkEditorWindow.Position, "Group", _groupsTextGrid[selectedGroup].text, "Entire group", DeleteGroup);

                                void DeleteGroup()
                                {
                                    _dataContainer.DeleteGroup(_groupsTextGrid[selectedGroup].text, _dataContainer.Language);

                                    _deleteGroup = false;
                                }

                                SetGroups();
                                return;
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
                var pageName = _groupsTextGrid[_currentGroup].text;
                var talkPage = _navigator.PushTalkPage(pageName);

                _deleteGroup = false;
                _currentGroup = -1;
            }
        }
    }
}
