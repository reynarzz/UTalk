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
    public class PageNavigator
    {
        private GUIStyle _navigationButtons;

        private PagesFactory _pages;
        private List<IPage> _navigatedPages;
        public IPage _currentPage;

        public PageNavigator(TalkDataContainer dataContainer)
        {
            _pages = new PagesFactory(this, dataContainer);

            _navigatedPages = new List<IPage>();

            _navigationButtons = new GUIStyle(EditorStyles.label);
            _navigationButtons.margin.left = 0;
            _navigationButtons.margin.right = 0;
            _navigationButtons.font = GUI.skin.font;
            _navigationButtons.fontStyle = FontStyle.Bold;
        }

        public void OnGUI()
        {
            NavigatorUI();

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

        public T PushPage<T>() where T: class, IPage
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

                for (int i = _navigatedPages.Count -1; i > index; i--)
                {
                    _navigatedPages.RemoveAt(i);
                }

                //_navigatedPages.Print();
            }
        }
    }
}
