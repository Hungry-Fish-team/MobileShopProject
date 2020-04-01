using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScript : MonoBehaviour
{
    public Image progressCircle;

    private string bundleURL = "https://drive.google.com/uc?export=download&id=1xkNqCPtBRhUDblqQF3C5uciJvv5Iff6P";
    public int version = 0;
    public List<ItemScript> items;
    public TypeOfItemScript typeOfItems;

    public bool isNetworkAllow = false;

    private void Start()
    {
        progressCircle = GameObject.Find("FirstWindow").transform.GetChild(3).GetChild(0).GetComponent<Image>();
    }

    public IEnumerator checkInternetConnection()
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            isNetworkAllow = false;
        }
        else
        {
            Debug.Log("Connected");
            isNetworkAllow = true;
        }
    }

    public void LoadFilesFromServer()
    {
        //StartCoroutine(checkInternetConnection());

        Caching.ClearCache();
        StartCoroutine(DownloadAndCache());
    }

    IEnumerator DownloadAndCache()
    {
        while (!Caching.ready)
        {
            yield return null;
        }

        var www = WWW.LoadFromCacheOrDownload(bundleURL, version);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }
        Debug.Log("IsLoaded");
        var assetBundle = www.assetBundle;

        var itemRequest = assetBundle.LoadAllAssetsAsync(typeof(TypeOfItemScript));
        while (!itemRequest.isDone)
        {
            float progressFloat = itemRequest.progress / 0.9f;
            progressCircle.fillAmount = progressFloat;
            yield return itemRequest;
        }

        Debug.Log("TypeOfItemLoaded");

        typeOfItems = itemRequest.asset as TypeOfItemScript;

        itemRequest = assetBundle.LoadAllAssetsAsync(typeof(ItemScript));

        progressCircle.fillAmount = 0;

        while (!itemRequest.isDone) {
            float progressFloat = itemRequest.progress / 0.9f;
            progressCircle.fillAmount = progressFloat;

            yield return itemRequest;
        }

        Debug.Log("ItemLoaded");

        Debug.Log(itemRequest.allAssets.Length);
        for (int i = 0; i < itemRequest.allAssets.Length; i++)
        {
            items.Add(itemRequest.allAssets[i] as ItemScript);
        }
    }
}
