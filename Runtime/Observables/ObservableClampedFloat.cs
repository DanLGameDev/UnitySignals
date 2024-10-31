using System;
using UnityEngine;

namespace ObservableGadgets.Observables
{
    public class ObservableClampedFloat : ObservableClampedNumeric<float>
    {
        public ObservableClampedFloat(float initialState, ClampMode clampMode, float min, float max) : base(initialState, clampMode, min, max) {}
    }
}