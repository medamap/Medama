using System.Collections.Generic;
using UnityEngine;

namespace Medama.EUGML
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        public Dictionary<string, Dictionary<string, Sprite>> sprites = new Dictionary<string, Dictionary<string, Sprite>>();
        public Dictionary<string, Dictionary<string, Sprite>> buildinsprites = new Dictionary<string, Dictionary<string, Sprite>>();
    }
}
