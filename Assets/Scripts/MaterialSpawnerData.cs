using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackProto
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Material Spawner Data", fileName = "Material Spawner Data")]
    public class MaterialSpawnerData : ScriptableObject
    {
        public float quantity;
        public float duration;
    }
}

