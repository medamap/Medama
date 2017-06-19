using UnityEngine;
using UnityEngine.UI;
using Medama.ObservableSsh;
using Medama.EUGML;
using System.Linq;
using UniRx;

public class rpmqa_thread : MonoBehaviour
{
    ObservableSSH ssh;

    void Start() {
        var loginxml = Resources.Load<TextAsset>("Medama/EUGML/login");
        var dc = EUGML.MedamaUIParseXml(loginxml.text);

        var host = dc.Where(gopair => gopair.Value.name == "InputHost").FirstOrDefault().Value.GetComponent<InputField>();
        var user = dc.Where(gopair => gopair.Value.name == "InputUser").FirstOrDefault().Value.GetComponent<InputField>();
        var password = dc.Where(gopair => gopair.Value.name == "InputPassword").FirstOrDefault().Value.GetComponent<InputField>();
        var button = dc.Where(gopair => gopair.Value.name == "ButtonLogin").FirstOrDefault().Value.GetComponent<Button>();

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
                    ssh.writeSshSubject.OnNext("rpm -qa");
                }
            })
            .AddTo(this);
    }

    private void OnDestroy() {
        if (ssh != null) {
            ssh.Dispose();
        }
    }
}
