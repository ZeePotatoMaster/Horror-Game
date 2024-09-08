using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfo : MonoBehaviour
{

    public TMP_FontAsset defaultGlobalFont;

    [SerializeField] private TMP_Text playerName;
    public string steamName;
    public ulong steamId;
    public GameObject readyImage;
    public bool isready;

    public void updateFont(TMP_FontAsset font) {

        defaultGlobalFont = font;
    }

    private void Start()
    {
        readyImage.SetActive(false);

        playerName.text = steamName;
        playerName.font = defaultGlobalFont;

        //setting to the gold color
        playerName.color = new Color32(255, 198, 114, 255);
        
        //making it so names no longer trunucate to the next line (if they still do then just increase the first v2 value until it doesnt, but 700 should be good for steam names i think)
        playerName.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 50);
    }
}
