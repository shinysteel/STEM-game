using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const int CAMERA_Z_DISTANCE = -5;
    [Header("Gameobject References")]
    [SerializeField] private Transform visualT;
    [SerializeField] private GameObject scannerGO;

    private const float TOP_OF_MAP = 7.5f;
    private const float PITCH_BLACK_DEPTH = -42.5f;

    [Header("Dive Data")]
    public float time;
    public int depth;
    public float weight;
    public float temperature;
    public float darkness;

    private Inventory inventory;

    public PlayerMovement playerMovement;
    public PlayerUI playerUI;
    public PlayerScanner playerScanner;
    private void Awake()
    {
        Physics2D.gravity = new Vector2(0f, 0f);
        playerMovement = GetComponent<PlayerMovement>(); playerMovement.player = this;
        playerUI = GetComponent<PlayerUI>(); playerUI.player = this;
        playerScanner = scannerGO.GetComponent<PlayerScanner>(); playerScanner.player = this;
    }
    private void Start()
    {
        inventory = new Inventory(4);
        inventory.AddItems(GC.Get<Item>("item:fish_research_paper"), 7);
        inventory.AddItems(GC.Get<Item>("item:coral_research_paper"), 12);
        inventory.RemoveItems(1, 25);
    }

    private void LateUpdate() { Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_Z_DISTANCE); }
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        visualT.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

        if (Input.GetKeyDown("e")) { playerScanner.TryNewScannerState(true); }
        if (Input.GetKeyUp("e")) { playerScanner.TryNewScannerState(false); }
        if (!Input.GetKey("e") && Input.GetMouseButton(0)) { playerMovement.Move(); }

        // time
        depth = Mathf.Abs(Mathf.Clamp(Mathf.RoundToInt((transform.position.y - TOP_OF_MAP)), -999, 0));
        // weight
        // temperature
        darkness = (transform.position.y - TOP_OF_MAP) / (PITCH_BLACK_DEPTH - TOP_OF_MAP);

        // time
        playerUI.SetDepth(depth);
        // weight
        // temperature
        playerUI.SetDarkness(darkness);
    }
}

/* Gameplay mechanics:

1. Buoyancy. Pulls you up near the water's surface. Use of BCD (Buoyancy Compensator) 
2. Sinking. Pressure compresses air inside lungs, falling at a rate amplified by depth.
3. Warmth.
4.

000 meters 030 meters
010 meters 040 meters
020 meters 050 meters

The goal is to maintain neutral buoyancy throughout a dive by changing volume and/or weight. 
Obtained when upthrust caused by downwards displaced water is counterbalanced by the diver's weight.
More diver weight is required on ocean dives (opposed to lakes, rivers) due the dissolved salts making water weigh more. 
Ocean water is on average 1.025 times heavier then fresh water. 
Cenotes are a fascinating phenomena (halocline) where horizontal layers of ocean and fresh water exist but do not mix. 
Neoprene equipment (thermal protection) compresses and reduces volume at greater depths.
Neoprene equipment 1/2, 2/3, 3/3 buoyancy at depths 33, 66, and 100 feet where it also loses its thermal isolation properties. 


*/
