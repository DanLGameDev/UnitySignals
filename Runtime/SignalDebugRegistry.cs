using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DGP.UnitySignals
{
    public static class SignalDebugRegistry
    {
#if UNITY_EDITOR
        public static event Action<SignalDebugEntry> OnEntryAdded;
        public static event Action<SignalDebugEntry> OnEntryRemoved;

        private static readonly List<SignalDebugEntry> _entries = new();
        public static IReadOnlyList<SignalDebugEntry> Entries => _entries;

        static SignalDebugRegistry()
        {
            UnityEditor.EditorApplication.playModeStateChanged += state =>
            {
                if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                    ClearAll();
            };
        }

        public static void ClearAll()
        {
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                var entry = _entries[i];
                entry.Signal.SignalDied -= OnSignalDied;
                OnEntryRemoved?.Invoke(entry);
            }
            _entries.Clear();
        }

        private static void OnSignalDied(IEmitSignals signal) => Unregister(signal);
#endif

        [Conditional("UNITY_EDITOR")]
        public static void Register(string title, IEmitSignals signal, string category = "General")
        {
#if UNITY_EDITOR
            if (signal == null) return;
            var entry = new SignalDebugEntry(title, category, signal);
            _entries.Add(entry);
            signal.SignalDied += OnSignalDied;
            OnEntryAdded?.Invoke(entry);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void Unregister(IEmitSignals signal)
        {
#if UNITY_EDITOR
            if (signal == null) return;
            int index = _entries.FindIndex(e => ReferenceEquals(e.Signal, signal));
            if (index < 0) return;
            var entry = _entries[index];
            _entries.RemoveAt(index);
            signal.SignalDied -= OnSignalDied;
            OnEntryRemoved?.Invoke(entry);
#endif
        }
    }

#if UNITY_EDITOR
    public class SignalDebugEntry
    {
        public string Title { get; }
        public string Category { get; }
        public IEmitSignals Signal { get; }

        public SignalDebugEntry(string title, string category, IEmitSignals signal)
        {
            Title    = title;
            Category = category;
            Signal   = signal;
        }
    }
#endif
}
