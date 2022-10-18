using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StackProto
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Material Data", fileName = "Material Data")]
    public class MaterialData : ScriptableObject
    {
        public List<UnityEngine.Material> materials;
        public List<UnityEngine.Mesh> meshes;
    }
}