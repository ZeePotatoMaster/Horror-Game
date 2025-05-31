using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSystem : MonoBehaviour
{
    private CamButton currentActiveButton;

    [SerializeField] private PowerScript ps;
    [SerializeField] private Canvas monitor;

    // Update is called once per frame
    void Update()
    {
        if (ps.power == 0)
        {
            if (monitor.enabled) monitor.enabled = false;
            return;
        }
    }

    public void ChangeCams(CamButton camButton)
    {
        if (currentActiveButton == camButton) return;
        camButton.Select(true);

        if (currentActiveButton != null) currentActiveButton.Select(false);

        currentActiveButton = camButton;
    }
}
