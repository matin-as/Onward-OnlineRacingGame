using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using TMPro;

public class Bot : MonoBehaviour
{
    PhotonView photonView;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameObject.name = "Bot";
    }
}
