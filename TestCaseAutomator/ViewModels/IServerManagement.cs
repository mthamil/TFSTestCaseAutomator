using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TestCaseAutomator.ViewModels
{
    public interface IServerManagement : INotifyPropertyChanged
    {
        void Add(Uri server);

        Uri CurrentUri { get; set; }

        IServer Selected { get; set; }

        /// <summary>
        /// All known servers.
        /// </summary>
        IList<IServer> All { get; }
    }
}