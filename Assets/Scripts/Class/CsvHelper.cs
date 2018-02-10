using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVHelper
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	public static Dictionary<string,Dictionary<string, object>> Read(string file)
	{
		Dictionary<string,Dictionary<string, object>> list = new Dictionary<string,Dictionary<string, object>>();
		TextAsset data = Resources.Load (file) as TextAsset;
		//string data = DownloadController.Instance.LoadEncodedFile("csv", file) as string;

		if (data == null) {
			Debug.Log ("No file found : " + file);
			return list;
		}
		string[] lines = Regex.Split (data.ToString(), LINE_SPLIT_RE);

		if(lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for(int i=1; i < lines.Length; i++) {

			var values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 ||values[0] == "") continue;

			var entry = new Dictionary<string, object>();
			for(int j=0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n");
				object finalvalue = value;
				int n;
				float f;
				if(int.TryParse(value, out n)) {
					finalvalue = n;
				} else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
				header[j] = header[j].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n");
				entry[header[j]] = finalvalue;
			}
			values[0] = values[0].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n");
			list[values[0]] = entry;
		}
		return list;
	}
}