#if MEDAMA_USE_UNIRX && MEDAMA_USE_OBSERVABLE_SSH
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
        var dc = EUGML.MedamaUIParseXml(loginxml.text);

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
#endif
