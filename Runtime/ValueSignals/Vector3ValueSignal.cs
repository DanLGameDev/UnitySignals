using UnityEngine;

namespace DGP.UnitySignals.ValueSignals
{
    public class Vector3ValueSignal : ValueSignal<Vector3>
    {
        public Vector3ValueSignal(Vector3 value = default) : base(value) { }
    }
}