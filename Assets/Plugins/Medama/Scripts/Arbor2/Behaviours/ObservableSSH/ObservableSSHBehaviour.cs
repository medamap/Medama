#if MEDAMA_USE_UNIRX && MEDAMA_USE_OBSERVABLE_SSH && MEDAMA_USE_ARBOR2
using System;
using UnityEngine;
using Arbor;
using UniRx;
using Medama.ObservableSsh;

namespace Medama.Arbor2
{
    public class ObservableSSHBehaviour : StateBehaviour
    {
        public ObservableSSHMonoBehaviour ssh = null;
        public string command = null;
        public string preWait = null;
        public string postWait = null;
        public StateLink nextState = null;
        public StateLink errorState = null;
        public bool dump = false;
        public bool setEnv = false;

        bool first = true;
        bool next = false;
        public CompositeDisposable compositeDisposable = null;

        public override void OnStateBegin() {

            try {
                if (dump)
                    Debug.Log("*** OnStateBegin : " + state.name + " ***");

                compositeDisposable = new CompositeDisposable();

                ssh = (ssh == null) ? GetComponent<ObservableSSHMonoBehaviour>() : ssh;

                // check ssh connection
                if (ssh == null || !ssh.con.IsAuthenticated) {
                    throw new Exception("ssh is not set or connection is not open.");
                }

                // check command string
                if ((preWait.Trim().Length < 1 && postWait.Trim().Length < 1) || command.Trim().Length < 1) {
                    throw new Exception("invalid command or invalid waitstring.");
                }

                // monitor ssh stdout
                ssh
                    .ssh
                    .stdOutSubject
                    .Where(_ => dump)
                    .Subscribe(l => Debug.Log(l))
                    .AddTo(compositeDisposable);

                // monitor ssh status and send command
                ssh
                    .ssh
                    .statusSubject
                    //.DistinctUntilChanged()
                    .Subscribe(CheckStatus)
                    .AddTo(compositeDisposable);

                // post wait only
                if (preWait.Trim().Length < 1) {
                    if (!setEnv) {
                        var backup = ssh.commandPrefix;
                        ssh.commandPrefix = "";
                        ssh.writeSshSubject.OnNext(command);
                        ssh.commandPrefix = backup;
                    } else {
                        ssh.writeSshSubject.OnNext(command);
                    }
                    first = false;
                }

            } catch (Exception ex) {
                Debug.LogWarning(ex);
                if (errorState != null && Transition(errorState) && dump) {
                    Debug.Log("*** Transition : " + state.name + " ***");
                    return;
                }
            }
        }

        /// <summary>
        /// monitor ssh status and send command
        /// </summary>
        /// <param name="status"></param>
        void CheckStatus(ObservableSshStatus status) {
            switch (status) {
                // monitor ssh status and send command
                case ObservableSshStatus.EndOfStream:
                    if (first) {
                        if (ssh.CheckBuffer(preWait)) {
                            if (!setEnv) {
                                var backup = ssh.commandPrefix;
                                ssh.commandPrefix = "";
                                ssh.writeSshSubject.OnNext(command);
                                ssh.commandPrefix = backup;
                            } else {
                                ssh.writeSshSubject.OnNext(command);
                            }
                            first = false;
                            next = (postWait.Trim().Length < 1);
                        }
                    } else {
                        if (postWait.Trim().Length > 0 && ssh.CheckBuffer(postWait)) {
                            next = true;
                        }
                    }
                    if (next && nextState != null && Transition(nextState)) {
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

        // Use this for exit state
        public override void OnStateEnd() {
            if (dump)
                Debug.Log("*** OnStateEnd : " + state.name + " ***");
            compositeDisposable.Dispose();
            compositeDisposable = null;
        }
    }
}
#endif
