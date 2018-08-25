using System;
using UnityEngine;
using System.Collections.Generic;

public class Utility
{
	public enum PrimaryType {
		Num = 1,
		Inv = 2,
		Wrd = 3,
		Dic = 4,
		Fiv = 5,
		Rom = 6,
		Clk = 7,
	}

	public enum SecondaryType{
		DUO = 1,
		MIR = 2,
		MUL = 3,
	}

	public enum Tertiary{
		SIN = 1,
		DOU = 2,
		TRI = 3,
	}

	public enum PlayerPrefKey{
		BEST, //best score
	}

    public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            WrapperArray<T> wrapper = JsonUtility.FromJson<WrapperArray<T>>(newJson);
            return wrapper.array;
        }

        public static Dictionary<string, T> getJsonDictionary<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            WrapperDic<T> wrapper = JsonUtility.FromJson<WrapperDic<T>>(newJson);
            return wrapper.dic;
        }

        [Serializable]
        private class WrapperArray<T>
        {
            public T[] array;
        }

        [Serializable]
        private class WrapperDic<T>
        {
            public Dictionary<string,T> dic;
        }
    }
}

