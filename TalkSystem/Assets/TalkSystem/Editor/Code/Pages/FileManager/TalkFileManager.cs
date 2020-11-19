using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace uTalk.Editor
{
    public class TalkFileManager : IPage
    {
        private readonly TalkDataContainer _container;
        public string NavigationName => "Importer";

        private readonly TalkImporter _importer;

        public TalkFileManager(TalkDataContainer container)
        {
            _importer = new TalkImporter(container);

            _container = container;
        }
        private Vector2 _scroll;

        public void OnGUI()
        {

            Visualize();


            FooterButtons();
        }

        private void Visualize()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _scroll = GUILayout.BeginScrollView(_scroll);

            if (_importer.ContainsData)
            {
                var closedFolder = EditorGUIUtility.IconContent("d_Project@2x");
                var openFolder = EditorGUIUtility.IconContent("FolderOpened On Icon");

                var folderIcon = closedFolder;

                var fileIcon = EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow");

                for (int i = 0; i < _importer.Groups.Count; i++)
                {
                    var group = _importer.Groups[i];

                    EditorGUILayout.BeginHorizontal();

                    group.Selected = EditorGUILayout.Toggle(group.Selected, GUILayout.Width(12));

                    if (group.Selected)
                    {
                        folderIcon = openFolder;
                    }
                    else
                    {
                        folderIcon = closedFolder;
                    }

                    folderIcon.text = group.Name;

                    if (group.SubGroups.Count > 0)
                    {
                        group.Selected = EditorGUILayout.Foldout(group.Selected, folderIcon, true);
                    }
                    else
                    {
                        folderIcon = closedFolder;
                        folderIcon.text = group.Name;

                        EditorGUILayout.LabelField(folderIcon);
                    }


                    if (!group.Selected)
                    {
                        for (int j = 0; j < group.SubGroups.Count; j++)
                        {
                            var sub = group.SubGroups[j];
                            sub.Selected = false;

                            for (int k = 0; k < sub.Talks.Count; k++)
                            {
                                sub.Talks[k].Selected_Editor = false;
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    for (int j = 0; j < _importer.Groups[i].SubGroups.Count; j++)
                    {
                        var subGroup = _importer.Groups[i].SubGroups[j];

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(30);

                        subGroup.Selected = EditorGUILayout.Toggle(subGroup.Selected, GUILayout.Width(12));

                        if (subGroup.Selected)
                        {
                            folderIcon = openFolder;
                        }
                        else
                        {
                            folderIcon = closedFolder;
                        }

                        folderIcon.text = subGroup.Name;

                        if (subGroup.Talks.Count > 0)
                        {
                            subGroup.Selected = EditorGUILayout.Foldout(subGroup.Selected, folderIcon, true);
                        }
                        else
                        {
                            folderIcon = closedFolder;
                            folderIcon.text = subGroup.Name;

                            EditorGUILayout.LabelField(folderIcon);
                        }


                        if (subGroup.Selected)
                        {
                            _importer.Groups[i].Selected = true;
                        }
                        else
                        {
                            for (int k = 0; k < subGroup.Talks.Count; k++)
                            {
                                subGroup.Talks[k].Selected_Editor = false;
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        if (subGroup.Selected)
                        {
                            for (int k = 0; k < subGroup.Talks.Count; k++)
                            {
                                var talk = subGroup.Talks[k];

                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(60);

                                fileIcon.text = talk.TalkInfo.TalkName;
                                talk.Selected_Editor = EditorGUILayout.Toggle(talk.Selected_Editor, GUILayout.Width(12));
                                EditorGUILayout.LabelField(fileIcon);

                                // talk.Selected_Editor = EditorGUILayout.Foldout(talk.Selected_Editor, fileIcon, true);

                                if (talk.Selected_Editor)
                                {
                                    _importer.Groups[i].Selected = true;
                                    _importer.Groups[i].SubGroups[j].Selected = true;
                                }

                                EditorGUILayout.EndHorizontal();

                                //if (talk.Selected_Editor)
                                //{
                                //    for (int m = 0; m < talk.PagesCount; m++)
                                //    {
                                //        EditorGUILayout.BeginHorizontal();
                                //        GUILayout.Space(80);

                                //        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                                //        var page = talk.GetPage(m);

                                //        EditorGUILayout.LabelField(page.Text);
                                //        EditorGUILayout.EndHorizontal();
                                //        EditorGUILayout.EndHorizontal();


                                //    }
                                //}
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void FooterButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("All", GUILayout.Width(60)))
            {
                AllFileSelection(true);
            }

            if (GUILayout.Button("None", GUILayout.Width(60)))
            {
                AllFileSelection(false);
            }

            GUILayout.ExpandWidth(true);
            GUILayout.Space(20);

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var root = EditorUtility.OpenFolderPanel("Import Talk", "", "");

                _importer.ImportAll(root);
            }

            if (GUILayout.Button("Import", GUILayout.MinWidth(50)))
            {

            }

            GUILayout.EndHorizontal();
        }

        private void AllFileSelection(bool select)
        {
            if (_importer.ContainsData)
            {
                for (int i = 0; i < _importer.Groups.Count; i++)
                {
                    _importer.Groups[i].Selected = select;

                    var sub = _importer.Groups[i].SubGroups;

                    for (int j = 0; j < sub.Count; j++)
                    {
                        sub[j].Selected = select;

                        for (int k = 0; k < sub[j].Talks.Count; k++)
                        {
                            sub[j].Talks[k].Selected_Editor = select;
                        }
                    }
                }
            }
        }
    }
}
