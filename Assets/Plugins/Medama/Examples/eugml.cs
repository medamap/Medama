using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Medama.EUGML;

public class eugml : MonoBehaviour {
    void Start () {
        // Create UI from XML.
        var dc = gameObject.MedamaUIParseXml(@"<?xml version='1.0'?>
<uGUI xmlins='http://megamin.jp/ns/unity3d/ugui/eugml'>
  
  <!-- Window form -->
  <AddNode
    name='FormMain'
    sprite='resources://Medama/EUGML/UI001#Window001'
    layout='StretchStretch' top='8' bottom='8' left='8' right='8'>

    <!-- Title bar -->
    <AddNode
      name='TitleMain' sprite='resources://Medama/EUGML/UI001#Header001'
      layout='TopStretch' height='24' top='8' left='8' right='8' spritetype='Simple'>
      <AddNode name='Text' layout='StretchStretch'>
        <SetText textstring=' [__] TEST SSH LOGIN' color='white' alignment='MiddleLeft' />
      </AddNode>
    </AddNode>
    
    <!-- Contents -->
    <AddNode
      name='GroupMain' sprite='resources://Medama/EUGML/UI001#Group001'
      top='40' bottom='8' left='8' right='8'>
      
      <!-- Input host address -->
      <AddNode
        name='TextLabelHost' layout='TopLeft' width='100' height='30' top='8' left='8'>
        <SetText textstring='HOST : ' alignment='MiddleLeft' />
      </AddNode>
      <AddNode
        name='InputHost' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='8' left='108'>
        <SetInputField />
      </AddNode>
      
      <!-- Input user name -->
      <AddNode
        name='TextLabelUser' layout='TopLeft' width='100' height='30' top='48' left='8'>
        <SetText textstring='USER : ' alignment='MiddleLeft' />
      </AddNode>
      <AddNode
        name='InputUser' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='48' left='108'>
        <SetInputField />
      </AddNode>
      
      <!-- Input password -->
      <AddNode
        name='TextLabelPassword' layout='TopLeft' width='100' height='30' top='88' left='8'>
        <SetText textstring='PASSWORD : ' alignment='MiddleLeft' />
      </AddNode>
      <AddNode
        name='InputPassword' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='88' left='108'>
        <SetInputField />
      </AddNode>
      
      <!-- Submit button -->
      <AddNode
        name='ButtonLogin' sprite='resources://Medama/EUGML/UI001#Button003'
        layout='TopLeft' width='160' height='30' top='128' left='8'>
        <SetButton textButton='LOGIN' />
      </AddNode>
    </AddNode>
  </AddNode>
</uGUI>");

        // Get UI components.
        // * Caution: Null not checking *
#pragma warning disable 219
        var inputHost = dc
            .Where(gopair => gopair.Value.name == "InputHost")
            .First()
            .Value
            .GetComponent<InputField>();

        var inputUser = dc
            .Where(gopair => gopair.Value.name == "InputUser")
            .First()
            .Value
            .GetComponent<InputField>();

        var inputPassword = dc
            .Where(gopair => gopair.Value.name == "InputPassword")
            .First()
            .Value
            .GetComponent<InputField>();

        var buttonLogin = dc
            .Where(gopair => gopair.Value.name == "ButtonLogin")
            .First()
            .Value
            .GetComponent<Button>();
#pragma warning restore 219
    }
}
