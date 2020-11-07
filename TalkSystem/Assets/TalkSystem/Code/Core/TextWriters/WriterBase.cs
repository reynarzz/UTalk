using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkSystem;
using UnityEngine;

namespace TalkSystem
{
    public enum WriteSpeedType
    {
        Normal, Fast
    }

    public abstract class WriterBase
    {
        public abstract event Action OnPageWritten;

        private TextControl _control;
        protected TextControl Control => _control;

        private WaitForSeconds _normalSpeed;
        private WaitForSeconds _fastSpeed;
        private WaitForSeconds _default;

        private WaitForSeconds _writeSpeed;
        protected WaitForSeconds WriteSpeed { get => _writeSpeed; set => _writeSpeed = value; }

        private readonly MonoBehaviour _mono;
        private readonly TextAnimationControl _animationControl;

        private WriteSpeedType _writeSpeedType;
        protected WriteSpeedType WriteSpeedType => _writeSpeedType;

        public WriterBase(MonoBehaviour mono, TextAnimationControl animationControl)
        {
            _mono = mono;
            _animationControl = animationControl;
            _default = new WaitForSeconds(0);
        }

        /// <summary>Write the page.</summary>
        protected abstract IEnumerator Write(TextControl control, TextPage page, TextAnimationControl animationControl);

        public void SetTextControl(TextControl control)
        {
            _control = control;
        }

        public void WritePage(TextPage page)
        {
            switch (page.WriteType)
            {
                case WriteType.Instant:
                    _normalSpeed = _default;
                    _fastSpeed = _default;
                    break;
                case WriteType.CharByChar:
                    //i need to cache this.
                    _normalSpeed = new WaitForSeconds(page.CharByCharInfo.NormalWriteSpeed);
                    _fastSpeed = new WaitForSeconds(page.CharByCharInfo.FastWriteSpeed);
                    break;
            }

            _writeSpeed = _normalSpeed;

            _mono.StartCoroutine(StartWriter(page));
        }

        /// <summary>This does the necessary pre-work to start writing properly.</summary>
        private IEnumerator StartWriter(TextPage page)
        {
            _control.SetText(page.Text);
            _animationControl.Init(_control, page);

            //The next code have to be done in a different frame, that's the reason of this line.
            yield return 0;

            _control.ReloadCharsVertices();

            yield return Write(_control, page, _animationControl);
        }

        public void Update()
        {
            _animationControl.Update();
        }

        public void SetWriteTypeSpeed(WriteSpeedType speedType)
        {
            _writeSpeedType = speedType;

            switch (_writeSpeedType)
            {
                case WriteSpeedType.Normal:
                    _writeSpeed = _normalSpeed;
                    break;
                case WriteSpeedType.Fast:
                    _writeSpeed = _fastSpeed;
                    break;
            }
        }
        
        public void OnLanguageChanged(TextPage textPage)
        {

        }

        public virtual void OnExitTalk()
        {
            _animationControl.OnExitPage();
            _control.ClearColors();
        }
    }
}
