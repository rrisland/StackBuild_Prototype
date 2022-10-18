using System;
using System.Collections.Generic;
using UnityEngine;

namespace StackProto
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Player Data", fileName = "Player Data")]
    public class PlayerData : ScriptableObject
    {
        public float accelaration;
        public float attenuation;
        public float moveSpeed;
        public float catchupPower;
        public float catchupRange;
        public float releasePower;

        public Vector2 playAreaMin;
        public Vector2 playAreaMax;
    }
}