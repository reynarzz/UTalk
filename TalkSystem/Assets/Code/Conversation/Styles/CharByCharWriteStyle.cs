using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class CharByCharWriteStyle : IWriteStyle
    {
        public void Write(MonoBehaviour mono, TextMeshControl control, TextPage page)
        {
            var coroutine = mono.StartCoroutine(WriteByChar(control, page));
        }

        private IEnumerator WriteByChar(TextMeshControl control, TextPage page)
        {
            control.Write(page.Text);

            while (true)
            {
                yield return 0;
            }
        }
    }
}
