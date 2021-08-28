using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEditor;

public class GC : MonoBehaviour
{
    private static Hashtable referenceHash;
    private static Hashtable instanceHash;
    private static Transform instanceParent;
    private static int instanceIDCounter = 0;
    public static Transform CanvasT; public Transform _CanvasT;
    private void Awake()
    {
        referenceHash = new Hashtable();
        referenceHash.Add("sound:swim_dash1", Resources.Load<AudioClip>("Sounds/swim_dash1"));
        referenceHash.Add("sound:swim_dash2", Resources.Load<AudioClip>("Sounds/swim_dash2"));
        referenceHash.Add("sprite:builtin:background", AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"));
        referenceHash.Add("sprite:builtin:knob", AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"));
        referenceHash.Add("font:arial", Resources.GetBuiltinResource<Font>("Arial.ttf"));
        referenceHash.Add("prefab:fish", Resources.Load<GameObject>("Prefabs/Fish"));
        referenceHash.Add("prefab:coral", Resources.Load<GameObject>("Prefabs/Coral"));
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/items1"), "sprite:coral_research_paper", 
            "sprite:crab_research_paper", "sprite:fish_research_paper", "sprite:squid_research_paper");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/species1"), "sprite:blue_fish",
            "sprite:crab_attack", "sprite:crab_idle", "sprite:giant_squid_swim1",
            "sprite:giant_squid_swim2", "sprite:orange_coral_idle1", "sprite:orange_coral_idle2",
            "sprite:purple_fish", "sprite:yellow_coral_idle1", "sprite:yellow_coral_idle2");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/icons1"), "sprite:dollar_icon", "sprite:gear_icon");
        AddItemToReferenceHash("item:fish_research_paper", "Fish Research Paper", 2f, 3, "sprite:fish_research_paper");
        AddItemToReferenceHash("item:coral_research_paper", "Coral Research Paper", 4.5f, 2, "sprite:coral_research_paper");
        AddItemToReferenceHash("item:squid_research_paper", "Squid Research Paper", 50f, 1, "sprite:squid_research_paper");
        referenceHash.Add("creature:blue_fish", new Fish("sprite:blue_fish", 0.05f, "Blue Fish", null, "item:fish_research_paper", null, -1, "creature:blue_fish"));
        referenceHash.Add("creature:purple_fish", new Fish("sprite:purple_fish", 0.075f, "Purple Fish", null, "item:fish_research_paper", null, -1, "creature:purple_fish"));
        referenceHash.Add("creature:giant_squid", new Fish("sprite:giant_squid_swim1", 0.085f, "Giant Squid", "OMG! It's a Giant Squid!", "item:squid_research_paper", null, -1, "creature:giant_squid"));
        referenceHash.Add("creature:orange_coral", new Coral(new string[] { "sprite:orange_coral_idle1", "sprite:orange_coral_idle2" }, "Orange Coral", null, "item:coral_research_paper", null, -1, "creature:orange_coral"));
        referenceHash.Add("creature:yellow_coral", new Coral(new string[] { "sprite:yellow_coral_idle1", "sprite:yellow_coral_idle2" }, "Yellow Coral", null, "item:coral_research_paper", null, -1, "creature:yelow_coral"));

        instanceHash = new Hashtable();
        instanceParent = new GameObject("Instances").transform;

        CanvasT = _CanvasT;
        for (int i = 0; i < 5; i++)
        {
            CreateInstance("creature:blue_fish", -10f, -15f);
            CreateInstance("creature:purple_fish", -10f, -15f);
        }
        CreateInstance("creature:giant_squid", 10f, -102f);
        CreateInstance("creature:orange_coral", -14.6f, -25.9f);
        CreateInstance("creature:yellow_coral", -16.7f, -25.5f);
    }

    public static int GetNewInstanceID()
    {
        return instanceIDCounter++;
    }
    public static T GetReference<T>(string id)
    {
        if (id == null) return default(T);
        return ((T)referenceHash[id]);
    }
    public static GameObject GetInstanceByID(int id)
    {
        return (GameObject)instanceHash[id];
    }
    public static bool TryGetDeepClone<T>(string id, float x, float y, out T output) where T : ReferenceBase
    {
        output = default(T);
        T reference = GetReference<T>(id);
        if (reference != null) output = (T)reference.DeepClone(x, y); 
        return output != default(T);
    }
    public static void CreateInstance(string id, float x, float y)
    {
        if (TryGetDeepClone<InstanceBase>(id, x, y, out InstanceBase output)) instanceHash.Add(output.InstanceID, output.GO);
    }
    public static GameObject CreatePrefab(string prefabID, float x, float y)
    {
        return Instantiate(GetReference<GameObject>(prefabID), new Vector3(x, y), Quaternion.identity, instanceParent);
    }
    public static void InitBehaviour<T>(GameObject go, InstanceBase classReference) where T : IClassSetable
    {
        go.GetComponent<T>().SetClass(classReference);
    }
    private static void AddItemToReferenceHash(string key, string name, float currencyValue, int maxStack, string spriteID)
    {
        referenceHash.Add(key, new InventoryItem(key, name, currencyValue, maxStack, spriteID));
    }
    private static void AddSpriteSheetToReferenceHash(Sprite[] spriteSheet, params string[] keys)
    {
        for (int i = 0; i < keys.Length; i++) { referenceHash.Add(keys[i], spriteSheet[i]); }
    }
    public static void PlaySound(string id, float volume = 1f, float pitch = 1f, int variation = 0)
    {
        if (variation > 0)
        {
            int startNum; if (!int.TryParse(id[id.Length - 1].ToString(), out startNum)) { return; }
            id = id.Substring(0, id.Length - 1) + Random.Range(startNum, startNum + variation + 1);
        }
        AudioClip clip = GetReference<AudioClip>(id);
        if (clip == null) { return; }

        GameObject GO = new GameObject(id);
        AudioSource AS = GO.AddComponent<AudioSource>();
        AS.clip = clip; AS.volume = volume; AS.pitch = pitch; AS.Play();
        Destroy(GO, clip.length);
    }
    public static float ApproachValue(float currentValue, float targetValue, float rate)
    {
        return (currentValue + rate * (targetValue - currentValue));
    }
    public static Vector3 GetWorldMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public static void GetVelocityToMouse(Vector3 castPos, float force, out float velocityX, out float velocityY)
    {
        Vector3 mousePos = GetWorldMousePos();
        Vector3 dir = (mousePos - castPos).normalized;
        float radians = Mathf.Atan2(dir.y, dir.x);
        velocityX = Mathf.Cos(radians) * force;
        velocityY = Mathf.Sin(radians) * force;
    }
    private static GameObject BuildUI(string name, Vector2 pos, Vector2 scale, Vector2 sizeDelta)
    {
        GameObject output = new GameObject(name); output.transform.parent = CanvasT;
        RectTransform RT = output.AddComponent<RectTransform>();
        output.transform.localPosition = pos; output.transform.localScale = scale; RT.sizeDelta = sizeDelta;
        return (output);
    }
    public static Image BuildUIImage(string name, Vector2 pos, Vector2 sizeDelta, string spriteID, Color color, bool isCutout = false)
    {
        GameObject ui = BuildUI(name, pos, Vector2.one, sizeDelta);
        Image I;
        if (isCutout) I = ui.AddComponent<CutoutMaskUI>();
        else I = ui.AddComponent<Image>();
        I.sprite = GetReference<Sprite>(spriteID); I.color = color; I.type = Image.Type.Sliced;
        return (I);
    }
    public static Text BuildUIText(string name, Vector2 pos, Vector2 sizeDelta, string text, int fontSize, TextAnchor alignment, string fontID, FontStyle fontStyle, Color color, int resolution)
    {
        GameObject ui = BuildUI(name, pos, Vector2.one / resolution, sizeDelta * resolution);
        Text T = ui.AddComponent<Text>(); T.text = text; T.fontSize = fontSize * resolution; T.alignment = alignment; T.font = GetReference<Font>(fontID); T.fontStyle = fontStyle; T.color = color;
        return (T);
    }
    //public static void BuildUISlider
}
