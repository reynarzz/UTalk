using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace TalkSystem.Editor
{
    public enum SubGroup
    {
        Default, Custom
    }

    public class TalksPage : IPage
    {
        //private struct TalkData
        //{
        //    public string Name { get; set; }
        //    public List<TextPage> Pages { get; set; }
        //}

        private readonly PageNavigator _navigator;
        private GUIStyle _groupButtonStyle;
        private Dictionary<string, List<TalkData>> _talkData;
        public string NavigationName { get; set; }

        private List<string> _subGroupsList;
        private Vector2 _scroll;
        private string _searchText;

        public TalksPage(PageNavigator navigator)
        {
            _navigator = navigator;

            _groupButtonStyle = new GUIStyle(GUI.skin.button);
            _groupButtonStyle.alignment = TextAnchor.MiddleLeft;
            _groupButtonStyle.margin.right = 20;
            _groupButtonStyle.margin.bottom = 10;
            _groupButtonStyle.padding.left = 20;
            _groupButtonStyle.wordWrap = true;

            var talkData = new TalkData() { TalkName = "Calling jhon" };
            var talkData2 = new TalkData() { TalkName = "Second talk" };
            talkData.AddPage(new TextPage("Hello", new SDictionary<int, Highlight>()));
            talkData2.AddPage(new TextPage("Not way", new SDictionary<int, Highlight>()));

            _talkData = new Dictionary<string, List<TalkData>>()
            {
                {
                   "Default", new List<TalkData>()
                   {
                       talkData,
                       talkData2
                       //new TalkData() { TalkName = "Telling jhon i found something" },
                       //new TalkData() { TalkName = "Going to my home" },
                   }
                }//,
                //{
                //   "", new List<TalkData>()
                //   {
                //       new TalkData() { TalkName = "Something random" },
                //       new TalkData() { TalkName = "Starting game" },
                //       new TalkData() { TalkName = "Closing a door" },
                //   }
                //}
            };

            _subGroupsList = new List<string>() { "Default", "Custom" };
        }

        public void OnGUI()
        {
            ToolBar();
            ShowTalks();
        }

        private void ToolBar()
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
                AddTalk();
                return;
            }

            //if (GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.MaxWidth(30)))
            //{
            //    //_deleteGroup = true;
            //}

            GUILayout.EndHorizontal();
        }

        private void AddTalk()
        {
            Context.ShowCreateTalk(TalkEditorWindow.Position, "Talk", _subGroupsList, (sGroup, tName) =>
            {
                if (!_talkData.ContainsKey(sGroup))
                {
                    var talk = new TalkData() { TalkName = tName };
                    talk.CreateEmptyPage();

                    _talkData.Add(sGroup, new List<TalkData>() { talk });

                    _subGroupsList.Add(sGroup);
                }
                else
                {
                    var talk = new TalkData() { TalkName = tName };
                    talk.CreateEmptyPage();

                    _talkData[sGroup].Add(talk);
                }
            });
        }

        private void ShowTalks()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _talkData.Count; i++)
            {
                var key = _talkData.Keys.ElementAt(i);

                var talksOfSubGroup = _talkData[key];

                if (_subGroupsList.Contains(key))
                {
                    GUILayout.Space(5);
                    var color = GUI.color;
                    //GUI.color = Color.green;

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.color = color;

                    GUILayout.Label(key);
                    GUILayout.Space(5);
                }

                for (int j = 0; j < talksOfSubGroup.Count; j++)
                {
                    GUILayout.Space(7);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);

                    if(GUILayout.Button("X", GUILayout.Width(40), GUILayout.MinHeight(40)))
                    {

                    }
                    GUILayout.Space(7);

                    if (GUILayout.Button(talksOfSubGroup[j].TalkName + " | Pages: " + talksOfSubGroup[j].PagesCount, _groupButtonStyle, GUILayout.MinHeight(40)))
                    {
                        var editPage = _navigator.PushPage<EditPageText>();
                        editPage.NavigationName = talksOfSubGroup[j].TalkName;

                        editPage.SetCurrentTalkData(talksOfSubGroup[j]);
                    }
                    GUILayout.EndHorizontal();
                }

                if (_subGroupsList.Contains(key))
                {
                    GUILayout.Space(15);
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();


        }
    }
}
