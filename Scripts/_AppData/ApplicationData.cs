using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Appdata
{
    public enum GraphicQuality
    {
        LOW, MEDIUM, HIGH
    }

    [System.Serializable]
    public class OptionData
    {
        public int graphicQuality;
        public int frameRate;
        public float sfx_volume;
        public float bg_volume;

        public OptionData(int quality, int frame, float sfx, float bg)
        {
            graphicQuality = quality;
            frameRate = frame;
            sfx_volume = sfx;
            bg_volume = bg;
        }
    }

    public static class ApplicationData
    {
        private static string fileName = "AppData_Setting.apd";
        private static string serverURL = "https://unitaemin.run.goorm.io/hellfight/";

        public static OptionData optionData;

        public static void SaveApplicationData(OptionData data)
        {
            string path = Application.persistentDataPath + "/" + fileName;

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static void LoadApplicationData()
        {
            string path = Application.persistentDataPath + "/" + fileName;

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                OptionData data = formatter.Deserialize(stream) as OptionData;

                optionData = data;
                stream.Close();
            }
            else
            {
                optionData = new OptionData((int)GraphicQuality.MEDIUM, 60, 100, 100);
                //optionData.graphicQuality = (int)GraphicQuality.MEDIUM;
                //optionData.frameRate = 60;
                //optionData.bg_volume = 100.0f;
                //optionData.sfx_volume = 100.0f;
                SaveApplicationData(optionData);
            }
        }

        public static string GetServerURL()
        {
            return serverURL;
        }
    }
}
