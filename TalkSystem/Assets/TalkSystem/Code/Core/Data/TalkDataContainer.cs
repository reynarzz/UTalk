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
using UnityEngine;
using UnityEngine.UIElements;

namespace TalkSystem
{
    [Serializable]
    public class TalkGroupsByNameData
    {
        [SerializeField] private SDictionary<string, TalksGroupData> _groups;
        public SDictionary<string, TalksGroupData> Groups => _groups;

        public TalkGroupsByNameData()
        {
            _groups = new SDictionary<string, TalksGroupData>();
        }

        public TalkGroupsByNameData GetDeepCopy()
        {
            var copy = MemberwiseClone() as TalkGroupsByNameData;

            copy._groups = new SDictionary<string, TalksGroupData>();

            for (int i = 0; i < _groups.Count; i++)
            {
                var key = _groups.ElementAt(i).Key;

                copy._groups.Add(key, _groups[key].GetDeepCopy());
            }

            return copy;
        }
    }

    [Serializable]
    public class TalksGroupData
    {
        [SerializeField] private string _groupName;
        [SerializeField] private Language _language;
        [SerializeField] private SDictionary<string, TalksSubGroupsData> _talks; //K: SubGroup, V: Talks

        public Language Language => _language;
        public SDictionary<string, TalksSubGroupsData> SubGroups => _talks;
        public string GroupName { get => _groupName; set => _groupName = value; }

        public TalksGroupData(string name, Language language, SDictionary<string, TalksSubGroupsData> talkDictionary)
        {
            _groupName = name;
            _language = language;
            _talks = talkDictionary;
        }

        public TalksGroupData GetDeepCopy()
        {
            var copy = MemberwiseClone() as TalksGroupData;

            copy._talks = new SDictionary<string, TalksSubGroupsData>();

            for (int i = 0; i < _talks.Count; i++)
            {
                var key = _talks.ElementAt(i).Key;

                copy._talks.Add(key, _talks[key].GetDeepCopy());
            }

            return copy;
        }

        [Serializable]
        public class TalksSubGroupsData
        {
            [SerializeField] private List<TalkData> _talks;
            public List<TalkData> Talks => _talks;

            public TalksSubGroupsData()
            {
                _talks = new List<TalkData>();
            }

            public TalksSubGroupsData GetDeepCopy()
            {
                var copy = MemberwiseClone() as TalksSubGroupsData;

                copy._talks = new List<TalkData>();

                for (int i = 0; i < _talks.Count; i++)
                {
                    copy._talks.Add(_talks[i].GetDeepCopy());
                }

                return copy;
            }
        }
    }

    /// <summary>Container for all the talks inside the game.</summary>
    [Serializable]
    public class TalkDataContainer
    {
        [SerializeField, HideInInspector] private Language _language;
        public Language Language { get => _language; set => _language = value; }


        [SerializeField] private SDictionary<Language, TalkGroupsByNameData> _groups;

        public TalkDataContainer()
        {
            _groups = new SDictionary<Language, TalkGroupsByNameData>();
        }

        public TalkDataContainer GetDeepCopy()
        {
            var copy = MemberwiseClone() as TalkDataContainer;

            copy._groups = new SDictionary<Language, TalkGroupsByNameData>();

            for (int i = 0; i < _groups.Count; i++)
            {
                var key = _groups.ElementAt(i).Key;

                copy._groups.Add(key, _groups[key].GetDeepCopy());
            }

            return copy;
        }

        public TalkData GetTalkAsset(TalkInfo talkInfo)
        {
            if (_groups.ContainsKey(_language))
            {
                var groups = _groups[_language].Groups;
                var talkData = default(TalkData);

                if (groups.ContainsKey(talkInfo.GroupName))
                {
                    var talks = groups[talkInfo.GroupName].SubGroups;

                    if (talks.ContainsKey(talkInfo.SubGroupName))
                    {
                        for (int i = 0; i < talks[talkInfo.SubGroupName].Talks.Count; i++)
                        {
                            var talk = talks[talkInfo.SubGroupName].Talks[i];

                            if (talk.TalkInfo.TalkName == talkInfo.TalkName)
                            {
                                talkData = talk;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Sub-Group: " + talkInfo.SubGroupName + " doesn't exist!");

                    }

                }
                else
                {
                    Debug.LogError("Group: " + talkInfo.GroupName + " doesn't exist!");
                }

                if (!talkData)
                {
                    Debug.LogError("Talk: " + talkInfo.TalkName + " doesn't exist!");
                }

                return talkData;
            }

            return null;
        }

        public bool ContainsLanguage(Language language)
        {
            return _groups.ContainsKey(language);
        }



        /// <summary>Returns true if had the subGroup name already</summary>
        /// <returns></returns>
        public bool CreateTalkData(string groupName, string subGroup, string talkName, Language language)
        {
            var subGroupTalks = _groups[language].Groups[groupName].SubGroups;

            var talk = new TalkData(new TalkInfo(groupName, subGroup, talkName, language));
            talk.CreateEmptyPage();

            if (!subGroupTalks.ContainsKey(subGroup))
            {
                var subGroups = new TalksGroupData.TalksSubGroupsData();

                subGroups.Talks.Add(talk);

                subGroupTalks.Add(subGroup, subGroups);
                return false;
            }
            else
            {
                subGroupTalks[subGroup].Talks.Add(talk);
                return true;
            }
        }

        public bool ContainsTalk(string groupName, string subGroup, string talkName, Language language)
        {
            var subGroupTalks = _groups[language].Groups[groupName].SubGroups;

            if (subGroupTalks.ContainsKey(subGroup))
            {
                return subGroupTalks[subGroup].Talks.Exists(x => x.TalkInfo.TalkName == talkName);
            }
            //if (ContainsLanguage(language))
            //{
            //    return _talks[language].Talks.ContainsKey(talkName);
            //}

            return false;
        }

        public void AddTalkData(TalkData talkAsset)
        {
            //if (!_talks.ContainsKey(talkAsset.Language))
            //{
            //    _talks.Add(talkAsset.Language, new TalksGroup(talkAsset.Language, new SDictionary<string, TalkData>()));
            //}

            //var dict = _talks[talkAsset.Language];

            //if (!dict.Talks.ContainsKey(talkAsset.TalkName))
            //{
            //    dict.Talks.Add(talkAsset.TalkName, talkAsset);

            //    Debug.Log("Added");
            //}
            //else
            //{
            //    Debug.Log(talkAsset);
            //    dict.Talks[talkAsset.TalkName] = talkAsset;
            //}
        }

        public void CreateGroup(string groupName, Language language)
        {
            var groupData = new TalksGroupData(groupName, language, new SDictionary<string, TalksGroupData.TalksSubGroupsData>());

            if (_groups.ContainsKey(language))
            {
                _groups[language].Groups.Add(groupName, groupData);
            }
            else
            {
                var talksByNameGroup = new TalkGroupsByNameData();

                talksByNameGroup.Groups.Add(groupName, groupData);

                _groups.Add(language, talksByNameGroup);
            }
        }

        public int GetGroupCount(Language language)
        {
            if (_groups.ContainsKey(language))
            {
                return _groups[language].Groups.Count;
            }
            else
            {
                return 0;
            }
        }

        public void DeleteGroup(string groupName, Language language)
        {
            _groups[language].Groups.Remove(groupName);
        }

        public TalkGroupsByNameData GetGroupByIndex(Language language)
        {
            if (!_groups.ContainsKey(language))
            {
                CreateGroup("Default", language);
            }

            return _groups[language];
        }

        internal void GetTalkAsset(object talkPath)
        {
            throw new NotImplementedException();
        }

        
    }
}
