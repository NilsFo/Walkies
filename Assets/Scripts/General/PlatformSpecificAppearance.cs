using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpecificAppearance : MonoBehaviour
{
    [Header("Select condition where this GameObject appears")]
    public bool unityEditor = true; // UNITY_EDITOR

    public bool unityEditorWin = true; // UNITY_EDITOR_WIN
    public bool unityEditorOSX = true; // UNITY_EDITOR_OSX
    public bool unityEditorUnix = true; // UNITY_EDITOR_LINUX
    public bool unityEmbeddedUnix = true; // UNITY_EMBEDDED_LINUX
    public bool unityIPhone = true; // UNITY_IPHONE
    public bool unityAndroid = true; // UNITY_ANDROID
    public bool unityTVOS = true; // UNITY_TVOS
    public bool unityStandaloneWin = true; // UNITY_STANDALONE_WIN
    public bool unityStandaloneOSX = true; // UNITY_STANDALONE_OSX
    public bool unityStandaloneUnix = true; // UNITY_STANDALONE_LINUX
    public bool unityWebGL = true; // UNITY_WEBGL

    // Start is called before the first frame update
    void Start()
    {
        bool keepGameObject = false;

        if (unityEditor)
        {
#if UNITY_EDITOR
            keepGameObject = true;
#endif
        } // UNITY_EDITOR

        if (unityEditorWin)
        {
#if UNITY_EDITOR_WIN
            keepGameObject = true;
#endif
        } // UNITY_EDITOR_WIN

        if (unityEditorOSX)
        {
#if UNITY_EDITOR_OSX
keepGameObject = true;
#endif
        } // UNITY_EDITOR_OSX

        if (unityEditorUnix)
        {
#if UNITY_EDITOR_LINUX
keepGameObject = true;
#endif
        } // UNITY_EDITOR_LINUX

        if (unityEmbeddedUnix)
        {
#if UNITY_EMBEDDED_LINUX
keepGameObject = true;
#endif
        } // UNITY_EMBEDDED_LINUX

        if (unityIPhone)
        {
#if UNITY_IPHONE
keepGameObject = true;
#endif
        } // UNITY_IPHONE

        if (unityAndroid)
        {
#if UNITY_ANDROID
keepGameObject = true;
#endif
        } // UNITY_ANDROID

        if (unityTVOS)
        {
#if UNITY_TVOS
keepGameObject = true;
#endif
        } // UNITY_TVOS

        if (unityStandaloneWin)
        {
#if UNITY_STANDALONE_WIN
keepGameObject = true;
#endif
        } // UNITY_STANDALONE_WIN

        if (unityStandaloneOSX)
        {
#if UNITY_STANDALONE_OSX
keepGameObject = true;
#endif
        } // UNITY_STANDALONE_OSX

        if (unityStandaloneUnix)
        {
#if UNITY_STANDALONE_LINUX
keepGameObject = true;
#endif
        } // UNITY_STANDALONE_LINUX

        if (unityWebGL)
        {
#if UNITY_WEBGL
            keepGameObject = true;
#endif
        } // UNITY_WEBGL

        if (!keepGameObject)
        {
            Destroy(gameObject);
            return;
        }
    }
}