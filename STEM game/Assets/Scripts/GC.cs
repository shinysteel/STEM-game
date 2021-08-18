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
        AddItemToReferenceHash("item:fish_research_paper", "Fish Research Paper", 2f, 3, Get<Sprite>("sprite:fish_research_paper"));
        AddItemToReferenceHash("item:coral_research_paper", "Coral Research Paper", 4.5f, 2, Get<Sprite>("sprite:coral_research_paper"));
        AddItemToReferenceHash("item:squid_research_paper", "Squid Research Paper", 50f, 1, Get<Sprite>("sprite:squid_research_paper"));
        referenceHash.Add("creature:blue_fish", new Fish(Get<Sprite>("sprite:blue_fish"), 0.05f, "item:fish_research_paper", -1));
        referenceHash.Add("creature:purple_fish", new Fish(Get<Sprite>("sprite:purple_fish"), 0.075f, "item:fish_research_paper", -1));
        referenceHash.Add("creature:giant_squid", new Fish(Get<Sprite>("sprite:giant_squid_swim1"), 0.085f, "item:squid_research_paper", -1));
        referenceHash.Add("creature:orange_coral", new Coral(new Sprite[] { Get<Sprite>("sprite:orange_coral_idle1"), Get<Sprite>("sprite:orange_coral_idle2") }, "item:coral_research_paper", -1));
        referenceHash.Add("creature:yellow_coral", new Coral(new Sprite[] { Get<Sprite>("sprite:yellow_coral_idle1"), Get<Sprite>("sprite:yellow_coral_idle2") }, "item:coral_research_paper", -1));

        instanceHash = new Hashtable();
        instanceParent = new GameObject("Instances").transform;

        CanvasT = _CanvasT;
        for (int i = 0; i < 5; i++)
        {
            CreateInstance("creature:blue_fish", -6f, -3f);
            CreateInstance("creature:purple_fish", -7f, -3.5f);
        }
        CreateInstance("creature:giant_squid", -3f, -1.5f);
        CreateInstance("creature:orange_coral", -1f, -4.1f);
        CreateInstance("creature:yellow_coral", -3.9f, -4.1f);
    }

    public static int GetNewInstanceID() { return instanceIDCounter++; }
    public static T Get<T>(string id) { return ((T)referenceHash[id]); }
    public static GameObject GetInstanceByID(int id) { return (GameObject)instanceHash[id]; }
    public static void CreateInstance(string id, float x, float y) { Get<InstanceBase>(id).DeepClone(x, y); }
    public static void CreatePrefab<T>(string prefabID, InstanceBase classReference, float x, float y) where T : IClassSetable
    {
        GameObject prefab = Instantiate(Get<GameObject>(prefabID), new Vector3(x, y), Quaternion.identity, instanceParent);
        prefab.GetComponent<T>().SetClass(classReference);
        instanceHash.Add(classReference.InstanceID, prefab);
    }

    private static void AddItemToReferenceHash(string key, string name, float currencyValue, int maxStack, Sprite sprite) { referenceHash.Add(key, new Item(key, name, currencyValue, maxStack, sprite)); }
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
        AudioClip clip = Get<AudioClip>(id);
        if (clip == null) { return; }

        GameObject GO = new GameObject(id);
        AudioSource AS = GO.AddComponent<AudioSource>();
        AS.clip = clip; AS.volume = volume; AS.pitch = pitch; AS.Play();
        Destroy(GO, clip.length);
    }
    public static float ApproachValue(float currentValue, float targetValue, float rate) { return (currentValue + rate * (targetValue - currentValue)); }

    private static GameObject BuildUI(string name, Vector2 pos, Vector2 scale, Vector2 sizeDelta)
    {
        GameObject output = new GameObject(name); output.transform.parent = CanvasT;
        RectTransform RT = output.AddComponent<RectTransform>();
        output.transform.localPosition = pos; output.transform.localScale = scale; RT.sizeDelta = sizeDelta;
        return (output);
    }
    public static Image BuildUIImage(string name, Vector2 pos, Vector2 sizeDelta, string spriteID, Color color)
    {
        GameObject ui = BuildUI(name, pos, Vector2.one, sizeDelta);
        Image I = ui.AddComponent<Image>(); I.sprite = Get<Sprite>(spriteID); I.color = color; I.type = Image.Type.Sliced;
        return (I);
    }
    public static Text BuildUIText(string name, Vector2 pos, Vector2 sizeDelta, string text, int fontSize, TextAnchor alignment, string fontID, FontStyle fontStyle, Color color, int resolution)
    {
        GameObject ui = BuildUI(name, pos, Vector2.one / resolution, sizeDelta * resolution);
        Text T = ui.AddComponent<Text>(); T.text = text; T.fontSize = fontSize * resolution; T.alignment = alignment; T.font = Get<Font>(fontID); T.fontStyle = fontStyle; T.color = color;
        return (T);
    }
    //public static void BuildUISlider
}
