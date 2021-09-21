using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUI : MonoBehaviour
{
    public static bool GetMouseButtonDown(int num)
    {
        if (Input.GetMouseButtonDown(num))
        {
            return true;
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                return true;
            }
        }

        return false;
    }

    public static bool GetMouseButtonUp(int num)
    {
        if (Input.GetMouseButtonUp(num))
        {
            return true;
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Ended)
            {
                return true;
            }
        }

        return false;
    }
}
