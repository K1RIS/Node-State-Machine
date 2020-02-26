using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Editor
{
    public class NodesInfo : ScriptableObject
    {
        [SerializeField, ReadOnly] private int statesCount = 0;
        [SerializeField, ReadOnly] private List<string> names = new List<string>();
        [SerializeField, ReadOnly] private List<Rect> rects = new List<Rect>();

        public int StatesCount { get => statesCount; set => statesCount = value; }
        public List<string> Names { get => names; set => names = value; }
        public List<Rect> Rects { get => rects; set => rects = value; }
    }
}