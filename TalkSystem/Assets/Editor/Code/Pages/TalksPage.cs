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
        public string NavigationName => "TalksPage";

        private List<string> _subGroupsList;
        private Vector2 _scroll;

        public TalksPage(PageNavigator navigator)
        {
            _navigator = navigator;

            _groupButtonStyle = new GUIStyle(GUI.skin.button);
            _groupButtonStyle.alignment = TextAnchor.MiddleLeft;
            _groupButtonStyle.margin.left = 20;
            _groupButtonStyle.margin.right = 20;
            _groupButtonStyle.margin.top = 10;
            _groupButtonStyle.margin.bottom = 10;
            _groupButtonStyle.padding.left = 20;
            _groupButtonStyle.wordWrap = true;

            _talkData = new Dictionary<string, List<TalkData>>()
            {
                {
                   "Neighbor House", new List<TalkData>()
                   {
                       new TalkData() { TalkName = "Calling jhon"  },
                       new TalkData() { TalkName = "Telling jhon i found something" },
                       new TalkData() { TalkName = "Going to my home" },
                   }
                },
                {
                   "", new List<TalkData>()
                   {
                       new TalkData() { TalkName = "Something random" },
                       new TalkData() { TalkName = "Starting game" },
                       new TalkData() { TalkName = "Closing a door" },
                   }
                }
            };

            _subGroupsList = new List<string>() { "Neighbor House" };
        }

        public void OnGUI()
        {
            ShowTalks();
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
                    GUI.color = Color.red;

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.color = color;

                    GUILayout.Label("SubGroup: " + key);
                    GUILayout.Space(5);
                }

                for (int j = 0; j < talksOfSubGroup.Count; j++)
                {
                    if (GUILayout.Button(talksOfSubGroup[j].TalkName + " | Pages: " + talksOfSubGroup[j].PagesCount, _groupButtonStyle, GUILayout.MinHeight(40)))
                    {
                        var editPage = _navigator.PushPage<EditPageText>();


                        //_test = new TalkData();
                        //var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas varius ligula ac dui \nermentum, sed finibus tortor aliquam.ni";

                        //_test.AddPage(new TextPage(text, new SDictionary<int, Highlight> { { 1, new Highlight(1, 1, 3, Color.green) },
                        //                                                       { 8, new Highlight(8, 0, 8, Color.yellow) },
                        //                                                       { 16, new Highlight(16, 0, 6, Color.red) }}));

                        editPage.SetCurrentTalkData(talksOfSubGroup[j]);
                    }
                }

                if (_subGroupsList.Contains(key))
                {
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();


        }
    }
}
