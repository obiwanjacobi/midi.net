namespace CannedBytes.Midi.Components
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
            Contract.Requires(sender != null);
            Check.IfArgumentNull(sender, "sender");

            this.sender = sender;
            this.MidiPort = sender as TPort;
        }

        /// <summary>
        /// Gets the current chain component.
        /// </summary>
        /// <remarks>Can return null if the component does not implement the
        /// <see cref="IChainOf&lt;T&gt;"/> interface.</remarks>
        public IChainOf<TSender> CurrentChain
        {
            get { return this.sender as IChainOf<TSender>; }
        }

        /// <summary>
        /// Backing field for the <see cref="Sender"/> property.
        /// </summary>
        private TSender sender;

        /// <summary>
        /// Gets the current (last) sender component.
        /// </summary>
        public TSender Sender
        {
            get
            {
                return this.sender;
            }

            private set
            {
                ((IChainOf<TSender>)value).Successor = this.sender;
                this.sender = value;
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
            Contract.Requires(senderComponent != null);
            Contract.Requires(senderComponent is IChainOf<TSender>);
            Check.IfArgumentNull(senderComponent, "sender");
            Check.IfArgumentNotOfType<IChainOf<TSender>>(senderComponent, "sender");
            ThrowIfDisposed();

            this.Sender = senderComponent;
        }

        /// <summary>
        /// Initializes the sender chain components that implement the <see cref="IInitializeByMidiPort"/>.
        /// </summary>
        public virtual void Initialize()
        {
            if (this.MidiPort == null)
            {
                throw new InvalidOperationException("The Midi Port property was not initialized.");
            }

            foreach (var senderComponent in this.Senders)
            {
                IInitializeByMidiPort init = senderComponent as IInitializeByMidiPort;

                if (init != null)
                {
                    init.Initialize(this.MidiPort);
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
                TSender sender = this.Sender;

                while (sender != null)
                {
                    yield return sender;

                    IChainOf<TSender> chain = sender as IChainOf<TSender>;

                    if (chain != null)
                    {
                        sender = chain.Successor;
                    }
                    else
                    {
                        sender = default(TSender);
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
                    foreach (var senderComponent in this.Senders)
                    {
                        if (this.MidiPort != null)
                        {
                            IInitializeByMidiPort init = senderComponent as IInitializeByMidiPort;

                            if (init != null)
                            {
                                init.Uninitialize(this.MidiPort);
                            }
                        }

                        IDisposable disposable = senderComponent as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    this.sender = null;
                }
            }
        }
    }
}