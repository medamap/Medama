# Medama library

## Description

The Medama library supports shell stream communication and other functions using SSH.NET and UniRx.

## Features
* Automatically generate uGUI from XML
* Thread type ObservableSSH
* Micro coroutine type Observable SSH
* Control of Observable SSH using Arbor 2
* Automatically generate Canvas and EventSystem at UI auto generation

## Requirement
- Reactive Extensions for Unity https://github.com/neuecc/UniRx
- SSH.NET https://github.com/sshnet/SSH.NET

## Automatically generate uGUI from XML
This function has not been finalized yet. Specifications may change greatly.

```cs
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
  <AddNode name='FormMain' sprite='resources://Medama/EUGML/UI001#Window001'
        layout='StretchStretch' top='8' bottom='8' left='8' right='8'>
    <!-- Title bar -->
    <AddNode name='TitleMain' sprite='resources://Medama/EUGML/UI001#Header001'
        layout='TopStretch' height='24' top='8' left='8' right='8' spritetype='Simple'>
      <AddNode name='Text' layout='StretchStretch'>
        <SetText textstring=' [■] TEST SSH LOGIN' color='white' alignment='MiddleLeft' />
      </AddNode>
    </AddNode>
    <!-- Contents -->
    <AddNode name='GroupMain' sprite='resources://Medama/EUGML/UI001#Group001'
        top='40' bottom='8' left='8' right='8'>
      <!-- Input host address -->
      <AddNode name='TextLabelHost' layout='TopLeft' width='100' height='30' top='8' left='8'>
        <SetText textstring='HOST：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputHost' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='8' left='108'>
        <SetInputField />
      </AddNode>
      <!-- Input user name -->
      <AddNode name='TextLabelUser' layout='TopLeft' width='100' height='30' top='48' left='8'>
        <SetText textstring='USER：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputUser' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='48' left='108'>
        <SetInputField />
      </AddNode>
      <!-- Input password -->
      <AddNode name='TextLabelPassword' layout='TopLeft' width='100' height='30' top='88' left='8'>
        <SetText textstring='PASSWORD：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputPassword' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='88' left='108'>
        <SetInputField />
      </AddNode>
      <!-- Submit button -->
      <AddNode name='ButtonLogin' sprite='resources://Medama/EUGML/UI001#Button003'
        layout='TopLeft' width='160' height='30' top='128' left='8'>
        <SetButton textButton='LOGIN' />
      </AddNode>
    </AddNode>
  </AddNode>
</uGUI>");

        // Get UI components.
        var inputHost = dc.Where(gopair => gopair.Value.name == "InputHost").First().Value.GetComponent<InputField>();
        var inputUser = dc.Where(gopair => gopair.Value.name == "InputUser").First().Value.GetComponent<InputField>();
        var inputPassword = dc.Where(gopair => gopair.Value.name == "InputPassword").First().Value.GetComponent<InputField>();
        var buttonLogin = dc.Where(gopair => gopair.Value.name == "ButtonLogin").First().Value.GetComponent<Button>();
    }
}
```

## Thread type ObservableSSH

How to use ObservableSSH (Thread version)

Resources/Medama/EUGML/login.xml
```xml
<?xml version='1.0'?>
<uGUI xmlins='http://megamin.jp/ns/unity3d/ugui/eugml'>
  <!-- Window form -->
  <AddNode name='FormMain' sprite='resources://Medama/EUGML/UI001#Window001'
        layout='StretchStretch' top='8' bottom='8' left='8' right='8'>
    <!-- Title bar -->
    <AddNode name='TitleMain' sprite='resources://Medama/EUGML/UI001#Header001'
        layout='TopStretch' height='24' top='8' left='8' right='8' spritetype='Simple'>
      <AddNode name='Text' layout='StretchStretch'>
        <SetText textstring=' [■] TEST SSH LOGIN' color='white' alignment='MiddleLeft' />
      </AddNode>
    </AddNode>
    <!-- Contents -->
    <AddNode name='GroupMain' sprite='resources://Medama/EUGML/UI001#Group001'
        top='40' bottom='8' left='8' right='8'>
      <!-- Input host address -->
      <AddNode name='TextLabelHost' layout='TopLeft' width='100' height='30' top='8' left='8'>
        <SetText textstring='HOST：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputHost' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='8' left='108'>
        <SetInputField />
      </AddNode>
      <!-- Input user name -->
      <AddNode name='TextLabelUser' layout='TopLeft' width='100' height='30' top='48' left='8'>
        <SetText textstring='USER：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputUser' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='48' left='108'>
        <SetInputField />
      </AddNode>
      <!-- Input password -->
      <AddNode name='TextLabelPassword' layout='TopLeft' width='100' height='30' top='88' left='8'>
        <SetText textstring='PASSWORD：' alignment='MiddleLeft' />
      </AddNode>
      <AddNode name='InputPassword' sprite='resources://Medama/EUGML/UI001#Text001'
        layout='TopLeft' width='160' height='30' top='88' left='108'>
        <SetInputField />
      </AddNode>
      <!-- Submit button -->
      <AddNode name='ButtonLogin' sprite='resources://Medama/EUGML/UI001#Button003'
        layout='TopLeft' width='160' height='30' top='128' left='8'>
        <SetButton textButton='LOGIN' />
      </AddNode>
    </AddNode>
  </AddNode>
</uGUI>
```

command_thread.cs
```cs
using UnityEngine;
using UnityEngine.UI;
using Medama.ObservableSsh;
using Medama.EUGML;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class command_thread : MonoBehaviour {
    ObservableSSH ssh;
    public Queue<string> commands = new Queue<string>();

    void Start() {
        // Create UI.
        var loginxml = Resources.Load<TextAsset>("Medama/EUGML/login");
        var dc = gameObject.MedamaUIParseXml(loginxml.text);

        // Get UI components.
        var host = dc.Where(gopair => gopair.Value.name == "InputHost").FirstOrDefault().Value.GetComponent<InputField>();
        var user = dc.Where(gopair => gopair.Value.name == "InputUser").FirstOrDefault().Value.GetComponent<InputField>();
        var password = dc.Where(gopair => gopair.Value.name == "InputPassword").FirstOrDefault().Value.GetComponent<InputField>();
        var button = dc.Where(gopair => gopair.Value.name == "ButtonLogin").FirstOrDefault().Value.GetComponent<Button>();

        // Regist command queue
        commands.Enqueue("ps");
        commands.Enqueue("rpm -qa");
        commands.Enqueue("df -h");

        // Button click event
        button
            .OnClickAsObservable()
            .Subscribe(_ => {
                ssh = new ObservableSSH(host: host.text, user: user.text, password: password.text);
                // Monitor stdout
                if (ssh.con.IsAuthenticated) {
                    ssh
                        .stdOutSubject
                        .Subscribe(line => Debug.Log(line))
                        .AddTo(ssh.compositeDisposable);

                    ssh
                        .statusSubject
                        .Where(status => status == ObservableSshStatus.EndOfStream && ssh.CheckBuffer("]$ ") && commands.Count > 0)
                        .Subscribe(status => ssh.writeSshSubject.OnNext(commands.Dequeue()));
                }
                button.enabled = false;
            })
            .AddTo(this);
    }

    private void OnDestroy() {
        if (ssh != null) {
            ssh.Dispose();
        }
    }
}
```

## Micro coroutine type Observable SSH

```cs
using UnityEngine;
using UnityEngine.UI;
using Medama.ObservableSsh;
using Medama.EUGML;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class command_microcoroutine : MonoBehaviour {
    ObservableSSHMonoBehaviour ssh;
    public Queue<string> commands = new Queue<string>();

    void Start() {
        // Create UI.
        var loginxml = Resources.Load<TextAsset>("Medama/EUGML/login");
        var dc = gameObject.MedamaUIParseXml(loginxml.text);

        // Get UI components.
        var host = dc.Where(gopair => gopair.Value.name == "InputHost").FirstOrDefault().Value.GetComponent<InputField>();
        var user = dc.Where(gopair => gopair.Value.name == "InputUser").FirstOrDefault().Value.GetComponent<InputField>();
        var password = dc.Where(gopair => gopair.Value.name == "InputPassword").FirstOrDefault().Value.GetComponent<InputField>();
        var button = dc.Where(gopair => gopair.Value.name == "ButtonLogin").FirstOrDefault().Value.GetComponent<Button>();

        // Regist command queue
        commands.Enqueue("ps");
        commands.Enqueue("rpm -qa");
        commands.Enqueue("df -h");

        // Button click event
        button
            .OnClickAsObservable()
            .Subscribe(_ => {
                ssh = gameObject.AddComponent<ObservableSSHMonoBehaviour>();
                ssh.InitializeAndStart(host: host.text, user: user.text, password: password.text);
                // Monitor stdout
                if (ssh.con.IsAuthenticated) {
                    ssh
                        .stdOutSubject
                        .Subscribe(line => Debug.Log(line))
                        .AddTo(this);

                    ssh
                        .statusSubject
                        .Where(status => status == ObservableSshStatus.EndOfStream && ssh.CheckBuffer("]$ ") && commands.Count > 0)
                        .Subscribe(status => ssh.writeSshSubject.OnNext(commands.Dequeue()));
                }
                button.enabled = false;
            })
            .AddTo(this);
    }

    // The micro coroutine version automatically dispose SSH.
    //private void OnDestroy() {
    //    if (ssh != null) {
    //        ssh.Dispose();
    //    }
    //}

}
```

