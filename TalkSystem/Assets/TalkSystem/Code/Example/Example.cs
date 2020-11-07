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
    public class Example : MonoBehaviour
    {
        [SerializeField] private TalkCloudBase _talkCloud;

        [SerializeField] private TextMeshProUGUI _spaceText;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                if (!Talk.Inst.IsTalking)
                {
                    Talk.Inst.StartTalk(_talkCloud, new TalkInfo("Default", "SubGroup", "Talk1", Language.English), Handler);
                }
                else
                {
                    var movedToNextPage = Talk.Inst.NextPage();

                    if (!movedToNextPage)
                    {
                        Talk.Inst.SetWriteSpeed(WriteSpeedType.Fast);
                    }
                }
            }
            else if (Talk.Inst.IsTalking && Input.GetKeyUp(KeyCode.Space))
            {
                Talk.Inst.SetWriteSpeed(WriteSpeedType.Normal);
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
}