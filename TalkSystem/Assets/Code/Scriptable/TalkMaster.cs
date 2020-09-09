using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    /// <summary>Container for all the talks inside the game.</summary>
    [Serializable]
    public struct Talks
    {
        [SerializeField] private Language _language;
        [SerializeField] private TalkAsset[] _talks;
    }

    [CreateAssetMenu]
    public class TalkMaster : ScriptableObject
    {
        [SerializeField] private Talks[] _talks;

        public Talks[] Talks => _talks;
    }

}
