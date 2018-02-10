using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini1QuizFactory : QuizFactory {

	private Dictionary<string, object> m_rollInformation;
	private string m_lv = "";

	public override Quiz generateQuiz(int _lv){
		if (!GameInformationMaster.Instance.lvMasterData.ContainsKey (_lv.ToString ()))
			return null;
		m_lv = _lv.ToString ();
		m_rollInformation = null;
		Quiz _quiz = new Mini1Quiz ();
		int ter = randomTertiaryType ();
		for(int i = 0 ; i < ter ; i++) {
			Line _line = new Line();
			Utility.PrimaryType[] pri = new Utility.PrimaryType[2];
			pri[0] = randomPrimaryType ();
			pri[1] = pri[0];
			bool mul = randomMultipleType ();
			if (mul) { pri [1] = randomPrimaryType (); }

			for (int j = 0; j < 2; j++) {
				int no = Random.Range (0, 13);
				bool duo = randomDuoType ();
				bool mir = false;

				if (duo) {
					int no2 = Random.Range (0, no + 1);
					no -= no2;
					mir = randomMirrorType ();
					Block _block2 = new Block (no2, pri [j], mir);
					_line.addBlock (_block2, j);
				}

				mir = randomMirrorType ();
				Block _block = new Block (no, pri [j], mir);
				_line.addBlock (_block, j);
			}
			((Mini1Quiz)_quiz).addLine (_line);
		}
		return _quiz;
	}

	private int randomTertiaryType(){
		//Random 0-1
		float _ranValue = Random.Range(0f,1f);
		//Build chance list
		float _accumulative = 0f;
		foreach (Utility.Tertiary _type in System.Enum.GetValues(typeof(Utility.Tertiary))) {
			string _typeStr = _type.ToString ().ToLower ();
			if (GameInformationMaster.Instance.lvMasterData [m_lv].ContainsKey (_typeStr)) {
				_accumulative += float.Parse (GameInformationMaster.Instance.lvMasterData [m_lv] [_typeStr].ToString ());

				if (_ranValue < _accumulative) {
					return (int)_type;
				}
			}
		}
		return 1;
	}

	private Utility.PrimaryType randomPrimaryType(){
		//Random 0-1
		float _ranValue = Random.Range(0f,1f);
		//Build chance list
		float _accumulative = 0f;
		foreach (Utility.PrimaryType _type in System.Enum.GetValues(typeof(Utility.PrimaryType))) {
			string _typeStr = _type.ToString ().ToLower ();
			if (GameInformationMaster.Instance.lvMasterData [m_lv].ContainsKey (_typeStr)) {
				_accumulative += float.Parse (GameInformationMaster.Instance.lvMasterData [m_lv] [_typeStr].ToString ());

				if (_ranValue < _accumulative) {
					return _type;
				}
			}
		}
		return Utility.PrimaryType.Num;
	}
		
	private bool randomMultipleType(){
		float _ranValue = Random.Range(0f,1f);
		string _typeStr = Utility.SecondaryType.MUL.ToString ().ToLower ();
		if (GameInformationMaster.Instance.lvMasterData [m_lv].ContainsKey (_typeStr)) {
			float _chance = float.Parse (GameInformationMaster.Instance.lvMasterData [m_lv] [_typeStr].ToString ());
			if (_ranValue < _chance) {
				return true;
			}
		}
		return false;
	}

	private bool randomDuoType(){
		float _ranValue = Random.Range(0f,1f);
		string _typeStr = Utility.SecondaryType.DUO.ToString ().ToLower ();
		if (GameInformationMaster.Instance.lvMasterData [m_lv].ContainsKey (_typeStr)) {
			float _chance = float.Parse (GameInformationMaster.Instance.lvMasterData [m_lv] [_typeStr].ToString ());
			if (_ranValue < _chance) {
				//Debug.Log ("Duo ma leawwww!! " + _ranValue + " " + _chance);
				return true;
			}
		}
		return false;
	}

	private bool randomMirrorType(){
		float _ranValue = Random.Range(0f,1f);
		string _typeStr = Utility.SecondaryType.MIR.ToString ().ToLower ();
		if (GameInformationMaster.Instance.lvMasterData [m_lv].ContainsKey (_typeStr)) {
			float _chance = float.Parse (GameInformationMaster.Instance.lvMasterData [m_lv] [_typeStr].ToString ());
			if (_ranValue < _chance) {
				return true;
			}
		}
		return false;
	}
}
