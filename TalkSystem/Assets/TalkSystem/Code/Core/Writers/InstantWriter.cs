using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class InstantWriter : WriterBase
    {
        public override event Action OnPageWriten;

        public InstantWriter(MonoBehaviour mono) : base(mono) { }

        public override void Update()
        {

        }

        protected override IEnumerator Write(TextControl control, TextPage page)
        {
            yield return false;
        }
    }
}
