using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public interface IWriteStyle : ITextStyle
    {
        /// <summary>
        /// Write in the target conversation cloud
        /// </summary>
        /// <param name="target">Controls how the text will be displayed</param>
        /// <param name="page">Current page with the text and information to write.</param>
        void Write(MonoBehaviour mono, TextMeshControl cloud, TextPage page);
    }
}
