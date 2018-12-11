using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayHelper {
    
    public static bool Contains<T>(T[] array, T item) {
        for(int i = 0; i < array.Length; i++) {
            if(array[i].Equals(item)) {
                return true;
            }
        }
        return false;
    }

    public static bool Contains<T>(T[] array, T item, out int index) {
        index = -1;
        for (int i = 0; i < array.Length; i++) {
            if (array[i].Equals(item)) {
                index = i;
                return true;
            }
        }
        return false;
    }
}
