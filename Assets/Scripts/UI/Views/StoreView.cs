using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreView : MonoBehaviour {

    public Canvas canvas;
    public MapController map;
    public ClothStoreItem clothStoreItemPrefab;
    public WeaponSkinStoreItem weaponSkinStoreItemPrefab;
    public BottleStoreItem bottleStoreItemPrefab;
    public LockpickStoreItem lockpickStoreItemPrefab;

    public Text clothCounterLabel;
    public Text weaponSkinCounterLabel;
    public Text bottleCounterLabel;
    public Text bottleAmountLabel;
    public Text lockpickCounterLabel;
    public Text lockpickAmountLabel;

    public Button[] categoryButtons;
    public Sprite[] categoryUnactiveSprites;
    public Sprite[] categoryActiveSprites;

    public SelectorController[] storeSelectorControllers;


    // Use this for initialization
    void Start () {

        int i;

        categoryButtons[0].onClick.AddListener(delegate() {
            Open(0);
        });
        categoryButtons[1].onClick.AddListener(delegate () {
            Open(1);
        });
        categoryButtons[2].onClick.AddListener(delegate () {
            Open(2);
        });

        for (i = 0; i < storeSelectorControllers.Length; i++)
        {
            storeSelectorControllers[i].OnClose += CloseSelector;
        }

        storeSelectorControllers[0].OnPositionUpdate += ClothSelectorUpdate;
        storeSelectorControllers[1].OnPositionUpdate += LockpickSelectorUpdate;
        storeSelectorControllers[2].OnPositionUpdate += BottleSelectorUpdate;

        map.OnPlayerInventoryLoaded += OnUpdateInventory;

    }

    /******************************/

    void ClothSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            clothCounterLabel.text = (e.item.position + 1) + " / " + e.count;
        }
    }

    void WeaponSkinSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            weaponSkinCounterLabel.text = (e.item.position + 1) + " / " + e.count;
        }
    }

    void BottleSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            bottleCounterLabel.text = (e.item.position + 1) + " / " + e.count;
            bottleAmountLabel.text = ((BottleStoreItem)e.item).amount.ToString();
        }
    }

    void LockpickSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            lockpickCounterLabel.text = (e.item.position + 1) + " / " + e.count;
            lockpickAmountLabel.text = ((LockpickStoreItem)e.item).amount.ToString();
        }
    }

    void CloseSelector(object sender, EventArgs e)
    {
        Close(true);
    }

    void RefreshLockpickAmount(LockpickStoreItem item)
    {
        item.amount = GetItemAmount(item.value);
    }

    void OnUpdateInventory(object sender, PlayerDataEventArgs e)
    {
        Debug.Log("ON UPDATE INVENTORY");
        int i;
        SelectorController selector;
        LinkedListNode<SelectorItem> node;
        for (i = 0; i < storeSelectorControllers.Length; i++)
        {
            selector = storeSelectorControllers[i];
            if (selector.canvas.enabled)
            {
                switch (selector.name)
                {
                    case "CanvasLockpickStore":
                        node = selector.items.First;
                        while (node != null)
                        {
                            RefreshLockpickAmount((LockpickStoreItem)node.Value);
                            node = node.Next;
                        }
                        break;
                }
                selector.UpdatePosition();
            }
        }
    }

    void BuyItem(string value)
    {
        map.loginController.BuyItem(Int32.Parse(value));
    }

    /******************************/

    // Update is called once per frame
    void Update () {
	
	}

    public void Open()
    {
        Open(0);
    }

    public void Open(int category)
    {
        Open(category, "");
    }

    public void Open(int category, string itemCode)
    {
        int i;
        SelectorController selector;
        ClothStoreItem clothItem;
        WeaponSkinStoreItem skinItem;
        BottleStoreItem bottleItem;
        LockpickStoreItem lockpickItem;
        for(i = 0; i < categoryButtons.Length; i++)
        {
        }
        for(i = 0; i < storeSelectorControllers.Length; i++)
        {
            if (i == category)
            {
                categoryButtons[i].image.sprite = categoryActiveSprites[i];
                if (!storeSelectorControllers[i].enabled)
                {
                    selector = storeSelectorControllers[i];
                    selector.CleanupList();
                    switch (i)
                    {
                        case 0:
                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "10001";
                            clothItem.label.text = "Гимнастерка искателя приключений";
                            clothItem.subLabel.text = "Отличный спортивный костюм из атласной ткани";
                            clothItem.priceLabel.text = "0";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body10001");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.GOLD);
                            clothItem.HideBuyButton();
                            clothItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);

                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "10002";
                            clothItem.label.text = "Пальто стеснительной лягушки";
                            clothItem.subLabel.text = "Отличный спортивный костюм из атласной ткани";
                            clothItem.priceLabel.text = "10";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body10002");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.GOLD);
                            if (GetItemAmount(clothItem.value) > 0)
                            {
                                clothItem.HideBuyButton();
                            }
                            clothItem.buyButton.onClick.AddListener(delegate() {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);

                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "10003";
                            clothItem.label.text = "Мумия возвращается";
                            clothItem.subLabel.text = "Отличный спортивный костюм из атласной ткани";
                            clothItem.priceLabel.text = "20";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body10003");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.GOLD);
                            if (GetItemAmount(clothItem.value) > 0)
                            {
                                clothItem.HideBuyButton();
                            }
                            clothItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);

                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "10004";
                            clothItem.label.text = "Летняя хоккейная форма";
                            clothItem.subLabel.text = "Отличный спортивный костюм из атласной ткани";
                            clothItem.priceLabel.text = "5";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body10004");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.EMERALD);
                            if (GetItemAmount(clothItem.value) > 0)
                            {
                                clothItem.HideBuyButton();
                            }
                            clothItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);

                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "10005";
                            clothItem.label.text = "Сбежавший с полей китаец";
                            clothItem.subLabel.text = "Отличный спортивный костюм из атласной ткани";
                            clothItem.priceLabel.text = "10";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body10005");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.EMERALD);
                            if (GetItemAmount(clothItem.value) > 0)
                            {
                                clothItem.HideBuyButton();
                            }
                            clothItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);
                            break;
                        case 1:
                            lockpickItem = GameObject.Instantiate<GameObject>(lockpickStoreItemPrefab.gameObject).GetComponent<LockpickStoreItem>();
                            lockpickItem.value = "15001";
                            lockpickItem.label.text = "Ржавая отмычка";
                            lockpickItem.subLabel.text = "Шанс взлома - 25%";
                            lockpickItem.priceLabel.text = "1";
                            lockpickItem.LoadMesh("Prefabs/Lockpicks/", "lockpick15001");
                            lockpickItem.amount = GetItemAmount(lockpickItem.value);
                            lockpickItem.SetCurrency(LockpickStoreItem.CURRENCY.GOLD);
                            lockpickItem.buyButton.onClick.AddListener(delegate () {
                                BuyItem("15001");
                            });
                            selector.items.AddLast(lockpickItem);

                            lockpickItem = GameObject.Instantiate<GameObject>(lockpickStoreItemPrefab.gameObject).GetComponent<LockpickStoreItem>();
                            lockpickItem.value = "15002";
                            lockpickItem.label.text = "Отмычка с бамбуковой ручкой";
                            lockpickItem.subLabel.text = "Шанс взлома - 50%";
                            lockpickItem.priceLabel.text = "1";
                            lockpickItem.LoadMesh("Prefabs/Lockpicks/", "lockpick15002");
                            lockpickItem.amount = GetItemAmount(lockpickItem.value);
                            lockpickItem.SetCurrency(LockpickStoreItem.CURRENCY.GOLD);
                            lockpickItem.buyButton.onClick.AddListener(delegate () {
                                BuyItem("15002");
                            });
                            selector.items.AddLast(lockpickItem);

                            lockpickItem = GameObject.Instantiate<GameObject>(lockpickStoreItemPrefab.gameObject).GetComponent<LockpickStoreItem>();
                            lockpickItem.value = "15003";
                            lockpickItem.label.text = "Отмычка в алом шарфе!";
                            lockpickItem.subLabel.text = "Шанс взлома - 100%";
                            lockpickItem.priceLabel.text = "1";
                            lockpickItem.LoadMesh("Prefabs/Lockpicks/", "lockpick15003");
                            lockpickItem.amount = GetItemAmount(lockpickItem.value);
                            lockpickItem.SetCurrency(LockpickStoreItem.CURRENCY.GOLD);
                            lockpickItem.buyButton.onClick.AddListener(delegate () {
                                BuyItem("15003");
                            });
                            selector.items.AddLast(lockpickItem);
                            break;
                        case 2:
                            bottleItem = GameObject.Instantiate<GameObject>(bottleStoreItemPrefab.gameObject).GetComponent<BottleStoreItem>();
                            bottleItem.value = "1";
                            bottleItem.label.text = "Виноградное вино";
                            bottleItem.subLabel.text = "Лучшее средство для облегчения боли";
                            bottleItem.priceLabel.text = "10";
                            bottleItem.LoadMesh("Prefabs/Bottles/", "bottle1");
                            bottleItem.amount = GetItemAmount(bottleItem.value);
                            bottleItem.SetCurrency(BottleStoreItem.CURRENCY.GOLD);
                            bottleItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(bottleItem);

                            bottleItem = GameObject.Instantiate<GameObject>(bottleStoreItemPrefab.gameObject).GetComponent<BottleStoreItem>();
                            bottleItem.value = "2";
                            bottleItem.label.text = "Чудное варево";
                            bottleItem.subLabel.text = "Лечит любые травмы. Применяется после боя";
                            bottleItem.priceLabel.text = "5";
                            bottleItem.LoadMesh("Prefabs/Bottles/", "bottle2");
                            bottleItem.amount = GetItemAmount(bottleItem.value);
                            bottleItem.SetCurrency(BottleStoreItem.CURRENCY.EMERALD);
                            bottleItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(bottleItem);
                            break;
                    }
                    selector.Open(itemCode);
                }
            }
            else
            {
                categoryButtons[i].image.sprite = categoryUnactiveSprites[i];
                if (storeSelectorControllers[i].enabled)
                {
                    storeSelectorControllers[i].Close();
                }
            }
        }
        canvas.enabled = true;
    }

    public void Close()
    {
        Close(false);
    }

    public void Close(bool leave)
    {
        canvas.enabled = false;
        if (leave)
        {
            map.Open();
        }
    }

    /************************************/

    public int GetItemAmount(string id)
    {
        int itemId = Int32.Parse(id);
        if(map.playerInventory == null)
        {
            return 0;
        }
        LinkedListNode<PlayerItemNode> node = map.playerInventory.items.First;
        while(node != null)
        {
            if(node.Value.itemId == itemId)
            {
                return node.Value.itemAmount;
            }
            node = node.Next;
        }
        return 0;
    }

}
