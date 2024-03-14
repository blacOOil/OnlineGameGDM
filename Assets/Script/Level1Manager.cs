using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Level1Manager : NetworkBehaviour
{
    public Transform[] NetworkObjectSpawPoint;
    public GameObject[] NetworkLevelGameObject;
    private float LevelStage = 0;
    private float[][] SpawningIdex;
    public float[] howmanyObjUneed;
    void FixedUpdate()
    {
        if(LevelStage == 0)
        {
            SpawningOrderPerStage(LevelStage);
            LevelStage = 0.5f; //make some change in Level Stage to prevent from spawn too many
        }
        if(LevelStage == 1) //Spawn or event you want it to happen in stage 2
        {

        }
    }
    public void ServerObjectSpawnObj(int GameObjectId,int SpawnPosionId)
    {
        GameObject ObjectPrefab = NetworkLevelGameObject[GameObjectId]; 
        Transform SpanwPositon = NetworkObjectSpawPoint[SpawnPosionId];
        GameObject ServerObj = Instantiate(ObjectPrefab, SpanwPositon.position, SpanwPositon.rotation);
    }
    public void SpawningOrderPerStage(float LevelStage)
    {
        for (float i = LevelStage; i <= howmanyObjUneed[(int)LevelStage]; i++)
        {

            int x = (int)i;
            ServerObjectSpawnObj(x, x);
        }
    }
}
