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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace uTalk
{
    public class CharByCharWriter : WriterBase
    {
        public override event Action OnPageWritten;
        private bool _hightlightWriteSpeed;

        public CharByCharWriter(MonoBehaviour mono, TextAnimationControl animationControl) : base(mono, animationControl) { }

        protected override IEnumerator Write(TextControl control, TextPage page, TextAnimationControl animationControl)
        {
            var wordIndex = 0;
            var whiteSpacesCount = -1;
            var highlightedWord = -1;

            for (int i = 0; i < page.Text.Length; i++)
            {
                if (highlightedWord != wordIndex && page.Highlight.ContainsKey(wordIndex))
                {
                    highlightedWord = wordIndex;

                    var highlight = page.Highlight[wordIndex];
                    var highlightLength = highlight.HighlighLength;
                    var startChar = highlight.StartLocalChar;


                    var prevWriteSpeed = WriteSpeedType;

                    //Set default color to chars not colored by the highlight color.
                    for (int j = 0; j < startChar; j++)
                    {
                        var charIndex = i + j;
                     
                        control.ShowChar(charIndex);
                    }

                    i += startChar;

                    for (int j = 0; j < highlightLength; j++)
                    {
                        var charIndex = i + j;

                        control.ShowChar(charIndex, highlight.Color);
                        animationControl.HighlightedChar(charIndex, highlight);

                        yield return WriteSpeed;

                        //if (highlight.WriteSpeedType == Highlight.HighlightWriteSpeed.Default || WriteSpeedType == WriteSpeedType.Fast)
                        //{
                        //    yield return WriteSpeed;
                        //    prevWriteSpeed = WriteSpeedType;

                        //    continue;
                        //}
                        //else if (!_hightlightWriteSpeed)
                        //{
                        //    _hightlightWriteSpeed = true;

                        //    WriteSpeed = new WaitForSeconds(highlight.NormalWriteSpeed);
                        //}

                        //if (highlight.WriteSpeedType == Highlight.HighlightWriteSpeed.Custom)
                        //{
                        //    yield return WriteSpeed;
                        //}
                    }

                    SetWriteTypeSpeed(prevWriteSpeed);
                    _hightlightWriteSpeed = false;

                    var target = i + highlightLength;

                    //When it could be possible to write normal chars in a highlighted word
                    while (page.Text.ElementAtOrDefault(i).IsValidChar())
                    {
                        if (i >= target)
                        {
                            control.ShowChar(i);
                            animationControl.NormalChar(i);

                            if(WriteSpeedType == WriteSpeedType.Normal || WriteSpeedType == WriteSpeedType.Fast && page.CharByCharInfo.FastWriteSpeed > 0)
                            yield return WriteSpeed;
                        }

                        i++;

                        whiteSpacesCount = 0;
                    }
                }
                else
                {
                    //When is not in highlight
                    while (page.Text.ElementAtOrDefault(i).IsValidChar())
                    {
                        control.ShowChar(i);
                        animationControl.NormalChar(i);

                        if (WriteSpeedType == WriteSpeedType.Normal || WriteSpeedType == WriteSpeedType.Fast && page.CharByCharInfo.FastWriteSpeed > 0)
                        {
                            yield return WriteSpeed;
                        }

                        i++;
                    }

                    //Only if the next char is the start of a word.
                    if (page.Text.ElementAtOrDefault(i + 1).IsValidChar())
                        whiteSpacesCount = 0;
                }

                whiteSpacesCount++;

                if (whiteSpacesCount == 1)
                {
                    wordIndex++;
                }
            }

            OnPageWritten?.Invoke();
        }
    }
}