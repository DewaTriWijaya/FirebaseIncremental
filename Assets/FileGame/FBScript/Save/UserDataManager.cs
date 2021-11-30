using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress = new UserProgressData();

    public static void LoadFromLocal()
    {
        // Cek apakah ada data yg tersimpan sebagai PROGRESS_KEY
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // jika tidak ada, maka buat data baru
            //Progress = new UserProgressData();

            // dan upload ke cloud
            Save(true);
        }
        else
        {
            // jika ada, maka timpa progress dengan yang sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }


    public static IEnumerator LoadFromCloud(Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();

        bool isCompleted = false;
        bool isSuccessful = false;
        const long maxAllowedSize = 1024 * 1024; // sama dengan 1 MB

        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith
            ((Task<byte[]> task) =>
            {
                if (!task.IsFaulted)
                {
                    string json = Encoding.Default.GetString(task.Result);
                    Progress = JsonUtility.FromJson<UserProgressData>(json);
                    isSuccessful = true;
                }
                isCompleted = true;
            });

        while (!isCompleted)
        {
            yield return null;
        }

        // jika sukses mendownload, maka simpan data hasil download
        if (isSuccessful)
        {
            Save();
        }
        else
        {
            // jika tidak ada data di cloud, maka load data dari local
            LoadFromLocal();
        }
        onComplete?.Invoke();
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);

        if (uploadToCloud)
        {
            AnalyticsManager.SetUserProperties("gold", Progress.Gold.ToString());

            byte[] data = Encoding.Default.GetBytes(json);
            StorageReference targetStorage = GetTargetCloudStorage();

            targetStorage.PutBytesAsync(data);
        }
    }

    private static StorageReference GetTargetCloudStorage()
    {
        // gunakan device id sebagai nama file yang akan disimpan di cloud
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceID}");
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }
}
