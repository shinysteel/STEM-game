using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScanner : MonoBehaviour
{
    public Player player;
    [SerializeField] private bool isScanning;
    [SerializeField] private List<int> scannedInstances = new List<int>();
    [SerializeField] private bool isScanningCreature;
    [SerializeField] private int targetInstanceID;
    [SerializeField] private GameObject researchUIGO;
    [SerializeField] private Transform researchBarT;
    [SerializeField] private SpriteRenderer scannedCreatureIcon;
    [SerializeField] private float researchTimer;
    [SerializeField] private float timeToResearch = 1.5f;

    private void Update()
    {
        if (targetInstanceID != -1)
        {
            if (!scannedInstances.Contains(targetInstanceID))
            {
                isScanning = false;
                TryNewScannerState(false);
            }
        }   
        if (isScanning) { researchTimer += Time.deltaTime; }
        float researchRatio = Mathf.Clamp((researchTimer / timeToResearch), 0f, 1f);
        researchBarT.localScale = new Vector3(researchRatio, 1f);
        if (researchRatio >= 1f)
        {
            player.playerUI.ObtainItems(GC.Get<Item>(GC.GetInstanceByID(targetInstanceID).GetComponent<CreatureBehaviour>().GetResearchItemID()), 1);
            TryNewScannerState(false);
        }
    }

    public void TryNewScannerState(bool newState)
    {
        researchTimer = 0f;
        if (isScanningCreature && newState == true)
        {
            isScanning = true;
            researchUIGO.SetActive(true);

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
            scannedCreatureIcon.sprite = GC.GetInstanceByID(targetInstanceID).GetComponent<CreatureBehaviour>().GetSpriteIcon();
        }
        else
        { 
            isScanning = false;
            researchUIGO.SetActive(false);
            targetInstanceID = -1;
        }
    }
    public bool IsScanning() { return isScanning; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Creature")
        {
            int instanceID = collision.GetComponent<CreatureBehaviour>().GetInstanceID();
            if (scannedInstances.Contains(instanceID) == false) { scannedInstances.Add(instanceID); }
            isScanningCreature = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Creature")
        {
            int instanceID = collision.GetComponent<CreatureBehaviour>().GetInstanceID();
            if (scannedInstances.Contains(instanceID)) { scannedInstances.Remove(instanceID); }

            if (scannedInstances.Count <= 0)
            {
                isScanningCreature = false;
                TryNewScannerState(false);
            }
        }
    }
}
