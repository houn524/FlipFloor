using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public float scaleValue;

    void Awake() {
        Vector3 leftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightUp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float width = rightUp.x - leftDown.x;
        float height = rightUp.y - leftDown.y;

        scaleValue = (16.0f / 9.0f) / (height / width);
    }
}
