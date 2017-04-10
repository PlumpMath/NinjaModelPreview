using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectorController : MonoBehaviour {

    public GameObject basicPresentContainer;
    public Text basicPresentLabel;

    public Canvas canvas;
    public Button closeButton;
    public Transform objectTransform;
    public RectTransform descriptionTransform;
    public MeshRenderer selectionEffect;

    public LinkedList<SelectorItem> items;

    public int Length = 0;

    public int position = 0;

    public float objectMarginStep = 5.0f;
    public float descriptionMarginStep = 800.0f;
    public float marginX = 0.0f;
    public float lastPointerPositionX = 0.0f;
    public float pointerSpeedX = 0.0f;
    public float pointerTime = 0.0f;

    public event EventHandler<EventArgs> OnClose;
    public event EventHandler<SelectorCounterEventArgs> OnPositionUpdate;
    public event EventHandler<SelectorCounterEventArgs> OnItemActivate;

    // Use this for initialization
    void Start () {

        items = new LinkedList<SelectorItem>();

        closeButton.onClick.AddListener(delegate() {
            Close();
            EventHandler<EventArgs> handler = OnClose;
            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        });

        canvas.enabled = false;
        enabled = false;

    }

    public void Close()
    {
        CleanupList();
        selectionEffect.enabled = false;
        canvas.enabled = false;
        enabled = false;
    }

    // Update is called once per frame
    void Update () {

        float f;

        objectTransform.localPosition += Vector3.right * Mathf.Max(-objectMarginStep * 0.5f, Mathf.Min(objectMarginStep * 0.5f, ((float)position - marginX) * -objectMarginStep - objectTransform.localPosition.x)) * Time.deltaTime * 10.0f;
        descriptionTransform.anchoredPosition += Vector2.right * Mathf.Max(-descriptionMarginStep * 0.5f, Mathf.Min(descriptionMarginStep * 0.5f, ((float)position - marginX) * -descriptionMarginStep - descriptionTransform.anchoredPosition.x)) * Time.deltaTime * 10.0f;

        if (Input.GetMouseButtonDown(0))
        {
            lastPointerPositionX = Input.mousePosition.x;
            marginX = 0.0f;
            pointerSpeedX = 0.0f;
            pointerTime = 0.0f;
        }
        if (Input.GetMouseButton(0))
        {
            f = (Input.mousePosition.x - lastPointerPositionX) / (float)Screen.width;
            lastPointerPositionX = Input.mousePosition.x;
            marginX += f;
            pointerSpeedX = pointerSpeedX * Mathf.Max(0.0f, 1.0f - Time.deltaTime * 5.0f) + f * 0.5f / Time.deltaTime;
            pointerTime += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(pointerTime <= 0.2f && Mathf.Abs(marginX) < 0.1f)
            {
                ActivateItem();
            }
            if(Mathf.Abs(marginX) < 0.15f)
            {
                pointerSpeedX = 0.0f;
            }
            f = pointerSpeedX * 0.4f;
            f = f / Mathf.Abs(f) * Mathf.Max(0.0f, Mathf.Abs(f) - 5.0f);
            position -= (int)Mathf.Round(marginX + marginX / Mathf.Abs(marginX) * 0.2f) + (int)(Mathf.Round(Mathf.Abs(marginX)) * f);
            if (position > Length - 1)
            {
                position = Length - 1;
            }
            if (position < 0)
            {
                position = 0;
            }
            marginX = 0.0f;
            pointerSpeedX = 0.0f;
            pointerTime = 0.0f;
            UpdatePosition();
        }

    }

    void ActivateItem()
    {
        EventHandler<SelectorCounterEventArgs> handler = OnItemActivate;
        if (handler != null)
        {
            handler(this, new SelectorCounterEventArgs(GetActiveItem(), Length));
        }
    }

    void UpdatePosition()
    {
        EventHandler<SelectorCounterEventArgs> handler = OnPositionUpdate;
        if(handler != null)
        {
            handler(this, new SelectorCounterEventArgs(GetActiveItem(), Length));
        }
    }

    public void CleanupList()
    {
        LinkedListNode<SelectorItem> node = items.First;
        while (node != null)
        {
            GameObject.Destroy(node.Value.present);
            GameObject.Destroy(node.Value.label.gameObject);
            node = node.Next;
        }
        items.Clear();
    }

    public SelectorItem GetActiveItem()
    {
        return GetItemByPosition(position);
    }

    public SelectorItem GetItemByPosition(int index)
    {
        LinkedListNode<SelectorItem> node = items.First;
        while(node != null)
        {
            if(node.Value.position == index)
            {
                return node.Value;
            }
            node = node.Next;
        }
        return null;
    }

    public void Open(string pathPrefix, string[] names, string[] values, string[] titles, string selected)
    {
        int i;
        GameObject obj;
        SelectorItem item;
        position = 0;
        selectionEffect.enabled = false;
        for (i = 0; i < names.Length; i++)
        {
            item = new SelectorItem();
            item.position = i;
            item.value = values[i];
            item.present = GameObject.Instantiate<GameObject>(basicPresentContainer);
            item.present.transform.parent = basicPresentContainer.transform.parent;
            item.present.transform.localPosition = basicPresentContainer.transform.localPosition + Vector3.right * ((float)i) * objectMarginStep;
            obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(pathPrefix + names[i]));
            obj.transform.parent = item.present.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            item.label = GameObject.Instantiate<GameObject>(basicPresentLabel.gameObject).GetComponent<Text>();
            item.label.transform.parent = basicPresentLabel.transform.parent;
            item.label.rectTransform.localScale = basicPresentLabel.rectTransform.localScale;
            item.label.rectTransform.anchoredPosition = basicPresentLabel.rectTransform.anchoredPosition + Vector2.right * ((float)i) * descriptionMarginStep;
            item.label.text = titles[i];
            item.label.enabled = true;
            items.AddLast(item);
            if(selected == item.value)
            {
                selectionEffect.transform.localPosition += Vector3.right * (((float)i) * objectMarginStep - selectionEffect.transform.localPosition.x);
                selectionEffect.enabled = true;
                position = i;
            }
        }
        Length = items.Count;
        UpdatePosition();

        lastPointerPositionX = Input.mousePosition.x;
        marginX = 0.0f;
        pointerSpeedX = 0.0f;
        pointerTime = 0.0f;

        enabled = true;
        canvas.enabled = true;

    }

}

public class SelectorItem
{

    public int position = -1;
    public string value = "";

    public GameObject present;
    public Text label;

}

public class SelectorCounterEventArgs : EventArgs
{
    public SelectorItem item;
    public int count;

    public SelectorCounterEventArgs(SelectorItem _item, int _count)
    {
        item = _item;
        count = _count;
    }
}