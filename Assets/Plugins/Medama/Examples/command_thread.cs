﻿using UnityEngine;
using UnityEngine.UI;
using Medama.ObservableSsh;
using Medama.EUGML;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class command_thread : MonoBehaviour {
    public string wait = "]$ ";
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
                            (ssh.CheckBuffer(wait.Trim() + " ") || ssh.CheckBuffer("]$ ") || ssh.CheckBuffer("]# ")) &&
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
