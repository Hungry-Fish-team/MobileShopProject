using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public string shopNameString;

    public string shopMail, shopMailPassword, shopSystemMail, shopSystemPassword;

    public string chousenItem;
    public string lastWindow, newWindow;

    public string pathToFolder;

    public List<ItemForCheck> itemsToMail = new List<ItemForCheck>();

    public LoadScript loadScript;
    public MobileNotificationAndroidScript mobileNotificationAndroidScript;

    public GameObject startWindow, mainWindow, catalogWindow, busketWindow, profileWindow, itemWindow, buyItemWindow, notificationWindow;
    public GameObject lowerPanel, topPanel;
    public InputField searchPanel;

    public Text helpMessageText;
    public GameObject loadingWindow;

    public GameObject objectForTypes;
    public GameObject objectForItems;
    public GameObject objectForItemsToBuy;
    public GameObject objectForProfileInfo;
    public GameObject objectForNotification;

    public List<ItemScript> sortingListOfItem;
    public List<string> sortingListOfType;
    public List<string> personBusket, personMarks;

    public class notificationObject
    {
        public string message;
        public string date;

        public notificationObject(){}

        public notificationObject(string newMessage, string newDate)
        {
            this.message = newMessage;
            this.date = newDate;
        }

    }

    public List<notificationObject> personNotification = new List<notificationObject>();

    public string fileForPersonBusket;
    public string fileForMarkSave;
    public string fileForNotificationSave;

    private void LoadFiles()
    {
        if (!File.Exists(Application.persistentDataPath + "/fileForPersonBusket.json"))
        {
            CreateFilesForSave("/fileForPersonBusket.json");
        }
        fileForPersonBusket = Application.persistentDataPath + "/fileForPersonBusket.json";
        if (!File.Exists(Application.persistentDataPath + "/fileForMarkSave.json"))
        {
            CreateFilesForSave("/fileForMarkSave.json");
        }
        fileForMarkSave = Application.persistentDataPath + "/fileForMarkSave.json";
        if (!File.Exists(Application.persistentDataPath + "/fileForNotificationSave.json"))
        {
            CreateFilesForSave("/fileForNotificationSave.json");
        }
        fileForNotificationSave = Application.persistentDataPath + "/fileForNotificationSave.json";
    }

    private void CreateFilesForSave(string nameOfFile)
    {
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
    }

    public void SaveAllDataToFile()
    {
        SavePersonBusket();
        SavePersonMarks();
        SavePersonNotification();
    }

    private void SavePersonBusket()
    {
        JSONObject personDATA = new JSONObject();
        JSONArray personBusketJSON = new JSONArray();
        if (personBusket.Count == 0)
        {
            File.Delete(fileForPersonBusket);
        }
        else
        {
            for (int i = 0; i < personBusket.Count; i++)
            {
                personBusketJSON.Add(personBusket[i]);
            }

            personDATA.Add("personBusket", personBusketJSON);

            if (File.Exists(fileForPersonBusket))
            {
                File.WriteAllText(fileForPersonBusket, personDATA.ToString());
            }
        }
    }

    private void SavePersonMarks()
    {
        JSONObject personDATA = new JSONObject();
        JSONArray personMarksJSON = new JSONArray();
        if (personMarks.Count == 0)
        {
            File.Delete(fileForMarkSave);
        }
        else
        {
            for (int i = 0; i < personMarks.Count; i++)
            {
                personMarksJSON.Add(personMarks[i]);
            }

            personDATA.Add("personMarks", personMarksJSON);

            if (File.Exists(fileForMarkSave))
            {
                File.WriteAllText(fileForMarkSave, personDATA.ToString());
            }
        }
    }

    private void SavePersonNotification()
    {
        JSONObject personDATA = new JSONObject();

        if (personNotification.Count == 0)
        {
            File.Delete(fileForNotificationSave);
        }
        else
        {
            for (int i = 0; i < personNotification.Count; i++)
            {
                JSONArray personNotificatioJSON = new JSONArray();

                personNotificatioJSON.Add("Message", personNotification[i].message);
                personNotificatioJSON.Add("Date", personNotification[i].date);

                personDATA.Add("personNotification" + i.ToString(), personNotificatioJSON);
            }

            if (File.Exists(fileForNotificationSave))
            {
                File.WriteAllText(fileForNotificationSave, personDATA.ToString());
            }
        }
    }

    public void LoadAllDataFromFile()
    {
        LoadPersonBusket();
        LoadPersonMarks();
        LoadPersonNotification();
    }

    private void LoadPersonBusket()
    {
        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForPersonBusket)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForPersonBusket));

            if (personDATA != null)
            {
                for (int i = 0; i < personDATA["personBusket"].Count; i++)
                {
                    personBusket.Add(personDATA["personBusket"].AsArray[i]);
                }
            }
        }
    }

    private void LoadPersonMarks()
    {
        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForMarkSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForMarkSave));

            if (personDATA != null)
            {
                for (int i = 0; i < personDATA["personMarks"].Count; i++)
                {
                    personMarks.Add(personDATA["personMarks"].AsArray[i]);
                }
            }
        }
    }

    private void LoadPersonNotification()
    {
        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForNotificationSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForNotificationSave));

            if (personDATA != null)
            {
                for (int i = 0; i < personDATA.Count; i++)
                {
                    personNotification.Add(new notificationObject(personDATA["personNotification" + i.ToString()].AsArray[0], personDATA["personNotification" + i.ToString()].AsArray[1]));
                }
            }
        }
    }

    public void CreateNewNotififcation()
    {
        //mobileNotificationAndroidScript.CreateAndSentNotification("Lol", "Privet", 5);
        mobileNotificationAndroidScript.CreateAndSentNotificationSecondVer("Lol", "Privet", 5);
        SaveAllDataToFile();

        ChouseMainWindow("mainWindow");
    }

    public void CloseNotificationWindowAndDeleteAllNotification()
    {
        personNotification.Clear();
        SaveAllDataToFile();

        ChouseMainWindow("mainWindow");
    }

    public void SendHelpMessage(string helpMessage)
    {
        helpMessageText.gameObject.SetActive(true);
        helpMessageText.text = helpMessage;
        StartCoroutine(ShowHelpMessage());
    }

    IEnumerator ShowHelpMessage()
    {
        yield return new WaitForSeconds(3f);
        helpMessageText.gameObject.SetActive(false);
    }

    private void StartSettings()
    {
        Application.targetFrameRate = 60;

        //PlayerSettings.Android.renderOutsideSafeArea = false;
    }

    void Start()
    {
        StartSettings();

        InitializationAllObjects();

        ChouseStartWindow();
        StartCoroutine(CloseStartWindow());

        loadScript.LoadFilesFromServer();

        LoadFiles();
        LoadAllDataFromFile();
    }

    private void InitializationAllObjects()
    {
        loadScript = GameObject.Find("GameManager").GetComponent<LoadScript>();
        mobileNotificationAndroidScript = GameObject.Find("GameManager").GetComponent<MobileNotificationAndroidScript>();

        lowerPanel = GameObject.Find("LowerPanelWithButton");
        topPanel = GameObject.Find("TopPanelWithShopName");

        searchPanel = topPanel.transform.GetChild(1).gameObject.GetComponent<InputField>();

        helpMessageText = GameObject.Find("HelpMessageText").GetComponent<Text>();
        helpMessageText.gameObject.SetActive(false);

        loadingWindow = GameObject.Find("LoadingWindow");
        loadingWindow.gameObject.SetActive(false);
    }

    IEnumerator CloseStartWindow()
    {
        StartCoroutine(loadScript.checkInternetConnection());
        startWindow.transform.GetChild(2).GetComponent<Text>().text = "Welcome!";
        yield return new WaitForSeconds(3f);
        while (loadScript.isNetworkAllow != true)
        {
            startWindow.transform.GetChild(2).GetComponent<Text>().text = "No Connection to Internet";
            StartCoroutine(loadScript.checkInternetConnection());
            yield return new WaitForSeconds(1f);
        }
        startWindow.transform.GetChild(2).GetComponent<Text>().text = "Welcome!";
        startWindow.GetComponent<Animator>().SetTrigger("CloseStartWindow");
        yield return new WaitForSeconds(1f);
        ChouseMainWindow("mainWindow");
    }

    private void InputSettings()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnButtonFunc();
            }
        }
    }

    void Update()
    {
        SearchPanelFunc();

        InputSettings();

        StartAsyncLoadItems(sortingListOfItemForAsyncLoad);
    }

    public void ChouseStartWindow()
    {
        startWindow.SetActive(true);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(false);
        topPanel.SetActive(false);

        StateOfTopPanel(0);

        ReloadAllText();
    }

    public void ChouseMainWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(true);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(0);

        LastAndNewWindow(windowName);

        ReloadAllText();
        ReloadCountOfNotification();

        GameObject.Find("SecondWindow").transform.GetChild(1).GetComponent<LoadMainWindowScript>().DestroyAllElementsOfMainWindow();
    }

    public void ChouseCatalogWindow(string windowName)
    {
        startWindow.SetActive(true);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(true);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(1);

        LastAndNewWindow(windowName);

        ReloadAllText();
        LoadAllTypesOfItems(loadScript.typeOfItems.typesOfItem);
    }

    public void ChouseChousenItemsWindow(string metodName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(true);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(1);

        ReloadAllText();

        if (metodName != "busketsItems")
        {
            busketWindow.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            busketWindow.transform.GetChild(2).gameObject.SetActive(true);
        }

        LastAndNewWindow(metodName);

        LoadAllPrefabFromItem(metodName);
    }

    public void ChouseProfileWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(true);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);

        LastAndNewWindow(windowName);

        ReloadAllText();
    }

    public void ChouseItemWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 0);

        ReloadAllText();
    }

    public void ChouseAboutUsWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 1);

        ReloadAllText();
    }

    public void ChouseToContactUsWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 2);

        ReloadAllText();
    }

    public void ChouseLoginWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 3);

        ReloadAllText();
    }

    public void ChouseBuyItemWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(true);
        notificationWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        LastAndNewWindow(windowName);

        ReloadAllText();
    }

    public void ChouseNotificationWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(true);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(true);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        LastAndNewWindow(windowName);

        ReloadAllText();

        LoadAllNotificationObjects();
    }

    private void LastAndNewWindow(string windowName)
    {
        if (newWindow != windowName)
        {
            lastWindow = newWindow;
            newWindow = windowName;
        }
    }

    public void ReturnButtonFunc()
    {
        ClearSearchPanel();

        if (lastWindow == "mainWindow") ChouseMainWindow("mainWindow");
        else
        if (lastWindow == "profileWindow") ChouseProfileWindow("profileWindow");
        else
        if (lastWindow == "busketsItems") ChouseChousenItemsWindow("busketsItems");
        else
        if (lastWindow == "catalogItem") ChouseCatalogWindow("catalogItem");
    }

    public void ClearSearchPanel()
    {
        searchPanel.text = "";
    }

    private void StateOfTopPanel(int stateOfTopPanel)
    {
        if (stateOfTopPanel == 0)
        {
            topPanel.GetComponent<Animator>().SetBool("CloseNamePanel", false);
            topPanel.GetComponent<Animator>().SetBool("CloseSearchPanel", false);
        }
        else if (stateOfTopPanel == 1)
        {
            topPanel.GetComponent<Animator>().SetBool("CloseNamePanel", true);
            topPanel.GetComponent<Animator>().SetBool("CloseSearchPanel", false);
        }
        else if (stateOfTopPanel == 2)
        {
            topPanel.GetComponent<Animator>().SetBool("CloseNamePanel", true);
            topPanel.GetComponent<Animator>().SetBool("CloseSearchPanel", true);
        }
    }

    private void ReloadAllText()
    {
        ReloadShopName();
    }

    private void ReloadShopName()
    {
        if (GameObject.FindGameObjectWithTag("ShopNameTag"))
        {
            GameObject.FindGameObjectWithTag("ShopNameTag").GetComponent<Text>().text = shopNameString;
        }
    }

    private void ReloadCountOfNotification()
    {   
        GameObject.Find("CountMessageText").GetComponent<Text>().text = personNotification.Count.ToString();
    }

    public void BuyItemFunc()
    {
        if (newWindow == "busketsItems")
        {
            GameObject content = GameObject.Find("Content");
            int countSelectedItems = 0;
            for (int i = 0; i < content.transform.childCount; i++)
            {
                if (content.transform.GetChild(i).GetChild(8).GetComponent<Toggle>().isOn)
                {
                    countSelectedItems++;
                }
            }

            if (countSelectedItems != 0)
            {

                DestroyAllItemsToBuy();
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    if (content.transform.GetChild(i).GetChild(8).GetComponent<Toggle>().isOn)
                    {
                        GameObject newItemToBuy = Instantiate(objectForItemsToBuy, buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0));
                        newItemToBuy.GetComponent<InfoAboutItemToBuyScript>().item = content.transform.GetChild(i).GetComponent<InfoAboutItem>().item;
                    }
                }

                Instantiate(objectForProfileInfo, buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0));

                ChouseBuyItemWindow("buyWindow");
            }
            else
            {
                SendHelpMessage("Select any items from list");
            }
        }

        if (newWindow == "buyWindow")
        {
            if (ReturnCountItemWithChouseSize() == buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount - 1)
            {
                ProfileDataScript profileDataScript = buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount - 1).GetComponent<ProfileDataScript>();

                if (profileDataScript.isProfileMailConfirmed == true)
                {
                    if (profileDataScript.cashlessPaymentToggle.isOn == true || profileDataScript.cashPaymentToggle.isOn == true) {
                        CreatePackageOfItems(itemsToMail);
                        profileDataScript.SendMessageToCreatorMail(profileDataScript.CreateCheckMessage());
                        SendHelpMessage("Your order is accepted. You will be contacted soon");

                        ChouseMainWindow("mainWindow");

                        personBusket.Clear();

                        SaveAllDataToFile();
                    }
                    else
                    {
                        SendHelpMessage("Chouse payment method");
                    }
                }
                else
                {
                    SendHelpMessage("Your mail is not confirmed");
                    ChouseLoginWindow();
                }
            }
            else
            {
                SendHelpMessage("Chouse size to all items");
            }
        }
    }

    public class ItemForCheck
    {
        public string nameItem;
        public string vendorCode;
        public string firstTypeItem;
        public string secondTypeItem;

        public string firstCostItem, secondCostItem;
        public string sizeOfItem;

        public string briefInfoOfItem;
        public string compositionOfItem;
        public string manufacturingFirm;

        public string allCostOfItem;

        public ItemForCheck(string nameItem, string vendorCode, string firstTypeItem, string secondTypeItem, string firstCostItem, string secondCostItem, string sizeOfItem, string briefInfoOfItem, string compositionOfItem, string manufacturingFirm, string allCostOfItem)
        {
            this.nameItem = nameItem;
            this.vendorCode = vendorCode;
            this.firstTypeItem = firstTypeItem;
            this.secondTypeItem = secondTypeItem;
            this.firstCostItem = firstCostItem;
            this.secondCostItem = secondCostItem;
            this.sizeOfItem = sizeOfItem;
            this.briefInfoOfItem = briefInfoOfItem;
            this.compositionOfItem = compositionOfItem;
            this.manufacturingFirm = manufacturingFirm;
            this.allCostOfItem = allCostOfItem;
        }
    }

    private void CreatePackageOfItems(List<ItemForCheck> itemsToMail)
    {
        ClearPackageOfItems(itemsToMail);

        for (int i = 0; i < buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount - 1; i++)
        {
            GameObject itemObject = buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).gameObject;
            ItemScript item = itemObject.GetComponent<InfoAboutItemToBuyScript>().item;
            itemsToMail.Add(new ItemForCheck(item.nameItem, item.vendorCode, item.firstTypeItem, item.secondTypeItem, item.firstCostItem.ToString(), item.secondCostItem.ToString(), itemObject.GetComponent<InfoAboutItemToBuyScript>().chousenSize, item.briefInfoOfItem, item.compositionOfItem, item.manufacturingFirm, itemObject.GetComponent<InfoAboutItemToBuyScript>().summIntText.text.ToString()));
        }
    }

    private void ClearPackageOfItems(List<ItemForCheck> itemsToMail)
    {
        if (itemsToMail.Count != 0)
        {
            itemsToMail.Clear();
        }
    }

    private int ReturnCountItemWithChouseSize()
    {
        int countItemsWithChousenSize = 0;
        for (int i = 0; i < buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount; i++)
        {
            if (buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).name != "ProfileInfoPrefab(Clone)")
            {
                if (buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetComponent<InfoAboutItemToBuyScript>().chousenSize != "")
                {
                    countItemsWithChousenSize++;
                }
            }
        }
        return countItemsWithChousenSize;
    }

    private void DestroyAllNotificationObjects(Transform content)
    { 
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    private void LoadAllNotificationObjects()
    {
        Transform content = notificationWindow.transform.GetChild(1).GetChild(0).GetChild(0);

        Debug.Log(content.name);
        DestroyAllNotificationObjects(content);
        foreach (notificationObject notObj in personNotification)
        {
            GameObject newNotificationObject = Instantiate(objectForNotification, content);
            newNotificationObject.GetComponent<InfoAboutNotificationScript>().message = notObj.message;
            newNotificationObject.GetComponent<InfoAboutNotificationScript>().date = notObj.date;
        }

    }

    private void DestroyAllItemsToBuy()
    {
        for (int i = 0; i < buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount; i++)
        {
            Destroy(buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).gameObject);
            Debug.Log(buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).name);
        }
    }

    public void LoadAllTypesOfItems(List<string> typeOfItems)
    {
        DestroyAllTypesOfItems();
        for (int i = 0; i < typeOfItems.Count; i++)
        {
            GameObject newTypeOfItem = Instantiate(objectForTypes, catalogWindow.transform.GetChild(1).GetChild(0).GetChild(0));
            newTypeOfItem.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = loadScript.typeOfItems.typesOfItem[i];

            newTypeOfItem.GetComponent<ChouseTypeOfItemScript>().nameOfType = loadScript.typeOfItems.typesOfItem[i];
        }
    }

    private void DestroyAllTypesOfItems()
    {
        for (int i = 0; i < catalogWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount; i++)
        {
            Destroy(catalogWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).gameObject);
        }
    }

    public void LoadAllPrefabFromItem(string parameterOfItem)
    {
        if (parameterOfItem == "allItems")
        {
            CreateAllPrefabFromItem(loadScript.items);
        }
        else if (parameterOfItem == "busketsItems")
        {
            FindItemsInBusket();
            CreateAllPrefabFromItem(sortingListOfItem);
        }
        else if (parameterOfItem == "marksItems")
        {
            FindItemsInMarks();
            CreateAllPrefabFromItem(sortingListOfItem);
        }
        else
        {
            FindItemByTypes(parameterOfItem);
            CreateAllPrefabFromItem(sortingListOfItem);
        }
    }

    private void FindItemsInBusket()
    {
        ClearSortingList();
        foreach (ItemScript item in loadScript.items)
        {
            for (int i = 0; i < personBusket.Count; i++)
            {
                if (item.name == personBusket[i])
                {
                    sortingListOfItem.Add(item);
                    break;
                }
            }
        }
    }

    private void FindItemsInMarks()
    {
        ClearSortingList();
        foreach (ItemScript item in loadScript.items)
        {
            for (int i = 0; i < personMarks.Count; i++)
            {
                if (item.name == personMarks[i])
                {
                    sortingListOfItem.Add(item);
                    break;
                }
            }
        }
    }

    private void FindItemByTypes(string typeOfitem)
    {
        ClearSortingList();
        foreach (ItemScript item in loadScript.items)
        {
            if (typeOfitem.Equals(item.firstTypeItem, System.StringComparison.OrdinalIgnoreCase))
            {
                sortingListOfItem.Add(item);
            }
        }

        foreach (ItemScript item in loadScript.items)
        {
            if (typeOfitem.Equals(item.secondTypeItem, System.StringComparison.OrdinalIgnoreCase))
            {
                sortingListOfItem.Add(item);
            }
        }
    }

    private void FindTypeByTypes(string typeOfitem)
    {
        ClearSortingList();
        foreach (string type in loadScript.typeOfItems.typesOfItem)
        {
            if (typeOfitem.Equals(type, System.StringComparison.OrdinalIgnoreCase))
            {
                sortingListOfType.Add(type);
            }
        }
    }

    string lastTextFromSeacrh = "";

    private void SearchPanelFunc()
    {
        while (searchPanel.text != lastTextFromSeacrh)
        {
            Debug.Log(searchPanel.text);
            ClearSortingList();
            SpecialSearchFunc(searchPanel.text);
            lastTextFromSeacrh = searchPanel.text;
        }
    }

    private void SpecialSearchFunc(string textFromSearch)
    {
        if (newWindow != "catalogItem")
        {
            FindItemByTypes(textFromSearch);
            CreateAllPrefabFromItem(sortingListOfItem);

            if (textFromSearch == "")
            {
                LoadAllPrefabFromItem(newWindow);
            }
        }
        else
        {
            FindTypeByTypes(textFromSearch);
            LoadAllTypesOfItems(sortingListOfType);

            if (textFromSearch == "")
            {
                LoadAllTypesOfItems(loadScript.typeOfItems.typesOfItem);
            }
        }
    }

    private void ClearSortingList()
    {
        sortingListOfType.Clear();
        sortingListOfItem.Clear();
    }

    private void CreateAllPrefabFromItem(List<ItemScript> sortingListOfItem)
    {
        DeleteAllPrefabFromItem();

        sortingListOfItemForAsyncLoad = sortingListOfItem;
        countLoadedItems = 0;
    }

    int countLoadedItems;
    List<ItemScript> sortingListOfItemForAsyncLoad;

    private void StartAsyncLoadItems(List<ItemScript> sortingListOfItem)
    {
        if (sortingListOfItem != null)
        {
            if (sortingListOfItem.Count != countLoadedItems)
            {
                Transform content = GameObject.Find("Content").transform;

                while ((countLoadedItems % 10 != 0 || countLoadedItems == 0 || (410 * (countLoadedItems / 2 - 3) + 15 * (countLoadedItems / 2 - 3)) <= content.localPosition.y) && sortingListOfItem.Count != countLoadedItems)
                {

                    ItemScript item = sortingListOfItem[countLoadedItems];

                    GameObject newObjectForItems = GameObject.Instantiate(objectForItems, content);
                    newObjectForItems.GetComponent<InfoAboutItem>().item = item;

                    countLoadedItems++;
                }
            }
        }
    }

    private void DeleteAllPrefabFromItem()
    {
        if (GameObject.Find("Content") != null)
        {
            if (GameObject.Find("Content").transform.childCount != 0)
            {
                for (int i = 0; i < GameObject.Find("Content").transform.childCount; i++)
                {
                    Debug.Log(GameObject.Find("Content").transform.GetChild(i).gameObject + " Delete");
                    Destroy(GameObject.Find("Content").transform.GetChild(i).gameObject);
                }
            }
        }
    }

}
