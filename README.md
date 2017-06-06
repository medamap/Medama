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
- Arbor2 https://www.assetstore.unity3d.com/jp/#!/content/47081 (option)

## Get start

### Use tool

- Unity5.6
- TortoiseGit
- VisualStudio 2017

### Clone Medama library repository

- Here, we will assume that it is cloned in E: \ temp or less. Right-click on the blank part of the File Explorer and click GitClone.
- ここでは E:\temp 以下に Clone するという前提で進めます。ファイルエクスプローラの何も無い空間を右クリックして GitClone をクリック

![01](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_01.png)

- Enter https://github.com/medamap/Medama.git in the URL and click OK
- URL に https://github.com/medamap/Medama.git と入力し、OK をクリック

![02](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_02.png)

- Click Close when Clone is over
- Clone が終わったら Close をクリック

![03](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_03.png)

- Start Unity 5 and click on Open
- Unity5 を起動して、Open をクリック

![04](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_04.png)

- Select the folder you just clone and click on the folder selection
- 先ほど Clone したフォルダを選び、フォルダの選択をクリック

![05](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_05.png)

### ImportUniRx

- Click on Unity Window menu and click on Asset Store
- UnityのWindowメニューをクリックし、Asset Storeをクリック

![06](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_06.png)

- Since Asset Store opens, enter UniRx in the search keyword input field and press the Enter key
- Asset Store が開くので、検索キーワード入力欄に UniRx を入力して Enter キーを押下する

![07](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_07.png)

- Since it is displayed slightly below the search result, scroll down a bit to find UniRx and click
- 検索結果のちょっと下の方に表示されているので、少し下にスクロールさせて UniRx を見つけたらクリック

![08](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_08.png)

- Let's download UniRx and import it (Since this example has already been obtained many times already, it is displayed as imported from the beginning)
- UniRx をダウンロードしてインポートしましょう（この例はすでに何度も取得してるので最初からインポートと表示されています）

![09](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_09.png)

- Click Import
- Importをクリック

![10](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_10.png)

### Import Arbor2 (option)

- Although Arbor 2 correspondence is planned to be improved from now on, if you do not import this asset, deleting Assets \ Plugins \ Medama \ Scripts \ ArborScripts of the eyeball library will not cause an error. (I hope there is a function to compile condition by automatic detection of asset presence)
- Arbor2対応はこれから充実させていく予定なのですが、このアセットをインポートしない場合は目玉ライブラリの Assets\Plugins\Medama\Scripts\ArborScripts を削除する事でエラーが出なくなります。（アセットの存在を自動検知で条件コンパイルする機能があるといいなぁ）

- Click on the Asset Store home button
- Asset Store のホームボタンをクリック

![11](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_11.png)

- Enter Arbor 2 in the search keyword input field and press the Enter key
- 検索キーワード入力欄に Arbor2 を入力して Enter キーを押下する

![12](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_12.png)

- Since it is displayed slightly below the search result, scroll down a bit to find UniRx and click
- 検索結果のちょっと下の方に表示されているので、少し下にスクロールさせて UniRx を見つけたらクリック

![13](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_13.png)

- Let's download Arbor 2 and import it (Since this example has already been acquired many times already, it is displayed as imported from the beginning)
- Arbor2 をダウンロードしてインポートしましょう（この例はすでに何度も取得してるので最初からインポートと表示されています）

![14](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_14.png)

- Click Import
- Importをクリック

![15](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_15.png)

### Clone SSH.NET from GitHub and build DLL and import

- Open E: \ temp again in File Explorer, right click on the empty spot and click Git Clone
- 再び E:\temp をファイルエクスプローラで開き、何も無いところを右クリックして Git Clone をクリック

![16](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_16.png)

- Enter https://github.com/sshnet/SSH.NET.git in the URL and click OK
- URL に https://github.com/sshnet/SSH.NET.git を入力し、OK をクリック

![17](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_17.png)

- When Clone is completed, go to SSH.NET \ src and double click on Renci.ShNet.VS2017.sln to start VisualStudio 2017
- Clone が完了したら SSH.NET\src に移動し、Renci.SshNet.VS2017.sln をダブルクリックして VisualStudio2017 を起動する

![18](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_18.png)

- After opening safely in VisualStudio 2017, select Renci.ShNet.NET 35 in Solution Explorer and select Release Build
- VisualStudio2017 で無事開けたら、ソリューションエクスプローラーの Renci.SshNet.NET35 を選択し、Release ビルドを選択する

![19](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_19.png)

- Right-click on Renci.ShNet.NET 35 in Solution Explorer and click Build
- ソリューションエクスプローラーの Renci.SshNet.NET35 を右クリックし、ビルドをクリック

![20](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_20.png)

- Right-click Plugins in Unity's Project View and click Create -> Folder
- Unity のプロジェクトビューの Plugins を右クリックし、Create → Folder をクリック

![21](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_21.png)

- Make the new folder name Renci.ShNet
- 新規フォルダ名を Renci.SshNet にする

![22](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_22.png)

- Since the DLL that was built with VisualStudio 2017 earlier is in E: \ temp \ SSH.NET \ src \ Renci.SshNet.NET 35 \ bin \ Release, drop Renci.SshNet.dll to Unity's project view Plugins / Renci.SshNet To import
- 先ほど VisualStudio2017 でビルドした DLL が E:\temp\SSH.NET\src\Renci.SshNet.NET35\bin\Release にできているので、Renci.SshNet.dll を Unity のプロジェクトビュー Plugins/Renci.SshNet にドロップしてインポートする

![23](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_23.png)

- Click on the black triangle on the left of the Plugins / Renci.SshNet folder in the Unity project view, expand the tree, select Plugins / Renci.ShNet / Renci.SshNet and set the inspector view settings as shown (Any Platform Check status, uncheck Exclude Platforms) and click Apply
- Unity プロジェクトビューの Plugins/Renci.SshNet フォルダの左隣の黒三角をクリックしてツリーを展開し、Plugins/Renci.SshNet/Renci.SshNet を選択し、インスペクタービューの設定を図の通り（Any Platform をチェック状態、Exclude Platforms のチェックを全て外す）にして Apply をクリック

![24](http://megamin.jp/wp-content/uploads/2017/06/howto20170605_24.png)

- That's it, I can take it to a state that can be used for the time being.
- 以上で、とりあえず使える状態にもっていけます。

## Automatically generate uGUI from XML
This function has not been finalized yet. Specifications may change greatly.

eugml.cs
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
        // * Caution: Null not checking *
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

        // Regist command queue.
        commands.Enqueue("ps");
        commands.Enqueue("rpm -qa");
        commands.Enqueue("df -h");

        // Button click event.
        buttonLogin
            .OnClickAsObservable()
            .Subscribe(_ => {
                // Initialize SSH.
                ssh = new ObservableSSH(
                    host: inputHost.text,
                    user: inputUser.text,
                    password: inputPassword.text);

                if (ssh.con.IsAuthenticated) {
                    // Monitor stdout.
                    ssh
                        .stdOutSubject
                        .Subscribe(line => Debug.Log(line))
                        .AddTo(ssh.compositeDisposable);
                    // Check stream status and send shell command.
                    ssh
                        .statusSubject
                        .Where(status =>
                            status == ObservableSshStatus.EndOfStream &&
                            ssh.CheckBuffer("]$ ") &&
                            commands.Count > 0)
                        .Subscribe(status => ssh.writeSshSubject.OnNext(commands.Dequeue()));
                }
                // Double login prevention.
                buttonLogin.enabled = false;
            })
            .AddTo(this);
    }

    // The thread version require dispose SSH on destroy.
    private void OnDestroy() {
        if (ssh != null) {
            ssh.Dispose();
        }
    }
}
```

## Micro coroutine type Observable SSH

command_microcoroutine.cs
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
        // * Caution: Null not checking *
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

        // Regist command queue.
        commands.Enqueue("ps");
        commands.Enqueue("rpm -qa");
        commands.Enqueue("df -h");

        // Button click event.
        buttonLogin
            .OnClickAsObservable()
            .Subscribe(_ => {
                // Initialize SSH.
                ssh = gameObject.AddComponent<ObservableSSHMonoBehaviour>();
                ssh.InitializeAndStart(
                    host: inputHost.text,
                    user: inputUser.text,
                    password: inputPassword.text);
                
                if (ssh.con.IsAuthenticated) {
                    // Monitor stdout.
                    ssh
                        .stdOutSubject
                        .Subscribe(line => Debug.Log(line))
                        .AddTo(this);
                    // Check stream status and send shell command.
                    ssh
                        .statusSubject
                        .Where(status =>
                            status == ObservableSshStatus.EndOfStream &&
                            ssh.CheckBuffer("]$ ") &&
                            commands.Count > 0)
                        .Subscribe(status => ssh.writeSshSubject.OnNext(commands.Dequeue()));
                }
                // Double login prevention.
                buttonLogin.enabled = false;
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

