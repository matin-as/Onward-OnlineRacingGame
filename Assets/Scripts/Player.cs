using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.AI;
using randome = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private PhotonView photonView;
    private NavMeshAgent agent;
    private GameObject target;
    public string my_char_name;
    bool aded_money;
    [SerializeField] GameObject text_name;
    [SerializeField] TextMeshProUGUI text_number;
    [SerializeField] TextMeshProUGUI text_rank;
    private TextMeshProUGUI text_players;
    public GameObject Wait_ga;
    public bool is_start;
    [SerializeField] FixedJoystick joystick;
    public int m_cam;
    [SerializeField] GameObject content;
    private GameObject faza;
    public bool can_move = true;
    private GameObject convas_lsoe;
    public GameObject sp;
    public int coin = 0;
    private float time = 1.2f*60;
    private float roundedv;
    [SerializeField] TextMeshProUGUI text_coin;
    [SerializeField] TextMeshProUGUI text_time;
    // Movment
    private Rigidbody RB;
    private float moveInput;
    private float turnInput;
    public float speed;
    public float rewSpeed;
    public float turnSpeed;
    public bool is_but;
    public List<string> bot_name = new List<string>() { "Adams", "Baker", "Clark", "Davis", "Evans", "Frank", "Ghosh", "Hills", "Valdo" };
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(is_but)
        {
            agent = GetComponent<NavMeshAgent>();
            sp = transform.GetChild(1).gameObject;
            sp.GetComponent<spcode>().m_parent = gameObject;
            if (photonView.IsMine)
            {
                photonView.RPC("set_naem", RpcTarget.AllBuffered, "Allcars/" + PlayerPrefs.GetString("CurectCar"));
                photonView.RPC("set_name_bot", RpcTarget.AllBuffered);
            }
            text_name.GetComponent<TextMeshProUGUI>().text = gameObject.name;
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
                if(photonView.IsMine)
                {
                    photonView.RPC("set_naem", RpcTarget.AllBuffered, "Allcars/" + PlayerPrefs.GetString("CurectCar"));
                }
            gameObject.name = photonView.Controller.NickName;
            text_name.GetComponent<TextMeshProUGUI>().text = photonView.Controller.NickName;
            if (!photonView.IsMine)
            {
                transform.GetChild(4).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(6).gameObject.SetActive(false);
            }
            faza = Resources.Load<GameObject>("Faza");
            convas_lsoe = transform.GetChild(5).gameObject;
            RB = transform.GetChild(1).GetComponent<Rigidbody>();
            sp = transform.GetChild(1).gameObject;
            sp.GetComponent<spcode>().m_parent = gameObject;
            text_players = transform.GetChild(4).GetChild(7).GetComponent<TextMeshProUGUI>();
            RB.transform.parent = null;
        }
    }
    private void Update()
    {
        if(is_but)
        {
            check_target();
        }
        else
        {
            if (photonView.IsMine)
            {
                movment();
                handle();
                manage_Skin();
            }
        }
        handle_start();
    }
    private void FixedUpdate()
    {
        if(!is_but)
        {
            if(RB != null)
            {
                RB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
            }
        }
    }
    private void movment()
    {
        if(can_move&&is_start)
        {
            moveInput = joystick.Vertical;
            turnInput = joystick.Horizontal;
            moveInput *= moveInput > 0 ? speed : rewSpeed;
        }
        transform.position = RB.transform.position;
        float newRot = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(0, newRot, 0, Space.World);
    }
    private void handle()
    {
        text_players.text = GameObject.FindGameObjectsWithTag("Player").Length + "/" +PhotonNetwork.CurrentRoom.MaxPlayers;
        if (is_start)
        {
            Wait_ga.SetActive(false);
        }
        else
        {
            Wait_ga.SetActive(true);
        }
        for (int i = 0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = (i+1).ToString();
            content.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = numbe(i).ToString();
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if(g.GetComponent<spcode>().m_parent.GetComponent<Player>().coin.ToString()== content.transform.GetChild(i).GetChild(2).GetComponent<Text>().text)
                {
                        content.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = g.GetComponent<spcode>().m_parent.gameObject.name;
                }
            }
        }
        text_coin.text = coin.ToString();
        text_time.text = roundedv.ToString();
        if(is_start)
        {
            if (time >= 0)
            {
                time -= Time.deltaTime;
                roundedv = MathF.Round(time);

            }
            else
            {
                // end game
                lose();
            }
        }
    }
    void lose()
    {
        if(photonView.IsMine)
        {
            int Gived;
            if (num() == "1")
            {
                Gived = 100;
            }
            else
            {
                if (num() == "2")
                {
                    Gived = 75;
                }
                else
                {
                    if (num() == "3")
                    {
                        Gived = 50;
                    }
                    else
                    {
                        Gived = 25;
                    }
                }
            }
            convas_lsoe.SetActive(true);
            if (aded_money == false)
            {
                PlayerPrefs.SetInt("mony", PlayerPrefs.GetInt("mony") + Gived * PlayerPrefs.GetInt("zarib"));
                aded_money = true;
            }
            text_number.text = Gived.ToString();
            text_rank.text = "Number : " + num();
            can_move = false;
        }
    }
    public void main_menu()
    {
        SceneManager.LoadScene(1);
        PhotonNetwork.LeaveRoom();
    }
    public void creat_faza()
    {
        if(!is_but)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
            {
                GameObject g = Instantiate(faza, content.transform.transform);
                g.transform.GetChild(2).GetComponent<Text>().text = GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<spcode>().m_parent.GetComponent<Player>().coin.ToString();
                g.transform.GetChild(0).GetComponent<Text>().text = GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<spcode>().m_parent.gameObject.name;
            }
        }
    }
    private int numbe(int i)
    {
        List<int> i_ls = new List<int>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            i_ls.Add(g.GetComponent<spcode>().m_parent.GetComponent<Player>().coin);
        }
        i_ls.Sort();
        i_ls.Reverse();
        return i_ls[i];
    }
    private string num()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            if(content.transform.GetChild(i).GetChild(0).GetComponent<Text>().text==photonView.Controller.NickName)
            {

                return content.transform.GetChild(i).GetChild(1).GetComponent<Text>().text;
            }
        }
        return null;
    }
    private void manage_Skin()
    {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(g.GetComponent<spcode>().m_parent.gameObject.transform.childCount==6)
            {
                print(g.GetComponent<spcode>().m_parent.GetComponent<Player>().my_char_name);
                GameObject prefab = Resources.Load<GameObject>(g.GetComponent<spcode>().m_parent.GetComponent<Player>().my_char_name);
                GameObject v = Instantiate(prefab, g.GetComponent<spcode>().m_parent.transform.position, Quaternion.identity);
                v.transform.parent = g.GetComponent<spcode>().m_parent.transform;
            }
        }
    }
    private void handle_start()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length==PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            is_start = true;
            can_move = true;
        }
    }
    public string GetNameBot()
    {
        List<string> used_name = new List<string> { };
        List<string> avaireble_name = new List<string> { };
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            used_name.Add(g.GetComponent<spcode>().m_parent.gameObject.name);
        }
        foreach (String item in bot_name)
        {
            if(!used_name.Contains(item))
            {
                avaireble_name.Add(item);
            }
        }
        if(avaireble_name.Count>0)
        {
            return avaireble_name[randome.Range(0, avaireble_name.Count)];
        }
        else
        {
            return null;
        }
    }
    private void check_target()
    {
        if(is_start&&can_move)
        {
            if(target==null)
            {
                set_target();
            }
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (dist < 0.1)
            {
                set_target();
            }
            agent.SetDestination(target.transform.position);
        }
    }
    private bool check(string s)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
                return content.transform.GetChild(i).GetChild(0).GetComponent<Text>().text == s;// text name
   
        }
        return false;
    }
    private void set_target()
    {
        target = GameObject.FindGameObjectsWithTag("Group")[randome.Range(0, GameObject.FindGameObjectsWithTag("Group").Length)];
    }
    [PunRPC]
    private void c()
    {
        coin += 25;
    }
    [PunRPC]
    void f()
    {
        if(coin>=200)
        {
            coin -= 200;
        }
    }
    [PunRPC]
    void set_naem(string name)
    {
        my_char_name = name;
    }
    [PunRPC]
    void set_name_bot()
    {
        gameObject.name = bot_name[randome.Range(0, bot_name.Count)];
    }
    [PunRPC]
    void set_but()
    {
        is_but = true;
    }
}
