using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponEquipItem : SelectorItem {

    public GameObject present;
    public Text label;

    void Start () {

    }
	
	void Update () {
	
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

        //present = GameObject.Instantiate<GameObject>(basicPresentContainer);
        present.transform.parent = presentContainer;
        present.transform.localPosition += Vector3.right * (presentShift + presentContainer.transform.localPosition.x);
        //label = GameObject.Instantiate<GameObject>(basicPresentLabel.gameObject).GetComponent<Text>();
        //label.transform.parent = descriptionContainer;
        //label.rectTransform.localScale = basicPresentLabel.rectTransform.localScale;
        //label.rectTransform.anchoredPosition += Vector2.right * descriptionShift;
        //label.text = titles[i];
        //label.enabled = true;

        base.Setup(presentContainer, presentShift, descriptionContainer, descriptionShift);

    }

    public override void Destroy()
    {
        GameObject.Destroy(present);
        base.Destroy();
    }

}
