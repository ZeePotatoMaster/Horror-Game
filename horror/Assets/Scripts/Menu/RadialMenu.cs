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
    [SerializeField] private CurseObject[] curseObjects;
    [SerializeField] private float radius = 3f;
    [HideInInspector] public Curse currentAbility;

    // Start is called before the first frame update
    void Start()
    {
        Entries = new List<RadialMenuEntry>();

        //"start"
        for (int i=0; i < curseObjects.Length; i++) {
            Curse formed = (Curse)this.gameObject.AddComponent(Type.GetType(curseObjects[i].abilityType));
            AddEntry(curseObjects[i].curseName, curseObjects[i].image, curseObjects[i].cost, formed);
        }
        Rearrange();
    }

    void AddEntry(string pLabel, Texture pIcon, float pCost, Curse pAbility)
    {
        GameObject entry = Instantiate(entryPrefab, transform);
        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();
        rme.SetLabel(pLabel);
        rme.SetIcon(pIcon);
        rme.SetCost(pCost);
        rme.SetAbility(pAbility);

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
