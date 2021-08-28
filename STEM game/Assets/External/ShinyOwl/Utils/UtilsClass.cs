using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace ShinyOwl.Utils
{
    public static class UtilsClass
    {
        private const int DefaultTextSize = 3;
        private const int DefaultTextResolution = 10;
        private const TextAnchor DefaultTextAnchor = TextAnchor.UpperLeft;
        private const TextAlignment DefaultTextAlignment = TextAlignment.Left;
        public const int DefaultSortingOrder = 5000;

        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 worldPosition = GetMouseWorldPositionWithZ();
            worldPosition.z = 0f;
            return worldPosition;
        }
        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        public static TextMesh CreateWorldText(string text, Vector3 localPosition, int size = DefaultTextSize, Color? color = null, TextAnchor anchor = DefaultTextAnchor, TextAlignment alignment = DefaultTextAlignment, int resolution = DefaultTextResolution)
        {
            return CreateWorldText(text, null, localPosition, size, color, anchor, alignment, DefaultSortingOrder, resolution);
        }
        public static TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, int size, Color? color, TextAnchor anchor, TextAlignment alignment, int sortingOrder, int resolution)
        {
            if (color == null) { color = Color.white; }
            GameObject go = new GameObject("World Text", typeof(TextMesh));
            Transform transform = go.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.localScale = new Vector3(transform.localScale.x / resolution, transform.localScale.y / resolution);
            TextMesh textMesh = go.GetComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = size * resolution;
            textMesh.color = (Color)color;
            textMesh.anchor = anchor;
            textMesh.alignment = alignment;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
        public static FunctionUpdater CreateWorldTextUpdater(Func<string> func, Vector3 localPosition, int size = DefaultTextSize, Color? color = null, TextAnchor anchor = DefaultTextAnchor, TextAlignment alignment = DefaultTextAlignment, int resolution = DefaultTextResolution)
        {
            return CreateWorldTextUpdater(func, null, localPosition, size, color, anchor, alignment, DefaultSortingOrder, resolution);
        }
        public static FunctionUpdater CreateWorldTextUpdater(Func<string> func, Transform parent, Vector3 localPosition, int size, Color? color, TextAnchor anchor, TextAlignment alignment, int sortingOrder, int resolution)
        {
            TextMesh textMesh = CreateWorldText(func(), parent, localPosition, size, color, anchor, alignment, sortingOrder, resolution);
            return FunctionUpdater.Create(() => textMesh.text = func());
        }
        public static void CreateWorldTextPopup(string text, Vector3 localPosition, Color? color = null, float timer = 1f)
        {
            CreateWorldTextPopup(text, null, localPosition, localPosition + new Vector3(0f, 3f), timer, DefaultTextSize, color, DefaultTextResolution);
        }
        public static void CreateWorldTextPopup(string text, Transform parent, Vector3 localPosition, Vector3 destinationPosition, float timer = 1f, int size = DefaultTextSize, Color? color = null, int resolution = DefaultTextResolution)
        {
            TextMesh textMesh = CreateWorldText(text, parent, localPosition, size, color, DefaultTextAnchor, DefaultTextAlignment, DefaultSortingOrder, resolution);
            Transform transform = textMesh.transform;
            Vector3 moveVector = (destinationPosition - localPosition) / timer;
            FunctionUpdater.Create(() =>
            {
                transform.position += moveVector * Time.unscaledDeltaTime;
                timer -= Time.unscaledDeltaTime;
                if (timer <= 0f)
                {
                    UnityEngine.Object.Destroy(transform.gameObject);
                    return true;
                }
                else
                {
                    return false;
                }
            }, "WorldText Popup");
        }
    }

    public sealed class FunctionTimer
    {
        private static List<FunctionTimer> activeTimers;
        private static GameObject initGO;
        private static void InitIfNeeded()
        {
            if (initGO == null)
            {
                activeTimers = new List<FunctionTimer>();
                initGO = new GameObject("FunctionTimer InitGO");
            }
        }

        public sealed class MonoBehaviourHook : MonoBehaviour
        {
            public Action OnUpdate;
            private void Update()
            {
                if (OnUpdate != null) { OnUpdate(); }
            }
        }
        public static FunctionTimer Create(Action action, float timer, string name = null)
        {
            InitIfNeeded();
            FunctionTimer functionTimer = new FunctionTimer(action, timer, name, new GameObject("FunctionTimer Instance", typeof(MonoBehaviourHook)));
            functionTimer.go.GetComponent<MonoBehaviourHook>().OnUpdate = functionTimer.Update;
            activeTimers.Add(functionTimer);
            return functionTimer;
        }
        public static void DestroyTimer(FunctionTimer functionTimer)
        {
            InitIfNeeded();
            if (functionTimer != null) { functionTimer.DestroySelf(); }
        }
        public static void DestroyTimerWithName(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < activeTimers.Count; i++)
            {
                FunctionTimer functionTimer = activeTimers[i];
                if (functionTimer.name == name)
                {
                    functionTimer.DestroySelf();
                    return;
                }
            }
        }
        public static void DestroyAllTimersWithName(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < activeTimers.Count; i++)
            {
                FunctionTimer functionTimer = activeTimers[i];
                if (functionTimer.name == name)
                {
                    functionTimer.DestroySelf();
                    i--;
                }
            }
        }

        private Action action; 
        private float timer;
        private string name;
        private GameObject go;

        private FunctionTimer(Action _Action, float _Timer, string _Name, GameObject _GO)
        {
            action = _Action;
            timer = _Timer;
            name = _Name;
            go = _GO;
        }
        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                action();
                DestroySelf();
            }
        }
        private void DestroySelf()
        {
            RemoveTimerFromActiveList(this);
            UnityEngine.Object.Destroy(go);
        }
        private static void RemoveTimerFromActiveList(FunctionTimer functionTimer)
        {
            InitIfNeeded();
            activeTimers.Remove(functionTimer);
        }
    }
    public sealed class FunctionUpdater
    {
        private static List<FunctionUpdater> activeUpdaters;
        private static GameObject initGO;
        private static void InitIfNeeded()
        {
            if (initGO == null)
            {
                activeUpdaters = new List<FunctionUpdater>();
                initGO = new GameObject("FunctionUpdater InitGO");
            }
        }

        public sealed class MonoBehaviourHook : MonoBehaviour
        {
            public Action OnUpdate;
            private void Update()
            {
                if (OnUpdate != null) { OnUpdate(); }
            }
        }
        public static FunctionUpdater Create(Action func, string name = null, bool isActive = true)
        {
            return Create(() => { func(); return false; }, name, isActive);
        }
        public static FunctionUpdater Create(Func<bool> func, string name = null, bool isActive = true)
        {
            InitIfNeeded();
            FunctionUpdater functionUpdater = new FunctionUpdater(func, name, isActive, new GameObject("FunctionUpdater Instance", typeof(MonoBehaviourHook)));
            functionUpdater.go.GetComponent<MonoBehaviourHook>().OnUpdate = functionUpdater.Update;
            activeUpdaters.Add(functionUpdater);
            return functionUpdater;
        }
        public static void DestroyUpdater(FunctionUpdater functionUpdater)
        {
            InitIfNeeded();
            if (functionUpdater != null) { functionUpdater.DestroySelf(); }
        }
        public static void DestroyUpdaterWithName(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < activeUpdaters.Count; i++)
            {
                FunctionUpdater functionUpdater = activeUpdaters[i];
                if (functionUpdater.name == name)
                {
                    functionUpdater.DestroySelf();
                    return;
                }
            }
        }
        public static void DestroyAllUpdatersWithName(string name)
        {
            InitIfNeeded();
            for (int i = 0; i < activeUpdaters.Count; i++)
            {
                FunctionUpdater functionUpdater = activeUpdaters[i];
                if (functionUpdater.name == name)
                {
                    functionUpdater.DestroySelf();
                    i--;
                }
            }
        }

        private Func<bool> func;
        private string name;
        private bool isActive;
        private GameObject go;

        private FunctionUpdater(Func<bool> _Func, string _Name, bool _IsActive, GameObject _GO)
        {
            func = _Func;
            name = _Name;
            isActive = _IsActive;
            go = _GO;
        }
        private void Update()
        {
            if (!isActive) { return; }
            if (func() == true) { DestroySelf(); }
        }
        public void Pause()
        {
            isActive = false;
        }
        public void Resume()
        {
            isActive = true;
        }
        private void DestroySelf()
        {
            RemoveUpdaterFromActiveList(this);
            UnityEngine.Object.Destroy(go);
        }
        private static void RemoveUpdaterFromActiveList(FunctionUpdater functionUpdater)
        {
            InitIfNeeded();
            activeUpdaters.Remove(functionUpdater);
        }
    }
}

