using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class enemy : MonoBehaviour
{
    private PhotonView photonView;
    public List<string> efects;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        efects.Add("CFX2_WWExplosion_C");
        efects.Add("CFX_Explosion_B_Smoke+Text");
        efects.Add("CFX_Hit_A Red+RandomText");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
        {
            if(gameObject.tag=="cam")
            {
                GameObject.Find(transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text). GetComponent<PhotonView>().RPC("f", RpcTarget.All);
            }
            else
            {
                other.transform.GetComponent<spcode>().m_parent.GetComponent<PhotonView>().RPC("c", RpcTarget.All);
            }
            Destroy(gameObject);
            PhotonNetwork.Instantiate(Resources.Load<GameObject>(efects[Random.Range(0,efects.Count)]).name, transform.position, Quaternion.identity);
        }
    }
    [PunRPC]
    void set_name(string s)
    {
        transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = s;
    }
}
