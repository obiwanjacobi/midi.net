namespace CannedBytes.Midi.Components
{
    using System;
    using System.Collections.Generic;

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
            Check.IfArgumentNull(port, nameof(port));

            RootChain = port as IChainOf<TReceiver>;
            MidiPort = port;
        }

        /// <summary>
        /// Constructs a new instance using the <paramref name="rootChain"/> as root chain component.
        /// </summary>
        /// <param name="rootChain">A reference to a chain component. Must not be null.</param>
        protected MidiReceiverChainManager(IChainOf<TReceiver> rootChain)
        {
            Check.IfArgumentNull(rootChain, nameof(rootChain));

            RootChain = rootChain;
            MidiPort = rootChain as TPort;
        }

        /// <summary>
        /// Backing field for the <see cref="RootChain"/> property.
        /// </summary>
        private IChainOf<TReceiver> _root;

        /// <summary>
        /// Gets the Root chain component.
        /// </summary>
        /// <remarks>Derived classes can also set this property.</remarks>
        public IChainOf<TReceiver> RootChain
        {
            get
            {
                return _root;
            }

            protected set
            {
                _root = value;
                _receiver = null;
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
                if (_receiver == null)
                {
                    return _root;
                }

                return _receiver as IChainOf<TReceiver>;
            }
        }

        /// <summary>
        /// Backing field for the <see cref="Receiver"/> property.
        /// </summary>
        private TReceiver _receiver;

        /// <summary>
        /// Gets the last added chain component.
        /// </summary>
        public TReceiver Receiver
        {
            get
            {
                return _receiver;
            }

            private set
            {
                CurrentChain.Successor = value;
                _receiver = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the end of the chain (true).
        /// </summary>
        public bool EndOfChain
        {
            get { return CurrentChain == null; }
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
            Check.IfArgumentNull(receiverComponent, nameof(_receiver));
            ThrowIfDisposed();
            if (EndOfChain)
            {
                throw new InvalidOperationException(
                    Properties.Resources.MidiReceiverChainManager_EndOfChain);
            }

            Receiver = receiverComponent;
        }

        /// <summary>
        /// Initializes all chain components that implement the <see cref="T:IInitializeByMidiPort"/>.
        /// </summary>
        public virtual void Initialize()
        {
            ThrowIfDisposed();

            // initialize all receivers that implement IInitializeByMidiPort
            foreach (var receiverComponent in Receivers)
            {
                if (receiverComponent is IInitializeByMidiPort init)
                {
                    init.Initialize(MidiPort);
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
                IChainOf<TReceiver> chain = RootChain;

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
            if (!IsDisposed &&
                disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                foreach (var receiverComponent in Receivers)
                {
                    if (MidiPort != null &&
                        receiverComponent is IInitializeByMidiPort init)
                    {
                        init.Uninitialize(MidiPort);
                    }


                    if (receiverComponent is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                var disposableChain = RootChain as IDisposable;

                // clears RootChain, CurrentChain and Receiver
                RootChain = null;

                if (disposableChain != null)
                {
                    disposableChain.Dispose();
                }
            }
        }
    }
}