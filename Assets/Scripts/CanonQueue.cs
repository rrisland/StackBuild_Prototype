using System.Collections.Generic;
using UnityEngine;

namespace StackProto
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Canon Queue", fileName = "Canon Queue")]
    public class CanonQueue : ScriptableObject
    {
        public Queue<GameObject> Objects = new Queue<GameObject>();
    }
}