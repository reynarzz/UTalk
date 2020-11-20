using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace uTalk.Editor
{
    public class TalkGroupsPage : IPage
    {
        private readonly TalkDataContainer _dataContainer;
        private readonly PageNavigator _navigator;

        private string _searchText;
        private GUIStyle _groupButtonStyle;
        private GUIStyle _navigationButtons;
        private GUIStyle _groupGridButtons;

        public string NavigationName => "Groups";

        private GUIContent[] _groupsTextGrid;
        private bool _deleteGroup;

        private List<SDictionary<string, TalksGroupData>> _groups;
        private List<GUIContent> _groupsTextGridList;

        private Vector2 _groupsScroll;

        private const int _groupsPerRow = 2;
        private List<bool> _deleteToggles;


        private GUIContent _editIcon;

        public TalkGroupsPage(TalkDataContainer dataContainer, PageNavigator navigator)
        {
            _dataContainer = dataContainer;
            _navigator = navigator;

            _groupGridButtons = new GUIStyle(GUI.skin.button);

            _groupGridButtons.normal.background = GetNewTex(Color.clear);
            _groupGridButtons.active.background = GetNewTex(Color.black * 0.3f);

            _groupGridButtons.hover.background = GetNewTex(Color.black * 0.1f);

            _groupGridButtons.wordWrap = true;

            _groupGridButtons.margin.top = 10;
            _groupGridButtons.margin.bottom = 10;
            _groupGridButtons.margin.right = 35;
            _groupGridButtons.margin.left = 35;

            _groupGridButtons.padding.bottom = 10;
            _groupGridButtons.padding.top = 10;

            //_groupGridButtons.padding.left = 1;
            //_groupGridButtons.padding.right = 1;

            //_groupGridButtons.contentOffset = new Vector2(-20, 0);
            _groupGridButtons.imagePosition = ImagePosition.ImageAbove;
            _groupGridButtons.normal.textColor = Color.white;
            _groupGridButtons.fontSize = 10;

            _groups = new List<SDictionary<string, TalksGroupData>>();
            _groupsTextGridList = new List<GUIContent>();
            _deleteToggles = new List<bool>();

            _editIcon = EditorGUIUtility.IconContent("d_editicon.sml");


            SetGroups();
        }

        private Texture2D GetNewTex(Color color)
        {
            var tex = new Texture2D(Texture2D.whiteTexture.width, Texture2D.whiteTexture.height);

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    tex.SetPixel(x, y, color);
                }
            }

            tex.Apply();

            return tex;
        }

        private void SetGroups()
        {
            _groups.Clear();
            _groupsTextGridList.Clear();
            _deleteToggles.Clear();
            _deleteGroup = false;

            var groups = _dataContainer.GetGroupsByLanguage(_dataContainer.Language).Groups;
            _groups.Add(groups);

            var groupWithContentIcon = EditorGUIUtility.IconContent("Folder On Icon");
            var groupNoContentIcon = groupWithContentIcon;// EditorGUIUtility.IconContent("FolderEmpty On Icon");
            //groupNoContentIcon.image.height = 20;
            //groupNoContentIcon.image.width = 20;

            var icon = default(GUIContent);

            for (int i = 0; i < groups.Count; i++)
            {
                icon = groups[groups.Keys.ElementAt(i)].SubGroups.Count > 0 ? groupWithContentIcon : groupNoContentIcon;

                icon.text = groups.Keys.ElementAt(i);

                _groupsTextGridList.Add(new GUIContent(icon));
                _deleteToggles.Add(false);
            }

            _groupsTextGrid = _groupsTextGridList.ToArray();
        }

        public void OnGUI()
        {
            var currentEvent = Event.current;

            if (currentEvent.control && currentEvent.type == EventType.KeyUp)
            {
                SetGroups();
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            var search = EditorStyles.toolbarSearchField;

            search.margin.left = 5;
            search.margin.right = 5;
            search.margin.top = 5;
            search.padding.left = 20;

            _searchText = EditorGUILayout.TextField(_searchText, search);

            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.MaxWidth(30)))
            {
                _deleteGroup = false;

                AddGroup();
                return;
            }

            var prevDelete = _deleteGroup;

            _deleteGroup = GUILayout.Toggle(_deleteGroup, "x", EditorStyles.toolbarButton, GUILayout.MaxWidth(30));

            if (!_deleteGroup && prevDelete)
            {
                RestartDeleteSelection();
            }

            var prevLang = _dataContainer.Language;

            _dataContainer.Language = (Language)EditorGUILayout.EnumPopup(_dataContainer.Language, GUILayout.Width(_dataContainer.Language.ToString().Length * 10));


            //GUI.SetNextControlName("Edit languages");
            if (GUILayout.Button(_editIcon, EditorStyles.toolbarButton, GUILayout.MaxWidth(30)))
            {
                //GUI.FocusControl("Edit languages");
            }

            LanguageSwitchedUpdate(_dataContainer.Language, prevLang);


            GUILayout.EndHorizontal();

            Groups();
        }

        private void RestartDeleteSelection()
        {
            for (int i = 0; i < _deleteToggles.Count; i++)
            {
                _deleteToggles[i] = false;
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
            Context.ShowCreateGroup(UTalkEditorWindow.Position, "Group", x =>
            {
                _dataContainer.CreateGroup(x, _dataContainer.Language);

                SetGroups();
            });
        }

        private void Groups()
        {
            GUILayout.Space(4);

            _groupsScroll = GUILayout.BeginScrollView(_groupsScroll);

            var index = 0;

            for (int i = 0; i < _groupsTextGrid.Length; i++)
            {
                GUILayout.BeginHorizontal();

                for (int j = 0; j < _groupsPerRow; j++)
                {
                    if (GUILayout.Button(_groupsTextGrid[index], _groupGridButtons))
                    {
                        TalksPage(_groupsTextGrid[index].text);
                    }

                    index++;

                    if (index == _groupsTextGrid.Length)
                    {
                        i = _groupsTextGrid.Length;

                        break;
                    }
                }

                GUILayout.EndHorizontal();
            }

            if (_deleteGroup)
            {
                var selectedGroup = 0;

                for (int i = 0; i < Mathf.CeilToInt((float)_groupsTextGrid.Length / 2); i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (_groupsTextGrid.Length > selectedGroup)
                        {
                            _deleteToggles[selectedGroup] = GUI.Toggle(new Rect(15 + j * (Screen.width / 2 - 16), i * 110 + 10, 20, 20), _deleteToggles[selectedGroup], "");

                        }

                        selectedGroup++;
                    }
                }

                if (_deleteToggles.Any(x => x))
                {
                    if (GUI.Button(new Rect(0, Screen.height - 123, Screen.width, 20), $"Delete ({_deleteToggles.Count(x => x)})"))
                    {
                        //Context.Delete(UTalkEditorWindow.Position, "Group", "Selected", "groups", DeleteGroup);

                        //void DeleteGroup()
                        {
                            UTalkEditorWindow.RecordToUndo("Delete group");

                            for (int i = 0; i < _deleteToggles.Count; i++)
                            {
                                if (_deleteToggles[i])
                                {
                                    _dataContainer.DeleteGroup(_groupsTextGrid[i].text, _dataContainer.Language);
                                }
                            }

                            SetGroups();
                            _deleteGroup = false;
                        }

                        return;
                    }
                }
            }


            GUILayout.EndScrollView();
        }

        private void TalksPage(string groupName)
        {
            _navigator.PushTalkPage(groupName);

            RestartDeleteSelection();
            _deleteGroup = false;
        }
    }
}
