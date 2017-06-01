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
        var loginxml = Resources.Load<TextAsset>("Medama/EUGML/login");
        var dc = gameObject.MedamaUIParseXml(loginxml.text);

        var host = dc.Where(gopair => gopair.Value.name == "InputHost").FirstOrDefault().Value.GetComponent<InputField>();
        var user = dc.Where(gopair => gopair.Value.name == "InputUser").FirstOrDefault().Value.GetComponent<InputField>();
        var password = dc.Where(gopair => gopair.Value.name == "InputPassword").FirstOrDefault().Value.GetComponent<InputField>();
        var button = dc.Where(gopair => gopair.Value.name == "ButtonLogin").FirstOrDefault().Value.GetComponent<Button>();

        commands.Enqueue("ps");
        commands.Enqueue("rpm -qa");
        commands.Enqueue("df -h");

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

            })
            .AddTo(this);
    }
}
