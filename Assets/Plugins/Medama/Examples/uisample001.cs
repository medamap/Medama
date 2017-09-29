using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Medama.EUGML;

public class uisample001 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var uisample001 = Resources.Load<TextAsset>("Medama/EUGML/uisample001");
        var dc = EUGML.MedamaUIParseXml(uisample001.text);

    }

}
