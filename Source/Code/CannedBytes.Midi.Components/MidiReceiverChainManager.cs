using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverChainManager class manages midi receiver chain components.
    /// </summary>
    /// <typeparam name="T">The interface type that is common to all chain components.</typeparam>
    public class MidiReceiverChainManager<T> : DisposableBase
        where T : class
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        protected MidiReceiverChainManager()
        { }

        /// <summary>
        /// Constructs a new instance using the <paramref name="rooChain"/> as root chain component.
        /// </summary>
        /// <param name="rootChain">A reference to a chain component. Must not be null.</param>
        public MidiReceiverChainManager(IChainOf<T> rootChain)
        {
            Contract.Requires(rootChain != null);
            Throw.IfArgumentNull(rootChain, "rootChain");

            RootChain = rootChain;
        }

        private IChainOf<T> root;

        /// <summary>
        /// Gets the Root chain component.
        /// </summary>
        /// <remarks>Derived classes can also set this property.</remarks>
        public IChainOf<T> RootChain
        {
            get { return this.root; }
            protected set
            {
                this.root = value;
                this.receiver = null;
            }
        }

        /// <summary>
        /// Gets the last <see cref="IMidiReceiverChain&lt;T&gt;"/> implementation
        /// of the most recently added chain component.
        /// </summary>
        /// <remarks>If this property is null, it indicates the end of the chain, for
        /// no new chain components can be hooked up onto the last added chain component.</remarks>
        public IChainOf<T> CurrentChain
        {
            get
            {
                if (this.receiver == null)
                {
                    return this.root;
                }

                return this.receiver as IChainOf<T>;
            }
        }

        private T receiver;

        /// <summary>
        /// Gets the last added chain component.
        /// </summary>
        public T Receiver
        {
            get { return this.receiver; }
            private set
            {
                CurrentChain.Next = value;
                this.receiver = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the end of the chain (true).
        /// </summary>
        public bool EndOfChain
        {
            get { return (CurrentChain == null); }
        }

        /// <summary>
        /// Adds the specified <paramref name="receiver"/> to the end of the chain.
        /// </summary>
        /// <param name="receiver">The chain component. Must not be null.</param>
        /// <remarks>If the specified <paramref name="receiver"/> does not implement
        /// the <see cref="IMidiReceiverChain&lt;T&gt;"/> interface no more components can be added.</remarks>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="EndOfChain"/>
        /// property return true.</exception>
        public virtual void Add(T receiver)
        {
            Contract.Requires(receiver != null);
            Throw.IfArgumentNull(receiver, "receiver");
            ThrowIfDisposed();
            if (EndOfChain)
            {
                throw new InvalidOperationException(
                    Properties.Resources.MidiReceiverChainManager_EndOfChain);
            }

            Receiver = receiver;
        }

        /// <summary>
        /// Initializes all chain components that implement the <see cref="T:IInitializeByMidiPort"/>
        /// interface with the specified <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi Port used for initialization. Must not be null.</param>
        public virtual void InitializeByMidiPort(IMidiPort port)
        {
            Throw.IfArgumentNull(port, "port");
            ThrowIfDisposed();

            MidiPort = port;

            // initialize all receivers that implement IInitializeByMidiPort
            foreach (var receiver in Receivers)
            {
                IInitializeByMidiPort init = receiver as IInitializeByMidiPort;

                if (init != null)
                {
                    init.Initialize(port);
                }
            }
        }

        /// <summary>
        /// The Midi Port that was passed to <see cref="Initialize"/>.
        /// </summary>
        protected IMidiPort MidiPort { get; private set; }

        /// <summary>
        /// Gets an enumerable object for enumerating all the receivers T.
        /// </summary>
        public IEnumerable<T> Receivers
        {
            get
            {
                IChainOf<T> chain = RootChain;

                if (chain != null)
                {
                    T receiver = chain.Next;

                    while (chain != null && receiver != null)
                    {
                        yield return receiver;

                        chain = receiver as IChainOf<T>;

                        if (chain != null)
                        {
                            receiver = chain.Next;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disposes all components in the chain.
        /// </summary>
        /// <param name="disposing">True when called from the <see cref="Dispose"/> method,
        /// false when called from the Finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed)
                {
                    if (disposing)
                    {
                        foreach (var receiver in Receivers)
                        {
                            if (MidiPort != null)
                            {
                                IInitializeByMidiPort init = receiver as IInitializeByMidiPort;

                                if (init != null)
                                {
                                    init.Uninitialize(MidiPort);
                                }
                            }

                            IDisposable disposable = receiver as IDisposable;

                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }

                        IDisposable disposableChain = RootChain as IDisposable;

                        // clears RootChain, CurrentChain and Receiver
                        RootChain = null;

                        if (disposableChain != null)
                        {
                            disposableChain.Dispose();
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}