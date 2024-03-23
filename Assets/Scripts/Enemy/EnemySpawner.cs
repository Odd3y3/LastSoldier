using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wave Spawn 속성
/// </summary>
[Serializable]
struct WaveInfo
{
    //이번 웨이브 시간
    public float waveTime;
    //웨이브 Enemy spawn 정보
    public WaveEnemyInfo[] waveEnemyInfo;
}

/// <summary>
/// 웨이브 Enemy spawn 정보
/// </summary>
[Serializable]
struct WaveEnemyInfo
{
    //Enemy Prefab
    public GameObject enemyPrefab;

    //spawnNumAtOnce * spawnCount == 총 소환되는 enemy 수
    [Header("한번에 소환하는 enemy 수")]
    public int spawnNumAtOnce;
    [Header("몇번 소환할지")]
    public int spawnCount;

    [Header("몇초 뒤에 소환을 시작할건지")]
    public float spawnDelayTime;
    [Header("몇초 마다 소환을 할지")]
    public float spawnTimeInterval;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    Transform[] spawnPositions;

    [SerializeField]
    WaveInfo[] waveInfos;


    public void StartWave(int waveNum)
    {
        SpawnWave(waveInfos[waveNum]);
    }   

    public void StopWave()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Wave 시작
    /// </summary>
    private void SpawnWave(WaveInfo waveInfo)
    {
        StartCoroutine(StartingNextWave(waveInfo.waveTime));
        foreach (WaveEnemyInfo enemyInfo in waveInfo.waveEnemyInfo)
        {
            StartCoroutine(SpawningEnemy(enemyInfo));
        }
    }

    /// <summary>
    /// time초 후에 다음 Wave시작
    /// </summary>
    IEnumerator StartingNextWave(float time)
    {
        //time이 0이면 NextWave로 넘어가지 않음
        if(time == 0f) yield break;

        yield return new WaitForSeconds(time);
        int waveNum = ++GameManager.inst.inGameManager.WaveNum;
        StartWave(waveNum);
    }

    /// <summary>
    /// Enemy 소환
    /// </summary>
    IEnumerator SpawningEnemy(WaveEnemyInfo enemyInfo)
    {
        //spawnDelayTime초 후 소환 시작
        yield return new WaitForSeconds(enemyInfo.spawnDelayTime);


        WaitForSeconds waitInterval = new WaitForSeconds(enemyInfo.spawnTimeInterval);

        //spawnCount번 소환
        for (int i = 0; i < enemyInfo.spawnCount; ++i)
        {
            //한번에 spawnNumAtOnce 마리 소환
            for (int n = 0; n < enemyInfo.spawnNumAtOnce; ++n)
            {
                //Enemy 소환
                SpawnEnemy(enemyInfo.enemyPrefab);
            }

            yield return waitInterval;
        }
    }

    /// <summary>
    /// enemyPrefab 으로 enemy 생성
    /// (spawnPosition중에 랜덤하게 선택하여 그 위치에 Enemy 생성)
    /// </summary>
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        //spawnPosition중에 랜덤하게 선택하여 그 위치에 Enemy 생성
        int spawnPosNum = UnityEngine.Random.Range(0, spawnPositions.Length);

        int enemyNum = 0;
        if (enemyPrefab.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemyNum = enemy.enemyNum;
        }

        GameObject enemyInst = ObjectPool.inst.GetObject("Enemy" + enemyNum.ToString(), enemyPrefab,
            GameManager.inst.inGameManager.enemysRoot);
        enemyInst.transform.position = spawnPositions[spawnPosNum].position;
        enemyInst.transform.rotation = spawnPositions[spawnPosNum].rotation;
    }

}
