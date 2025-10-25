
using UnityEngine;

namespace DGP.UnitySignals.Signals
{
    public class Vector2ValueSignal : ValueSignal<Vector2> 
    {
        public Vector2ValueSignal(Vector2 value = default(Vector2)) : base(value) {}
    }

    public class Vector3ValueSignal : ValueSignal<Vector3> 
    {
        public Vector3ValueSignal(Vector3 value = default(Vector3)) : base(value) {}
    }
}