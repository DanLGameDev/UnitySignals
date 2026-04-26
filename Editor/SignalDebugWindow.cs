using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DGP.UnitySignals.Editor
{
    public class SignalDebugWindow : EditorWindow
    {
        [MenuItem("DGP/Signal Debugger")]
        private static void OpenWindow()
        {
            var window = GetWindow<SignalDebugWindow>("Signal Debugger");
            window.minSize = new Vector2(400f, 300f);
            window.Show();
        }

        // ── Flash ─────────────────────────────────────────────────────────────
        private readonly Dictionary<IEmitSignals, double> _flashTimestamps = new();
        private const double FlashDuration = 0.6;
        private static readonly Color FlashColor   = new Color(1f, 0.85f, 0f, 1f);
        private static readonly Color NormalBg     = new Color(0.20f, 0.20f, 0.20f, 1f);
        private static readonly Color CategoryBg   = new Color(0.13f, 0.13f, 0.13f, 1f);

        // ── Badge colors ──────────────────────────────────────────────────────
        private static readonly Color ValueBadgeColor    = new Color(0.20f, 0.45f, 0.85f, 1f);
        private static readonly Color ComputedBadgeColor = new Color(0.60f, 0.30f, 0.70f, 1f);
        private static readonly Color CollectionBadgeColor = new Color(0.20f, 0.55f, 0.35f, 1f);

        // ── Category foldouts ─────────────────────────────────────────────────
        private readonly Dictionary<string, bool> _foldouts = new();
        private Vector2 _scrollPos;

        // ── Reflection cache (static: survives window close/reopen) ───────────
        private static readonly Dictionary<System.Type, PropertyInfo> _valuePropertyCache = new();

        // ── GUIStyles (lazy: must not init before first OnGUI) ────────────────
        private GUIStyle _titleStyle;
        private GUIStyle _valueStyle;
        private GUIStyle _badgeStyle;
        private GUIStyle _categoryStyle;

        // ─────────────────────────────────────────────────────────────────────
        // Lifecycle
        // ─────────────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            SignalDebugRegistry.OnEntryAdded   += HandleEntryAdded;
            SignalDebugRegistry.OnEntryRemoved += HandleEntryRemoved;
            EditorApplication.update           += OnEditorUpdate;

            foreach (var entry in SignalDebugRegistry.Entries)
            {
                EnsureCategoryTracked(entry.Category);
                entry.Signal.SignalChanged += HandleSignalChanged;
            }
        }

        private void OnDisable()
        {
            SignalDebugRegistry.OnEntryAdded   -= HandleEntryAdded;
            SignalDebugRegistry.OnEntryRemoved -= HandleEntryRemoved;
            EditorApplication.update           -= OnEditorUpdate;

            foreach (var entry in SignalDebugRegistry.Entries)
                entry.Signal.SignalChanged -= HandleSignalChanged;
        }

        private void HandleEntryAdded(SignalDebugEntry entry)
        {
            EnsureCategoryTracked(entry.Category);
            entry.Signal.SignalChanged += HandleSignalChanged;
            Repaint();
        }

        private void HandleEntryRemoved(SignalDebugEntry entry)
        {
            entry.Signal.SignalChanged -= HandleSignalChanged;
            _flashTimestamps.Remove(entry.Signal);
            Repaint();
        }

        private void HandleSignalChanged(IEmitSignals signal)
        {
            _flashTimestamps[signal] = EditorApplication.timeSinceStartup;
        }

        private void EnsureCategoryTracked(string category)
        {
            if (!_foldouts.ContainsKey(category))
                _foldouts[category] = true;
        }

        private void OnEditorUpdate()
        {
            double now = EditorApplication.timeSinceStartup;
            foreach (var kvp in _flashTimestamps)
            {
                if (now - kvp.Value < FlashDuration)
                {
                    Repaint();
                    return;
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // GUI
        // ─────────────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            InitStyles();
            DrawToolbar();

            float toolbarH = EditorGUIUtility.singleLineHeight + 6f;
            GUILayout.BeginArea(new Rect(0f, toolbarH, position.width, position.height - toolbarH));
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var entries = SignalDebugRegistry.Entries;

            if (entries.Count == 0)
            {
                EditorGUILayout.LabelField("No signals registered.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                var categories = new List<string>();
                var byCategory = new Dictionary<string, List<SignalDebugEntry>>();
                foreach (var entry in entries)
                {
                    if (!byCategory.TryGetValue(entry.Category, out var list))
                    {
                        byCategory[entry.Category] = list = new List<SignalDebugEntry>();
                        categories.Add(entry.Category);
                    }
                    list.Add(entry);
                }

                double now = EditorApplication.timeSinceStartup;
                foreach (var category in categories)
                    DrawCategory(category, byCategory[category], now);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Signal Debugger", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear All", EditorStyles.toolbarButton))
                SignalDebugRegistry.ClearAll();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCategory(string category, List<SignalDebugEntry> entries, double now)
        {
            Rect headerRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(headerRect, CategoryBg);

            if (!_foldouts.TryGetValue(category, out bool open))
                open = true;
            _foldouts[category] = EditorGUILayout.Foldout(open, $"  {category}", true, _categoryStyle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(1f);

            if (!_foldouts[category]) return;

            foreach (var entry in entries)
                DrawSignalRow(entry, now);

            EditorGUILayout.Space(2f);
        }

        private void DrawSignalRow(SignalDebugEntry entry, double now)
        {
            float flashT = 0f;
            if (_flashTimestamps.TryGetValue(entry.Signal, out double ts))
                flashT = Mathf.Clamp01(1f - (float)((now - ts) / FlashDuration));

            Color rowBg = Color.Lerp(NormalBg, FlashColor, Mathf.SmoothStep(0f, 1f, flashT));

            Rect rowRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(rowRect, rowBg);

            Color prevContent = GUI.contentColor;
            GUI.contentColor = flashT > 0.05f ? Color.black : Color.white;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            GUILayout.Label(entry.Title, _titleStyle, GUILayout.Width(130f));
            GUILayout.Label(GetValueString(entry.Signal), _valueStyle, GUILayout.ExpandWidth(true));
            DrawBadge(GetTypeBadge(entry.Signal));
            GUILayout.Space(4f);
            EditorGUILayout.EndHorizontal();

            GUI.contentColor = prevContent;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(1f);
        }

        private void DrawBadge(string text)
        {
            Color color = text switch
            {
                "Computed" => ComputedBadgeColor,
                "List"     => CollectionBadgeColor,
                "Dict"     => CollectionBadgeColor,
                _          => ValueBadgeColor,
            };

            float width = text.Length * 6f + 10f;
            Rect r = GUILayoutUtility.GetRect(width, EditorGUIUtility.singleLineHeight, GUILayout.Width(width));
            EditorGUI.DrawRect(r, color);
            GUI.Label(r, text, _badgeStyle);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Reflection helpers
        // ─────────────────────────────────────────────────────────────────────

        private static string GetValueString(IEmitSignals signal)
        {
            var type = signal.GetType();
            if (!_valuePropertyCache.TryGetValue(type, out var prop))
                _valuePropertyCache[type] = prop = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            var value = prop?.GetValue(signal);
            if (value == null) return "null";
            if (value is ICollection col) return $"[{col.Count} items]";
            return value.ToString();
        }

        private static string GetTypeBadge(IEmitSignals signal)
        {
            var name = signal.GetType().Name;
            if (name.Contains("Computed"))        return "Computed";
            if (name.Contains("ObservableList"))  return "List";
            if (name.Contains("ObservableDict"))  return "Dict";
            return "Value";
        }

        // ─────────────────────────────────────────────────────────────────────
        // Style init (lazy — GUIStyle can't be created before first OnGUI)
        // ─────────────────────────────────────────────────────────────────────

        private void InitStyles()
        {
            if (_titleStyle != null) return;

            _titleStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontStyle = FontStyle.Bold,
                normal    = { textColor = new Color(0.85f, 0.85f, 0.85f, 1f) }
            };

            _valueStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = new Color(0.65f, 0.85f, 0.65f, 1f) }
            };

            _badgeStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                padding   = new RectOffset(2, 2, 1, 1),
                normal    = { textColor = Color.white }
            };

            _categoryStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                normal    = { textColor = new Color(0.80f, 0.80f, 0.80f, 1f) },
                onNormal  = { textColor = new Color(0.80f, 0.80f, 0.80f, 1f) }
            };
        }
    }
}
