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
using TMPro;
using UnityEngine;

namespace TalkSystem
{
    public class Example : MonoBehaviour, ISerializeThis
    {
        [SerializeField] private TalkCloudBase _talkCloud;

        [SerializeField] private TextMeshProUGUI _spaceText;
        
        [ISerialize(typeof(ISerializeThis))]
        public IWrapper<ISerializeThis> _in;

        public string Name => name;

        private void Awake()
        {
            Debug.Log(_in.Interface.Name);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Talk.Inst.Language = Language.English;
                
                if (!Talk.Inst.IsTalking)
                {
                    Talk.Inst.StartTalk(_talkCloud, "Default", "Different", "Saying Hi!", Handler);
                }
                else
                {
                    Talk.Inst.NextPage();
                }
            }
        }

        private void Handler(TalkEvent talkEvent)
        {
            switch (talkEvent)
            {
                case TalkEvent.Started:
                    _spaceText.enabled = false;
                    break;
                case TalkEvent.Finished:
                    _spaceText.enabled = true;
                    break;
                case TalkEvent.PageChanged:
                    var pageIndex = Talk.Inst.PageIndex;
                    break;
            }
        }
    }
    
    [Serializable]
    public class IWrapper<T> where T: class
    {
        [SerializeField] private UnityEngine.Object _inst;
        public T Interface => _inst as T;
    }

    public interface ISerializeThis
    {
        string Name { get; }
    }
}