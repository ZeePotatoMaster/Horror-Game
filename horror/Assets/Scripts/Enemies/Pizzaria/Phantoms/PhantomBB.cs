using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomBB : MonoBehaviour
{
    private Camera currentCam;
    [SerializeField] private DefaultBot db;

    private bool lookedAt = false;

    private float currentLook = 0f;
    [SerializeField] private float lookTime = 0f;
    [SerializeField] private Transform playerCam;

    // Update is called once per frame
    void Update()
    {
        if (currentCam.enabled && !lookedAt)
        {
            lookedAt = true;
            db.enabled = false;
        }

        if (currentCam.enabled) currentLook += Time.deltaTime;

        if (currentLook >= lookTime) Jumpscare();

        if (!currentCam.enabled && lookedAt) Reset();
    }

    void Jumpscare()
    {
        Reset();
        db.enabled = false;
        transform.SetPositionAndRotation(playerCam.position, playerCam.rotation);
        transform.SetParent(playerCam);
        //aniamtor play jumpscare
        //console cause error

        Invoke(nameof(ResetJumpscare), 2f);
    }

    void Reset()
    {
        db.enabled = true;

        db.Move(db.startingPosition);
        currentLook = 0f;
        lookedAt = false;
    }

    void ResetJumpscare()
    {
        db.enabled = true;
        transform.SetParent(null);
        db.Move(db.startingPosition);
        //animation reset ?
    }
}
