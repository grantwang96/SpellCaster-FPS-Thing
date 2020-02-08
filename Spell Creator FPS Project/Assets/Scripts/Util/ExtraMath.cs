using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtraMath
{
    public static float ReduceAbsolute(float f, float amount) {
        float value = 0f;
        int positive = 1;
        if(f < 0) {
            positive = -1;
        }
        value = f - (amount * positive);
        if(positive > 0 && f < 0) {
            value = 0f;
        } else if(positive < 0 && f > 0) {
            value = 0f;
        }
        return value;
    }
}
