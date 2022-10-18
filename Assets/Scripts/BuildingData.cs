using System;
using System.Collections.Generic;
using UnityEngine;

namespace StackProto
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Building Data", fileName = "Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Serializable]
        public struct BuildingDataStruct
        {
            public UnityEngine.Material beforeMaterial;
            public float requiredAmount;
            public UnityEngine.Material afterMaterial;
        }

        public bool isFloorDown;
        public float floorHeight;
        public float floorHeightMax;
        public GameObject floorTemplate;
        public List<BuildingDataStruct> list;
    }
}