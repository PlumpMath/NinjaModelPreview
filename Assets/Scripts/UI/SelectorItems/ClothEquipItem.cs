using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClothEquipItem : SelectorItem
{

    public GameObject present;
    public Text label;

    void Start()
    {

    }

    void Update()
    {

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
        present.transform.localPosition = selectorController.objectTransformDirection * presentShift + Vector3.up * present.transform.localPosition.y /* + presentContainer.transform.localPosition */;
        base.Setup(presentContainer, presentShift, descriptionContainer, descriptionShift);
    }

    public override void Destroy()
    {
        GameObject.Destroy(present);
        base.Destroy();
    }

}
