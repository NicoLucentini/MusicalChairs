using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtension 
{
    public static string ToColor(this string msg, Color color) {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{msg}</color>";
    }
}
