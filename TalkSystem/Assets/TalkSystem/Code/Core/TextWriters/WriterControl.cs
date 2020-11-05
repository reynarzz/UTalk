using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class WriterControl
    {
        private WriterFactory _writersFactory;
        private WriterBase _writer;

        /// <summary>The current object that is writing in the text cloud.</summary>
        public WriterBase Writer => _writer;

        public WriterControl(MonoBehaviour mono, Action onPageWriten)
        {
            _writersFactory = new WriterFactory(mono, onPageWriten);
        }

        public void SetPage(TextPage page, TextControl textControl)
        {
            _writer = _writersFactory.GetWriter(page.WriteType);
            _writer.Clear(textControl);
        }

        public void Clear()
        {
            _writer = null;
        }
    }
}
