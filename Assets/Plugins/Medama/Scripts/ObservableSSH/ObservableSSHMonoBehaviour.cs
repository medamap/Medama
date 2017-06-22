#if MEDAMA_USE_UNIRX && MEDAMA_USE_OBSERVABLE_SSH
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Renci.SshNet;
using UniRx;

namespace Medama.ObservableSsh
{
    public class ObservableSSHMonoBehaviour : MonoBehaviour
    {
        public ObservableSSH ssh = null;
        public ConnectionInfo con { get { return ssh != null ? ssh.con : null; } }
        public SshClient sshClient { get { return ssh != null ? ssh.sshClient : null; } }
        public bool loop { get { return ssh != null ? ssh.loop : false; } set { if (ssh != null) ssh.loop = value; } }
        public ShellStream shellStream { get { return ssh != null ? ssh.shellStream : null; } }
        public Subject<string> writeSshSubject { get { return ssh != null ? ssh.writeSshSubject : null; } }
        public Subject<string> stdOutSubject { get { return ssh != null ? ssh.stdOutSubject : null; } }
        public Subject<ObservableSshStatus> statusSubject { get { return ssh != null ? ssh.statusSubject : null; } }
        public string commandPrefix { get { return ssh != null ? ssh.commandPrefix : null; } set { if (ssh != null) ssh.commandPrefix = value; } }
        public CompositeDisposable compositeDisposable { get { return ssh != null ? ssh.compositeDisposable : null; } }
        public StreamWriter stdInStreamWriter { get { return ssh != null ? ssh.stdInStreamWriter : null; } }
        public ReactiveProperty<Buffer> observableSSHBuffer { get { return ssh != null ? ssh.observableSSHBuffer : null; } }

        /// <summary>
        /// Initialize ssh and start
        /// </summary>
        public bool InitializeAndStart(
            string host = Const.host,
            string user = Const.user,
            string password = Const.password,
            string keyfile = Const.keyfile,
            string passphrase = Const.passphrase,
            int port = Const.port,
            string terminalname = Const.terminalname,
            uint columns = Const.columns,
            uint rows = Const.rows,
            uint width = Const.width,
            uint height = Const.height,
            int buffersize = Const.buffersize
            ) {

            // Initialize ssh
            ssh = new ObservableSSH(
                host: host,
                user: user,
                password: password,
                keyfile: keyfile,
                passphrase: passphrase,
                port: port,
                terminalname: terminalname,
                columns: columns,
                rows: rows,
                width: width,
                height: height,
                buffersize: buffersize,
                initialize: false);

            if (con == null || !con.IsAuthenticated) {
                if (ssh != null) {
                    ssh.Dispose();
                    ssh = null;
                }
                return false;
            }

            // Run micro coroutine of read ssh stream
            MainThreadDispatcher.StartUpdateMicroCoroutine(SshReader());

            // Run micro coroutine of monitor connection status
            MainThreadDispatcher.StartUpdateMicroCoroutine(CheckConnectionStatus());

            return true;
        }

        /// <summary>
        /// Read stream of ssh
        /// </summary>
        IEnumerator SshReader() {
            var wait1 = Observable.ToYieldInstruction(WaitForSecondsCoroutine(0.3f));
            while (!wait1.IsDone) {
                yield return null;
            }
            using (StreamReader reader = new StreamReader(shellStream)) {
                while (loop) {
                    // Wait end of stream
                    if (reader.EndOfStream) {
                        statusSubject.OnNext(ObservableSshStatus.EndOfStream);
                        yield return null;
                        continue;
                    }
                    // Read ssh stram
                    if (shellStream.CanRead && reader.Peek() != -1) {
                        try {
                            // Read ssh and notify to stdout subject
                            observableSSHBuffer.SetValueAndForceNotify(observableSSHBuffer.Value.UpdateFromReadStream(reader));
                        } catch (Exception ex) {
                            Debug.LogError(ex.Message + "\r\n" + ex.StackTrace);
                            loop = false;
                        }
                    }
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Monitor connection status
        /// </summary>
        IEnumerator CheckConnectionStatus() {
            var wait1 = Observable.ToYieldInstruction(WaitForSecondsCoroutine(0.3f));
            while (!wait1.IsDone) {
                yield return null;
            }
            while (loop) {
                yield return null;
            }
            statusSubject.OnNext(ObservableSshStatus.End);
            statusSubject.OnCompleted();
        }

        /// <summary>
        /// Wait coroutine
        /// </summary>
        IEnumerator WaitForSecondsCoroutine(float second) {
            yield return new WaitForSeconds(second);
        }

        /// <summary>
        /// Check ssh read buffer
        /// </summary>
        public bool CheckBuffer(string CheckWord) { return ssh != null ? ssh.CheckBuffer(CheckWord) : false; }

        /// <summary>
        /// Dispose ssh object
        /// </summary>
        private void OnDestroy() {
            if (ssh != null)
                ssh.Dispose();
        }
    }
}
#endif
