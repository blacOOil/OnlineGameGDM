using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BombSpawnerScript : NetworkBehaviour
{
    public GameObject bombPrefab;
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnBombServerRPC();
        }
    }
    [ServerRpc]
    void SpawnBombServerRPC() { 
            Vector3 spawnPos = transform.position +
                (transform.forward * -1.5f) + (transform.up * 1.5f);
            Quaternion spawnRot = transform.rotation;
            GameObject bomb = Instantiate(bombPrefab, spawnPos, spawnRot);
            bomb.GetComponent<NetworkObject>().Spawn();
        }
    }

