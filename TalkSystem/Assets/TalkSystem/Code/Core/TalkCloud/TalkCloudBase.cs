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
using UnityEngine.UI;

namespace TalkSystem
{
    public abstract class TalkCloudBase : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _text;

        private TextControl _texControl;
        public TextControl TextControl => _texControl;

        public event Action OnCloudShown;
        public event Action OnCloudHidden;

        private int _pagesCount;

        public void Init(int pagesCount)
        {
            _pagesCount = pagesCount;

            if (_texControl == null)
            {
                _texControl = new TextControl(_text);
            }

            Clear();
        }

        /// <summary>Called everytime the page is changed.</summary>
        protected virtual void OnPageSet(string talkerName, int pageIndex, int maxPages) { }
        public abstract void OnShowCloud();

        /// <summary>Called when it's needed to close the talk cloud.</summary>
        public abstract void OnCloseCloud();

        //All the extra behaviour should be in another class.
        public virtual void SetPage(TextPage currentPage, int pageIndex)
        {
            OnPageSet(currentPage.TalkerName, pageIndex, _pagesCount);
        }

        /// <summary>Triggers the event "OnCloudShown" to notify the talk system.</summary>
        protected void OnCloudFullyShown()
        {
            OnCloudShown?.Invoke();
        }

        /// <summary>Triggers the event "OnCloudHidden" to notify the talk system.</summary>
        protected void OnCloudFullyHidden()
        {
            OnCloudHidden?.Invoke();
        }

        public void CloseCloud()
        {
            OnCloseCloud();
        }

        private void Clear()
        {
            OnCloudShown = null;
            OnCloudHidden = null;
        }
    }
}
