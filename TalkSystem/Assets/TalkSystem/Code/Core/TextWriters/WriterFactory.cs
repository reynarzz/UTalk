using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace uTalk
{
    public class WriterFactory
    {
        private Dictionary<Type, WriterBase> _writers;
        private TextAnimationControl _animationControl;

        public WriterFactory(MonoBehaviour mono, Action onPageWriten)
        {
            _animationControl = new TextAnimationControl();

            var instant = new InstantWriter(mono, _animationControl);
            var charByChar = new CharByCharWriter(mono, _animationControl);

            instant.OnPageWritten += onPageWriten;
            charByChar.OnPageWritten += onPageWriten;

            _writers = new Dictionary<Type, WriterBase>()
            {
                { typeof(InstantWriter), instant },
                { typeof(CharByCharWriter), charByChar },
            };
        }

        public WriterBase GetWriter<T>() where T : WriterBase
        {
            if (_writers.ContainsKey(typeof(T)))
            {
                return _writers[typeof(T)];
            }
            else
            {
                Debug.LogError($"Type: {typeof(T).Name} is not in the factory.");
                return null;
            }
        }

        public WriterBase GetWriter(WriteType writerType, TextControl textControl)
        {
            var writer = default(WriterBase);

            switch (writerType)
            {
                case WriteType.Instant:
                    writer = _writers[typeof(InstantWriter)];
                    break;
                case WriteType.CharByChar:
                    writer = _writers[typeof(CharByCharWriter)];
                    break;
                default:
                    Debug.LogError($"Type: {writerType} is not in the factory.");
                    return null;
            }

            writer.SetTextControl(textControl);

            return writer;
        }
    }
}
