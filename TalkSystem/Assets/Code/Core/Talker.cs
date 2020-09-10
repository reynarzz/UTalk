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

namespace TalkSystem
{
    public enum TalkEvent
    {
        Started,
        Finished,
        PageChanged
    }

    /// <summary>Entry point to control the talk system.</summary>
    public class Talker : MonoSingleton<Talker>
    {
        [SerializeField] private TalkMaster _master;
        [SerializeField] private TalkCloud _talkCloud;

        private TalkAsset _talkAsset;

        private const int _firstPage = 0;
        private bool _isLastPage;
        private int _currentPage;

        public int CurrentPage => _currentPage;
        public bool IsLastPage => _isLastPage;

        private bool _canShowNextPage;
        private bool _isTalking;

        private Action<TalkEvent> _talkCallback;
        private Action<string> _onWordEventCallBack;

        public bool IsTalking => _isTalking;

        private IWriter _currentWriter;

        #region Writers
        private CharByCharWriter _charByCharWriter;
        #endregion

        /// <summary>Sets the target language (English default). If the player change it when the text is being displayed, all the text will be updated to the selected language.</summary>
        public Language Language
        {
            get => _master.Language;
            set
            {
                _master.Language = value;

                if (_talkAsset)
                {
                    _currentWriter.OnLanguageChanged(_talkAsset.GetPage(_currentPage));
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _talkCloud.Init();

            _charByCharWriter = new CharByCharWriter(this);

            _charByCharWriter.OnPageWriten += OnPageWriten;

            _currentWriter = _charByCharWriter;

            _talkCloud.OnCloudShown += OnCloudShown;
            _talkCloud.OnCloudHidden += OnCloudHidden;
        }

        public void StartTalk(string talkName)
        {
            if (!_isTalking)
            {
                _talkAsset = _master.GetTalkAsset(talkName);

                if (_talkAsset)
                {
                    _isTalking = true;

                    _currentPage = default;
                    _isLastPage = default;
                    _canShowNextPage = default;

                    _currentWriter.Clear(_talkCloud.TextControl);

                    _talkCloud.ShowCloud();
                    _talkCallback?.Invoke(TalkEvent.Started);
                }
                else
                {
                    Debug.LogError($"Talk Asset \"{talkName}\" wasn't found");
                }
            }
        }

        public void StartTalk(string talkName, Action<TalkEvent> talkCallback)
        {
            _talkCallback = talkCallback;

            StartTalk(talkName);
        }

        public void StartTalk(string talkName, Action<TalkEvent> talkCallback, Action<string> wordEventCallback)
        {
            _onWordEventCallBack = wordEventCallback;

            StartTalk(talkName, talkCallback);
        }

        public void StartTalk(string talkName, Action<string> wordEventCallback)
        {
            _onWordEventCallBack = wordEventCallback;

            StartTalk(talkName);
        }

        public void NextPage()
        {
            if (_isTalking)
            {
                if (!_isLastPage)
                {
                    if (_canShowNextPage)
                    {
                        _canShowNextPage = false;

                        _currentPage++;

                        _talkCallback?.Invoke(TalkEvent.PageChanged);

                        _currentWriter.Write(_talkCloud.TextControl, _talkAsset.GetPage(_currentPage));
                    }
                    else
                    {
                        //Do something random.
                    }
                }
                else
                {
                    _currentWriter.Clear(_talkCloud.TextControl);

                    _talkCloud.CloseCloud();
                }
            }
            else
            {
                Debug.LogError("Trying to advance to a next page without starting the conversation.");
            }
        }

        private void OnCloudShown()
        {
            var startingPage = _talkAsset.GetPage(_firstPage);

            _currentWriter.Write(_talkCloud.TextControl, startingPage);
        }

        private void OnCloudHidden()
        {
            _isTalking = false;
            _talkAsset = null;

            _talkCallback?.Invoke(TalkEvent.Finished);

            _talkCallback = null;
        }

        private void OnPageWriten()
        {
            _isLastPage = _currentPage + 1 == _talkAsset.PagesCount;

            _canShowNextPage = true;
        }
    }
}