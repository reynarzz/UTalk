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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TalkSystem
{
    public class MoveCharWrite : IWriter
    {
        private WaitForSeconds _normalSpeed;
        private WaitForSeconds _fastSpeed;

        private WaitForSeconds _writeSpeed;
        private readonly MonoBehaviour _mono;

        private char[] _splitPattern;

        public event Action OnPageWriten;

        private TextControl _textControl;
        private List<List<int>> _highlightedChars;

        private float _speed = 15;
        private float _freq = 10;
        private float _amp = 0.1f;

        public MoveCharWrite(MonoBehaviour mono)
        {
            _mono = mono;
            _highlightedChars = new List<List<int>>();
            _splitPattern = new char[] { ' ', '\n' };
        }

        public void Write(TextControl control, TextPage page)
        {
            _textControl = control;
            control.SetText(page.Text);

            _normalSpeed = new WaitForSeconds(page.CharByCharInfo.NormalWriteSpeed);
            _fastSpeed = new WaitForSeconds(page.CharByCharInfo.FastWriteSpeed);

            _writeSpeed = _normalSpeed;

            _mono.StartCoroutine(WriteByChar(control, page));
        }

        public void Update()
        {
            if (_highlightedChars.Count > 0)
            {

                for (int i = 0; i < _highlightedChars.Count; i++)
                {
                    var unit = 360 / _highlightedChars[i].Count;

                    for (int j = 0; j < _highlightedChars[i].Count; j++)
                    {
                        var angle = unit * (j + 1);
                        //Debug.Log(angle);
                        _textControl.MoveChar(_highlightedChars[i][j], new Vector2(0, Mathf.Sin(j + Time.time * _freq) * _amp));
                    }
                }
            }
            
        }

        //this have to be fix. If the text has more inconsisten whitespaces the highlight will not work.
        private IEnumerator WriteByChar(TextControl control, TextPage page)
        {
            _highlightedChars.Clear();
            //show chars in the next frame.
            yield return 0;

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

                    i += startChar;

                    var list = new List<int>();

                    for (int j = 0; j < highlightLength; j++)
                    {
                        list.Add(i + j);
                    }

                    _highlightedChars.Add(list.ToList());

                    for (int j = 0; j < highlightLength; j++)
                    {
                        control.ShowChar(i + j, highlight.Color);

                        yield return _writeSpeed;
                    }

                    var target = i + highlightLength;

                    //When is could be possible to write normal chars in a highlighted word
                    while (page.Text.ElementAtOrDefault(i).IsValidChar())
                    {
                        if (i >= target)
                        {
                            control.ShowChar(i);
                            yield return _writeSpeed;
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
                        yield return _writeSpeed;
                        i++;

                        whiteSpacesCount = 0;
                    }
                }

                whiteSpacesCount++;

                if (whiteSpacesCount == 1)
                {
                    wordIndex++;
                }
            }

            OnPageWriten?.Invoke();
        }

        public void OnLanguageChanged(TextPage textPage)
        {

        }

        public void Clear(TextControl control)
        {
            control.ClearColors();
        }

    }
}