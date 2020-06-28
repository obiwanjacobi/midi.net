namespace CannedBytes.Midi.Components
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The MidiReceiverChainManager class manages midi receiver chain components.
    /// </summary>
    /// <typeparam name="TReceiver">The interface type that is common to all chain components.</typeparam>
    /// <typeparam name="TPort">The midi port type.</typeparam>
    public abstract class MidiReceiverChainManager<TReceiver, TPort> : DisposableBase
        where TReceiver : class
        where TPort : class, IMidiPort
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        /// <param name="port">The midi port and root of the chain. Must not be null.</param>
        protected MidiReceiverChainManager(TPort port)
        {
            Check.IfArgumentNull(port, "port");

            this.RootChain = port as IChainOf<TReceiver>;
            this.MidiPort = port;
        }

        /// <summary>
        /// Constructs a new instance using the <paramref name="rootChain"/> as root chain component.
        /// </summary>
        /// <param name="rootChain">A reference to a chain component. Must not be null.</param>
        protected MidiReceiverChainManager(IChainOf<TReceiver> rootChain)
        {
            Contract.Requires(rootChain != null);
            Check.IfArgumentNull(rootChain, "rootChain");

            this.RootChain = rootChain;
            this.MidiPort = rootChain as TPort;
        }

        /// <summary>
        /// Backing field for the <see cref="RootChain"/> property.
        /// </summary>
        private IChainOf<TReceiver> root;

        /// <summary>
        /// Gets the Root chain component.
        /// </summary>
        /// <remarks>Derived classes can also set this property.</remarks>
        public IChainOf<TReceiver> RootChain
        {
            get
            {
                return this.root;
            }

            protected set
            {
                this.root = value;
                this.receiver = null;
            }
        }

        /// <summary>
        /// Gets the last <see cref="IChainOf&lt;ReceiverT&gt;"/> implementation
        /// of the most recently added chain component.
        /// </summary>
        /// <remarks>If this property is null, it indicates the end of the chain, for
        /// no new chain components can be hooked up onto the last added chain component.</remarks>
        public IChainOf<TReceiver> CurrentChain
        {
            get
            {
                if (this.receiver == null)
                {
                    return this.root;
                }

                return this.receiver as IChainOf<TReceiver>;
            }
        }

        /// <summary>
        /// Backing field for the <see cref="Receiver"/> property.
        /// </summary>
        private TReceiver receiver;

        /// <summary>
        /// Gets the last added chain component.
        /// </summary>
        public TReceiver Receiver
        {
            get
            {
                return this.receiver;
            }

            private set
            {
                this.CurrentChain.Successor = value;
                this.receiver = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the end of the chain (true).
        /// </summary>
        public bool EndOfChain
        {
            get { return this.CurrentChain == null; }
        }

        /// <summary>
        /// Adds the specified <paramref name="receiverComponent"/> to the end of the chain.
        /// </summary>
        /// <param name="receiverComponent">The chain component. Must not be null.</param>
        /// <remarks>If the specified <paramref name="receiverComponent"/> does not implement
        /// the <see cref="IChainOf&lt;T&gt;"/> interface no more components can be added.</remarks>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="EndOfChain"/>
        /// property return true.</exception>
        public virtual void Add(TReceiver receiverComponent)
        {
            Contract.Requires(receiverComponent != null);
            Check.IfArgumentNull(receiverComponent, "receiver");
            ThrowIfDisposed();
            if (this.EndOfChain)
            {
                throw new InvalidOperationException(
                    Properties.Resources.MidiReceiverChainManager_EndOfChain);
            }

            this.Receiver = receiverComponent;
        }

        /// <summary>
        /// Initializes all chain components that implement the <see cref="T:IInitializeByMidiPort"/>.
        /// </summary>
        public virtual void Initialize()
        {
            ThrowIfDisposed();

            // initialize all receivers that implement IInitializeByMidiPort
            foreach (var receiverComponent in this.Receivers)
            {
                IInitializeByMidiPort init = receiverComponent as IInitializeByMidiPort;

                if (init != null)
                {
                    init.Initialize(this.MidiPort);
                }
            }
        }

        /// <summary>
        /// Gets the Midi Port that was passed to constructor.
        /// </summary>
        protected TPort MidiPort { get; private set; }

        /// <summary>
        /// Gets an enumerable object for enumerating all the receivers T.
        /// </summary>
        public IEnumerable<TReceiver> Receivers
        {
            get
            {
                IChainOf<TReceiver> chain = this.RootChain;

                if (chain != null)
                {
                    TReceiver receiver = chain.Successor;

                    while (chain != null && receiver != null)
                    {
                        yield return receiver;

                        chain = receiver as IChainOf<TReceiver>;

                        if (chain != null)
                        {
                            receiver = chain.Successor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disposes all components in the chain.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (!IsDisposed)
            {
                if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
                {
                    foreach (var receiverComponent in this.Receivers)
                    {
                        if (this.MidiPort != null)
                        {
                            IInitializeByMidiPort init = receiverComponent as IInitializeByMidiPort;

                            if (init != null)
                            {
                                init.Uninitialize(this.MidiPort);
                            }
                        }

                        IDisposable disposable = receiverComponent as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    IDisposable disposableChain = this.RootChain as IDisposable;

                    // clears RootChain, CurrentChain and Receiver
                    this.RootChain = null;

                    if (disposableChain != null)
                    {
                        disposableChain.Dispose();
                    }
                }
            }
        }
    }
}