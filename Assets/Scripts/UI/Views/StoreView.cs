using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

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
            lockpickAmountLabel.text = ((BottleStoreItem)e.item).amount.ToString();
        }
    }

    void CloseSelector(object sender, EventArgs e)
    {
        Close(true);
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
                            clothItem.value = "1";
                            clothItem.label.text = "Синий";
                            clothItem.subLabel.text = "Отличный спортивный костюм из атласной ткани";
                            clothItem.priceLabel.text = "2 999";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body1");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.GOLD);
                            clothItem.HideBuyButton();
                            clothItem.buyButton.onClick.AddListener(delegate() {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);

                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "2";
                            clothItem.label.text = "Зелёный";
                            clothItem.subLabel.text = "Маскировочный костюм. Отлично подходит для скрытного передвижения по лесу";
                            clothItem.priceLabel.text = "3 999";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body2");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.GOLD);
                            clothItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);

                            clothItem = GameObject.Instantiate<GameObject>(clothStoreItemPrefab.gameObject).GetComponent<ClothStoreItem>();
                            clothItem.value = "3";
                            clothItem.label.text = "Красный";
                            clothItem.subLabel.text = "Парадный костюм вождя краснокожих";
                            clothItem.priceLabel.text = "199";
                            clothItem.LoadMesh("Prefabs/Bodies/", "body3");
                            clothItem.SetCurrency(ClothStoreItem.CURRENCY.EMERALD);
                            clothItem.buyButton.onClick.AddListener(delegate () {
                                map.errorNoticeView.Open("Ошибка!", "При покупке возникла ошибка.");
                            });
                            selector.items.AddLast(clothItem);
                            break;
                        case 1:
                            break;
                        case 2:
                            bottleItem = GameObject.Instantiate<GameObject>(bottleStoreItemPrefab.gameObject).GetComponent<BottleStoreItem>();
                            bottleItem.value = "1";
                            bottleItem.label.text = "Виноградное вино";
                            bottleItem.subLabel.text = "Лучшее средство для облегчения боли";
                            bottleItem.priceLabel.text = "10";
                            bottleItem.LoadMesh("Prefabs/Bottles/", "bottle1");
                            bottleItem.amount = 0;
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
                            bottleItem.amount = 5;
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

}
