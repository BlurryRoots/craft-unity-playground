using UnityEngine;
using System.Collections;

public class ValueComponent : MonoBehaviour {

    public float Value;

    void Awake () {
        this.meshRenderer = this.GetComponent<MeshRenderer> ();
    }

    void Update () {
        var pos = this.gameObject.transform.position;
        pos.y = this.Value;
        this.gameObject.transform.position = pos;
    }

    private MeshRenderer meshRenderer;

}
