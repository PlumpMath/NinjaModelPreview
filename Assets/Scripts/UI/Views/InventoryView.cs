using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InventoryView : MonoBehaviour
{

    public Canvas canvas;
    public MapController map;
    public BottleUseItem bottleUseItemPrefab;
    public ChestUseItem chestUseItemPrefab;
    //public LockpickUseItem lockpickUseItemPrefab;

    public Text bottleCounterLabel;
    public Text bottleAmountLabel;
    public Text chestCounterLabel;
    public Text chestAmountLabel;
    public Text lockpickCounterLabel;
    public Text lockpickAmountLabel;

    public Button[] categoryButtons;
    public Sprite[] categoryUnactiveSprites;
    public Sprite[] categoryActiveSprites;

    public SelectorController[] inventorySelectorControllers;

    public RectTransform chestCarousel;
    public RectTransform chestCarouselItemsContainer;
    public Text[] chestLockpicksAmount = new Text[0];
    public Button[] chestLockpickButtons = new Button[0];
    public RectTransform chestLockpickSelector;
    public float chestCarouselPeriod = 640.0f;
    public string chestCarouselTargetId = "";
    public short chestCarouselResult = -1;
    public float chestCarouselTargetPosition = 0.0f;
    public string useLockpickId = "15001";
    public int networkWaitingLoops = 0;
    public int freeLoops = 0;
    public float chestClosingCooldown = 0.0f;

    /**************************************/

    public void OnInventoryLoaded(object sender, PlayerDataEventArgs e)
    {
        Debug.Log("ON UPDATE INVENTORY");
        int i;
        SelectorController selector;
        LinkedListNode<SelectorItem> node;
        for (i = 0; i < inventorySelectorControllers.Length; i++)
        {
            selector = inventorySelectorControllers[i];
            if (selector.canvas.enabled)
            {
                switch (selector.name)
                {
                    case "CanvasChestInventory":
                        node = selector.items.First;
                        while (node != null)
                        {
                            ((ChestUseItem)node.Value).amount = map.storeView.GetItemAmount(node.Value.value);
                            node = node.Next;
                        }
                        break;
                }
                selector.UpdatePosition();
            }
        }
        chestLockpicksAmount[0].text = map.storeView.GetItemAmount("15001").ToString();
        chestLockpicksAmount[1].text = map.storeView.GetItemAmount("15002").ToString();
        chestLockpicksAmount[2].text = map.storeView.GetItemAmount("15003").ToString();
    }

    public void UseChest(string chestId)
    {
        if(chestCarousel.anchoredPosition.y > 0.0f || map.storeView.GetItemAmount(chestId) < 1 || map.storeView.GetItemAmount(useLockpickId) < 1)
        {
            return;
        }
        chestCarouselTargetId = "";
        chestCarouselTargetPosition = chestCarouselPeriod * (Mathf.Floor(UnityEngine.Random.Range(0.0f, 4.999f)) / 5.0f);
        networkWaitingLoops = 1;
        freeLoops = 2;
        chestCarouselResult = -1;
        chestClosingCooldown = 3.0f;
        chestCarousel.anchoredPosition += new Vector2(0.0f, 108.0f - chestCarousel.anchoredPosition.y);
        map.loginController.OpenChest(Int32.Parse(chestId), Int32.Parse(useLockpickId));
    }

    /**************************************/

    // Use this for initialization
    void Start()
    {

        int i;

        categoryButtons[0].onClick.AddListener(delegate () {
            Open(0);
        });
        categoryButtons[1].onClick.AddListener(delegate () {
            Open(1);
        });
        categoryButtons[2].onClick.AddListener(delegate () {
            Open(2);
        });

        chestLockpickButtons[0].onClick.AddListener(delegate() {
            useLockpickId = "15001";
            chestLockpickSelector.anchoredPosition += Vector2.right * (-200.0f - chestLockpickSelector.anchoredPosition.x);
        });
        chestLockpickButtons[1].onClick.AddListener(delegate () {
            useLockpickId = "15002";
            chestLockpickSelector.anchoredPosition += Vector2.right * (-chestLockpickSelector.anchoredPosition.x);
        });
        chestLockpickButtons[2].onClick.AddListener(delegate () {
            useLockpickId = "15003";
            chestLockpickSelector.anchoredPosition += Vector2.right * (200.0f - chestLockpickSelector.anchoredPosition.x);
        });

        for (i = 0; i < inventorySelectorControllers.Length; i++)
        {
            inventorySelectorControllers[i].OnClose += CloseSelector;
        }

        inventorySelectorControllers[0].OnPositionUpdate += ChestSelectorUpdate;
        inventorySelectorControllers[1].OnPositionUpdate += LockpickSelectorUpdate;
        inventorySelectorControllers[2].OnPositionUpdate += BottleSelectorUpdate;

        map.OnPlayerInventoryLoaded += OnInventoryLoaded;

        chestCarousel.anchoredPosition += new Vector2(0.0f, -2000.0f);

    }

    /******************************/

    void BottleSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            bottleCounterLabel.text = (e.item.position + 1) + " / " + e.count;
            bottleAmountLabel.text = ((BottleStoreItem)e.item).amount.ToString();
        }
    }

    void ChestSelectorUpdate(object sender, SelectorCounterEventArgs e)
    {
        if (e.item != null)
        {
            chestCounterLabel.text = (e.item.position + 1) + " / " + e.count;
            chestAmountLabel.text = ((ChestUseItem)e.item).amount.ToString();
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

    /******************************/

    // Update is called once per frame
    void Update()
    {
        if(!canvas.enabled)
        {
            return;
        }

        if (chestCarousel.anchoredPosition.y > 0.0f)
        {
            if(networkWaitingLoops <= 0 && freeLoops <= 0 && chestCarouselItemsContainer.anchoredPosition.x <= -chestCarouselTargetPosition)
            {
                if (chestClosingCooldown > 0.0f)
                {
                    chestClosingCooldown -= Time.deltaTime;
                }
                else
                {
                    if (chestCarouselResult != 1)
                    {
                        map.errorNoticeView.Open("ВСЁ СЛОМАЛОСЬ!", "Используй качественные отмычки.");
                    }
                    else
                    {
                        map.errorNoticeView.Open("ПОЗДРАВЛЯЕМ!", "Забирай награбленное.");
                    }
                    chestCarousel.anchoredPosition += new Vector2(0.0f, -2000.0f);
                    chestCarouselTargetId = "";
                    chestCarouselResult = -1;
                    chestCarouselTargetPosition = 0.0f;
                }
            }
            else
            {
                if(networkWaitingLoops > 0)
                {
                    chestCarouselItemsContainer.anchoredPosition -= Vector2.right * Time.deltaTime * chestCarouselPeriod * 2.0f;
                }
                else if (freeLoops > 0)
                {
                    chestCarouselItemsContainer.anchoredPosition -= Vector2.right * Time.deltaTime * chestCarouselPeriod;
                }
                else
                {
                    chestCarouselItemsContainer.anchoredPosition -= Vector2.right * Time.deltaTime * 3.0f * (chestCarouselItemsContainer.anchoredPosition.x + chestCarouselTargetPosition + 5.0f);
                }
                if (chestCarouselItemsContainer.anchoredPosition.x < -chestCarouselPeriod)
                {
                    if(networkWaitingLoops > 0)
                    {
                        if (chestCarouselTargetId != "")
                        {
                            networkWaitingLoops--;
                        }
                    }
                    else if(freeLoops > 0)
                    {
                        freeLoops--;
                    }
                    chestCarouselItemsContainer.anchoredPosition += Vector2.right * chestCarouselPeriod;
                }
            }
        }
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
        ChestUseItem chestItem;
        BottleUseItem bottleItem;
        //LockpickUseItem lockpickItem;
        for (i = 0; i < categoryButtons.Length; i++)
        {
        }
        for (i = 0; i < inventorySelectorControllers.Length; i++)
        {
            if (i == category)
            {
                categoryButtons[i].image.sprite = categoryActiveSprites[i];
                if (!inventorySelectorControllers[i].enabled)
                {
                    selector = inventorySelectorControllers[i];
                    selector.CleanupList();
                    switch (i)
                    {
                        case 0:
                            chestItem = GameObject.Instantiate<GameObject>(chestUseItemPrefab.gameObject).GetComponent<ChestUseItem>();
                            chestItem.value = "13001";
                            chestItem.label.text = "Сундук с костюмами №1";
                            chestItem.subLabel.text = "Вкусняшки внутри! Открывай!";
                            chestItem.LoadMesh("Prefabs/Chests/", "chest13001");
                            chestItem.amount = GetItemAmount(chestItem.value);
                            chestItem.useButton.onClick.AddListener(delegate () {
                                UseChest("13001");
                            });
                            selector.items.AddLast(chestItem);

                            chestLockpicksAmount[0].text = map.storeView.GetItemAmount("15001").ToString();
                            chestLockpicksAmount[1].text = map.storeView.GetItemAmount("15002").ToString();
                            chestLockpicksAmount[2].text = map.storeView.GetItemAmount("15003").ToString();
                            break;
                        case 1:
                            /*
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
                            */
                            break;
                        case 2:
                            /*
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
                            */
                            break;
                    }
                    selector.Open(itemCode);
                }
            }
            else
            {
                categoryButtons[i].image.sprite = categoryUnactiveSprites[i];
                if (inventorySelectorControllers[i].enabled)
                {
                    inventorySelectorControllers[i].Close();
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
        LinkedListNode<PlayerItemNode> node = map.playerInventory.items.First;
        while (node != null)
        {
            if (node.Value.itemId == itemId)
            {
                return node.Value.itemAmount;
            }
            node = node.Next;
        }
        return 0;
    }

}
