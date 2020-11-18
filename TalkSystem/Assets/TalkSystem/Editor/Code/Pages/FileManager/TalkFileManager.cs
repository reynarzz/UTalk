using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TalkSystem.Editor
{
    public class TalkFileManager : IPage
    {
        private readonly TalkDataContainer _container;
        public string NavigationName => "File Manager";

        private readonly TalkImporter _importer;

        public TalkFileManager(TalkDataContainer container)
        {
            _importer = new TalkImporter(container);

            _container = container;
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Select Folder"))
            {
                var root = EditorUtility.OpenFolderPanel("Import Talk", "", "");

                _importer.ImportAll(root);
            }

            Visualize();
        }

        private void Visualize()
        {
            if (_importer.ContainsData)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                for (int i = 0; i < _importer.Groups.Count; i++)
                {
                    var group = _importer.Groups[i];
                    var groupName = group.Name;

                    EditorGUILayout.BeginHorizontal();
                    group.Selected = EditorGUILayout.Toggle(group.Selected, GUILayout.Width(20));

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

                    EditorGUILayout.LabelField(groupName);
                    EditorGUILayout.EndHorizontal();

                    for (int j = 0; j < _importer.Groups[i].SubGroups.Count; j++)
                    {
                        var subGroup = _importer.Groups[i].SubGroups[j];

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(25);
                        
                        subGroup.Selected = EditorGUILayout.Toggle(subGroup.Selected, GUILayout.Width(20));

                        subGroup.Selected = EditorGUILayout.Foldout(subGroup.Selected, subGroup.Name, true);

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
                                GUILayout.Space(45);

                                talk.Selected_Editor = EditorGUILayout.Toggle(talk.Selected_Editor, GUILayout.Width(20));
                                talk.Selected_Editor = EditorGUILayout.Foldout(talk.Selected_Editor, talk.TalkInfo.TalkName, true);

                                if (talk.Selected_Editor)
                                {
                                    _importer.Groups[i].Selected = true;
                                    _importer.Groups[i].SubGroups[j].Selected = true;
                                }

                                EditorGUILayout.EndHorizontal();

                                if (talk.Selected_Editor)
                                {
                                    for (int m = 0; m < talk.PagesCount; m++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(70);

                                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                                        var page = talk.GetPage(m);

                                        EditorGUILayout.LabelField(page.Text);
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.EndHorizontal();


                                    }
                                }
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Import Selected"))
                {

                }
            }
        }

    }
}
