using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpawnerScript : NetworkBehaviour
{
    public Behaviour[] scripts;
    private Behaviour[] renderers;
    MainPlayerScript mainPlayer;
    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = gameObject.GetComponent<MainPlayerScript>();

    }
    void SetPlayerState(bool state)
    {
        foreach (var script in scripts) { script.enabled = state; }
        foreach (var renderer in renderers) { renderer.enabled = state; }

    }
    private Vector3 GetRandomPos()
    {
        Vector3 randPos = new Vector3(Random.Range(-3, 3f), 1f, Random.Range(-3f, 3f));
        return randPos;
    }
    public void Respawn()
    {
        RespawnServerRpc();
    }
    [ServerRpc]
    void RespawnServerRpc()
    {
        Vector3 pos = GetRandomPos();
        RespawnClientRpc(pos);
    }
    [ClientRpc]
    void RespawnClientRpc(Vector3 spawnPos)
    {
        StartCoroutine(RespawnCoroutine(spawnPos));
    }
    IEnumerator RespawnCoroutine(Vector3 SpawnPos)
    {
        SetPlayerState(false);
        transform.position = SpawnPos;
        yield return new WaitForSeconds(3f);
        SetPlayerState(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
