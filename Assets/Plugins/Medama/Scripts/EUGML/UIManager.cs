using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Medama.EUGML
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        public Dictionary<string, Dictionary<string, Sprite>> sprites = new Dictionary<string, Dictionary<string, Sprite>>(); // Dictionary<格納パス, Dictionary<スプライト名, スプライト>>
        public Dictionary<string, Dictionary<string, Sprite>> buildinsprites = new Dictionary<string, Dictionary<string, Sprite>>(); // Dictionary<格納パス, Dictionary<スプライト名, スプライト>>

    }
}
