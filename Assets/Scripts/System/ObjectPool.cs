using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool inst;

    private void Awake()
    {
        inst = this;
    }
    
    //Object들을 저장할 Pool
    Dictionary<string, Queue<GameObject>> myPool = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// 오브젝트 풀 안에 있는 GameObject중 typeName이 같은 것이 있을 경우, 그 Object를 재사용
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="org"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public GameObject GetObject(string typeName, GameObject org, Transform p = null)  
    {
        if (myPool.ContainsKey(typeName))
        {
            if (myPool[typeName].Count > 0)
            {
                GameObject obj = myPool[typeName].Dequeue();
                if(obj != null)
                {
                    obj.SetActive(true);
                    obj.transform.SetParent(p);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                }
                return obj;
            }
        }

        return Instantiate(org, p);
    }
    /// <summary>
    /// 오브젝트 풀에 typeName별로 넣고, 비활성화 시킴.
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="obj"></param>
    public void ReleaseObject(string typeName, GameObject obj)
    {
        if (obj == null)
            return;

        obj.transform.SetParent(transform);
        obj.SetActive(false);
        
        if (!myPool.ContainsKey(typeName))
        {
            myPool[typeName] = new Queue<GameObject>();
        }
        myPool[typeName].Enqueue(obj);
    }

    /// <summary>
    /// delayTime 후에 ReleaseObject 실행
    /// </summary>
    public void ReleaseObject(string typeName, GameObject obj, float delayTime)
    {
        StartCoroutine(DelayingReleaseObject(typeName, obj, delayTime));
    }
    IEnumerator DelayingReleaseObject(string typeName, GameObject obj, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ReleaseObject(typeName, obj);
    }
}
