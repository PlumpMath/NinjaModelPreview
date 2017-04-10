using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ProfileView : MonoBehaviour {

    public MapController map;

    public Canvas canvas;
    public Button closeButton;
    public Text nicknameLabel;
    public Image rankIcon;
    public Text countryLabel;
    public Image countryIcon;

    public GameObject clothContainer;
    public GameObject clothObject;

    public GameObject weaponContainer;
    public GameObject weaponObject;
    public MeshRenderer weaponBackQuad;

    public Button clothSelectionButton;
    public Button weaponSelectionButton;
    public SelectorController clothSelector;
    public SelectorController weaponSelector;
    public Text clothCounterLabel;
    public Text weaponCounterLabel;

    private int clothId = -1;
    private int weaponId = -1;
    private int weaponSkinId = -1;

    /******************************/

    void ClothSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        clothCounterLabel.text = (e.item.position + 1) + " / " + e.count;
    }

    void WeaponSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        weaponCounterLabel.text = (e.item.position + 1) + " / " + e.count;
    }

    void ClothSelectorActivate(object sender, SelectorCounterEventArgs e)
    {
        map.loginController.ChangeCloth(Int32.Parse(e.item.value));
    }

    void WeaponSelectorActivate(object sender, SelectorCounterEventArgs e)
    {
        string[] s = e.item.value.Split('_');
        map.loginController.ChangeWeapon(Int32.Parse(s[0]));
    }

    void CloseSelector(object sender, EventArgs e)
    {
        Open();
    }

    /******************************/

    // Use this for initialization
    void Start () {

        canvas.enabled = false;

        closeButton.onClick.AddListener(delegate () {
            Close(true);
        });

        clothSelector.OnPositionUpdate += ClothSelectorUpdate;
        clothSelector.OnItemActivate += ClothSelectorActivate;
        clothSelector.OnClose += CloseSelector;
        clothSelectionButton.onClick.AddListener(delegate() {
            ClothSelectorOpen();
        });

        weaponSelector.OnPositionUpdate += WeaponSelectorUpdate;
        weaponSelector.OnItemActivate += WeaponSelectorActivate;
        weaponSelector.OnClose += CloseSelector;
        weaponSelectionButton.onClick.AddListener(delegate () {
            WeaponSelectorOpen();
        });

    }

    // Update is called once per frame
    void Update () {
	
	}

    public void Reset(PlayerViewMessage playerView)
    {
        nicknameLabel.text = playerView.nickname;
        countryLabel.text = playerView.country;
        clothId = (int)playerView.cloth;
        weaponId = (int)playerView.weapon;
        weaponSkinId = (int)playerView.weaponSkin;

        if (canvas.enabled)
        {
            Close();
            Open();
        }
        else if(clothSelector.enabled)
        {
            clothSelector.Close();
            ClothSelectorOpen();
        }
        else if (weaponSelector.enabled)
        {
            weaponSelector.Close();
            WeaponSelectorOpen();
        }
        else
        {
            Close(true);
        }
    }

    void ClothSelectorOpen()
    {
        string[] names = new string[2];
        string[] values = new string[2];
        string[] titles = new string[2];

        names[0] = "body1";
        names[1] = "body2";

        values[0] = "1";
        values[1] = "2";

        titles[0] = "Одеяния Пустынных Бомжей";
        titles[1] = "Куртка Кожи С Жопы Дракона";

        clothSelector.Open("Prefabs/Bodies/", names, values, titles, clothId.ToString());

        Close();
    }

    void WeaponSelectorOpen()
    {
        string[] names = new string[6];
        string[] values = new string[6];
        string[] titles = new string[6];

        names[0] = "missile1_0";
        names[1] = "missile2_0";
        names[2] = "missile3_0";
        names[3] = "missile4_0";
        names[4] = "missile5_0";
        names[5] = "missile6_0";

        values[0] = "1_0";
        values[1] = "2_0";
        values[2] = "3_0";
        values[3] = "4_0";
        values[4] = "5_0";
        values[5] = "6_0";

        titles[0] = "Круглый сюрикен";
        titles[1] = "Круглый камуфляжный сюрикен";
        titles[2] = "Круглый узорчатый сюрикен";
        titles[3] = "Острый сюрикен";
        titles[4] = "Острый камуфляжный сюрикен";
        titles[5] = "Острый узорчатый сюрикен";

        weaponSelector.Open("Prefabs/Missiles/", names, values, titles, weaponId + "_" + weaponSkinId);

        Close();
    }

    public void Open()
    {
        canvas.enabled = true;
        weaponBackQuad.enabled = true;

        clothObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bodies/body" + clothId));
        clothObject.transform.parent = clothContainer.transform;
        clothObject.transform.localPosition = Vector3.zero;
        clothObject.transform.localRotation = Quaternion.identity;
        clothObject.transform.localScale = Vector3.one;
        clothObject.GetComponent<Animation>().Stop();

        weaponObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Missiles/missile" + weaponId + "_" + weaponSkinId));
        weaponObject.transform.parent = weaponContainer.transform;
        weaponObject.transform.localPosition = Vector3.zero;
        weaponObject.transform.localRotation = Quaternion.identity;
        weaponObject.transform.localScale = Vector3.one;

    }

    public void Close()
    {
        Close(false);
    }

    public void Close(bool leave)
    {
        canvas.enabled = false;
        weaponBackQuad.enabled = false;
        GameObject.Destroy(clothObject);
        GameObject.Destroy(weaponObject);
        if (leave)
        {
            map.Open();
        }
    }

}
