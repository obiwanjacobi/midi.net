namespace CannedBytes.Midi.Components
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The MidiSenderChainManager class manages building a chain of sender components.
    /// </summary>
    /// <typeparam name="TSender">The common interface to all sender components.</typeparam>
    /// <typeparam name="TPort">The midi port type.</typeparam>
    public abstract class MidiSenderChainManager<TSender, TPort> : DisposableBase
        where TSender : class
        where TPort : class, IMidiPort
    {
        /// <summary>
        /// Constructs the manager with the specified <paramref name="sender"/>.
        /// </summary>
        /// <param name="sender">A sender component / midi port. Must not be null.</param>
        protected MidiSenderChainManager(TSender sender)
        {
            Check.IfArgumentNull(sender, nameof(sender));

            _sender = sender;
            MidiPort = sender as TPort;
        }

        /// <summary>
        /// Gets the current chain component.
        /// </summary>
        /// <remarks>Can return null if the component does not implement the
        /// <see cref="IChainOf&lt;T&gt;"/> interface.</remarks>
        public IChainOf<TSender> CurrentChain
        {
            get { return _sender as IChainOf<TSender>; }
        }

        /// <summary>
        /// Backing field for the <see cref="Sender"/> property.
        /// </summary>
        private TSender _sender;

        /// <summary>
        /// Gets the current (last) sender component.
        /// </summary>
        public TSender Sender
        {
            get
            {
                return _sender;
            }

            private set
            {
                ((IChainOf<TSender>)value).Successor = _sender;
                _sender = value;
            }
        }

        /// <summary>
        /// Adds the <paramref name="senderComponent"/> component to the chain.
        /// </summary>
        /// <param name="senderComponent">A sender chain component.</param>
        /// <remarks>The method throws an exception when the <paramref name="senderComponent"/>
        /// does not implement the <see cref="IChainOf&lt;T&gt;"/> interface.</remarks>
        public virtual void Add(TSender senderComponent)
        {
            Check.IfArgumentNull(senderComponent, nameof(senderComponent));
            Check.IfArgumentNotOfType<IChainOf<TSender>>(senderComponent, nameof(senderComponent));
            ThrowIfDisposed();

            Sender = senderComponent;
        }

        /// <summary>
        /// Initializes the sender chain components that implement the <see cref="IInitializeByMidiPort"/>.
        /// </summary>
        public virtual void Initialize()
        {
            if (MidiPort == null)
            {
                throw new InvalidOperationException("The Midi Port property was not initialized.");
            }

            foreach (var senderComponent in Senders)
            {
                if (senderComponent is IInitializeByMidiPort init)
                {
                    init.Initialize(MidiPort);
                }
            }
        }

        /// <summary>
        /// The Midi Port that was passed to constructor.
        /// </summary>
        protected TPort MidiPort { get; set; }

        /// <summary>
        /// Gets an enumerable object that enumerate the Senders T.
        /// </summary>
        public IEnumerable<TSender> Senders
        {
            get
            {
                var sender = Sender;

                while (sender != null)
                {
                    yield return sender;

                    if (sender is IChainOf<TSender> chain)
                    {
                        sender = chain.Successor;
                    }
                    else
                    {
                        sender = default;
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
                foreach (var senderComponent in Senders)
                {
                    if (MidiPort != null &&
                        senderComponent is IInitializeByMidiPort init)
                    {
                        init.Uninitialize(MidiPort);
                    }

                    if (senderComponent is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                _sender = null;
            }
        }
    }
}