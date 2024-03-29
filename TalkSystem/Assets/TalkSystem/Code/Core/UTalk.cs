﻿//MIT License

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

namespace uTalk
{
    public enum TalkEvent
    {
        Started,
        Finished,
        PageChanged
    }

    /// <summary>Entry point to control the talk system. put this in your GM object.</summary>
    public class UTalk : MonoBehaviour
    {
        [SerializeField] private TalkDataContainerScriptable _scriptableContainer;

        private Action<string> _onWordEventCallBack;
        private Action<TalkEvent> _talkCallback;

        private WriterControl _writerControl;
        private TalkCloudBase _talkCloud;
        private TextPage _currentPage;
        private TalkData _talkData;

        private const int _firstPage = 0;
        private int _pageIndex;

        private bool _isLastPage;
        private bool _canShowNextPage;
        private bool _isTalking;

        public int PageIndex => _pageIndex;
        public bool IsRunning => _isTalking;
        public bool IsLastPage => _isLastPage;

        //Change write speed (if the current writer is different to "instant")
        public WriteSpeedType WriteSpeed
        {
            get
            {
                return _writerControl.Writer.WriteSpeedType;
            }
            set
            {
                if (_isTalking)
                {
                    _writerControl.Writer.SetWriteTypeSpeed(value);
                }
                else
                {
                    Debug.LogError("Not talking!");
                }
            }
        }

        protected void Awake()
        {
            _writerControl = new WriterControl(this, OnPageWriten);
        }

        private void Update()
        {
            if (_talkData)
            {
                _writerControl.Writer.Update();
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkData talkData)
        {
            if (!_isTalking)
            {
                _talkCloud = cloud;

                _talkCloud.Init(talkData.PagesCount);

                _talkCloud.OnCloudShown += OnCloudShown;
                _talkCloud.OnCloudHidden += OnCloudHidden;

                _talkData = talkData;

                _currentPage = _talkData.GetPage(_firstPage);
                _talkCloud.SetPage(_currentPage, _firstPage);

                if (_talkData)
                {
                    _isTalking = true;

                    _pageIndex = default;
                    _isLastPage = default;
                    _canShowNextPage = default;

                    _writerControl.Init(_currentPage, _talkCloud.TextControl);

                    _talkCloud.OnShowCloud();
                    _talkCallback?.Invoke(TalkEvent.Started);
                }
                else
                {
                    Debug.LogError($"Talk Data is null");
                }
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkData talkData, Action<TalkEvent> talkCallback)
        {
            if (!_isTalking)
            {
                _talkCallback = talkCallback;

                StartTalk(cloud, talkData);
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkData talkData, Action<string> wordEventCallback)
        {
            if (!_isTalking)
            {
                _onWordEventCallBack = wordEventCallback;

                StartTalk(cloud, talkData);
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkData talkData, Action<TalkEvent> talkCallback, Action<string> wordEventCallback)
        {
            if (!_isTalking)
            {
                _onWordEventCallBack = wordEventCallback;

                StartTalk(cloud, talkData, talkCallback);
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkInfo talkInfo)
        {
            if (!_isTalking)
            {
                _talkData = _scriptableContainer.Container.GetTalkAsset(talkInfo);

                StartTalk(cloud, _talkData);
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkInfo talkInfo, Action<TalkEvent> talkCallback)
        {
            if (!_isTalking)
            {
                _talkCallback = talkCallback;

                StartTalk(cloud, talkInfo);
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkInfo talkInfo, Action<string> wordEventCallback)
        {
            if (!_isTalking)
            {
                _onWordEventCallBack = wordEventCallback;

                StartTalk(cloud, talkInfo);
            }
        }

        public void StartTalk(TalkCloudBase cloud, TalkInfo talkInfo, Action<TalkEvent> talkCallback, Action<string> wordEventCallback)
        {
            if (!_isTalking)
            {
                _onWordEventCallBack = wordEventCallback;

                StartTalk(cloud, talkInfo, talkCallback);
            }
        }

        /// <summary>Advance to the next page if all the text was writen.</summary>
        /// <returns>True if advanced to the next page.</returns>
        public bool NextPage()
        {
            if (_isTalking)
            {
                if (!_isLastPage)
                {
                    if (_canShowNextPage)
                    {
                        _canShowNextPage = false;

                        _pageIndex++;

                        _talkCallback?.Invoke(TalkEvent.PageChanged);

                        _currentPage = _talkData.GetPage(_pageIndex);

                        _talkCloud.SetPage(_currentPage, _pageIndex);

                        _writerControl.Init(_currentPage, _talkCloud.TextControl);
                        _writerControl.Writer.WritePage(_currentPage);

                        return true;
                    }
                    else
                    {
                        //Do something random, while waiting to change to next page.
                    }
                }
                else
                {
                    _writerControl.Writer.OnExitTalk();

                    _talkCloud.CloseCloud();
                }
            }
            else
            {
                Debug.LogError("Trying to advance to a next page without starting the conversation.");
            }

            return false;
        }

    

        //Call it when a talk is running and the language is changed to update the cloud talk with the new text.
        /// <summary>Call when the language is changed.</summary>
        public void SetTalkOnLanguageChanged(TalkInfo info)
        {
            if (_scriptableContainer)
            {
                if (_talkData)
                {
                    _talkData = _scriptableContainer.Container.GetTalkAsset(info);
                    _writerControl.Writer.OnLanguageChanged(_talkData.GetPage(_pageIndex));
                }
            }
            else
            {
                Debug.LogError("Container reference wasn't added.");
            }
        }

        private void OnCloudShown()
        {
            _writerControl.Writer.WritePage(_currentPage);
        }

        private void OnCloudHidden()
        {
            _isTalking = false;
            _talkData = null;

            _talkCallback?.Invoke(TalkEvent.Finished);

            _talkCallback = null;
            _onWordEventCallBack = null;

            _writerControl.Clear();

            _talkCloud.OnCloudShown -= OnCloudShown;
            _talkCloud.OnCloudHidden -= OnCloudHidden;
        }

        private void OnDestroy()
        {
            _talkCallback = null;
            _onWordEventCallBack = null;
        }

        private void OnPageWriten()
        {
            var nextPageIndex = _pageIndex + 1;

            _isLastPage = nextPageIndex == _talkData.PagesCount;

            _canShowNextPage = true;
        }
    }
}