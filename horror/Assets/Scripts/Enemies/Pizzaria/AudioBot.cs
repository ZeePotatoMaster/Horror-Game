using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBot : MonoBehaviour
{
    [SerializeField] private DefaultBot db;
    [SerializeField] private float responseTime;
    [SerializeField] private float randomResponse;

    public IEnumerator AudioLure(BotPosition lurePos)
    {
        Debug.Log("springtrap go");
        bool canLure = false;
        foreach (BotPosition bp in lurePos.positions)
        {
            if (bp == db.currentPosition) canLure = true;
        }
        foreach (BotPosition bp in db.currentPosition.positions)
        {
            if (bp == lurePos) canLure = true;
        }
        if (!canLure) yield break;

        Debug.Log("springtrap going");

        BotPosition oldPos = db.currentPosition;

        float r = Random.Range(-randomResponse, randomResponse);
        yield return new WaitForSeconds(responseTime + r);

        if (db.currentPosition == oldPos) db.Move(lurePos);

        Debug.Log("springtrap gone");
    }
}
