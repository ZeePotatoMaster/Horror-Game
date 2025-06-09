using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSystem : MonoBehaviour
{
    private CamButton currentActiveButton;

    [SerializeField] private PowerScript ps;
    [SerializeField] private Canvas monitor;
    [SerializeField] private SealButton sb;

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

        if (currentActiveButton != null) currentActiveButton.Select(false);

        camButton.Select(true);

        currentActiveButton = camButton;

        sb.ChangeColor(false);
        if (sb.sealing) sb.StopSeal();
        if (currentActiveButton == sb.sealCam) sb.ChangeColor(true);
    }

    public void SealCam()
    {
        if (sb.sealCam == currentActiveButton) return;
        if (!currentActiveButton.canSeal) return;

        sb.UnSeal();
        sb.sealCam = currentActiveButton;
        sb.StartSeal();
    }
}
