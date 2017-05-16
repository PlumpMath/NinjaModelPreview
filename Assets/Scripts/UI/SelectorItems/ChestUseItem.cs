using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChestUseItem : SelectorItem
{

    public GameObject present;
    public Text label;
    public Text subLabel;
    public Button useButton;
    public Text useButtonLabel;

    public int amount = 0;

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
        present.transform.localPosition += Vector3.right * (presentShift + presentContainer.transform.localPosition.x);
        base.Setup(presentContainer, presentShift, descriptionContainer, descriptionShift);
    }

    public override void Destroy()
    {
        GameObject.Destroy(present);
        base.Destroy();
    }

}
