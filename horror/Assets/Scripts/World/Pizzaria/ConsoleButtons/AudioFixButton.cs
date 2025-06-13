using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFixButton : ConsoleButton
{
    [SerializeField] private AudioLureButton lb;
    public override void Activate()
    {
        lb.RebootLures();
    }
}
