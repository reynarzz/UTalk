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

namespace uTalk
{
    /// <summary>Container for all the talks inside the game.</summary>
    [CreateAssetMenu]
    public class TalkDataContainerScriptable : ScriptableObject
    {
        [SerializeField] private TalkDataContainer _talkDataContainer;

        public TalkDataContainer Container => _talkDataContainer;

        [Serializable]
        public class PageNavigatorState
        {
            public string _groupName;
            public string _subgroupName;
            public string _talkName;
        }

        [SerializeField, HideInInspector] private PageNavigatorState _pageState;
        public PageNavigatorState CurrentPageState => _pageState;

        [HideInInspector] public int CategoryIndex;
    }
}
