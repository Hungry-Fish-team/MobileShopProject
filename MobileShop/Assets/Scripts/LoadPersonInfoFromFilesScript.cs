using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEditor;

public class LoadPersonInfoFromFilesScript : MonoBehaviour
{
    public List<ItemScript> sortingListOfItem;
    public List<string> sortingListOfType;
    public List<string> personBusket, personMarks;

    public class notificationObject
    {
        public string message;
        public string date;

        public notificationObject() { }

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

    public void LoadFiles()
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
}
