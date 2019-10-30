using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ErrorManager {

	public static void LogError(string reportingClass, string message) {
        Debug.LogError($"[{reportingClass}] {message}");
    }

    public static void LogGameObjectError(string objectName, string message) {
        Debug.LogError($"[GameObject: {objectName}] {message}");
    }
}
