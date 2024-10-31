using System;
using UnityEngine;

namespace ObservableGadgets.Observables
{
    public class ObservableClampedInteger : ObservableClampedNumeric<int>
    {
        public ObservableClampedInteger(int initialState, ClampMode clampMode, int min, int max) : base(initialState, clampMode, min, max) {}
    }
}