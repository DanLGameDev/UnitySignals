using UnityEngine;

namespace DGP.UnitySignals.ValueSignals
{
    public class Vector2ValueSignal : ValueSignal<Vector2>
    {
        public Vector2ValueSignal(Vector2 value = default) : base(value) { }
    }
}