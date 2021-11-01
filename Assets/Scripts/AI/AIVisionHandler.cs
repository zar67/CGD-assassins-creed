using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVisionHandler : MonoBehaviour
{
    private bool lookingLeft = false;

    public void SetVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetScale(float length)
    {
        transform.localScale = new Vector3(length, transform.localScale.y, transform.localScale.z);
        transform.localPosition = new Vector3(((length / 2) + 0.5f) * (lookingLeft ? -1 : 1), transform.localPosition.y, transform.localPosition.z);
    }

    public void FlipView(bool _lookingLeft)
    {
        lookingLeft = _lookingLeft;

        if ((transform.localPosition.x < 0 && _lookingLeft) || (transform.localPosition.x > 0 && !_lookingLeft)) return;


        transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
}
