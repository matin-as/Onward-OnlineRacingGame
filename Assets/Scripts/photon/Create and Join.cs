using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using RTLTMPro;
using UnityEngine.SceneManagement;
using System.Text;
using Random = System.Random;
using System.Linq;
using WebSocketSharp;
public class CreateandJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject ads_panel;
    [SerializeField] GameObject more_ga;
    [SerializeField] RTLTextMeshPro text_car;
    [SerializeField] GameObject plan;
    [SerializeField] TextMeshProUGUI text_btn;
    [SerializeField] TextMeshProUGUI text_price;
    [SerializeField] GameObject shope_convas;
    [SerializeField] TextMeshProUGUI text_mony;
    [SerializeField] TMP_InputField creat_Input;
    [SerializeField] TMP_InputField Join_Input;
    [SerializeField] TMP_InputField NikName_Input;
    [SerializeField] GameObject creat_g;
    [SerializeField] GameObject join_g;
    [SerializeField] TMP_Dropdown Input_Max_Players;
    [SerializeField] ToggleGroup toggleGroup;
    int countet;
    int n;
    private string maxplayer = "0";
    private byte Max;
    bool is_Open_Cars;
    void Start()
    {
        PlayerPrefs.SetInt("zarib",1);
        PlayerPrefs.SetString("hidebanner", "no");
        set_car();
        NikName_Input.text = PlayerPrefs.GetString("player_name");
        Input_Max_Players.onValueChanged.AddListener(select);

    }
    void Update()
    {
        text_mony.text = PlayerPrefs.GetInt("mony").ToString();
        check();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Building_9")
                {
                    play_2();
                }
                if (hit.transform.name == "Building_7")
                {
                    creat_g.SetActive(true);
                    join_g.SetActive(false);
                    // create room
                }
                if (hit.transform.name == "Building_3")
                {
                    creat_g.SetActive(false);
                    join_g.SetActive(true);
                    // Join room
                }
                if (hit.transform.name == "Garage")
                {
                    countet = 0;
                    print(text_car.text);
                    if (!is_Open_Cars)
                    {
                        PlayerPrefs.SetString("hidebanner", "yes");
                        is_Open_Cars = true;
                        shope_convas.SetActive(true);
                        plan.SetActive(true);
                        int i = 0;
                        int p = 1000;
                        // Open store
                        foreach (GameObject g in Resources.LoadAll<GameObject>("Allcars"))
                        {
                            p += 200;
                            i += 3;
                            GameObject v = Instantiate(g, new Vector3(3.68f, 0, -6.55f), Quaternion.identity);
                            v.AddComponent<carprice>();
                            v.GetComponent<carprice>().price = p;
                            v.transform.parent = gameObject.transform;
                            v.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            v.transform.Translate(i, 0, 0);
                            v.transform.localPosition = new Vector3(v.transform.localPosition.x, v.transform.localPosition.y, 1.41f);
                            v.transform.Rotate(0, 180, 0);
                        }
                    }
                    else
                    {
                        PlayerPrefs.SetString("hidebanner", "no");
                        is_Open_Cars = false;
                        shope_convas.SetActive(false);
                        plan.SetActive(false);
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            Destroy(transform.GetChild(i).gameObject);
                        }
                    }
                }
            }
        }
    }
    public void creat_room()
    {
        if(string.IsNullOrEmpty(creat_Input.text))
        {
            _ShowAndroidToastMessage("set room name "); 
            return;
        }
        if (!string.IsNullOrEmpty(NikName_Input.text))
        {
                GameObject.Find("ads").GetComponent<ads>().show_rewarded_ad();
                bool is_vailed;
                Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
                if (toggle.name == "Toggle Private")
                {
                    is_vailed = false;
                }
                else
                {
                    is_vailed = true;
                }
                PhotonNetwork.NickName = NikName_Input.text;
                if (maxplayer == "0")
                {
                    maxplayer = "5";
                }
                byte.TryParse(maxplayer, out Max);
                PhotonNetwork.CreateRoom(creat_Input.text, new RoomOptions() { IsOpen = true, MaxPlayers = Max, IsVisible = is_vailed });
        }
        else
        {
            _ShowAndroidToastMessage("Set name !");
        }
    }
    public void join_room()
    {
        if (!string.IsNullOrEmpty(NikName_Input.text))
        {
            PhotonNetwork.NickName = NikName_Input.text;
            PhotonNetwork.JoinRoom(Join_Input.text);
        }
        else
        {
            _ShowAndroidToastMessage("Set name !");
        }
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(2);
        base.OnJoinedRoom();
    }
    public void play()
    {
        if (NikName_Input.text.Length != 0)
        {
            ads_panel.SetActive(true);
        }
        else
        {
            _ShowAndroidToastMessage("Set name !");
            // show Toast 
        }
    }
    public void play_2()
    {
        if(string.IsNullOrEmpty(NikName_Input.text))
        {
            _ShowAndroidToastMessage("Set name !");
            return;
        }
        PlayerPrefs.SetString("hidebanner", "yes");
        PlayerPrefs.SetString("player_name", NikName_Input.text);
        PhotonNetwork.NickName = NikName_Input.text;
        PhotonNetwork.JoinRandomRoom();
    }
    public void play3()
    {
        if (GameObject.Find("ads").GetComponent<ads>().can_play)
        {
            GameObject.Find("ads").GetComponent<ads>().show_rewarded_2();
        }
        else
        {
            _ShowAndroidToastMessage("Wait to load ad ...");
            //play_2();
        }
    }
    private void _ShowAndroidToastMessage(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        else
        {
            print(message);
        }
    }
    void set_car()
    {
        if (PlayerPrefs.GetString("CurectCar") == "")
        {
            PlayerPrefs.SetString("CurectCarFromGame", "car_6_black(Clone)");
            PlayerPrefs.SetString("CurectCar", "car_6_black");
            PlayerPrefs.SetString("car_6_black(Clone)", "yes");
        }
        foreach (GameObject g in Resources.LoadAll<GameObject>("Allcars"))
        {
            if (g.name == PlayerPrefs.GetString("CurectCar"))
            {
                GameObject v = Instantiate(g, new Vector3(3.68f, 0, -5.31f), Quaternion.identity);
                v.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                v.transform.Rotate(0, 180, 0);
                v.tag = "seted";
            }
        }
    }
    public void Next()
    {
        for (int i = 0; i < transform.childCount; i++)
        {

            if (countet < transform.childCount - 2)
            {
                GameObject g = transform.GetChild(i).gameObject;
                g.transform.Translate(3, 0, 0);
                if (g.transform.localPosition.x < 3)
                {
                    g.SetActive(false);
                }
                else
                {
                    g.SetActive(true);
                }
            }
        }
        if (countet < transform.childCount - 2)
        {
            countet++;
        }
    }
    public void Back()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (countet > -1)
            {
                GameObject g = transform.GetChild(i).gameObject;
                g.transform.Translate(-3, 0, 0);
                if (g.transform.localPosition.x < 3)
                {
                    g.SetActive(false);
                }
                else
                {
                    g.SetActive(true);
                }//12
            }
        }
        if (countet > -1)
        {
            countet--;
        }
    }
    private void check()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            if (g.transform.localPosition.x == 6)
            {
                if (PlayerPrefs.GetString(g.name) == "yes")
                {
                    if (g.name == PlayerPrefs.GetString("CurectCarFromGame"))
                    {
                        text_btn.text = "Selected";
                    }
                    else
                    {
                        text_btn.text = "Select";
                    }
                }
                else
                {
                    text_btn.text = "Buy";
                }
                text_price.text = g.GetComponent<carprice>().price.ToString();
            }
        }
    }
    public void Buy()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            int.TryParse(text_price.text, out n);
            if (g.transform.localPosition.x == 6)
            {
                if (PlayerPrefs.GetString(g.name) == "yes")
                {
                    Destroy(GameObject.FindGameObjectWithTag("seted"));
                    PlayerPrefs.SetString("CurectCarFromGame", g.name);
                    PlayerPrefs.SetString("CurectCar", g.name.Replace("(Clone)", ""));
                    GameObject t = Instantiate(Resources.Load<GameObject>("Allcars/" + PlayerPrefs.GetString("CurectCar")), new Vector3(3.68f, 0, -5.31f), Quaternion.identity);
                    t.tag = "seted";
                    t.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    t.transform.Rotate(0, 180, 0);
                }
                else
                {
                    if (PlayerPrefs.GetInt("mony") >= n)
                    {
                        Destroy(GameObject.FindGameObjectWithTag("seted"));
                        PlayerPrefs.SetInt("mony", PlayerPrefs.GetInt("mony") - n);
                        PlayerPrefs.SetString(g.name, "yes");
                        PlayerPrefs.SetString("CurectCarFromGame", g.name);
                        PlayerPrefs.SetString("CurectCar", g.name.Replace("(Clone)", ""));
                        GameObject t = Instantiate(Resources.Load<GameObject>("Allcars/" + PlayerPrefs.GetString("CurectCar")), new Vector3(3.68f, 0, -5.31f), Quaternion.identity);
                        t.tag = "seted";
                        t.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                        t.transform.Rotate(0, 180, 0);
                    }
                }
            }
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);
        PhotonNetwork.CreateRoom(GetRandomName(7), new RoomOptions { MaxPlayers = 5 });
        base.OnJoinRandomFailed(returnCode, message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _ShowAndroidToastMessage("Room not found ");
        base.OnJoinRoomFailed(returnCode, message);
    }
    private static string GetRandomName(int length)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        StringBuilder sb = new StringBuilder();
        Random rnd = new Random();
        for (int i = 0; i < length; i++)
        {
            int index = rnd.Next(chars.Length);
            sb.Append(chars[index]);
        }

        return sb.ToString();
    }
    private void select(int index)
    {
        maxplayer = Input_Max_Players.options[index].text;
    }
    public void On_rating_btn()
    {
        Application.OpenURL("myket://comment?id=com.mis.cargame");
    }
    public void On_applicationpage_btn()
    {
        Application.OpenURL("myket://details?id=com.mis.cargame");
    }
    public void On_List_btn()
    {
        Application.OpenURL("myket://developer/com.mis.cargame");
    }
    public void more()
    {
        if(more_ga.activeInHierarchy)
        {
            more_ga.SetActive(false);
        }
        else
        {
            more_ga.SetActive(true);
        }
    }
}


    public class carprice : MonoBehaviour
{
    public int price;
}
