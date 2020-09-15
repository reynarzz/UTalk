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
    public class RuntimeTextExample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _spaceText;
        [SerializeField] private TalkCloudBase _talkCloud;

        private TalkData _data;

        private void Start()
        {
            var text = "This is the first line.\nThis is the second and final.";

            var page = new TextPage(text,
                new SDictionary<int, Highlight>()
                {
                    { Highlight.GetStartingCharIndex(text, 0), new Highlight(0, "This".Length, Color.blue) },
                    { Highlight.GetStartingCharIndex(text, 3), new Highlight(3, "first".Length, Color.red) },
                    { Highlight.GetStartingCharIndex(text, 8), new Highlight(8, "second".Length, Color.green) }
                });

            _data = new TalkData(new List<TextPage>() { page })
            {
                TalkName = "SpanishTalk",
                Language = Language.Spanish
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!Talker.Inst.IsTalking)
                {
                    Talker.Inst.StartTalk(_talkCloud, _data, Handler);
                }
                else
                {
                    Talker.Inst.NextPage();
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
                    var pageIndex = Talker.Inst.PageIndex;
                    break;
            }
        }
    }
}
