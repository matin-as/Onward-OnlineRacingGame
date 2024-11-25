using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mstart : MonoBehaviour
{
    bool st = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        wait_to_start();
    }
    void wait_to_start()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length==PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if(st==false)
            {
                st = true;
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
                {
                    g.GetComponent<spcode>().m_parent.GetComponent<Player>().creat_faza();
                    g.GetComponent<spcode>().m_parent.GetComponent<Player>().is_start = true;
                    g.GetComponent<spcode>().m_parent.GetComponent<Player>().can_move = true;
                }
            }
        }
    }
}
