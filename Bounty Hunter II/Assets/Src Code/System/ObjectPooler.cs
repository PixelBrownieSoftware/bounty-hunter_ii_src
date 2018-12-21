using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IpoolObject {
    void SpawnStart();

}

public sealed class ObjectPooler : MonoBehaviour {

    public static readonly object lockthing = new object();

    public enum object_type {
        enemies,
        items,
        bullets
    };

    object_type obj_type;
    public static ObjectPooler instance = null;
    
    public Dictionary<string, Queue<GameObject>> objpool;
    public List<SuperPool> super_pools;

    [System.Serializable]
    public struct Pool {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [System.Serializable]
    public struct o_uniq_char {
        public string tag;
        public GameObject prefab;
    }

    public List<o_uniq_char> UniqueCharacters = new List<o_uniq_char>();
    public List<GameObject> uniq = new List<GameObject>();
    
    [System.Serializable]
    public struct SuperPool {
        public string tag;
        public List<Pool> pools;
    }
    
    #region singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        CreatePools();
    }
    #endregion

    public void CreatePools() {
        objpool = new Dictionary<string, Queue<GameObject>>();
        foreach (SuperPool pool in super_pools)
        {
            for (int i = 0; i < pool.pools.Count; i++) {

                Queue<GameObject> obj_pool = new Queue<GameObject>();
                for (int p = 0; p < pool.pools[i].size; p++)
                {
                    GameObject obj = Instantiate(pool.pools[i].prefab);
                    obj.SetActive(false);
                    obj_pool.Enqueue(obj);
                    obj.transform.SetParent(this.transform);
                }

                objpool.Add(pool.pools[i].tag, obj_pool);
            }
        }

        foreach (o_uniq_char ch in UniqueCharacters)
        {
            GameObject obj = Instantiate(ch.prefab);
            obj.name = ch.prefab.name;
            uniq.Add(obj);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
        }
    }

    public GameObject FindUniqueChar(string tag) {

        for (int i = 0; i < uniq.Count; i++)
        {
            if (uniq[i].GetComponent<o_character>().type == tag)
            {
                return uniq[i];
            }
        }
        return null;
    }

    public GameObject SpawnObject(string tag, Vector3 position, Quaternion rotation, Vector2 size, bool spawn_active)
    {
        GameObject obj = SpawnObject(tag, position, rotation,spawn_active);
        obj.transform.localScale = size;

        return obj;
    }

    public GameObject SpawnObject(string tag, Vector3 position, Quaternion rotation, bool spawn_active) {

        //spawn Generic objects only

        GameObject objtospawn = SpawnObject(tag,spawn_active);
        objtospawn.transform.position = position;
        objtospawn.transform.rotation = rotation;

        return objtospawn;
    }

    public GameObject SpawnObject(string tag, Vector3 position, bool spawn_active)
    {
        GameObject objtospawn = SpawnObject(tag,spawn_active);
        objtospawn.transform.position = position;
        return objtospawn;
    }

    public GameObject SpawnObject(string tag, bool spawn_active) {
        GameObject objtospawn;
        if (FindUniqueChar(tag)) {
            objtospawn = FindUniqueChar(tag);
        } else {
            objtospawn = objpool[tag].Dequeue();
            objpool[tag].Enqueue(objtospawn);
        }

        objtospawn.SetActive(true);
        o_character cha = objtospawn.GetComponent<o_character>();
        if (cha != null)
        {
            if (spawn_active)
                if (objtospawn.GetComponent<IpoolObject>() != null)
                    objtospawn.GetComponent<IpoolObject>().SpawnStart();
        }
        else 
        if (objtospawn.GetComponent<IpoolObject>() != null)
            objtospawn.GetComponent<IpoolObject>().SpawnStart();
        return objtospawn;
    }

    
}
