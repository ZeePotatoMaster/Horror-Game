using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomF : MonoBehaviour
{
    [SerializeField] private BotPosition lookPos;
    [SerializeField] private DefaultBot db;

    private float currentLook = 0f;
    [SerializeField] private float lookTime;

    private Camera playerCam;
    private Vector3 camDirection;

    // Update is called once per frame
    void Update()
    {
        if (db.currentPosition != lookPos && currentLook > 0f) currentLook = 0f;
        if (db.currentPosition != lookPos) return;

        camDirection = transform.position - playerCam.transform.position;
        if (Vector3.Angle(playerCam.transform.forward, camDirection) <= playerCam.fieldOfView) currentLook += Time.deltaTime;

        if (currentLook >= lookTime) Jumpscare();
    }

    void Jumpscare()
    {
        currentLook = 0f;
        db.Move(db.startingPosition);
        db.enabled = false;

        transform.SetPositionAndRotation(playerCam.transform.position, playerCam.transform.rotation);
        transform.SetParent(playerCam.transform);
        //aniamtor play jumpscare
        //console cause error

        Invoke(nameof(ResetJumpscare), 2f);
    }

    void ResetJumpscare()
    {
        db.enabled = true;
        transform.SetParent(null);
        db.Move(db.startingPosition);
        //animation reset ?
    }
}
