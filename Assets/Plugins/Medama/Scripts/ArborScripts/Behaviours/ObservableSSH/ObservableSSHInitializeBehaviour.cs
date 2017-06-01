using System;
using UnityEngine;
using UniRx;
using Arbor;

namespace Medama.ObservableSsh
{
    public class ObservableSSHInitializeBehaviour : StateBehaviour
    {
        public ObservableSSHMonoBehaviour ssh = null;
        public string host = null;
        public string user = null;
        public string password = null;
        public int port = 22;
        public string waitstring = null;
        public StateLink nextState = null;
        public StateLink errorState = null;
        public bool dump = false;
        public CompositeDisposable compositeDisposable = null;

        public override void OnStateBegin() {

            try {
                if (dump)
                    Debug.Log("*** OnStateBegin : " + state.name + " ***");

                compositeDisposable = new CompositeDisposable();

                ssh = (ssh == null) ? gameObject.AddComponent<ObservableSSHMonoBehaviour>() : ssh;

                // check property
                if (ssh == null) {
                    throw new Exception("ssh is not set.");
                }
                if (string.IsNullOrEmpty(host)) {
                    throw new Exception("host is not set.");
                }
                if (string.IsNullOrEmpty(user)) {
                    throw new Exception("user is not set.");
                }
                if (string.IsNullOrEmpty(password)) {
                    throw new Exception("password is not set.");
                }

                // initialize ssh
                ssh.InitializeAndStart(host: host, user: user, password: password, port: port);

                // monitor ssh status and send command
                ssh
                    .ssh
                    .statusSubject
                    .DistinctUntilChanged()
                    .Subscribe(CheckStatus)
                    .AddTo(compositeDisposable);

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
                // end of ssh stream
                case ObservableSshStatus.EndOfStream:
                    if (ssh.CheckBuffer(waitstring)) {
                        if (nextState != null && Transition(nextState) && dump) {
                            Debug.Log("*** Transition : " + state.name + " ***");
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Arbor2 exit state
        /// </summary>
        public override void OnStateEnd() {
            if (dump)
                Debug.Log("*** OnStateEnd : " + state.name + " ***");
            compositeDisposable.Dispose();
            compositeDisposable = null;
        }

    }
}