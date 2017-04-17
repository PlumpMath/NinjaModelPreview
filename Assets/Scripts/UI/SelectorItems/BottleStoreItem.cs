using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BottleStoreItem : SelectorItem
{

    public enum CURRENCY
    {
        NONE = 0,
        GOLD = 1,
        EMERALD = 2
    }

    public GameObject present;
    public Text label;
    public Text subLabel;
    public Text priceLabel;
    public Button buyButton;
    public Text buyButtonLabel;
    public Image currencyIcon;

    public Sprite[] currencySprites;
    public Color[] currencyColors;

    public int amount = 0;

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetCurrency(CURRENCY currency)
    {
        switch (currency)
        {
            case CURRENCY.GOLD:
                currencyIcon.sprite = currencySprites[0];
                currencyIcon.color = currencyColors[0];
                priceLabel.color = currencyColors[0];
                break;
            case CURRENCY.EMERALD:
                currencyIcon.sprite = currencySprites[1];
                currencyIcon.color = currencyColors[1];
                priceLabel.color = currencyColors[1];
                break;
            default:
                currencyIcon.sprite = null;
                break;
        }
    }

    public void HideBuyButton()
    {
        buyButton.enabled = false;
        buyButton.image.enabled = false;
        buyButtonLabel.enabled = false;
        currencyIcon.enabled = false;
        priceLabel.enabled = false;
    }

    public void LoadMesh(string pathPrefix, string name)
    {
        GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(pathPrefix + name));
        obj.transform.parent = present.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
    }

    public override void Setup(Transform presentContainer, float presentShift, RectTransform descriptionContainer, float descriptionShift)
    {
        present.transform.parent = presentContainer;
        present.transform.localPosition += Vector3.right * (presentShift + presentContainer.transform.localPosition.x);
        base.Setup(presentContainer, presentShift, descriptionContainer, descriptionShift);
    }

    public override void Destroy()
    {
        GameObject.Destroy(present);
        base.Destroy();
    }

}
