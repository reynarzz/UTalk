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
using UnityEditor;
using UnityEngine;

namespace TalkSystem.Editor
{
    public class PageNavigator
    {
        private GUIStyle _navigationButtons;
        private GUIStyle _pathLabel;
        private TalkDataContainer _dataContainer;
        private PagesFactory _pages;
        private List<IPage> _navigatedPages;
        public IPage _currentPage;
        private StringBuilder _path;

        private TalkDataContainerScriptable.PageNavigatorState _pageNavigatorState;

        public PageNavigator(TalkDataContainer dataContainer, TalkDataContainerScriptable.PageNavigatorState pageState)
        {
            _dataContainer = dataContainer;
            _pageNavigatorState = pageState;

            _pages = new PagesFactory(this, dataContainer);

            _navigatedPages = new List<IPage>();

            _navigationButtons = new GUIStyle(EditorStyles.label);

            _navigationButtons.margin.left = 0;
            _navigationButtons.margin.right = 0;
            _navigationButtons.font = GUI.skin.font;
            _navigationButtons.fontStyle = FontStyle.Bold;
            _path = new StringBuilder();

            _pathLabel = new GUIStyle(EditorStyles.toolbarButton);
            _pathLabel.alignment = TextAnchor.MiddleLeft;
            _pathLabel.richText = true;
            _pathLabel.fontSize = 11;

            LoadPageState();
        }

        public void OnGUI()
        {
            //NavigatorUI();
            ToolBar();

            _currentPage.OnGUI();
        }

        private void NavigatorUI()
        {
            var color = Color.white;
            color.a = 0.1f;

            EditorGUI.DrawRect(new Rect(0, 0, Screen.width, 22), color);

            GUILayout.BeginHorizontal();

            //GUILayout.SelectionGrid(0, new string[] { "" }, 3);

            for (int i = 0; i < _navigatedPages.Count; i++)
            {
                //var rect = GUILayoutUtility.GetRect(new GUIContent(_navigatedPages[i].NavigationName), GUI.skin.button);

                //rect.width = 
                if (GUILayout.Button(/*rect,*/ _navigatedPages[i].NavigationName, _navigationButtons, GUILayout.Width(_navigatedPages[i].NavigationName.Length * 7)))
                {

                    //Debug.Log($"Go to: {_navigatedPages[i].NavigationName}");

                    PopAllNeededToPage(i);

                }

                if (i + 1 < _navigatedPages.Count)
                {
                    _navigationButtons.fontStyle = FontStyle.Bold;

                    GUILayout.Label(">", _navigationButtons, GUILayout.MaxWidth(10));
                }
                else
                {
                    _navigationButtons.fontStyle = FontStyle.Normal;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void ToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            _path.Clear();

            for (int i = 0; i < _navigatedPages.Count; i++)
            {
                _path.Append(_navigatedPages[i].NavigationName);

                if (i + 1 < _navigatedPages.Count)
                    _path.Append("<color=#fff>/</color>");
            }

            if (GUILayout.Button("Back", GUILayout.MaxWidth(60)))
            {
                PopPage();
            }

            GUILayout.Label($"Path: {_path}", _pathLabel);
            GUILayout.EndHorizontal();
        }

        public bool PushTalkPage(string groupName)
        {
            var group = _dataContainer.GetGroup(groupName);

            if (group != null)
            {
                var talksPage = PushPage<TalksPage>();
                talksPage.NavigationName = groupName;
                talksPage.SetGroup(group);

                _pageNavigatorState._groupName = groupName;

                return true;
            }
            else
            {
                _pageNavigatorState._groupName = default;
            }

            return false;
        }

        public void PushEditPage(string subGroupName, string talkName)
        {
            if (!string.IsNullOrEmpty(subGroupName) && !string.IsNullOrEmpty(talkName))
            {
                var subGroup = _dataContainer.GetGroup(_pageNavigatorState._groupName).GetSubGroupSafe(subGroupName);
                 
                if (subGroup != null)
                { 
                    var editPage = PushPage<EditPageText>();
                    editPage.NavigationName = talkName;

                    editPage.SetCurrentTalkData(subGroup.Talks.First(x => x.TalkInfo.TalkName == talkName));

                    _pageNavigatorState._subgroupName = subGroupName;
                    _pageNavigatorState._talkName = talkName;
                }
            }
            else
            {
                _pageNavigatorState._subgroupName = default;
                _pageNavigatorState._talkName = default;
            }
        }

        private T PushPage<T>() where T : class, IPage
        {
            var page = _currentPage = _pages.GetPage<T>();

            _navigatedPages.Add(page);

            return page as T;
        }

        private void PopAllNeededToPage(int index)
        {
            var toPage = _navigatedPages[index];

            if (_currentPage != toPage)
            {
                _currentPage = toPage;

                for (int i = _navigatedPages.Count - 1; i > index; i--)
                {
                    PopPage();
                }
            }
        }

        private void PopPage()
        {
            if (_navigatedPages.Count > 1)
            {
                _navigatedPages.RemoveAt(_navigatedPages.Count - 1);
                _currentPage = _navigatedPages[_navigatedPages.Count - 1];

                _pageNavigatorState._subgroupName = default;
                _pageNavigatorState._talkName = default;

                if (_navigatedPages.Count == 1)
                {
                    _pageNavigatorState._groupName = default;
                }
            }
        }

        private void LoadPageState()
        {
            PushPage<TalkGroupsPage>();

            var pushed = PushTalkPage(_pageNavigatorState._groupName);

            if (pushed)
            {
                PushEditPage(_pageNavigatorState._subgroupName, _pageNavigatorState._talkName);
            }
        }
    }
}
