using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mime;

public class ProfileDataScript : MonoBehaviour
{
    public GameManager gameManager;

    public string profileName;
    public string profileAddress;
    public string phoneNumber;
    public string profileMail;

    string lastProfileName;
    string lastProfileAddress;
    string lastPhoneNumber;
    string lastProfileMail;

    public InputField profileNameInputField, profileAddressInputField, profilePhoneNumberInputField, profileMailInputField;
    public Toggle cashlessPaymentToggle, cashPaymentToggle;
    public Button saveButton;
    public GameObject mailConfirmation;

    public int codeToConfirm = -1;
    public bool isProfileMailConfirmed = false;

    private bool startConfirmFunc = false;

    void Start()
    {
        InitializationAllObjects();
        LoadAllDataFromFile();
        LoadAllDataToObjects();

        SaveStartProfileInfo();
        if (mailConfirmation != null)
        {
            if (isProfileMailConfirmed == false)
            {
                FirstStateOfMailConf();
            }
            else
            {
                ThirdStateOfMailConf();
            }
        }
    }

    private void Update()
    {
        TakeNewProfileInfo();

        CheckingChanges();

        if (mailConfirmation != null) { 
        if (startConfirmFunc == true)
            {
                WaitingTrueCode();
            }

            if (startConfirmFunc != true)
            {
                if (isProfileMailConfirmed != true)
                {
                    FirstStateOfMailConf();
                }
            }
        }
    }

    private void TakeNewProfileInfo()
    {
        profileName = profileNameInputField.text;
        profileAddress = profileAddressInputField.text;
        phoneNumber = profilePhoneNumberInputField.text;
        profileMail = profileMailInputField.text;
    }

    private void InitializationAllObjects()
    {

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (transform.name != "ProfileInfoPrefab(Clone)")
        {
            profileNameInputField = GameObject.Find("ProfileNameInputField").GetComponent<InputField>();
            profileAddressInputField = GameObject.Find("ProfileAddressInputField").GetComponent<InputField>();
            profilePhoneNumberInputField = GameObject.Find("ProfilePhoneNumberInputField").GetComponent<InputField>();
            profileMailInputField = GameObject.Find("ProfileMailInputField").GetComponent<InputField>();
            mailConfirmation = GameObject.Find("MailConfirmation");
            saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
        }
        else
        {
            profileMailInputField.interactable = false;
        }
    }

    private void SaveStartProfileInfo()
    {
        lastProfileName = profileName;
        lastProfileAddress = profileAddress;
        lastPhoneNumber = phoneNumber;
        lastProfileMail = profileMail;
    }

    public string fileForProfileSave;

    private void LoadFiles()
    {
        if (!File.Exists(Application.persistentDataPath + "/fileForProfileData.json"))
        {
            CreateFilesForSave("/fileForProfileData.json");
        }
        fileForProfileSave = Application.persistentDataPath + "/fileForProfileData.json";
    }

    private void CreateFilesForSave(string nameOfFile)
    {
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
    }

    public void SaveAllDataToFile()
    {
        JSONObject personDATA = new JSONObject();

        personDATA.Add("profileName", profileName);
        personDATA.Add("profileAddress", profileAddress);
        personDATA.Add("phoneNumber", phoneNumber);
        personDATA.Add("profileMail", profileMail);
        personDATA.Add("isProfileMailConfirmed", isProfileMailConfirmed);

        if (File.Exists(fileForProfileSave))
        {
            File.WriteAllText(fileForProfileSave, personDATA.ToString());
        }
    }

    public void LoadAllDataFromFile()
    {
        LoadFiles();

        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave));

            if (personDATA != null)
            {
                profileName = personDATA["profileName"];
                profileAddress = personDATA["profileAddress"];
                phoneNumber = personDATA["phoneNumber"];
                profileMail = personDATA["profileMail"];
                isProfileMailConfirmed = personDATA["isProfileMailConfirmed"];
            }
        }
    }

    private void LoadAllDataToObjects()
    {
        if (profileName != null)
        {
            profileNameInputField.text = profileName;
        }
        if (profileAddress != null)
        {
            profileAddressInputField.text = profileAddress;
        }
        if (phoneNumber != null)
        {
            profilePhoneNumberInputField.text = phoneNumber;
        }
        if (profileMail != null)
        {
            profileMailInputField.text = profileMail;
        }
    }

    public void SaveAllDataFromObjects()
    {
        profileName = profileNameInputField.text;
        profileAddress = profileAddressInputField.text;
        phoneNumber = profilePhoneNumberInputField.text;
        profileMail = profileMailInputField.text;

        SaveAllDataToFile();
    }

    private string FindTypeOfProfileMail(string mail)
    {
        if (mail != "")
        {
            string typeOfProfileMail = mail.Remove(0, (mail.IndexOf("@") + 1));
            return typeOfProfileMail;
        }
        return null;
    }

    public void SendMessageToProfileMail(string code)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.Body = CreateMailConfirmMessage(code);

        mailMessage.Subject = "Подтверждение почты. Магазин EGOIST";
        mailMessage.From = new MailAddress(gameManager.shopMail);
        mailMessage.To.Add(profileMail);
        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

        SmtpClient client = new SmtpClient();
        client.Host = "smtp." + FindTypeOfProfileMail(gameManager.shopMail);
        client.Port = 587;
        client.Credentials = new NetworkCredential(mailMessage.From.Address, gameManager.shopMailPassword);
        client.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
         delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
         { return true; };

        client.Send(mailMessage);
    }

    public void SendMessageToCreatorMail(string message)
    {
        MailMessage mailMessage = new MailMessage();

        mailMessage.Body = message;

        mailMessage.Subject = "Новый заказ. Магазин EGOIST";
        mailMessage.From = new MailAddress(gameManager.shopSystemMail);
        mailMessage.To.Add(gameManager.shopMail);
        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

        SmtpClient client = new SmtpClient();
        client.Host = "smtp." + FindTypeOfProfileMail(gameManager.shopMail);
        client.Port = 587;
        client.Credentials = new NetworkCredential(mailMessage.From.Address, gameManager.shopSystemPassword);
        client.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
         delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
         { return true; };

        client.Send(mailMessage);
    }

    public string CreateMailConfirmMessage(string code)
    {
        string message = "Код для подтверждения почты: " + code + "\r\n" + "C уважением, интернет-магазин EGOIST";
        return message;
    }

    public string CreateCheckMessage()
    {
        string newMessageString = "Новый заказ" + System.Environment.NewLine;

        newMessageString += "Имя клиента: " + profileName + System.Environment.NewLine;
        newMessageString += "Указанный город: " + profileAddress + System.Environment.NewLine;
        newMessageString += "Номер телефона клиента: " + phoneNumber + System.Environment.NewLine;
        newMessageString += "Почта клиента: " + profileMail + System.Environment.NewLine;

        string buffString = "";
        if(cashlessPaymentToggle.isOn == true)
        {
            buffString = "'Безналичный рассчет'";
        }
        if(cashPaymentToggle.isOn == true)
        {
            buffString = "'Наличный рассчет'";
        }

        newMessageString += "Оплата типа: " + buffString + System.Environment.NewLine + System.Environment.NewLine;

        for (int i = 0; i < gameManager.itemsToMail.Count; i++)
        {
            string nameItem = "Наименование товара: " + gameManager.itemsToMail[i].nameItem + System.Environment.NewLine;
            string vendorCode = "код товара: " + gameManager.itemsToMail[i].vendorCode + System.Environment.NewLine;
            string firstTypeItem = "Категория: " + gameManager.itemsToMail[i].firstTypeItem + System.Environment.NewLine;
            string secondTypeItem = "Категория: " + gameManager.itemsToMail[i].secondTypeItem + System.Environment.NewLine;

            string firstCostItem = "Первая цена: " + gameManager.itemsToMail[i].firstCostItem + System.Environment.NewLine;
            string secondCostItem = "Вторая цена: " + gameManager.itemsToMail[i].secondCostItem + System.Environment.NewLine;
            string sizeOfItem = "Выбранные размер: " + gameManager.itemsToMail[i].sizeOfItem + System.Environment.NewLine;

            string briefInfoOfItem = "Краткая информация: " + gameManager.itemsToMail[i].briefInfoOfItem + System.Environment.NewLine;
            string compositionOfItem = "Состав товара: " + gameManager.itemsToMail[i].compositionOfItem + System.Environment.NewLine;
            string manufacturingFirm = "Фирма: " + gameManager.itemsToMail[i].manufacturingFirm + System.Environment.NewLine;

            int countOfItemInt = 0, buffInt = 0;

            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out countOfItemInt);
            int.TryParse(gameManager.itemsToMail[i].secondCostItem, out buffInt);
            countOfItemInt = countOfItemInt / buffInt;

            string countOfItem = "Количество единиц товара: " + countOfItemInt + System.Environment.NewLine;

            string allCostOfItem = "ОБЩАЯ СТОИМОСТЬ ДАННОГО ТОВАРА: " + gameManager.itemsToMail[i].allCostOfItem + System.Environment.NewLine;

            newMessageString = newMessageString + nameItem + vendorCode + firstTypeItem + secondTypeItem + firstCostItem + secondCostItem + sizeOfItem + briefInfoOfItem + compositionOfItem + manufacturingFirm + countOfItem + allCostOfItem + System.Environment.NewLine;
        }

        int buff = 0;
        int allCostInt = 0;

        for (int i = 0; i < gameManager.itemsToMail.Count; i++)
        {
            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out buff);
            allCostInt += buff;
        }
        string allCost = "ОБЩАЯ СТОИМОСТЬ ЗАКАЗА: " + allCostInt.ToString() + System.Environment.NewLine;

        newMessageString += allCost;

        newMessageString += System.Environment.NewLine + "C уважением, интернет-магазин EGOIST";

        return newMessageString;
    }

    private void FirstStateOfMailConf()
    {
        mailConfirmation.transform.GetChild(0).gameObject.SetActive(true);
        mailConfirmation.transform.GetChild(1).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void SecondStateOfMailConf()
    {
        mailConfirmation.transform.GetChild(0).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(1).gameObject.SetActive(true);
        mailConfirmation.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void ThirdStateOfMailConf()
    {
        mailConfirmation.transform.GetChild(0).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(1).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void MailConfirmationFunc()
    {
        if(profileMail != "")
        {
            GenerateNewCode();

            SendMessageToProfileMail(codeToConfirm.ToString());

            InputField profileInputCode = mailConfirmation.transform.GetChild(1).GetComponent<InputField>();
            profileInputCode.text = "";

            SecondStateOfMailConf();

            startConfirmFunc = true;
        }
    }

    private void WaitingTrueCode()
    {
        InputField profileInputCode = mailConfirmation.transform.GetChild(1).GetComponent<InputField>();

        if (profileInputCode.text != codeToConfirm.ToString())
        {
            profileInputCode.transform.GetChild(1).GetComponent<Text>().color = Color.red;
        }
        if (profileInputCode.text == codeToConfirm.ToString())
        {
            startConfirmFunc = false;
            isProfileMailConfirmed = true;
            ThirdStateOfMailConf();
            SaveAllDataFromObjects();
        }
    }

    private void GenerateNewCode()
    {
        codeToConfirm = Random.Range(100000, 999999);
    }

    private void CheckingChanges()
    {
        if(lastProfileMail != profileMail)
        {
            lastProfileMail = profileMail;
            isProfileMailConfirmed = false;
        }
    }   
}
