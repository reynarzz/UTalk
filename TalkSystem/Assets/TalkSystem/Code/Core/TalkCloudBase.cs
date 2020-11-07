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
        [SerializeField] private TextMeshProUGUI _talkerNameText;
        [SerializeField] private Image[] _talkerImages;

        protected Image[] TalkerImages => _talkerImages;
        protected TextMeshProUGUI TalkerNameText => _talkerNameText;

        private string _talkerName;
        private TextControl _texControl;

        public TextControl TextControl => _texControl;


        public event Action OnCloudShown;
        public event Action OnCloudHidden;

        private bool _turnOffOnNoSpriteFound;

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
        protected virtual void OnPageChanged(string talkerName, int pageIndex, int maxPages) { }
        //All the extra behaviour should be in another class.
        public void SetPage(TextPage currentPage, int pageIndex)
        {
            ClearSprites();

            for (int i = 0; i < currentPage.Sprites.Count; i++)
            {
                if (i < _talkerImages.Length)
                {
                    bool hasSprite = currentPage.Sprites[i];

                    if (_turnOffOnNoSpriteFound && !hasSprite)
                    {
                        _talkerImages[i].enabled = false;
                    }
                    else
                    {
                        _talkerImages[i].enabled = true;
                    }

                    _talkerImages[i].sprite = currentPage.Sprites[i];
                }
            }

            if (_talkerNameText)
            {
                if (string.IsNullOrEmpty(currentPage.TalkerName))
                {
                    _talkerNameText.enabled = false;
                }
                else
                {
                    _talkerNameText.enabled = true;

                    _talkerNameText.text = currentPage.TalkerName;
                }
            }

            OnPageChanged(currentPage.TalkerName, pageIndex, _pagesCount);
        }

        protected void TurnImageOffWhenNotSpriteIsFound(bool value)
        {
            _turnOffOnNoSpriteFound = value;
        }

        private void ClearSprites()
        {
            if (_talkerImages != null)
            {
                for (int i = 0; i < _talkerImages.Length; i++)
                {
                    _talkerImages[i].sprite = default;
                    _talkerImages[i].enabled = false;
                }
            }
        }

        /// <summary>Triggers the event "OnCloudShown"</summary>
        protected void OnCloudShow()
        {
            OnCloudShown?.Invoke();
        }

        /// <summary>Triggers the event "OnCloudHidden"</summary>
        protected void OnCloudHidde()
        {
            
            OnCloudHidden?.Invoke();
        }

        public abstract void OnShowCloud();
        public abstract void OnCloseCloud();

        /// <summary>Called when it's needed to close the talk cloud.</summary>
        public void CloseCloud()
        {
            _talkerNameText.enabled = false;
            ClearSprites();

            OnCloseCloud();
        }

        private void Clear()
        {
            OnCloudShown = null;
            OnCloudHidden = null;
        }
    }
}
