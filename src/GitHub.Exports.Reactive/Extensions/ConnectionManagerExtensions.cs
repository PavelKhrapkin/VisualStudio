﻿using System.Reactive.Linq;
using GitHub.Models;
using System;
using ReactiveUI;
using GitHub.Primitives;
using System.Linq;

namespace GitHub.Extensions
{
    public static class ConnectionManagerExtensions
    {
        public static IObservable<bool> IsLoggedIn(this IConnectionManager cm, IRepositoryHosts hosts)
        {
            Guard.ArgumentNotNull(hosts, nameof(hosts));
            return cm.Connections.ToObservable()
                    .SelectMany(c => c.Login())
                    .Any(c => hosts.LookupHost(c.HostAddress).IsLoggedIn);
        }

        public static IObservable<bool> IsLoggedIn(this IConnectionManager cm, IRepositoryHosts hosts, HostAddress address)
        {
            Guard.ArgumentNotNull(hosts, nameof(hosts));
            Guard.ArgumentNotNull(address, nameof(address));
            return cm.Connections.ToObservable()
                    .Where(c => c.HostAddress.Equals(address))
                    .SelectMany(c => c.Login())
                    .Any(c => hosts.LookupHost(c.HostAddress).IsLoggedIn);
        }

        public static IObservable<bool> IsLoggedIn(this IConnection connection, IRepositoryHosts hosts)
        {
            Guard.ArgumentNotNull(hosts, nameof(hosts));
            return connection?.Login().Any(c => hosts.LookupHost(c.HostAddress).IsLoggedIn) ?? Observable.Return(false);
        }

        public static IObservable<IConnection> GetLoggedInConnections(this IConnectionManager cm, IRepositoryHosts hosts)
        {
            Guard.ArgumentNotNull(hosts, nameof(hosts));
            return cm.Connections.ToObservable()
                    .SelectMany(c => c.Login())
                    .Where(c => hosts.LookupHost(c.HostAddress).IsLoggedIn);
        }

        public static IObservable<IConnection> LookupConnection(this IConnectionManager cm, ISimpleRepositoryModel repository)
        {
            return Observable.Return(repository?.CloneUrl != null
                ? cm.Connections.FirstOrDefault(c => c.HostAddress.Equals(HostAddress.Create(repository.CloneUrl)))
                : null);
        }
    }
}
