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
        private GUIStyle _groupButtonStyle;
        private Dictionary<string, List<TalkData>> _talkData;

        private struct TalkData
        {
            public string Name { get; set; }
            public int Pages { get; set; }
        }

        private List<string> _subGroups;
        private Vector2 _scroll;

        public TalksPage()
        {
            _groupButtonStyle = new GUIStyle(GUI.skin.button);
            _groupButtonStyle.alignment = TextAnchor.MiddleLeft;
            _groupButtonStyle.margin.left = 20;
            _groupButtonStyle.margin.right = 20;
            _groupButtonStyle.margin.top = 10;
            _groupButtonStyle.margin.bottom = 10;
            _groupButtonStyle.padding.left = 20;

            _talkData = new Dictionary<string, List<TalkData>>()
            {
                {
                   "Neighbor House", new List<TalkData>() 
                   {
                       new TalkData(){ Name = "Calling jhon", Pages = 3 },
                       new TalkData(){ Name = "Telling jhon i found something", Pages = 12 },
                       new TalkData(){ Name = "Going to my home", Pages = 5},
                   }
                },
                {
                   "", new List<TalkData>()
                   {
                       new TalkData(){ Name = "Something random", Pages = 2 },
                       new TalkData(){ Name = "Starting game", Pages = 1 },
                       new TalkData(){ Name = "Closing a door", Pages = 1},
                   }
                }
            };

            _subGroups = new List<string>() { "Neighbor House" };
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

                if (_subGroups.Contains(key))
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
                    GUILayout.Button(talksOfSubGroup[j].Name + " | Pages: " + talksOfSubGroup[j].Pages, _groupButtonStyle, GUILayout.MinHeight(40));
                }

                if (_subGroups.Contains(key))
                {
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();


        }
    }
}
