using System.Collections;
using System.Collections.Generic;
using System;
using Firebase.Database;

[Serializable]
public class RoundData{
    public int Combo;
    public int Bonus;
    public int AccuracyHit;
    public int AccuracyTotal;
    public int Score;
    public float AverageSpeed;

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["score"] = Score;
        result["combo"] = Combo;
        result["bonus"] = Bonus;
        result["accuracyhit"] = AccuracyHit;
        result["accuracytotal"] = AccuracyTotal;
        result["avgspeed"] = AverageSpeed;
        return result;
    }
}

[Serializable]
public class Match{
    public string Id;
    public string PlayerId1;
    public string PlayerId2 = "";
    public int Score1 = 0;
    public int Score2 = 0;
    public int PlayerNo;
    public int OpponentNo;
    public Dictionary<string, bool> Done = new Dictionary<string, bool>();
    public Dictionary<string, string> Names = new Dictionary<string, string>();
    public Dictionary<string, string> ProfilePictureUrls = new Dictionary<string, string>();
    public Dictionary<string, RoundData> RoundList = new Dictionary<string, RoundData>();

    public bool IsCompleted
    {
        get
        {
            return RoundList.Count >= 6 || (Done.ContainsKey(PlayerId1) && Done.ContainsKey(PlayerId2) && Done[PlayerId1] && Done[PlayerId2]);
        }
    }

    public bool IsNewMatch
    { //No player 2 yet
        get
        {
            return PlayerId2 == "";
        }
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["player_id1"] = PlayerId1;
        if(PlayerId2 != "") result["player_id2"] = PlayerId2;
        if (Done != null)
        {
            Dictionary<string, object> doneDic = new Dictionary<string, object>();
            foreach(var value in Done){
                doneDic[value.Key] = value.Value;
            }
            result["done"] = doneDic;
        }
        Dictionary<string, object> nameDic = new Dictionary<string, object>();
        foreach (var value in Names)
        {
            nameDic[value.Key] = value.Value;
        }
        result["name"] = nameDic;

        Dictionary<string, object> picDic = new Dictionary<string, object>();
        foreach (var value in ProfilePictureUrls)
        {
            picDic[value.Key] = value.Value;
        }
        result["picurl"] = picDic;

        if (RoundList.ContainsKey("0")) result["0"] = RoundList["0"].ToDictionary();
        if (RoundList.ContainsKey("1")) result["1"] = RoundList["1"].ToDictionary();
        if (RoundList.ContainsKey("2")) result["2"] = RoundList["2"].ToDictionary();
        if (RoundList.ContainsKey("3")) result["3"] = RoundList["3"].ToDictionary();
        if (RoundList.ContainsKey("4")) result["4"] = RoundList["4"].ToDictionary();
        if (RoundList.ContainsKey("5")) result["5"] = RoundList["5"].ToDictionary();

        result["timestamp"] = DateTime.UtcNow.ToString();

        return result;
    }

    public Match() { }

    /// <summary>
    /// Create Match from DataSnapShot
    /// </summary>
    /// <param name="snapshot">Firebase snapshot.</param>
    public Match(DataSnapshot snapshot, string userId){
        Id = snapshot.Key;
        PlayerId1 = snapshot.Child("player_id1").Value.ToString();
        PlayerId2 = snapshot.HasChild("player_id2") ? snapshot.Child("player_id2").Value.ToString() : "";

        PlayerNo = PlayerId1 == userId ? 0 : 1;
        OpponentNo = PlayerNo == 0 ? 1 : 0;

        Dictionary<string, string> nameTemp = new Dictionary<string, string>();
        foreach (DataSnapshot nameData in snapshot.Child("name").Children)
        {
            nameTemp[nameData.Key] = nameData.Value.ToString();
        }
        Names = nameTemp;

        Dictionary<string, string> urlTemp = new Dictionary<string, string>();
        foreach (DataSnapshot urlData in snapshot.Child("picurl").Children)
        {
            urlTemp[urlData.Key] = urlData.Value.ToString();
        }
        ProfilePictureUrls = urlTemp;

        for (int i = 0; i < 6; i++)
        {
            string roundNo = i.ToString();
            if (snapshot.HasChild(roundNo))
            {
                DataSnapshot data = snapshot.Child(roundNo);
                RoundData round = new RoundData()
                {
                    Score = int.Parse(data.Child("score").Value.ToString()),
                    Combo = int.Parse(data.Child("combo").Value.ToString()),
                    Bonus = int.Parse(data.Child("bonus").Value.ToString()),
                    AccuracyHit = int.Parse(data.Child("accuracyhit").Value.ToString()),
                    AccuracyTotal = int.Parse(data.Child("accuracytotal").Value.ToString()),
                    AverageSpeed = float.Parse(data.Child("avgspeed").Value.ToString()),
                };
                RoundList[roundNo] = round;
            }
        }
        CalculateScore();
    }

    public void UpdateMatch(DataSnapshot snapshot){
        PlayerId1 = snapshot.Child("player_id1").Value.ToString();
        PlayerId2 = snapshot.Child("player_id2").Value.ToString();

        var nameDic = Names;
        nameDic.Clear();
        foreach (DataSnapshot nameData in snapshot.Child("name").Children)
        {
            nameDic[nameData.Key] = nameData.Value.ToString();
        }

        var urlDic = ProfilePictureUrls;
        urlDic.Clear();
        foreach (DataSnapshot urlData in snapshot.Child("picurl").Children)
        {
            urlDic[urlData.Key] = urlData.Value.ToString();
        }
        //Round list
        for (int i = 0; i < 6; i++)
        {
            string roundNo = i.ToString();
            if (snapshot.HasChild(roundNo) && !RoundList.ContainsKey(roundNo))
            {
                DataSnapshot data = snapshot.Child(roundNo);
                RoundData round = new RoundData()
                {
                    Score = int.Parse(data.Child("score").Value.ToString()),
                    Combo = int.Parse(data.Child("combo").Value.ToString()),
                    Bonus = int.Parse(data.Child("bonus").Value.ToString()),
                    AccuracyHit = int.Parse(data.Child("accuracyhit").Value.ToString()),
                    AccuracyTotal = int.Parse(data.Child("accuracytotal").Value.ToString()),
                    AverageSpeed = float.Parse(data.Child("avgspeed").Value.ToString()),
                };
                RoundList[roundNo] = round;
            }
        }
        CalculateScore();
    }

    public void CalculateScore(){
        Score1 = 0;
        Score2 = 0;
        for (int i = 0; i < 5; i += 2)
        {
            string roundNo1 = i.ToString();
            string roundNo2 = (i + 1).ToString();
            if (RoundList.ContainsKey(roundNo1) && RoundList.ContainsKey(roundNo2))
            {
                int score1 = RoundList[roundNo1].Score;
                int score2 = RoundList[roundNo2].Score;
                if (score1 > score2) Score1 += 1;
                if (score1 < score2) Score2 += 1;
            }
        }
    }
}
