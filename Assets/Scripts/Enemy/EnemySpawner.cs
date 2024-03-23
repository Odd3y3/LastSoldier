using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wave Spawn �Ӽ�
/// </summary>
[Serializable]
struct WaveInfo
{
    //�̹� ���̺� �ð�
    public float waveTime;
    //���̺� Enemy spawn ����
    public WaveEnemyInfo[] waveEnemyInfo;
}

/// <summary>
/// ���̺� Enemy spawn ����
/// </summary>
[Serializable]
struct WaveEnemyInfo
{
    //Enemy Prefab
    public GameObject enemyPrefab;

    //spawnNumAtOnce * spawnCount == �� ��ȯ�Ǵ� enemy ��
    [Header("�ѹ��� ��ȯ�ϴ� enemy ��")]
    public int spawnNumAtOnce;
    [Header("��� ��ȯ����")]
    public int spawnCount;

    [Header("���� �ڿ� ��ȯ�� �����Ұ���")]
    public float spawnDelayTime;
    [Header("���� ���� ��ȯ�� ����")]
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
    /// Wave ����
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
    /// time�� �Ŀ� ���� Wave����
    /// </summary>
    IEnumerator StartingNextWave(float time)
    {
        //time�� 0�̸� NextWave�� �Ѿ�� ����
        if(time == 0f) yield break;

        yield return new WaitForSeconds(time);
        int waveNum = ++GameManager.inst.inGameManager.WaveNum;
        StartWave(waveNum);
    }

    /// <summary>
    /// Enemy ��ȯ
    /// </summary>
    IEnumerator SpawningEnemy(WaveEnemyInfo enemyInfo)
    {
        //spawnDelayTime�� �� ��ȯ ����
        yield return new WaitForSeconds(enemyInfo.spawnDelayTime);


        WaitForSeconds waitInterval = new WaitForSeconds(enemyInfo.spawnTimeInterval);

        //spawnCount�� ��ȯ
        for (int i = 0; i < enemyInfo.spawnCount; ++i)
        {
            //�ѹ��� spawnNumAtOnce ���� ��ȯ
            for (int n = 0; n < enemyInfo.spawnNumAtOnce; ++n)
            {
                //Enemy ��ȯ
                SpawnEnemy(enemyInfo.enemyPrefab);
            }

            yield return waitInterval;
        }
    }

    /// <summary>
    /// enemyPrefab ���� enemy ����
    /// (spawnPosition�߿� �����ϰ� �����Ͽ� �� ��ġ�� Enemy ����)
    /// </summary>
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        //spawnPosition�߿� �����ϰ� �����Ͽ� �� ��ġ�� Enemy ����
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
