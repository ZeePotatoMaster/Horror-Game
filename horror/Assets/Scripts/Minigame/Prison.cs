using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Prison : MinigameManager
{

    [HideInInspector] public List<ulong> prisoners = new List<ulong>();
    [HideInInspector] public List<ulong> guards = new List<ulong>();
    [SerializeField] private List<Transform> guardSpawns;
    [SerializeField] private List<Transform> prisonerSpawns;

    [SerializeField] private NetworkObject rifle;
    [SerializeField] private NetworkObject grenade;
    [SerializeField] private NetworkObject keycard;

    [SerializeField] private GameObject prisonDoors;
    [SerializeField] private GameObject prisonTriggers;
    [SerializeField] private GameObject guardDoor;

    [SerializeField] private NetworkObject downedForm;

    [HideInInspector] public List<ulong> imprisoned = new List<ulong>();
    [HideInInspector] public List<ulong> killedGuards = new List<ulong>();

    private bool started = false;
    private bool ended = false;
    private List<ulong> winningTeam;
    private GameObject winText;
    private int playerReturnedElevator = 0;

    private void StartMinigame()
    {
        dieNormally = false;

        foreach (KeyValuePair<ulong, NetworkObject> e in TheOvergame.instance.elevators)
        {
            AssignTeams(e.Key);

            Vector3 pos = Vector3.zero;

            if (prisoners.Contains(e.Key))
            {
                int i = Random.Range(0, prisonerSpawns.Count);
                pos = prisonerSpawns[i].position;
                prisonerSpawns.Remove(prisonerSpawns[i]);
            }
            if (guards.Contains(e.Key))
            {
                int i = Random.Range(0, guards.Count);
                pos = guardSpawns[i].position;
                guardSpawns.Remove(guardSpawns[i]);

                rifle.GetComponent<WorldItem>().PickupItemRpc(RpcTarget.Single(e.Key, RpcTargetUse.Temp));
                grenade.GetComponent<WorldItem>().PickupItemRpc(RpcTarget.Single(e.Key, RpcTargetUse.Temp));
                keycard.GetComponent<WorldItem>().PickupItemRpc(RpcTarget.Single(e.Key, RpcTargetUse.Temp));
            }

            FirstTeleportRpc(pos, RpcTarget.Single(e.Key, RpcTargetUse.Temp));
            e.Value.transform.position = pos;

            Debug.Log("a ");
        }

        Invoke(nameof(OpenPrisonerDoors), 10f);
        Invoke(nameof(OpenGuardDoor), 30f);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void FirstTeleportRpc(Vector3 pos, RpcParams rpcParams = default)
    {
        NetworkObject p = NetworkManager.Singleton.LocalClient.PlayerObject;
        p.GetComponent<CharacterController>().enabled = false;
        p.transform.position = pos + new Vector3(0, 2, 0);

        PlayerBase pb = p.GetComponent<PlayerBase>();
        pb.SetLockPositionRpc(pos, 2f);

        p.GetComponent<CharacterController>().enabled = true;
    }

    private void AssignTeams(ulong p)
    {
        int playerCount = TheOvergame.instance.elevators.Count;

        if (prisoners.Count == Mathf.Ceil(playerCount / 2)) { guards.Add(p); return; }
        if (guards.Count == Mathf.Floor(playerCount / 2)) { prisoners.Add(p); return; }

        int coin = Random.Range(0, 1);
        if (coin == 0) prisoners.Add(p);
        if (coin == 1) guards.Add(p);
    }

    void OpenPrisonerDoors()
    {
        foreach (PrisonDoor p in prisonDoors.GetComponentsInChildren<PrisonDoor>())
        {
            p.OpenRpc(5f);
        }

        foreach (ulong i in prisoners)
        {
            NetworkObject p = NetworkManager.Singleton.ConnectedClients[i].PlayerObject;

            p.GetComponent<PlayerHealth>().invulnerable = true;
            p.GetComponent<PlayerBase>().SetSpeedMultiplerRpc(2f);
        }
        Invoke(nameof(CalmPrisoners), 8f);
    }

    void CalmPrisoners()
    {
        foreach (ulong i in prisoners)
        {
            NetworkObject p = NetworkManager.Singleton.ConnectedClients[i].PlayerObject;

            p.GetComponent<PlayerHealth>().invulnerable = false;
            p.GetComponent<PlayerBase>().SetSpeedMultiplerRpc(1f);
        }
        prisonTriggers.SetActive(true);
    }

    void OpenGuardDoor()
    {
        guardDoor.GetComponent<Animator>().SetBool("isOpen", true);
    }

    void Update()
    {
        if (!IsServer) return;
        if (TheOvergame.instance.gameStarted && !started)
        {
            started = true;
            StartMinigame();
        }
    }

    public override void OnPlayerDeath(ulong id)
    {
        if (ended) return;
        if (prisoners.Contains(id)) SpawnDownedRpc(id);

        else
        {
            NetworkObject p = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
            p.GetComponent<PlayerHealth>().SummonRagdollServerRpc(p.transform.position, id);
            killedGuards.Add(id);
            if (killedGuards.Count == guards.Count) EndGameRpc(false);
        }
    }

    [Rpc(SendTo.Server)]
    void SpawnDownedRpc(ulong id)
    {
        NetworkObject p = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;

        NetworkObject down = Instantiate(downedForm, p.transform.position, p.transform.rotation);
        p.Despawn();
        down.SpawnAsPlayerObject(id, false);
    }

    public override void OnPlayerEnterElevator(ulong id)
    {
        if (!ended) return;

        if (winningTeam.Contains(id))
        {
            ResetPlayersRpc(id);
        }
    }

    [Rpc(SendTo.Server)]
    public void ChangeImprisonedRpc(ulong i, bool add)
    {
        if (add) imprisoned.Add(i);
        else imprisoned.Remove(i);
        if (imprisoned.Count == prisoners.Count) EndGameRpc(true);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void EndGameRpc(bool guardsWon)
    {
        if (ended) return;

        ended = true;
        if (guardsWon)
        {
            winningTeam = guards;
            foreach (ulong id in killedGuards) winningTeam.Remove(id);

            foreach (ulong id in prisoners) TheOvergame.instance.elevators.Remove(id);
        }
        else
        {
            winningTeam = prisoners;

            foreach (ulong id in guards) TheOvergame.instance.elevators.Remove(id);
        } 

        GameObject recover = GameObject.Find("RecoverImage");
        Destroy(recover);

        foreach (PrisonDoor p in prisonDoors.GetComponentsInChildren<PrisonDoor>())
        {
            p.OpenRpc(-1f);
        }

        winText = Instantiate(winText, GameObject.Find("Canvas").transform);
        winText.GetComponent<TMP_Text>().text = guardsWon ? "Guards Win!!!!!!" : "Prisoners Win!!!!";
        Invoke(nameof(DestroyText), 2f);
    }

    [Rpc(SendTo.Server)]
    void ResetPlayersRpc(ulong id)
    {
        playerReturnedElevator++;

        NetworkObject p = NetworkManager.ConnectedClients[id].PlayerObject;
        p.GetComponent<CharacterController>().enabled = false;
        p.GetComponent<PlayerHealth>().invulnerable = true;

        if (playerReturnedElevator != winningTeam.Count) return;

        foreach (ulong i in winningTeam)
        {
            NetworkObject player = NetworkManager.ConnectedClients[i].PlayerObject;

            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<PlayerHealth>().invulnerable = false;
        }

        TheOvergame o = TheOvergame.instance;
        o.gameStarted = false;
        int rand = Random.Range(0, o.levels.Length);
        o.LoadLevel(o.levels[rand]);
    }

    void DestroyText()
    {
        Destroy(winText);
    }
}
