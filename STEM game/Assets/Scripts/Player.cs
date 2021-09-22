using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using ShinyOwl.Utils;

public class Player : MonoBehaviour
{
    private const int CAMERA_Z_DISTANCE = -5;
    [Header("Gameobject References")]
    public Transform visualT;
    [SerializeField] private GameObject scannerGO;

    public const float TOP_OF_MAP = -8.5f;
    public const float PITCH_BLACK_DEPTH = -190f;
    public const float LUNG_CAPACITY = 35f;

    [Header("Dive Data")]
    public float time;
    public int depth;
    public float weight;
    public float temperature;
    public float darkness;
    public float balance;
    public float oxygen;
    [Header("Other Data")]
    [SerializeField] private float bcInflation;
    public float BCInflation
    {
        get { return bcInflation; }
        set { bcInflation = Mathf.Clamp(value, -BC_INFLATION_BOUND, BC_INFLATION_BOUND); }
    }
    public const float BC_INFLATION_BOUND = 5f;

    public IPlayerMoveable playerMovement;
    [SerializeField] private bool paused;
    public PlayerUI playerUI;
    public PlayerScanner playerScanner;
    public PlayerAnimator playerAnimator;
    private Vector3 spawnPos;
    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>(); playerUI.player = this;
        playerScanner = scannerGO.GetComponent<PlayerScanner>(); playerScanner.player = this;
        playerAnimator = GetComponent<PlayerAnimator>(); playerAnimator.player = this;
    }
    private void Start()
    {
        PlayerMovementWalking playerMovementWalking = gameObject.AddComponent<PlayerMovementWalking>();
        playerMovementWalking.player = this;
        playerMovement = playerMovementWalking;

        paused = GC.paused;
        GC.OnPause += Player_UpdatePauseState;
        spawnPos = transform.position;
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(Mathf.Clamp(transform.position.x, -14.45f, 35.5f), transform.position.y, CAMERA_Z_DISTANCE);
    }
    private void Update()
    {
        if (!DoUpdate()) return;
        if (oxygen <= 0f)
        {
            transform.position = spawnPos;
            oxygen = LUNG_CAPACITY;
            GC.PlaySound("sound:bubble1", 0.7f, 1f, pitchRandomness: 0f);
            UtilsClass.CreateWorldTextPopup("You ran out of breath!", transform, Vector3.zero, new Vector3(0f, 2.5f), 1.5f, 4, Color.red);
            return;
        }

        playerMovement = playerMovement.UpdateMovementType();

        if (Input.GetKeyDown("e")) { playerScanner.TryNewScannerState(true); }
        if (Input.GetKeyUp("e")) { playerScanner.TryNewScannerState(false); }
        playerMovement.Move();

        // time
        depth = Mathf.Abs(Mathf.Clamp(Mathf.RoundToInt((transform.position.y - TOP_OF_MAP) * 0.5f), -999, 0));
        // weight
        // temperature
        darkness = (transform.position.y - TOP_OF_MAP) / (PITCH_BLACK_DEPTH - TOP_OF_MAP);
        // balance

        // time
        playerUI.SetDepth(depth);
        // weight
        // temperature
        playerUI.SetDarkness(darkness);
        playerUI.SetBalance(balance);
    }

    public void AddBalance(float amount)
    {
        balance += amount;
    }
    public void Player_UpdatePauseState(object sender, EventArgs e)
    {
        if (this == null) return;
        playerMovement.RB.simulated = !playerMovement.RB.simulated;
        paused = !paused;
    }
    public bool DoUpdate()
    {
        return !paused;
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
