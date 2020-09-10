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

namespace TalkSystem
{
    [Serializable]
    public struct TalksByLanguage
    {
        [Serializable]
        public class TalksDictionary : SDictionary<string, TalkAsset> { }

        [SerializeField] private Language _language;
        [SerializeField] private TalksDictionary _talks;

        public Language Language => _language;
        public TalksDictionary Talks => _talks;
    }

    /// <summary>Container for all the talks inside the game.</summary>
    [CreateAssetMenu]
    public class TalkMaster : ScriptableObject
    {
        [SerializeField, HideInInspector] private Language _language;

        public Language Language { get => _language; set => _language = value; }

        [Serializable]
        private class TalksByLanguageDictionary : SDictionary<Language, TalksByLanguage> { }

        [SerializeField] private TalksByLanguageDictionary _talks;

        public TalkAsset GetTalkAsset(string talkName)
        {
            if (ContainsTalk(_language, talkName))
            {
                return _talks[_language].Talks[talkName];
            }

            return null;
        }

        public bool ContainsLanguage(Language language)
        {
            return _talks.ContainsKey(language);
        }

        public bool ContainsTalk(Language language, string talkName)
        {
            if (ContainsLanguage(language))
            {
                return _talks[language].Talks.ContainsKey(talkName);
            }

            return false;
        }
    }
}
