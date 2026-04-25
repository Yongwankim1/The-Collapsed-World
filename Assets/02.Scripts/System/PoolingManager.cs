using UnityEngine;
using System.Collections.Generic;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;
    Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //풀에 있는지 검사
    bool IsRegistred(GameObject poolingObject)
    {
        if (pool.ContainsKey(poolingObject.name))
        {
            return true;
        }
        return false;
    }
    //풀을 만들기
    void CreatePool(GameObject poolingObject)
    {
        //참이 아니니 새로 만듬
        Queue<GameObject> queue = new Queue<GameObject>();
        pool.Add(poolingObject.name, queue);
        Debug.Log($"풀을 새로 만들었습니다 {poolingObject.name}");

    }
    //풀에 담기
    void EnQueue(GameObject poolingObject)
    {
        if (!IsRegistred(poolingObject))
        {
            CreatePool(poolingObject);
        }
        pool[poolingObject.name].Enqueue(poolingObject);
        poolingObject.SetActive(false);
    }
    //풀에서 꺼내기
    bool DeQueue(GameObject poolingObject, out GameObject go)
    {
        go = null;
        if (!IsRegistred(poolingObject))
        {
            CreatePool(poolingObject);
            return false;
        }
        if (pool[poolingObject.name].Count == 0)
        {
            return false;
        }
        go = pool[poolingObject.name].Dequeue();
        return true;
    }
    public void Return(GameObject poolingObject) => EnQueue(poolingObject);
    public GameObject Get(GameObject poolingObject)
    {
        if (!DeQueue(poolingObject, out GameObject go))
        {
            go = null;
            Debug.LogWarning("풀에서 꺼낼 오브젝트가 없습니다");
        }
        return go;
    }

}
