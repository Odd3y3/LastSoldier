using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʈ Ǯ��
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool inst;

    private void Awake()
    {
        inst = this;
    }
    
    //Object���� ������ Pool
    Dictionary<string, Queue<GameObject>> myPool = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// ������Ʈ Ǯ �ȿ� �ִ� GameObject�� typeName�� ���� ���� ���� ���, �� Object�� ����
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
    /// ������Ʈ Ǯ�� typeName���� �ְ�, ��Ȱ��ȭ ��Ŵ.
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
    /// delayTime �Ŀ� ReleaseObject ����
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
