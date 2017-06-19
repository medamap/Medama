using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Medama.EUGML;

public class datagrid001 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var datagrid001 = Resources.Load<TextAsset>("Medama/EUGML/datagrid001");
        var dc = EUGML.MedamaUIParseXml(datagrid001.text);

        var list = new List<UserData>() {
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60),
            new UserData(1, "Jack", "Fighter", 100, 100, 5, 30, 60)
        };

        var group = dc.Select(x => x.Value).Where(x => x.name == "GroupMain").First();
        group.MedamaUISetDataGrid(list: list);
    }
}

public class UserData {
    public int player_id;
    public string player_name;
    public string job;
    public int vitality;
    public int strength;
    public int intelligence;
    public int luck;
    public int dexterity;
    public UserData(int player_id, string player_name, string job, int vitality, int strength, int intelligence, int luck, int dexterity) {
        this.player_id = player_id;
        this.player_name = player_name;
        this.job = job;
        this.vitality = vitality;
        this.strength = strength;
        this.intelligence = intelligence;
        this.luck = luck;
        this.dexterity = dexterity;
    }
}
