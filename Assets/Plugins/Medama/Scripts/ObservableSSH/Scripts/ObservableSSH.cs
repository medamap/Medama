using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using Renci.SshNet;
using UniRx;

namespace Medama.ObservableSsh
{
    public class ObservableSSH : IDisposable
    {
        public ConnectionInfo con = null;
        public SshClient sshClient = null;
        public bool loop = true;
        public ShellStream shellStream = null;
        public Subject<string> writeSshSubject = null;
        public Subject<string> stdOutSubject = null;
        public Subject<ObservableSshStatus> statusSubject = null;
        public string commandPrefix = "env LANG=en_US.UTF-8";
        public CompositeDisposable compositeDisposable = new CompositeDisposable();
        public StreamWriter stdInStreamWriter = null;
        public ReactiveProperty<Buffer> observableSSHBuffer = new ReactiveProperty<Buffer>(new Buffer());
        public string waitstring = null;

        /// <summary>
        /// Constructor
        /// Initialize ssh and start
        /// </summary>
        public ObservableSSH(
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
            int buffersize = Const.buffersize,
            bool initialize = true,
            string waitstring = ""
            ) {

            // Initialize ssh status subject
            statusSubject = new Subject<ObservableSshStatus>();

            // Initialize ssh connection
            if (!string.IsNullOrEmpty(keyfile) && !string.IsNullOrEmpty(passphrase)) {
                con = new ConnectionInfo(host, port, user, new AuthenticationMethod[] {
                    new PasswordAuthenticationMethod(user, password),
                    new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile[]{new PrivateKeyFile(keyfile, passphrase)})
                });
            } else if (!string.IsNullOrEmpty(keyfile)) {
                con = new PrivateKeyConnectionInfo(host, port, user, new PrivateKeyFile[] { new PrivateKeyFile(keyfile) });
            } else {
                con = new ConnectionInfo(host, port, user, new AuthenticationMethod[] { new PasswordAuthenticationMethod(user, password) });
            }
            sshClient = new SshClient(con);

            try {
                sshClient.Connect();
            } catch (Exception ex) {
                Debug.LogError(ex.Message + "\r\n" + ex.StackTrace);
                return;
            }

            // Initialize shell stream
            shellStream = sshClient.CreateShellStream(terminalname, columns, rows, width, height, buffersize);

            // Initialize shell stream writer
            stdInStreamWriter = new StreamWriter(shellStream);
            stdInStreamWriter.AutoFlush = true;

            // Initialize shell command subject
            writeSshSubject = new Subject<string>();
            writeSshSubject
                .Subscribe(SendCommand)
                .AddTo(compositeDisposable);

            // Thread pool version
            if (initialize) {
                // Run thread of read ssh stream
                SshReader().Subscribe().AddTo(compositeDisposable);
                
                // Run thread of monitor connection status
                CheckConnectionStatus().Subscribe().AddTo(compositeDisposable);

                // Monitor EndOfStream and check match string
                MonitorEndOfStream().Subscribe().AddTo(compositeDisposable);
            }

            // Initialize stdout subject
            stdOutSubject = new Subject<string>();

            // Initialize converter of ssh stream buffer to stdout
            observableSSHBuffer
                .Where(CheckBufferAvailable)
                .Subscribe(ConvertBufferToStdout)
                .AddTo(compositeDisposable);
        }

        /// <summary>
        /// Monitor EndOfStream and check match string
        /// </summary>
        private UniRx.IObservable<Unit> MonitorEndOfStream() {
            Thread.Sleep(500);
            return Observable.Create<Unit>(observer => {
                while (loop) {
                    Thread.Sleep(100);

                }
                return Disposable.Create(() => { });
            })
            .SubscribeOn(Scheduler.ThreadPool);
        }

        /// <summary>
        /// Read stream of ssh
        /// </summary>
        UniRx.IObservable<string> SshReader() {
            return Observable.Create<string>(observer => {
                Thread.Sleep(300);
                using (StreamReader reader = new StreamReader(shellStream)) {
                    while (loop) {
                        // Wait end of stream
                        if (reader.EndOfStream) {
                            statusSubject.OnNext(ObservableSshStatus.EndOfStream);
                            continue;
                        }
                        // Read ssh stram
                        if (shellStream.CanRead && reader.Peek() != -1) {
                            try {
                                // Read ssh and notify to stdout subject
                                observableSSHBuffer.SetValueAndForceNotify(observableSSHBuffer.Value.UpdateFromReadStream(reader));
                            } catch (Exception ex) {
                                Debug.LogError(ex.Message + "\r\n" + ex.StackTrace);
                                observer.OnError(ex);
                                loop = false;
                            }
                        }
                    }
                }
                return Disposable.Create(() => { });
            })
            .SubscribeOn(Scheduler.ThreadPool);
        }

        /// <summary>
        /// Monitor connection status
        /// </summary>
        UniRx.IObservable<Unit> CheckConnectionStatus() {
            return Observable.Create<Unit>(observer => {
                Thread.Sleep(300);
                while (loop) {
                    Thread.Sleep(1000);
                }
                statusSubject.OnNext(ObservableSshStatus.End);
                statusSubject.OnCompleted();
                Dispose();
                return Disposable.Create(() => { });
            })
            .SubscribeOn(Scheduler.ThreadPool);
        }

        /// <summary>
        /// Send ssh command
        /// </summary>
        void SendCommand(string command) {
            try {
                stdInStreamWriter.WriteLine(commandPrefix + (commandPrefix.Trim().Length > 0 ? " " : "") + command);
            } catch (Exception ex) {
                Debug.LogErrorFormat("{0}\n{1}", ex.Message, ex.StackTrace);
                loop = false;
            }
        }

        /// <summary>
        /// Check available of ssh buffer
        /// </summary>
        public bool CheckBufferAvailable(Buffer buffer) {
            return buffer != null && buffer.count > 0;
        }

        /// <summary>
        /// Convert ssh buffer to stdout subject
        /// </summary>
        public void ConvertBufferToStdout(Buffer buffer) {
            while (true) {
                var newline = buffer.ReadLine();
                if (newline == null)
                    break;
                stdOutSubject.OnNext(newline);
            }
        }

        /// <summary>
        /// Check ssh read buffer
        /// </summary>
        public bool CheckBuffer(string checkWord) {
            if (observableSSHBuffer == null ||
                observableSSHBuffer.Value == null ||
                observableSSHBuffer.Value.buffer == null ||
                observableSSHBuffer.Value.count == 0)
                return false;

            var buffer = observableSSHBuffer.Value;

            var index = 0;
            var result = true;
            foreach (var c in checkWord.ToCharArray()) {
                if (c != buffer.buffer[buffer.count - checkWord.Length + index++]) {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Close connection and dispose resources
        /// </summary>
        public void Dispose() {
            loop = false;
            if (stdInStreamWriter != null) {
                stdInStreamWriter.Close();
                stdInStreamWriter.Dispose();
                Debug.Log("Dispost stdInStreamWriter");
            }
            if (stdOutSubject != null) {
                stdOutSubject.OnCompleted();
            }
            if (sshClient != null) {
                if (sshClient.IsConnected) {
                    sshClient.Disconnect();
                }
                sshClient.Dispose();
                Debug.Log("Dispost sshClient");
            }
            compositeDisposable.Dispose();
        }
    }
}