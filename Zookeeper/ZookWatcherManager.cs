using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper watcher manager
    /// </summary>
    public sealed class ZookWatcherManager
    {
        #region Private Members
        /// <summary>
        /// key:client path
        /// </summary>
        private readonly Dictionary<string, HashSet<IWatcher>> _dataWatches =
            new Dictionary<string, HashSet<IWatcher>>();
        /// <summary>
        /// key:client path
        /// </summary>
        private readonly Dictionary<string, HashSet<IWatcher>> _existWatches =
            new Dictionary<string, HashSet<IWatcher>>();
        /// <summary>
        /// key:client path
        /// </summary>
        private readonly Dictionary<string, HashSet<IWatcher>> _childWatches =
            new Dictionary<string, HashSet<IWatcher>>();
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="defaultWatcher"></param>
        public ZookWatcherManager(IWatcher defaultWatcher)
        {
            this.DefaultWatcher = defaultWatcher;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// get default watcher
        /// </summary>
        public IWatcher DefaultWatcher
        {
            get;
            internal set;
        }
        #endregion

        #region Watcher
        /// <summary>
        /// register watcher
        /// </summary>
        /// <param name="container"></param>
        /// <param name="watcher"></param>
        /// <param name="clientPath"></param>
        /// <exception cref="ArgumentNullException">container is null</exception>
        private void RegisterWatcher(Dictionary<string, HashSet<IWatcher>> container, IWatcher watcher, string clientPath)
        {
            if (container == null) throw new ArgumentNullException("container");

            lock (container)
            {
                HashSet<IWatcher> set = null;
                if (!container.TryGetValue(clientPath, out set))
                {
                    set = new HashSet<IWatcher>();
                    container[clientPath] = set;
                }
                set.Add(watcher);
            }
        }
        /// <summary>
        /// register data watcher
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="clientPath"></param>
        public void RegisterDataWatcher(IWatcher watcher, string clientPath)
        {
            this.RegisterWatcher(this._dataWatches, watcher, clientPath);
        }
        /// <summary>
        /// register exist watcher
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="clientPath"></param>
        public void RegisterExistWatcher(IWatcher watcher, string clientPath)
        {
            this.RegisterWatcher(this._existWatches, watcher, clientPath);
        }
        /// <summary>
        /// register child watcher
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="clientPath"></param>
        public void RegisterChildWatcher(IWatcher watcher, string clientPath)
        {
            this.RegisterWatcher(this._childWatches, watcher, clientPath);
        }

        /// <summary>
        /// try remove and return <see cref="IWatcher"/> set by clientPath.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="clientPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">container is null</exception>
        private HashSet<IWatcher> TryRemoveWatcher(Dictionary<string, HashSet<IWatcher>> container, string clientPath)
        {
            if (container == null) throw new ArgumentNullException("container");

            lock (this)
            {
                HashSet<IWatcher> set = null;
                if (container.TryGetValue(clientPath, out set))
                {
                    container.Remove(clientPath);
                    return set;
                }
                return null;
            }
        }
        /// <summary>
        /// try remove and return data <see cref="IWatcher"/> set by clientPath.
        /// </summary>
        /// <param name="clientPath"></param>
        /// <returns></returns>
        public HashSet<IWatcher> TryRemoveDataWatcher(string clientPath)
        {
            return this.TryRemoveWatcher(this._dataWatches, clientPath);
        }
        /// <summary>
        /// try remove and return exist <see cref="IWatcher"/> set by clientPath.
        /// </summary>
        /// <param name="clientPath"></param>
        /// <returns></returns>
        public HashSet<IWatcher> TryRemoveExistWatcher(string clientPath)
        {
            return this.TryRemoveWatcher(this._existWatches, clientPath);
        }
        /// <summary>
        /// try remove and return child <see cref="IWatcher"/> set by clientPath.
        /// </summary>
        /// <param name="clientPath"></param>
        /// <returns></returns>
        public HashSet<IWatcher> TryRemoveChildWatcher(string clientPath)
        {
            return this.TryRemoveWatcher(this._childWatches, clientPath);
        }

        /// <summary>
        /// get data watch keys
        /// </summary>
        /// <returns></returns>
        public string[] GetDataWatchKeys()
        {
            lock (this._dataWatches) return this._dataWatches.Keys.ToArray();
        }
        /// <summary>
        /// get exist watch keys
        /// </summary>
        /// <returns></returns>
        public string[] GetExistWatchKeys()
        {
            lock (this._existWatches) return this._existWatches.Keys.ToArray();
        }
        /// <summary>
        /// get child watch keys
        /// </summary>
        /// <returns></returns>
        public string[] GetChildWatchKeys()
        {
            lock (this._childWatches) return this._childWatches.Keys.ToArray();
        }
        #endregion

        #region Invoke
        /// <summary>
        /// invoke
        /// </summary>
        /// <param name="wevent"></param>
        /// <exception cref="ArgumentNullException">wevent is null.</exception>
        public void Invoke(Data.WatchedEvent wevent)
        {
            if (wevent == null) throw new ArgumentNullException("wevent");

            var watcherSet = new HashSet<IWatcher>();
            switch (wevent.Type)
            {
                case Data.EventType.NodeCreated:
                case Data.EventType.NodeDataChanged:
                    this.CopyTo(this.TryRemoveDataWatcher(wevent.Path), watcherSet);
                    this.CopyTo(this.TryRemoveExistWatcher(wevent.Path), watcherSet);
                    break;
                case Data.EventType.NodeChildrenChanged:
                    this.CopyTo(this.TryRemoveChildWatcher(wevent.Path), watcherSet);
                    break;
                case Data.EventType.NodeDeleted:
                    this.CopyTo(this.TryRemoveDataWatcher(wevent.Path), watcherSet);
                    this.TryRemoveExistWatcher(wevent.Path);
                    this.CopyTo(this.TryRemoveChildWatcher(wevent.Path), watcherSet);
                    break;
            }

            if (watcherSet.Count > 0)
            {
                foreach (var childWatcher in watcherSet)
                {
                    var watcher = childWatcher;
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try { watcher.Invoke(wevent); }
                        catch (Exception ex) { Sodao.FastSocket.SocketBase.Log.Trace.Error(ex.Message, ex); }
                    });
                }
            }
        }
        /// <summary>
        /// copy to
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="ArgumentNullException">to is null</exception>
        private void CopyTo(HashSet<IWatcher> from, HashSet<IWatcher> to)
        {
            if (to == null) throw new ArgumentNullException("to");
            if (from == null) return;
            to.UnionWith(from);
        }
        #endregion
    }
}