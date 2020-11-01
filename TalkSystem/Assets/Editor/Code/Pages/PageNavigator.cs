using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace TalkSystem
{
    public class PageNavigator
    {
        private GUIStyle _navigationButtons;

        private PagesFactory _pages;
        private List<IPage> _navigatedPages;
        public IPage _currentPage;

        public PageNavigator()
        {
            _pages = new PagesFactory(this);

            _navigatedPages = new List<IPage>();

            _navigationButtons = new GUIStyle(GUI.skin.label);
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

            for (int i = 0; i < _navigatedPages.Count; i++)
            {
                var rect = GUILayoutUtility.GetRect(new GUIContent(_navigatedPages[i].NavigationName), GUI.skin.button);

                if (GUI.Button(rect, _navigatedPages[i].NavigationName, _navigationButtons))
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
