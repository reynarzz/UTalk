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
    public enum WriteSpeed
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
        protected WaitForSeconds _writeSpeed;

        private readonly MonoBehaviour _mono;
        private readonly TextAnimationControl _animationControl;

        public WriterBase(MonoBehaviour mono, TextAnimationControl animationControl)
        {
            _mono = mono;
            _animationControl = animationControl;
        }

        /// <summary>Write the page.</summary>
        protected abstract IEnumerator Write(TextControl control, TextPage page, TextAnimationControl animationControl);

        public void InitWriter(TextControl control, TextPage page)
        {
            _control = control;

            switch (page.WriteType)
            {
                case WriteType.Instant:
                    break;
                case WriteType.CharByChar:
                    _normalSpeed = new WaitForSeconds(page.CharByCharInfo.NormalWriteSpeed);
                    _fastSpeed = new WaitForSeconds(page.CharByCharInfo.FastWriteSpeed);
                    break;
            }

            _writeSpeed = _normalSpeed;

            _mono.StartCoroutine(StartWriter(control, page));
        }

        /// <summary>This does the necessary pre-work to start writing properly.</summary>
        private IEnumerator StartWriter(TextControl control, TextPage page)
        {
            control.SetText(page.Text);
            _animationControl.Init(control, page);

            //The next code have to be done in a different frame, that's the reason of this line.
            yield return 0;
            
            control.ReloadCharsVertices();

            yield return Write(control, page, _animationControl);
        }

        public void Update()
        {
            _animationControl.Update();
        }

        public void SetWriteSpeed(WriteSpeed speed)
        {
            switch (speed)
            {
                case WriteSpeed.Normal:
                    _writeSpeed = _normalSpeed;
                    break;
                case WriteSpeed.Fast:
                    _writeSpeed = _fastSpeed;
                    break;
            }
        }
        
        public void OnLanguageChanged(TextPage textPage)
        {

        }
        public virtual void Clear(TextControl control)
        {
            control.ClearColors();
        }

    }
}
