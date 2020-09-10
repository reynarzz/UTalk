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

namespace Talk
{
    public class TalkSystem : MonoSingleton<TalkSystem>
    {
        [SerializeField] private TalkCloud _talkCloud;
        [SerializeField] private TalkMaster _master;

        //testing
        [SerializeField] private TalkAsset _talkAsset;

        private const int _firstPage = 0;
        private bool _isLastPage;
        private int _currentPage;

        public event Action<string> OnWordEventTriggered;

        public event Action OnTalkBegan;
        public event Action OnTalkEnded;

        public event Action<int> OnPageChanged;

        public int CurrentPage => _currentPage;
        public bool IsLastPage => _isLastPage;

        private bool _canShowNextPage;
        private bool _talkStarted;

        public bool TalkStarted => _talkStarted;

        private IWriter _currentWriter;

        #region Writers
        private CharByCharWriter _charByCharWriter;
        #endregion

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

        public void StartTalk()
        {
            if (!_talkStarted)
            {
                _talkStarted = true;

                _currentPage = default;
                _isLastPage = default;
                _canShowNextPage = default;

                _currentWriter.Clear(_talkCloud.TextControl);

                _talkCloud.ShowCloud();
                OnTalkBegan?.Invoke();
            }
        }

        public void NextPage()
        {
            if (!_isLastPage)
            {
                if (_canShowNextPage)
                {
                    _canShowNextPage = false;

                    _currentPage++;

                    OnPageChanged?.Invoke(_currentPage);

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

        private void OnCloudShown()
        {
            var startingPage = _talkAsset.GetPage(_firstPage);

            _currentWriter.Write(_talkCloud.TextControl, startingPage);
        }

        private void OnCloudHidden()
        {
            _talkStarted = false;
            OnTalkEnded?.Invoke();
        }

        private void OnPageWriten()
        {
            _isLastPage = _currentPage + 1 == _talkAsset.PagesCount;

            _canShowNextPage = true;
        }
    }
}