//MIT License

//Copyright (c) 2020 Reynardo Perez (Reynarz)

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
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

    public class TalkPage : IPage
    {
        private readonly TalkDataContainer _dataContainer;

        private readonly PageNavigator _navigator;
        private GUIStyle _groupButtonStyle;
        private GUIStyle _centeredButtonLabel;

        private TalksGroupData _talkData;

        public string NavigationName { get; set; }

        private List<string> _subGroupsList;
        private Vector2 _scroll;
        private string _searchText;

        public TalkPage(TalkDataContainer dataContainer, PageNavigator navigator)
        {
            _dataContainer = dataContainer;
            _navigator = navigator;

            _groupButtonStyle = new GUIStyle(GUI.skin.button);
            _groupButtonStyle.alignment = TextAnchor.MiddleLeft;
            _groupButtonStyle.margin.right = 20;
            _groupButtonStyle.margin.bottom = 10;
            _groupButtonStyle.padding.left = 20;
            _groupButtonStyle.wordWrap = true;

            _centeredButtonLabel = new GUIStyle(GUI.skin.button);
            _centeredButtonLabel.alignment = TextAnchor.MiddleCenter;

            var talkData = new TalkData() { TalkName = "Calling jhon" };
            var talkData2 = new TalkData() { TalkName = "Second talk" };
            talkData.AddPage(new TextPage("Hello", new SDictionary<int, Highlight>()));
            talkData2.AddPage(new TextPage("Not way", new SDictionary<int, Highlight>()));

            //_talkData = new Dictionary<string, List<TalkData>>()
            //{
            //    {
            //       "Default", new List<TalkData>()
            //       {
            //           talkData,
            //           talkData2
            //           //new TalkData() { TalkName = "Telling jhon i found something" },
            //           //new TalkData() { TalkName = "Going to my home" },
            //       }
            //    }//,
            //    //{
            //    //   "", new List<TalkData>()
            //    //   {
            //    //       new TalkData() { TalkName = "Something random" },
            //    //       new TalkData() { TalkName = "Starting game" },
            //    //       new TalkData() { TalkName = "Closing a door" },
            //    //   }
            //    //}
            //};

            _subGroupsList = new List<string>() { "Default", "Custom" };
        }

        public void SetGroup(TalksGroupData group)
        {
            _talkData = group;
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
               var hadSubGroup = _dataContainer.CreateTalkData(tName, _talkData.GroupName, sGroup, _talkData.Language);

                if (!hadSubGroup)
                {
                    _subGroupsList.Add(sGroup);
                }
            });
        }

        private void ShowTalks()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _talkData.Talks.Count; i++)
            {
                var key = _talkData.Talks.Keys.ElementAt(i);

                var talksOfSubGroup = _talkData.Talks[key];

                if (_subGroupsList.Contains(key))
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(2);

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("x", _centeredButtonLabel, GUILayout.Width(22), GUILayout.MaxHeight(22)))
                    {
                        Context.Delete(TalkEditorWindow.Position, "Delete", key, "Group and all it's data", DeleteGroup);

                        void DeleteGroup()
                        {
                            _talkData.Talks.Remove(key);
                        }

                        return;
                    }
                    GUILayout.Label(key);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }

                for (int j = 0; j < talksOfSubGroup.Talks.Count; j++)
                {
                    GUILayout.Space(7);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);

                    var talk = talksOfSubGroup.Talks[j];

                    if (GUILayout.Button("X", GUILayout.Width(40), GUILayout.MinHeight(40)))
                    {
                        Context.Delete(TalkEditorWindow.Position, "Delete Talk", talk.TalkName, "Talk", RemoveTalk);

                        void RemoveTalk()
                        {
                            talksOfSubGroup.Talks.RemoveAt(j);
                        }
                        return;
                    }

                    GUILayout.Space(7);

                    if (GUILayout.Button(talk.TalkName + " | Pages: " + talk.PagesCount, _groupButtonStyle, GUILayout.MinHeight(40)))
                    {
                        var editPage = _navigator.PushPage<EditPageText>();
                        editPage.NavigationName = talksOfSubGroup.Talks[j].TalkName;

                        editPage.SetCurrentTalkData(talksOfSubGroup.Talks[j]);
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
