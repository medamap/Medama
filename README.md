# Medama library

##Description

The Medama library supports shell stream communication and other functions using SSH.NET and UniRx.

## Features
* Automatically generate uGUI from XML
* Thread type ObservableSSH
* Micro coroutine type Observable SSH

## Automatically generate uGUI from XML
This function has not been finalized yet. Specifications may change greatly.

```cs
using UnityEngine;
using Medama.EUGML;

public class eugml : MonoBehaviour {
	void Start () {
        var dc = gameObject.MedamaUIParseXml(@"<?xml version='1.0'?>
<uGUI xmlins='http://megamin.jp/ns/unity3d/ugui/eugml'>
  <!-- Window form -->
  <AddNode name='FormMain' sprite='resources://Medama/EUGML/UI001#Window001' layout='StretchStretch' top='8' bottom='8' left='8' right='8'>
    <!-- Title bar -->
    <AddNode name='TitleMain' sprite='resources://Medama/EUGML/UI001#Header001' layout='TopStretch' height='24' top='8' left='8' right='8' spritetype='Simple'>
      <AddNode name='Text' layout='StretchStretch'>
        <SetText textstring=' [■] TEST SSH LOGIN' color='white' alignment='MiddleLeft' />
      </AddNode>
    </AddNode>
    <!-- Contents -->
    <AddNode name='GroupMain' top='40' bottom='8' left='8' right='8' sprite='resources://Medama/EUGML/UI001#Group001'>
      <!-- Input host address -->
      <AddNode name='TextLabelHost' layout='TopLeft' width='100' height='30' top='8' left='8'>
        <SetText textstring='HOST：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputHost' layout='TopLeft' width='160' height='30' top='8' left='108' sprite='resources://Medama/EUGML/UI001#Text001'>
        <SetInputField />
      </AddNode>
      <!-- Input user name -->
      <AddNode name='TextLabelUser' layout='TopLeft' width='100' height='30' top='48' left='8'>
        <SetText textstring='USER：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputUser' layout='TopLeft' width='160' height='30' top='48' left='108' sprite='resources://Medama/EUGML/UI001#Text001'>
        <SetInputField />
      </AddNode>
      <!-- Input password -->
      <AddNode name='TextLabelPassword' layout='TopLeft' width='100' height='30' top='88' left='8'>
        <SetText textstring='PASSWORD：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputPassword' layout='TopLeft' width='160' height='30' top='88' left='108' sprite='resources://Medama/EUGML/UI001#Text001'>
        <SetInputField />
      </AddNode>
      <!-- Submit button -->
      <AddNode name='ButtonLogin' layout='TopLeft' width='160' height='30' top='128' left='8' sprite='resources://Medama/EUGML/UI001#Button003'>
        <SetButton textButton='LOGIN' />
      </AddNode>
    </AddNode>
  </AddNode>
</uGUI>");
	}
}
```


