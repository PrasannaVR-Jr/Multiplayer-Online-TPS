using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public string NetworkPrefabName;
    public GameObject SecondaryCam;

    void Start()
    {
        PhotonNetwork.Instantiate(NetworkPrefabName, new Vector3(Random.Range(250, 750), 25, Random.Range(250, 750)), Quaternion.identity, 0);
        SecondaryCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
