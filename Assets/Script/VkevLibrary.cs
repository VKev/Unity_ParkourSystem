using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VkevLibrary
{
    public static void StopCoroutine(Coroutine coroutine, MonoBehaviour mono)
    {
        if (coroutine != null)
        {
            mono.StopCoroutine(coroutine);
        }
    }

}
