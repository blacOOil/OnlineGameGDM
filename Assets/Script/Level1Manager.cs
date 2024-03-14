using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Level1Manager : NetworkBehaviour
{
    public Transform[] NetworkObjectSpawPoint;
    public GameObject[] NetworkLevelGameObject;
    private float LevelStage = 1; 
    void FixedUpdate()
    {
        if(LevelStage == 1)
        {
            ServerObjectSpawnObj(0,0); // Spawn firt object
            ServerObjectSpawnObj(1, 1); // Spawn Second Object
            LevelStage = 1.5f; //make some change in Level Stage to prevent from spawn too many
        }
        if(LevelStage == 2) //Spawn or event you want it to happen in stage 2
        {

        }
    }
    public void ServerObjectSpawnObj(int GameObjectId,int SpawnPosionId)
    {
        GameObject ObjectPrefab = NetworkLevelGameObject[GameObjectId]; 
        Transform SpanwPositon = NetworkObjectSpawPoint[SpawnPosionId];
        GameObject ServerObj = Instantiate(ObjectPrefab, SpanwPositon.position, SpanwPositon.rotation);
    }
}
