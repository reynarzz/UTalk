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
using TMPro;

namespace TalkSystem
{
    public class TalkCloud : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public event Action OnCloudShown;
        public event Action OnCloudHidden;

        //Use tween instead for more performance.
        [SerializeField] private Animator _animator;

        private TextControl _textControl;
        public TextControl TextControl => _textControl;

        public void Init()
        {
            _textControl = new TextControl(_text);
        }

        public void ShowCloud()
        {
            _animator.Play("Show");
        }

        public void CloseCloud()
        {
            _animator.Play("Hide");
        }

        //A = Animation
        private void A_OnCloudShown()
        {
            OnCloudShown?.Invoke();
        }

        //A = Animation
        private void A_OnCloudHidden()
        {
            OnCloudHidden?.Invoke();
        }
    }
}