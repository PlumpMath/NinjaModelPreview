﻿using UnityEngine;
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

    public ClothEquipItem clothEquipItemPrefab;
    public WeaponEquipItem weaponEquipItemPrefab;

    public Button clothSelectionButton;
    public Button weaponSelectionButton;
    public SelectorController clothSelector;
    public SelectorController weaponSelector;
    public Text clothCounterLabel;
    public Text weaponCounterLabel;

    public Button changeNicknameButton;

    private int clothId = -1;
    private int weaponId = -1;
    private int weaponSkinId = -1;

    /******************************/

    void ClothSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            clothCounterLabel.text = (e.item.position + 1) + " / " + e.count;
        }
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
        clothSelector.OnItemActivateHolding += ClothSelectorActivate;
        clothSelector.OnClose += CloseSelector;
        clothSelectionButton.onClick.AddListener(delegate() {
            ClothSelectorOpen();
        });

        weaponSelector.OnPositionUpdate += WeaponSelectorUpdate;
        weaponSelector.OnItemActivate += WeaponSelectorActivate;
        weaponSelector.OnItemActivateHolding += WeaponSelectorActivate;
        weaponSelector.OnClose += CloseSelector;
        weaponSelectionButton.onClick.AddListener(delegate () {
            WeaponSelectorOpen();
        });

        changeNicknameButton.onClick.AddListener(delegate() {
            map.changeNicknameView.Open();
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
        else if(map.mapCanvas.enabled)
        {
            Close(true);
        }
    }

    void ClothSelectorOpen()
    {
        ClothEquipItem item;
        string pathPrefix = "Prefabs/Bodies/";

        item = GameObject.Instantiate<GameObject>(clothEquipItemPrefab.gameObject).GetComponent<ClothEquipItem>();
        item.value = "10001";
        item.label.text = "Гимнастерка искателя приключений";
        item.LoadMesh(pathPrefix, "body10001");
        item.SetSelectorController(clothSelector);
        clothSelector.items.AddLast(item);

        if (map.storeView.GetItemAmount("10002") > 0)
        {
            item = GameObject.Instantiate<GameObject>(clothEquipItemPrefab.gameObject).GetComponent<ClothEquipItem>();
            item.value = "10002";
            item.label.text = "Пальто стеснительной лягушки";
            item.LoadMesh(pathPrefix, "body10002");
            item.SetSelectorController(clothSelector);
            clothSelector.items.AddLast(item);
        }

        if (map.storeView.GetItemAmount("10003") > 0)
        {
            item = GameObject.Instantiate<GameObject>(clothEquipItemPrefab.gameObject).GetComponent<ClothEquipItem>();
            item.value = "10003";
            item.label.text = "Мумия возвращается";
            item.LoadMesh(pathPrefix, "body10003");
            item.SetSelectorController(clothSelector);
            clothSelector.items.AddLast(item);
        }

        if (map.storeView.GetItemAmount("10004") > 0)
        {
            item = GameObject.Instantiate<GameObject>(clothEquipItemPrefab.gameObject).GetComponent<ClothEquipItem>();
            item.value = "10004";
            item.label.text = "Летняя хоккейная форма";
            item.LoadMesh(pathPrefix, "body10004");
            item.SetSelectorController(clothSelector);
            clothSelector.items.AddLast(item);
        }

        if (map.storeView.GetItemAmount("10005") > 0)
        {
            item = GameObject.Instantiate<GameObject>(clothEquipItemPrefab.gameObject).GetComponent<ClothEquipItem>();
            item.value = "10005";
            item.label.text = "Сбежавший с полей китаец";
            item.LoadMesh(pathPrefix, "body10005");
            item.SetSelectorController(clothSelector);
            clothSelector.items.AddLast(item);
        }

        if (map.storeView.GetItemAmount("10006") > 0)
        {
            item = GameObject.Instantiate<GameObject>(clothEquipItemPrefab.gameObject).GetComponent<ClothEquipItem>();
            item.value = "10006";
            item.label.text = "Растаманская броня";
            item.LoadMesh(pathPrefix, "body10006");
            item.SetSelectorController(clothSelector);
            clothSelector.items.AddLast(item);
        }

        clothSelector.Open(clothId.ToString());

        Close();
    }

    void WeaponSelectorOpen()
    {

        WeaponEquipItem item;
        string pathPrefix = "Prefabs/Missiles/";

        item = GameObject.Instantiate<GameObject>(weaponEquipItemPrefab.gameObject).GetComponent<WeaponEquipItem>();
        item.value = "11001_0";
        item.label.text = "Круглый сюрикен";
        item.LoadMesh(pathPrefix, "missile11001_0");
        item.SetSelectorController(weaponSelector);
        weaponSelector.items.AddLast(item);

        item = GameObject.Instantiate<GameObject>(weaponEquipItemPrefab.gameObject).GetComponent<WeaponEquipItem>();
        item.value = "11002_0";
        item.label.text = "Треугольный сюрикен";
        item.LoadMesh(pathPrefix, "missile11002_0");
        item.SetSelectorController(weaponSelector);
        weaponSelector.items.AddLast(item);

        item = GameObject.Instantiate<GameObject>(weaponEquipItemPrefab.gameObject).GetComponent<WeaponEquipItem>();
        item.value = "11003_0";
        item.label.text = "Маленький сюрикен";
        item.LoadMesh(pathPrefix, "missile11003_0");
        item.SetSelectorController(weaponSelector);
        weaponSelector.items.AddLast(item);

        item = GameObject.Instantiate<GameObject>(weaponEquipItemPrefab.gameObject).GetComponent<WeaponEquipItem>();
        item.value = "11004_0";
        item.label.text = "Звездонутый сюрикен";
        item.LoadMesh(pathPrefix, "missile11004_0");
        item.SetSelectorController(weaponSelector);
        weaponSelector.items.AddLast(item);

        weaponSelector.Open(weaponId + "_" + weaponSkinId);

        Close();
    }

    public void Open()
    {
        canvas.enabled = true;
        //weaponBackQuad.enabled = true;

        Debug.Log("CLOTH: " + clothId);
        clothObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bodies/body" + clothId));
        clothObject.transform.parent = clothContainer.transform;
        clothObject.transform.localPosition = Vector3.zero;
        clothObject.transform.localRotation = Quaternion.identity;
        clothObject.transform.localScale = Vector3.one;
        //clothObject.GetComponent<Animation>().Stop();

        Debug.Log("WEAPON: " + weaponId + "_" + weaponSkinId);
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
        //weaponBackQuad.enabled = false;
        GameObject.Destroy(clothObject);
        GameObject.Destroy(weaponObject);
        if (leave)
        {
            map.Open();
        }
    }

}
