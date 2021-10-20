using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShinyOwl.Utils;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using Random = UnityEngine.Random;

public class GC : MonoBehaviour
{
    private static Hashtable referenceHash;
    private static Hashtable instanceHash;
    private static Transform instanceParent; public static Transform InstanceParent { get { return instanceParent; } }
    private static int instanceIDCounter = 0;
    public static Transform CanvasT; public Transform _CanvasT;
    public static Canvas CanvasC;
    public static CanvasScaler CanvasS;
    public static Transform PlayerT; public Transform _PlayerT;
    public static event EventHandler OnPause;
    public static bool paused = true;
    [SerializeField] private GameObject titleScreenGO;
    [SerializeField] private GameObject pauseScreenGO;
    [SerializeField] private GameObject gameplayInterfaceGO;
    [SerializeField] private GameObject inventoryScreenGO;
    [SerializeField] private GameObject codexScreenGO;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider audioSlider;
    private static float musicVolume = 0.5f;
    private static float audioVolume = 0.5f;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private static float audioMultiplier = 0.5f;
    public static GameObject DEMain; public GameObject _DEMain;
    public static TextMeshProUGUI DESpeakerText; public TextMeshProUGUI _DESpeakerText;
    public static Image DEPortrait; public Image _DEPortrait;
    public static TextMeshProUGUI DEDialogueText; public TextMeshProUGUI _DEDialogueText;

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
        referenceHash.Add("sound:buoyancy_adjust1", Resources.Load<AudioClip>("Sounds/buoyancy_adjust1"));
        referenceHash.Add("sound:fish_dart1", Resources.Load<AudioClip>("Sounds/fish_dart1"));
        referenceHash.Add("sound:creature_scream1", Resources.Load<AudioClip>("Sounds/creature_scream1"));
        referenceHash.Add("sprite:builtin:background", null);
        referenceHash.Add("sprite:builtin:knob", null);
        referenceHash.Add("font:arial", Resources.GetBuiltinResource<Font>("Arial.ttf"));
        referenceHash.Add("font:pixels", Resources.Load<Font>("Fonts/pixels"));
        referenceHash.Add("font:pixels_small", Resources.Load<Font>("Fonts/pixels_small"));
        referenceHash.Add("prefab:fish", Resources.Load<GameObject>("Prefabs/Fish"));
        referenceHash.Add("prefab:coral", Resources.Load<GameObject>("Prefabs/Coral"));
        referenceHash.Add("prefab:developer:popup_sprite", Resources.Load<GameObject>("Prefabs/Developer/Popup Sprite"));
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/items1"), "sprite:coral_research_paper", 
            "sprite:crab_research_paper", "sprite:fish_research_paper", "sprite:squid_research_paper");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/species1"), "sprite:blue_fish",
            "sprite:old_crab_attack", "sprite:old_crab_idle", "sprite:giant_squid_swim1",
            "sprite:giant_squid_swim2", "sprite:orange_coral_idle1", "sprite:orange_coral_idle2",
            "sprite:purple_fish", "sprite:yellow_coral_idle1", "sprite:yellow_coral_idle2");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/icons1"), "sprite:dollar_icon", "sprite:gear_icon");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/widemouth_bass1"), "sprite:widemouth_bass");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/orange_cup_coral1"), "sprite:orange_cup_coral");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/western_blue_groper1"), "sprite:western_blue_groper");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/snapper1"), "sprite:snapper");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/squid1"), "sprite:squid_idle");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/squid2"), "sprite:squid_bend");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/bluefish1"), "sprite:bluefish");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/atlantic_croaker1"), "sprite:atlantic_croaker");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/species2"), "sprite:crab_idle", "sprite:crab_prepare", "sprite:hammerhead_shark_angry", "sprite:hammerhead_shark_idle");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/scanned_icon1"), "sprite:scanned_icon");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/emotions1"), "sprite:alert_icon");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/captain_kai_dennick_portrait1"), "sprite:captain_kai_dennick_portrait_neutral");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/dr_chelsea_reeding_portrait1"), "sprite:dr_chelsea_reeding_portrait_neutral");
        AddSpriteSheetToReferenceHash(Resources.LoadAll<Sprite>("Sprites/professor_christopher_tazien_portrait1"), "sprite:professor_christopher_tazien_portrait_neutral");
        AddItemToReferenceHash("item:fish_research_paper", "Fish Research Paper", 2f, 3, 0.1f, "sprite:fish_research_paper");
        AddItemToReferenceHash("item:shark_research_paper", "Shark Research Paper", 75f, 2, 0.1f, "sprite:fish_research_paper");
        AddItemToReferenceHash("item:coral_research_paper", "Coral Research Paper", 4.5f, 2, 0.1f, "sprite:coral_research_paper");
        AddItemToReferenceHash("item:squid_research_paper", "Squid Research Paper", 50f, 1, 0.1f, "sprite:squid_research_paper");
        AddItemToReferenceHash("item:small_ballast_weight", "Small Ballast Weight", 5f, 6, 2.5f, "sprite:alert_icon");
        referenceHash.Add("creature:squid", new Fish("sprite:squid_idle", 0.08f, 0.5f, 7f, 4f, 0.2f, "Giant Squid", "OMG! It's a Giant Squid!", "item:squid_research_paper", GetReference<Sprite>("sprite:squid_idle"), null, -1, "creature:squid"));
        referenceHash.Add("creature:widemouth_bass", new Fish("sprite:widemouth_bass", 0.05f, 2f, 4f, 2.7f, 0.3f, "Widemouth Bass", null, "item:fish_research_paper", GetReference<Sprite>("sprite:widemouth_bass"), null, -1, "creature:widemouth_bass"));
        referenceHash.Add("creature:snapper", new Fish("sprite:snapper", 0.05f, 1.5f, 3f, 3f, 0.3f, "Snapper", null, "item:fish_research_paper", GetReference<Sprite>("sprite:snapper"), null, -1, "creature:snapper"));
        referenceHash.Add("creature:bluefish", new Fish("sprite:bluefish", 0.05f, 1.5f, 2.5f, 2.75f, 0.4f, "Bluefish", null, "item:fish_research_paper", GetReference<Sprite>("sprite:bluefish"), null, -1, "creature:bluefish"));
        referenceHash.Add("creature:atlantic_croaker", new Fish("sprite:atlantic_croaker", 0.04f, 3f, 0.5f, 2.75f, 0.35f, "Atlantic Croaker", null, "item:fish_research_paper", GetReference<Sprite>("sprite:atlantic_croaker"), null, -1, "creature:atlantic_croaker"));
        referenceHash.Add("creature:western_blue_groper", new Fish("sprite:western_blue_groper", 0.05f, 1.2f, 4f, 3.4f, 0.25f, "Western Blue Groper", null, "item:fish_research_paper", GetReference<Sprite>("sprite:western_blue_groper"), null, -1, "creature:western_blue_groper"));
        referenceHash.Add("creature:hammerhead_shark", new Shark("sprite:hammerhead_shark_idle", 0.05f, 0.2f, 3f, 5f, 0.3f, "Hammerhead Shark", null, "item:shark_research_paper", GetReference<Sprite>("sprite:hammerhead_shark_idle"), null, -1, "creature:hammerhead_shark"));
        referenceHash.Add("creature:orange_coral", new Coral(new string[] { "sprite:orange_coral_idle1", "sprite:orange_coral_idle2" }, "Orange Coral", null, "item:coral_research_paper", GetReference<Sprite>("sprite:orange_coral_idle1"), null, -1, "creature:orange_coral"));
        referenceHash.Add("creature:yellow_coral", new Coral(new string[] { "sprite:yellow_coral_idle1", "sprite:yellow_coral_idle2" }, "Yellow Coral", null, "item:coral_research_paper", GetReference<Sprite>("sprite:yellow_coral_idle1"), null, -1, "creature:yellow_coral"));
        referenceHash.Add("creature:orange_cup_coral", new Coral(new string[] { "sprite:orange_cup_coral", "sprite:orange_cup_coral" }, "Orange Cup Coral", null, "item:coral_research_paper", GetReference<Sprite>("sprite:orange_cup_coral"), null, -1, "creature:orange_cup_coral"));

        instanceHash = new Hashtable();
        instanceParent = new GameObject("Instances").transform;

        CanvasT = _CanvasT;
        CanvasC = CanvasT.GetComponent<Canvas>();
        CanvasS = CanvasT.GetComponent<CanvasScaler>();
        PlayerT = _PlayerT;
        DEMain = _DEMain;
        DESpeakerText = _DESpeakerText;
        DEPortrait = _DEPortrait;
        DEDialogueText = _DEDialogueText;

        for (int i = 0; i < 5; i++)
        {
            float randX = Random.Range(-2.5f, -7.5f);
            float randY = Random.Range(-15f, -25f);
            CreateInstance("creature:widemouth_bass", randX, randY);
            CreateInstance("creature:snapper", randX, randY);
            CreateInstance("creature:bluefish", randX, randY);
            CreateInstance("creature:atlantic_croaker", randX, randY);
            CreateInstance("creature:western_blue_groper", randX, randY);
        }
        //CreateInstance("creature:hammerhead_shark", 17f, -22f);
        CreateInstance("creature:squid", -3f, -27f);
        CreateInstance("creature:orange_coral", -12f, -25.75f);
        CreateInstance("creature:yellow_coral", -14.6f, -25.9f);
        CreateInstance("creature:orange_cup_coral", -16.7f, -25.5f);
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
    private static void AddItemToReferenceHash(string key, string name, float currencyValue, int maxStack, float weight, string spriteID)
    {
        referenceHash.Add(key, new InventoryItem(key, name, currencyValue, maxStack, weight, spriteID));
    }
    private static void AddSpriteSheetToReferenceHash(Sprite[] spriteSheet, params string[] keys)
    {
        for (int i = 0; i < keys.Length; i++) { referenceHash.Add(keys[i], spriteSheet[i]); }
    }
    public static void PlaySound(string id, float volume = 1f, float pitch = 1f, int variation = 0, float startTime = 0f, float? cutoff = null, float pitchRandomness = 0.1f, float delay = 0f)
    {
        if (variation > 0)
        {
            int startNum; if (!int.TryParse(id[id.Length - 1].ToString(), out startNum)) { return; }
            id = id.Substring(0, id.Length - 1) + UnityEngine.Random.Range(startNum, startNum + variation + 1);
        }
        AudioClip clip = GetReference<AudioClip>(id);
        if (clip == null) { return; }

        FunctionTimer.Create(() =>
        {
            GameObject GO = new GameObject(id);
            AudioSource AS = GO.AddComponent<AudioSource>();
            volume = volume * audioMultiplier;
            AS.clip = clip; AS.volume = volume; AS.pitch = UnityEngine.Random.Range(pitch - pitchRandomness, pitch + pitchRandomness); AS.time = startTime; AS.Play();
            float duration = cutoff == null ? clip.length - startTime : (float)cutoff;
            Destroy(GO, duration);
        }, delay);
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
        GetVelocityToPos(castPos, mousePos, force, out velocityX, out velocityY);
    }
    public static void GetVelocityToPos(Vector3 castPos, Vector3 targetPos, float force, out float velocityX, out float velocityY)
    {
        Vector2 v1 = new Vector2(targetPos.x, targetPos.y);
        Vector2 v2 = new Vector2(castPos.x, castPos.y);
        Vector2 dir = (v1 - v2).normalized * force;
        velocityX = dir.x;
        velocityY = dir.y;
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
    public static Vector2 NormalizeVector(Vector2 v)
    {
        float magnitude = Mathf.Sqrt((Mathf.Pow(v.x, 2f) + Mathf.Pow(v.y, 2f)));
        return new Vector2(v.x / magnitude, v.y / magnitude);
    }
    public static void CreatePopupSprite(string spriteID, Transform parent, Vector3 pos)
    {
        GameObject popup = Instantiate(GetReference<GameObject>("prefab:developer:popup_sprite"), parent);
        popup.transform.localPosition = pos;
        popup.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetReference<Sprite>(spriteID);
        Destroy(popup, 3f);
    }
    public static float RoundUpNearestTen(float num)
    {
        int rem = Mathf.RoundToInt(num) % 10;
        return num - rem + 10;
    }
    public static int RoundUpNearestTen(int num)
    {
        return (int)RoundUpNearestTen((float)num);
    }
    public static float GetUIScale()
    {
        return Mathf.Pow(CanvasC.pixelRect.width / CanvasS.referenceResolution.x, 1f - CanvasS.matchWidthOrHeight) *
               Mathf.Pow(CanvasC.pixelRect.height / CanvasS.referenceResolution.y, CanvasS.matchWidthOrHeight);
    }
    public static void DialogueEvent(Conversation conversation)
    {
        bool isDirty = true;
        int convIndex = 0;
        string targetDialogue = "";
        int dialIndex = 0;
        float punctuationDelay = 0.1f;
        float letterDelay = 0.04f;
        float letterTimer = 0f;
        FunctionUpdater.Create(() =>
        {
            // Test if text / image objects need value update.
            if (isDirty)
            {
                GC.DESpeakerText.text = conversation.Speakers[convIndex];
                GC.DEPortrait.sprite = conversation.Portraits[convIndex];
                GC.DEDialogueText.text = "";
                targetDialogue = conversation.Dialogue[convIndex];
                dialIndex = 0;
                letterTimer = 0f;
                DEMain.SetActive(true);
                isDirty = false;
            }
            // Slowly build dialogue text.
            if (GC.DEDialogueText.text != targetDialogue)
            {
                letterTimer += Time.deltaTime;
                if (letterTimer >= letterDelay)
                {
                    char nextChar = targetDialogue[dialIndex];

                    GC.DEDialogueText.text = targetDialogue.Substring(0, dialIndex++);
                    letterTimer -= letterDelay;
                }
            }
            // Detecting conversation continue.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                convIndex++;
                if (convIndex == conversation.Length)
                {
                    DEMain.SetActive(false);
                    GC.DESpeakerText.text = "";
                    GC.DEPortrait.sprite = null;
                    GC.DEDialogueText.text = "";
                    return true;
                }
                isDirty = true;
            }
            return false;
        });
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
        codexScreenGO.SetActive(false);
        DEMain.SetActive(false);
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
    public void UpdateCodexState()
    {
        codexScreenGO.SetActive(!codexScreenGO.activeSelf);
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

public class Conversation {
    private string[] speakers; public string[] Speakers { get { return speakers; } }
    private Sprite[] portraits; public Sprite[] Portraits { get { return portraits; } }
    private string[] dialogue; public string[] Dialogue { get { return dialogue; } }
    public int Length { get { return speakers.Length; } }

    public Conversation(string[] _Speakers, Sprite[] _Portraits, string[] _Dialogue)
    {
        speakers = _Speakers;
        portraits = _Portraits;
        dialogue = _Dialogue;
    }
}
