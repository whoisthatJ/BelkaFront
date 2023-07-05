using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetStartPositionScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect content;
    [SerializeField] private bool isVertical;
    private void OnEnable() {
        if (isVertical)
        content.normalizedPosition = new Vector2(0, 1f);
        else content.normalizedPosition = new Vector2(1, 0f);
    }
}
