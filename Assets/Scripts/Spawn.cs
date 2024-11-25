using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject[] Spawn_list;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject[] spawn_pos;
    public List<string> players;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    private float timer = 10;
    void Start()
    {
        PhotonNetwork.Instantiate(Player.name, spawn_pos[0].transform.position, Quaternion.identity);
    }
    void Update()
    {
        _spawn();
    }
    void _spawn()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length==PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (GameObject.FindGameObjectsWithTag("Group").Length <= 30)
            {
                PhotonNetwork.Instantiate(Spawn_list[Random.Range(0, Spawn_list.Length)].name, new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ)), Quaternion.identity);
            }
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.GetComponent<spcode>().m_parent.GetComponent<Player>().m_cam <= 6)
                {
                    g.GetComponent<spcode>().m_parent.GetComponent<Player>().m_cam++;
                    GameObject v = PhotonNetwork.Instantiate(Resources.Load<GameObject>("CFX3_DarkMagicAura_A (1)").name, new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ)), new Quaternion(270, 0, 0, 0));//270x
                    v.GetComponent<PhotonView>().RPC("set_name",RpcTarget.All, g.GetComponent<spcode>().m_parent.GetComponent<Player>().name);
                    v.transform.Rotate(new Vector3(90, 0, 0));
                }
            }
        }
        else
        {
            if(timer>=0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers - GameObject.FindGameObjectsWithTag("Player").Length; i++)
                {
                    GameObject g = PhotonNetwork.Instantiate("Bot", spawn_pos[0].transform.position, Quaternion.identity);
                    g.GetComponent<PhotonView>().RPC("set_but", RpcTarget.AllBuffered);
                }
                timer = 7;
            }
            // spawn Bot
        }

    }
}
