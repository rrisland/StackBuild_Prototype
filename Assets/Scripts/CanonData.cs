using System.Collections.Generic;
using UnityEngine;

namespace StackProto
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Canon Data", fileName = "Canon Data")]
    public class CanonData : ScriptableObject
    {
        public float shotDuration = 0.3f;
        public float shotPower = 10;
        public float shotPowerRange = 15;
        public float shotAngleRange = 15;
    }
}