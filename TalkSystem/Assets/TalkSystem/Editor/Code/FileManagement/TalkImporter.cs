using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem.Editor
{
    public class TalkImporter
    {
        private readonly TalkDataContainer _container;
        private readonly List<Group> _groups;

        public List<Group> Groups => _groups;

        public bool ContainsData => _groups.Count > 0;

        public class Group
        {
            public string Name { get; set; }
            public string Language { get; set; }
            public List<SubGroup> _subGroups;
            public List<SubGroup> SubGroups => _subGroups;
            public bool Selected { get; set; }

            public Group()
            {
                _subGroups = new List<SubGroup>();
            }
        }

        public class SubGroup
        {
            public string Name { get; set; }

            private List<TalkData> _talks;
            public List<TalkData> Talks => _talks;
            public bool Selected { get; set; }

            public SubGroup()
            {
                _talks = new List<TalkData>();
            }
        }

        public TalkImporter(TalkDataContainer scriptable)
        {
            _container = scriptable;
            _groups = new List<Group>();

            JsonUtility.ToJson(false, true);

        }

        //Depth first search here!
        public void ImportAll(string root)
        {
            _groups.Clear();

            if (!string.IsNullOrEmpty(root) && root.EndsWith("Talks"))
            {
                var languages = Directory.GetDirectories(root);

                for (int i = 0; i < languages.Length; i++)
                {
                    var groupsFolders = Directory.GetDirectories(languages[i]);

                    for (int j = 0; j < groupsFolders.Length; j++)
                    {
                        LoadGroups(languages[i], groupsFolders[j]);

                        var subGroupsFolders = Directory.GetDirectories(groupsFolders[j]);

                        for (int k = 0; k < subGroupsFolders.Length; k++)
                        {
                            var talksFolders = Directory.GetDirectories(subGroupsFolders[k]);

                            LoadSubGroup(subGroupsFolders[k], j);

                            for (int m = 0; m < talksFolders.Length; m++)
                            {
                                LoadTalk(talksFolders[m], j, k);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Select \"Talks\" root folder!");
            }
        }

        private void LoadGroups(string languagePath, string groupPath)
        {
            var languageName = new DirectoryInfo(languagePath).Name;
            var groupName = new DirectoryInfo(groupPath).Name;

            var group = new Group() { Language = languageName, Name = groupName };

            //Debug.Log(languageName + ", " + groupName);

            _groups.Add(group);
        }

        private void LoadSubGroup(string path, int groupIndex)
        {
            var subGroupName = new DirectoryInfo(path).Name;

            var subGroup = new SubGroup();
            subGroup.Name = subGroupName;

            _groups[groupIndex].SubGroups.Add(subGroup);
        }

        private void LoadTalk(string path, int groupIndex, int subGroupIndex)
        {
            //if (!path.EndsWith(".meta"))
            {
                var talkName = new DirectoryInfo(path).Name;
                var group = _groups[groupIndex];

                var subGroups = group.SubGroups[subGroupIndex];

                var talkData = new TalkData(new TalkInfo(group.Name, subGroups.Name, talkName, Language.English));

                var pages = Directory.GetFiles(path);

                for (int i = 0; i < pages.Length; i++)
                {
                    var page = File.ReadAllText(pages[i]);

                    talkData.AddPage(new TextPage(page, new SDictionary<int, Highlight>()));
                }

                subGroups.Talks.Add(talkData);
            }
        }
    }
}
