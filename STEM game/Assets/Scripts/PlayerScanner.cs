using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShinyOwl.Utils;

public class PlayerScanner : MonoBehaviour
{
    public Player player;
    [SerializeField] private bool isScanning;
    [SerializeField] private List<int> scannedInstances = new List<int>();
    [SerializeField] private bool isScanningCreature;
    [SerializeField] private int targetInstanceID;
    [SerializeField] private Transform scannedCreatureT;
    [SerializeField] private float researchTimer;
    [SerializeField] private float timeToResearch = 1.5f;
    [SerializeField] private GameObject researchVisual;

    private void Start()
    {
        researchVisual.SetActive(false);
    }
    private void Update()
    {
        if (!player.DoUpdate()) return;
        if (targetInstanceID != -1)
        {
            if (!scannedInstances.Contains(targetInstanceID))
            {
                isScanning = false;
                TryNewScannerState(false);
            }
        }   
        if (isScanning)
        {
            if (scannedCreatureT != null) Debug.DrawLine(player.transform.position, scannedCreatureT.position, Color.red);
            researchTimer += Time.deltaTime;
        }
        float researchRatio = Mathf.Clamp((researchTimer / timeToResearch), 0f, 1f);
        if (researchRatio >= 1f)
        {
            CreatureBehaviour behaviour = scannedCreatureT.GetComponent<CreatureBehaviour>();
            CreatureBase creature = behaviour.Creature;
            InventoryItem newItem = GC.GetReference<InventoryItem>(creature.ResearchItemID);
            if (!player.playerUI.playerInventory.HasSpaceforItem(newItem))
            {
                UtilsClass.CreateWorldTextPopup("Inventory is full!", player.transform, Vector3.zero, new Vector3(0f, 2.5f), 1.5f, 4, Color.red);
                TryNewScannerState(false);
                return;
            }
            if (creature.TryDeclareResearched())
            {
                player.playerUI.ObtainItems(newItem, 1);
                scannedInstances.Remove(creature.InstanceID);
                player.playerUI.SetAnalysis(creature.Name, creature.Description, behaviour.GetSpriteIcon());
                player.playerUI.analysisAC.Play("analysis_display_show", -1, 0f);
                GC.PlaySound("sound:alert1", 0.6f, 1f, pitchRandomness: 0f);
            }
            TryNewScannerState(false);
        }
    }

    public void TryNewScannerState(bool newState)
    {
        researchTimer = 0f;
        if (isScanningCreature && newState == true)
        {
            player.playerUI.CreateFillBar(() => { return researchTimer / timeToResearch; }, () => { return isScanning == false; }, "Scanning", Color.green);
            researchVisual.SetActive(true);
            isScanning = true;
            GC.PlaySound("sound:scanner_on1", 0.8f, 1f);
            GC.PlaySound("sound:scanner_scanning1", 0.8f, 1f, cutoff: timeToResearch);
            player.playerAnimator.PlayAnimation("player_scan");

            float smallestDist = Mathf.Infinity;
            int closestInstanceID = -1;
            foreach (int id in scannedInstances)
            {
                GameObject GO = GC.GetInstanceByID(id);
                if (GO == null) { Debug.Log($"ERROR: Invalid InstanceID of {id} during GetInstanceByID() call."); return; }
                float dist = Vector3.Distance(player.transform.position, GO.transform.position);
                if (dist < smallestDist) { smallestDist = dist; closestInstanceID = id; }
            }
            targetInstanceID = closestInstanceID;
            if (targetInstanceID != -1) scannedCreatureT = GC.GetInstanceByID(targetInstanceID).transform;
        }
        else
        {
            researchVisual.SetActive(false);
            isScanning = false;
            targetInstanceID = -1;
        }
    }
    public bool IsScanning()
    {
        return isScanning;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Creature")
        {
            CreatureBase creature = collision.GetComponent<CreatureBehaviour>().Creature;
            if (creature.IsResearched) return;
            int instanceID = creature.InstanceID;
            if (scannedInstances.Contains(instanceID) == false) { scannedInstances.Add(instanceID); }
            isScanningCreature = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Creature")
        {
            CreatureBase creature = collision.GetComponent<CreatureBehaviour>().Creature;
            int instanceID = creature.InstanceID;
            if (scannedInstances.Contains(instanceID)) { scannedInstances.Remove(instanceID); }
            if (scannedInstances.Count <= 0)
            {
                isScanningCreature = false;
                TryNewScannerState(false);
            }
        }
    }
}
