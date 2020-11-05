//MIT License

//Copyright (c) 2020 Reynardo Perez(Reynarz)

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
using UnityEditor;
using UnityEngine;

namespace TalkSystem
{
    public class OffsetCharAnimation : TextAnimationBase
    {
        private List<TextControl.CharQuad> _startPos;
        private bool _setOffset;

        public OffsetCharAnimation() : base()
        {
            _startPos = new List<TextControl.CharQuad>();
        }

        public override void Init(TextControl textControl, TextPage page)
        {
            base.Init(textControl, page);

            _startPos.Clear();

            SetCharsStartPositions(textControl, page);

            SetOffset(textControl, page);
        }

        private void SetCharsStartPositions(TextControl textControl, TextPage page)
        {
            for (int i = 0; i < page.Text.Length; i++)
            {
                _startPos.Add(textControl.GetCharPos(i));
            }
        }

        private void SetOffset(TextControl textControl, TextPage page)
        {
            if (!_setOffset)
            {
                _setOffset = true;

                var dir = GetStartOffsetDir(page.CharByCharInfo.Animation, page.CharByCharInfo.Offset);
                textControl.OffsetText(dir);
            }
        }

        public override void Update()
        {
            for (int i = 0; i < CharIndexesToAnimate.Count; i++)
            {
                var index = CharIndexesToAnimate[i];
                var targetPos = TextControl.OffsetVectors(_startPos[index], new Vector2(0, -10));

                TextControl.SetCharPos(index, TextControl.LerpCharPos(TextControl.GetCharPos(index), targetPos, 30 * Time.deltaTime));
            }
        }

        public Vector2 GetStartOffsetDir(CharByCharInfo.OffsetStartPos animType, float offset)
        {
            switch (animType)
            {
                case CharByCharInfo.OffsetStartPos.Top:
                    return new Vector2(0, offset);

                case CharByCharInfo.OffsetStartPos.TopLeft:
                    return new Vector2(-offset, offset);

                case CharByCharInfo.OffsetStartPos.TopRight:
                    return new Vector2(offset, offset);

                case CharByCharInfo.OffsetStartPos.Bottom:
                    return new Vector2(0, -offset);

                case CharByCharInfo.OffsetStartPos.BottomLeft:
                    return new Vector2(-offset, -offset);

                case CharByCharInfo.OffsetStartPos.BottomRight:
                    return new Vector2(offset, -offset);
            }

            return default;
        }


        //private IEnumerator WriteByChar(TextControl control, TextPage page)
        //{
        //    _highlightedChars.Clear();
        //    _startPos.Clear();

        //    //Starts showing chars in the next frame.
        //    yield return 0;

        //    _textControl = control;
        //    _textControl.ReloadCharsVertices();

        //    for (int i = 0; i < page.Text.Length; i++)
        //    {
        //        _startPos.Add(control.GetCharPos(i));
        //    }

        //    yield return 0;

        //    var wordIndex = 0;
        //    var whiteSpacesCount = -1;
        //    var highlightedWord = -1;

        //    for (int i = 0; i < page.Text.Length; i++)
        //    {
        //        if (highlightedWord != wordIndex && page.Highlight.ContainsKey(wordIndex))
        //        {
        //            highlightedWord = wordIndex;

        //            var highlight = page.Highlight[wordIndex];
        //            var highlightLength = highlight.HighlighLength;
        //            var startChar = highlight.StartLocalChar;

        //            i += startChar;

        //            if (highlight.Type == HighlightAnimation.Sine)
        //            {
        //                var list = new List<int>();

        //                for (int j = 0; j < highlightLength; j++)
        //                {
        //                    list.Add(i + j);
        //                }

        //                _highlightedChars.Add(list.ToList());
        //            }

        //            for (int j = 0; j < highlightLength; j++)
        //            {
        //                control.ShowChar(i + j, highlight.Color);

        //                _charsToMove.Add(i + j);
        //                yield return _writeSpeed;
        //            }

        //            var target = i + highlightLength;

        //            //When is in a highllighted word, but not all chars were chosen to be highligted.
        //            while (page.Text.ElementAtOrDefault(i).IsValidChar())
        //            {
        //                if (i >= target)
        //                {
        //                    control.ShowChar(i);

        //                    _charsToMove.Add(i);

        //                    yield return _writeSpeed;
        //                }

        //                i++;

        //                whiteSpacesCount = 0;
        //            }
        //        }
        //        else
        //        {
        //            //When is not in a highlighted word.
        //            while (page.Text.ElementAtOrDefault(i).IsValidChar())
        //            {
        //                control.ShowChar(i);

        //                _charsToMove.Add(i);

        //                yield return _writeSpeed;
        //                i++;

        //                whiteSpacesCount = 0;
        //            }
        //        }

        //        whiteSpacesCount++;

        //        if (whiteSpacesCount == 1)
        //        {
        //            wordIndex++;
        //        }
        //    }

        //    OnPageWriten?.Invoke();
        //}
    }
}