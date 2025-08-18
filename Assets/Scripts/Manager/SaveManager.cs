using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{

    #region SAVE
    public static void SaveData(object data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/empresas.sav", FileMode.Create);

        bf.Serialize(fs, data);
        fs.Close();
    }
    public static void LoadData(System.Action<object> loaded)
    {
        if (File.Exists(Application.persistentDataPath + "/empresas.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Application.persistentDataPath + "/empresas.sav", FileMode.Open);


            object data = bf.Deserialize(fs);

            if (loaded != null)
                loaded(data);

            fs.Close();
            
        }
        else
        {
            object data = null;
            if (loaded != null)
                loaded(data);

            Debug.LogWarning("No Existe ningun registro de empresas");
        }
    }
    #endregion
}
