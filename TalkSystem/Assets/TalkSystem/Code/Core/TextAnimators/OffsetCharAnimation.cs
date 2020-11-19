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

namespace uTalk
{
    public class OffsetCharAnimation : TextAnimationBase
    {
        private List<TextControl.CharQuad> _startPos;
        private Vector2 _offsetToMoveBack;

        public OffsetCharAnimation()
        {
            _startPos = new List<TextControl.CharQuad>();
        }

        public override void Init(TextControl textControl, TextPage page)
        {
            base.Init(textControl, page);

            SetCharsStartPositions(textControl, page);

            SetOffset(textControl, page);
        }

        private void SetCharsStartPositions(TextControl textControl, TextPage page)
        {
            _startPos.Clear();

            for (int i = 0; i < page.Text.Length; i++)
            {
                _startPos.Add(textControl.GetCharPos(i));
            }
        }

        private void SetOffset(TextControl textControl, TextPage page)
        {
            var dir = GetStartOffsetDir(page.CharByCharInfo.OffsetType, page.CharByCharInfo.Offset);
            _offsetToMoveBack = -dir;

            //i have to find a way to offset the chars individually and not moving the entire rectTransform.
            textControl.OffsetText(dir);
        }

        public override void Update()
        {
            for (int i = 0; i < CharsToAnimateCount; i++)
            {
                var index = GetValidCharToAnimate(i);

                var targetPos = TextControl.OffsetVectors(_startPos[index], _offsetToMoveBack);

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

        public override void OnExitPage()
        {
            TextControl.OffsetText(_offsetToMoveBack);
        }
    }
}