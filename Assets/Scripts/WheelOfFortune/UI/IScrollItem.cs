using UnityEngine;

namespace WheelOfFortune.UI
{
    public interface IScrollItem
    {
        RectTransform RectTransform { get; }
        GameObject GameObject { get; }
    }
}