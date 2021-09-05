using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GC : MonoBehaviour
{
    private static Hashtable referenceHash;
    private static Hashtable instanceHash;
    private static Transform instanceParent;
    private static int instanceIDCounter = 0;
    public static Transform CanvasT; public Transform _CanvasT;
    public static event EventHandler OnPause;
    public static bool paused = true;
    [SerializeField] private GameObject titleScreenGO;
    [SerializeField] private GameObject pauseScreenGO;
    [SerializeField] private GameObject gameplayInterfaceGO;
    [SerializeField] private GameObject inventoryScreenGO;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider audioSlider;
    private static float musicVolume = 0.5f;
    private static float audioVolume = 0.5f;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private static float audioMultiplier = 0.5f;

    private void Awake()
    {
        referenceHash = new Hashtable();
        referenceHash.Add("sound:swim_dash1", Resources.Load<AudioClip>("Sounds/swim_dash1"));
        referenceHash.Add("sound:swim_dash2", Resources.Load<AudioClip>("Sounds/swim_dash2"));
        referenceHash.Add("sound:splash1", Resources.Load<AudioClip>("Sounds/splash1"));
        referenceHash.Add("sound:jump_splash1", Resources.Load<AudioClip>("Sounds/jump_splash1"));
        referenceHash.Add("sound:jump_splash2", Resources.Load<AudioClip>("Sounds/jump_splash2"));
        referenceHash.Add("sound:scanner_on1", Resources.Load<AudioClip>("Sounds/scanner_on1"));
        referenceHash.Add("sound:scanner_scanning1", Resources.Load<AudioClip>("Sounds/scanner_scanning1"));
        referenceHash.Add("sound:game_pause1", Resources.Load<AudioClip>("Sounds/game_pause1"));
        referenceHash.Add("sound:swimming_loop1", Resources.Load<AudioClip>("Sounds/swimming_loop1"));
        referenceHash.Add("sound:earn_money1", Resources.Load<AudioClip>("Sounds/earn_money1"));
        referenceHash.Add("sound:ui_click1", Resources.Load<AudioClip>("Sounds/ui_click1"));
        referenceHash.Add("sound:menu_close1", Resources.Load<AudioClip>("Sounds/menu_close1"));
        referenceHash.Add("sound:alert1", Resources.Load<AudioClip>("Sounds/alert1"));
        referenceHash.Add("sound:bubble1", Resources.Load<AudioClip>("Sounds/bubble1"));
        referenceHash.Add("sprite:builtin:background", null);
        referenceHash.Add("sprite:builtin:knob", null);
        referenceHash.Add("font:arial", Resources.GetBuiltinResource<Font>("Arial.ttf"));
        referenceHash.Add("font:pixels", Resources.Load<Font>("Fonts/pixels"));
        referenceHash.Add("font:pixels_small", Resources.Load<Font>("Fonts/pixels_small"));
        referenceHash.Add("prefab:fish", Resources.Load<GameObject>("Prefabs/Fish"));
        referenceHash.Add("prefab:coral", Resources.Load<GameObject>("Prefabs/Coral"));
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/items1"), "sprite:coral_research_paper", 
            "sprite:crab_research_paper", "sprite:fish_research_paper", "sprite:squid_research_paper");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/species1"), "sprite:blue_fish",
            "sprite:crab_attack", "sprite:crab_idle", "sprite:giant_squid_swim1",
            "sprite:giant_squid_swim2", "sprite:orange_coral_idle1", "sprite:orange_coral_idle2",
            "sprite:purple_fish", "sprite:yellow_coral_idle1", "sprite:yellow_coral_idle2");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/icons1"), "sprite:dollar_icon", "sprite:gear_icon");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/black_bass1"), "sprite:black_bass");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/orange_cup_coral1"), "sprite:orange_cup_coral");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/scanned_icon1"), "sprite:scanned_icon");
        AddItemToReferenceHash("item:fish_research_paper", "Fish Research Paper", 2f, 3, "sprite:fish_research_paper");
        AddItemToReferenceHash("item:coral_research_paper", "Coral Research Paper", 4.5f, 2, "sprite:coral_research_paper");
        AddItemToReferenceHash("item:squid_research_paper", "Squid Research Paper", 50f, 1, "sprite:squid_research_paper");
        referenceHash.Add("creature:blue_fish", new Fish("sprite:blue_fish", 0.05f, "Blue Fish", null, "item:fish_research_paper", null, -1, "creature:blue_fish"));
        referenceHash.Add("creature:purple_fish", new Fish("sprite:purple_fish", 0.075f, "Purple Fish", null, "item:fish_research_paper", null, -1, "creature:purple_fish"));
        referenceHash.Add("creature:giant_squid", new Fish("sprite:giant_squid_swim1", 0.085f, "Giant Squid", "OMG! It's a Giant Squid!", "item:squid_research_paper", null, -1, "creature:giant_squid"));
        referenceHash.Add("creature:orange_coral", new Coral(new string[] { "sprite:orange_coral_idle1", "sprite:orange_coral_idle2" }, "Orange Coral", null, "item:coral_research_paper", null, -1, "creature:orange_coral"));
        referenceHash.Add("creature:yellow_coral", new Coral(new string[] { "sprite:yellow_coral_idle1", "sprite:yellow_coral_idle2" }, "Yellow Coral", null, "item:coral_research_paper", null, -1, "creature:yelow_coral"));
        referenceHash.Add("creature:black_bass", new Fish("sprite:black_bass", 0.05f, "Black Bass", null, "item:fish_research_paper", null, -1, "creature:black_bass"));
        referenceHash.Add("creature:orange_cup_coral", new Coral(new string[] { "sprite:orange_cup_coral", "sprite:orange_cup_coral" }, "Orange Cup Coral", null, "item:coral_research_paper", null, -1, "creature:orange_cup_coral"));

        instanceHash = new Hashtable();
        instanceParent = new GameObject("Instances").transform;

        CanvasT = _CanvasT;
        for (int i = 0; i < 4; i++)
        {
            CreateInstance("creature:blue_fish", -10f, -15f);
            CreateInstance("creature:purple_fish", -10f, -15f);
            CreateInstance("creature:black_bass", -10f, -15f);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateInstance("creature:blue_fish", 9.3f, -23.3f);
            CreateInstance("creature:purple_fish", 9.3f, -23.3f);
            CreateInstance("creature:black_bass", 9.3f, -23.3f);
        }
        for (int i = 0; i < 3; i++)
        {
            CreateInstance("creature:blue_fish", 3.7f, -54.8f);
            CreateInstance("creature:purple_fish", 3.7f, -54.8f);
            CreateInstance("creature:black_bass", 3.7f, -54.8f);
        }
        for (int i = 0; i < 3; i++)
        {
            CreateInstance("creature:blue_fish", -12.05f, -59.24f);
            CreateInstance("creature:purple_fish", -12.05f, -59.24f);
            CreateInstance("creature:black_bass", -12.05f, -59.24f);
        }
        CreateInstance("creature:giant_squid", -3f, -27f);
        CreateInstance("creature:giant_squid", 3.7f, -54.8f);
        CreateInstance("creature:orange_coral", -12f, -25.75f);
        CreateInstance("creature:yellow_coral", -14.6f, -25.9f);
        CreateInstance("creature:orange_cup_coral", -16.7f, -25.5f);
        CreateInstance("creature:orange_cup_coral", 22.42f, -26.83f);
        CreateInstance("creature:orange_cup_coral", 24.69f, -26.55f);
        CreateInstance("creature:orange_cup_coral", 27.92f, -26.78f);
        CreateInstance("creature:orange_coral", 41.97f, -25.93f);
        CreateInstance("creature:yellow_coral", 44.8f, -25.25f);
        CreateInstance("creature:yellow_coral", 22.4f, -52.3f);
        CreateInstance("creature:yellow_coral", 24.7f, -51.73f);
        CreateInstance("creature:yellow_coral", 28.96f, -51.9f);
        CreateInstance("creature:orange_cup_coral", -2.69f, -64.28f);
        CreateInstance("creature:orange_cup_coral", -7.37f, -64.83f);
        CreateInstance("creature:orange_cup_coral", -12.23f, -63.64f);
    }
    private void Start()
    {
        musicSlider.onValueChanged.AddListener(delegate { OnMusicSliderChange(); });
        audioSlider.onValueChanged.AddListener(delegate { OnAudioSliderChange(); });
        OnPause += GC_RespondToPause;
        OpenTitleScreen();
    }

    private void GC_RespondToPause(object sender, EventArgs e)
    {
        if (this == null) return;
        paused = !paused;
        pauseScreenGO.SetActive(!pauseScreenGO.activeSelf);
        gameplayInterfaceGO.SetActive(!gameplayInterfaceGO.activeSelf);
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
    public static void PlaySound(string id, float volume = 1f, float pitch = 1f, int variation = 0, float startTime = 0f, float? cutoff = null, float pitchRandomness = 0.1f)
    {
        if (variation > 0)
        {
            int startNum; if (!int.TryParse(id[id.Length - 1].ToString(), out startNum)) { return; }
            id = id.Substring(0, id.Length - 1) + UnityEngine.Random.Range(startNum, startNum + variation + 1);
        }
        AudioClip clip = GetReference<AudioClip>(id);
        if (clip == null) { return; }

        GameObject GO = new GameObject(id);
        AudioSource AS = GO.AddComponent<AudioSource>();
        volume = volume * audioMultiplier;
        AS.clip = clip; AS.volume = volume; AS.pitch = UnityEngine.Random.Range(pitch - pitchRandomness, pitch + pitchRandomness); AS.time = startTime; AS.Play(); 
        float duration = cutoff == null ? clip.length - startTime : (float)cutoff;
        Destroy(GO, duration);
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
    private static GameObject BuildUI(string name, Transform parent, Vector2 pos, Vector2 scale, Vector2 sizeDelta)
    {
        GameObject output = new GameObject(name);
        if (parent is null) output.transform.SetParent(CanvasT);
        else output.transform.SetParent(parent);
        RectTransform RT = output.AddComponent<RectTransform>();
        output.transform.localPosition = pos; output.transform.localScale = scale; RT.sizeDelta = sizeDelta;
        return (output);
    }
    public static RectTransform BuildUIEmpty(string name, Transform parent, Vector2 pos)
    {
        return BuildUI(name, parent, pos, Vector2.one, new Vector2(100f, 100f)).GetComponent<RectTransform>();
    }
    public static Image BuildUIImage(string name, Transform parent, Vector2 pos, Vector2 sizeDelta, string spriteID, Color color, bool isCutout = false)
    {
        GameObject ui = BuildUI(name, parent, pos, Vector2.one, sizeDelta);
        Image I;
        if (isCutout) I = ui.AddComponent<CutoutMaskUI>();
        else I = ui.AddComponent<Image>();
        I.sprite = GetReference<Sprite>(spriteID); I.color = color; I.type = Image.Type.Sliced;
        return (I);
    }
    public static Text BuildUIText(string name, Transform parent, Vector2 pos, Vector2 sizeDelta, string text, int fontSize, TextAnchor alignment, string fontID, FontStyle fontStyle, Color color, int resolution)
    {
        GameObject ui = BuildUI(name, parent, pos, Vector2.one / resolution, sizeDelta * resolution);
        Text T = ui.AddComponent<Text>(); T.text = text; T.fontSize = fontSize * resolution; T.alignment = alignment; T.font = GetReference<Font>(fontID); T.fontStyle = fontStyle; T.color = color; T.font = GetReference<Font>("font:pixels_small");
        return (T);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    public void OpenTitleScreen()
    {
        titleScreenGO.SetActive(true);
        pauseScreenGO.SetActive(false);
        gameplayInterfaceGO.SetActive(false);
        inventoryScreenGO.SetActive(false);
    }
    public void StartGame()
    {
        titleScreenGO.SetActive(false);
        UpdatePausedState();
        pauseScreenGO.SetActive(false);
        gameplayInterfaceGO.SetActive(true);
    }
    public static void UpdatePausedState()
    {
        OnPause?.Invoke(null, EventArgs.Empty);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void UIClickInto()
    {
        PlaySound("sound:ui_click1", 0.3f, 1f, pitchRandomness: 0f);
    }
    public void UIClickOut()
    {
        PlaySound("sound:menu_close1", 0.3f, 1f, pitchRandomness: 0f);
    }
    public void UpdateInventoryState()
    {
        inventoryScreenGO.SetActive(!inventoryScreenGO.activeSelf);
    }
    
    public void OnMusicSliderChange()
    {
        bgmAudioSource.volume = musicSlider.value;
    }
    public void OnAudioSliderChange()
    {
        GC.audioMultiplier = audioSlider.value;
    }
}
