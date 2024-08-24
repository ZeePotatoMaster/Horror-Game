using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    private List<RadialMenuEntry> Entries;
    [HideInInspector] public CurseObject[] curseObjects;
    [SerializeField] private float radius = 3f;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup(GameObject pPlayer)
    {
        Entries = new List<RadialMenuEntry>();
        player = pPlayer;

        //"start"
        for (int i=0; i < curseObjects.Length; i++) {
            Curse formed = (Curse)player.GetComponent(Type.GetType(curseObjects[i].abilityType));
            AddEntry(curseObjects[i].curseName, curseObjects[i].image, curseObjects[i].cost, formed);
        }
        Rearrange();
    }

    public CurseManager GetCurseManager()
    {
        return player.GetComponent<CurseManager>();
    }

    void AddEntry(string pLabel, Texture pIcon, float pCost, Curse pAbility)
    {
        GameObject entry = Instantiate(entryPrefab, transform);
        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetIcon(pIcon);
        rme.SetAbility(pAbility);
        rme.SetCost(pCost);
        rme.SetLabel(pLabel);

        Entries.Add(rme);
    }

    void Rearrange() 
    {
        float radiansOfSeperation = 2 * Mathf.PI / Entries.Count;
        for (int i=0; i < Entries.Count; i++) {
            float x = Mathf.Sin(radiansOfSeperation * i) * radius;
            float y = Mathf.Cos(radiansOfSeperation * i) * radius;

            Entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        }
    }
}
