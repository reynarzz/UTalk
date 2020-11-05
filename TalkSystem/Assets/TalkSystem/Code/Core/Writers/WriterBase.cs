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
    public abstract class WriterBase
    {
        public abstract event Action OnPageWriten;

        private TextControl _control;
        protected TextControl Control => _control;

        private WaitForSeconds _normalSpeed;
        private WaitForSeconds _fastSpeed;
        protected WaitForSeconds _writeSpeed;

        private MonoBehaviour _mono;

        private List<Highlight> _highlightedChars;

        public WriterBase(MonoBehaviour mono)
        {
            _mono = mono;
            _highlightedChars = new List<Highlight>();
        }
        public abstract void Update();

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

            yield return 0;
            //this has to be done in the next frame.
            control.ReloadCharsVertices();

            yield return Write(control, page);
        }

        /// <summary>Write the page.</summary>
        protected abstract IEnumerator Write(TextControl control, TextPage page);

        public virtual void Clear(TextControl control)
        {
            control.ClearColors();
        }

        public void OnLanguageChanged(TextPage textPage)
        {

        }
    }
}
