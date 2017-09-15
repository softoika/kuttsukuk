using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger{

    public static void Log(string message = "",
                             //[CallerFileName] string file = "",
                             //[CallerLineNumber] int line = 0,
                             //[CallerMemberName] string member = "",
                             string color = "black"
                          )
    {
        Debug.Log($"<color={color}>{message}</color>");
    }
}