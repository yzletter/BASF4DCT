using System;
using TriLibCore;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;
using UnityEditor;

public class ModelLoader : MonoBehaviour
{
    public static ModelLoader Instance { get; private set; }
    public GameObject Model1 { get; private set; }
    public GameObject Model2 { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadModels(string path1, string path2)
    {
        LoadModelFromFile(path1, context => Model1 = context.WrapperGameObject);
        LoadModelFromFile(path2, context => Model2 = context.WrapperGameObject);
    }

    private static void LoadModelFromFile(string path, Action<AssetLoaderContext> onLoad)
    {
        AssetLoader.LoadModelFromFile(path,
            onLoad: onLoad,
            onError: error => Debug.LogError("Failed to load model: " + error),
            assetLoaderOptions: AssetLoader.CreateDefaultLoaderOptions(),
            wrapperGameObject: new GameObject("Loaded Model")
        );
    }
}
