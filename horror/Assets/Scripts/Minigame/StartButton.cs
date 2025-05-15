using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : Interactable
{
    public override void FinishInteract(GameObject player)
    {
        TheOvergame.instance.StartGame();
    }
}
