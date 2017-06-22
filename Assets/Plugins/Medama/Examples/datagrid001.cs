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
            new UserData(1,  "Jack",    "Fighter",    100, 100,   5,  30,  30),
            new UserData(2,  "Tiger",   "Thief",       45,  50,  50,  80, 100),
            new UserData(3,  "King",    "Monk",        80, 100,  30,  30,  30),
            new UserData(4,  "Andy",    "Wizard",      30,  25, 100,  40,  40),
            new UserData(5,  "Aimee",   "Tamer",       30,  10,  80,  60,  80),
            new UserData(6,  "Shannon", "Gambler",     30,  10,  80, 100,  60),
            new UserData(7,  "Brian",   "Blacksmith",  70,  80,  40,  30,  80),
            new UserData(8,  "Mikoto",  "Kunoichi",    90,  90,  60,  50,  80),
            new UserData(9,  "Sasuke",  "Ninja",      100, 100,  40,  50,  70),
            new UserData(10, "Goemon",  "Dorobou",     60,  60,  60,  80, 100),
            new UserData(11, "Nobu",    "Daimyou",     90,  90,  80,  70,  40),
            new UserData(12, "Oswald",  "Fighter",    100, 100,   5,  30,  60),
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
