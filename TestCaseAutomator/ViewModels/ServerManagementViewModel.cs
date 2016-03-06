using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SharpEssentials.Collections;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Net;
using SharpEssentials.Observable;

namespace TestCaseAutomator.ViewModels
{
    public class ServerManagementViewModel : ViewModelBase, IServerManagement
    {
        public ServerManagementViewModel(IEnumerable<Uri> knownUris) : this()
        {
            knownUris.Select(Create).ToSink(All);
            CurrentUri = All.Select(s => s.Uri).FirstOrDefault();
        }

        private ServerManagementViewModel()
        {
            _currentUri = Property.New(this, p => p.CurrentUri);
            _selectedServer = Property.New(this, p => p.Selected);
        }

        public void Add(Uri server)
        {
            var existing = All.FirstOrDefault(s => UriEqualityComparer.Instance.Equals(s.Uri, server));
            if (existing != null)
            {
                All.Remove(existing);
                All.Insert(0, existing);
                CurrentUri = existing.Uri;
            }
            else
            {
                All.Insert(0, Create(server));
            }
        }

        public Uri CurrentUri
        {
            get { return _currentUri.Value; }
            set { _currentUri.Value = value; }
        }

        public IServer Selected
        {
            get { return _selectedServer.Value; }
            set { _selectedServer.Value = value; }
        }

        /// <summary>
        /// All known servers.
        /// </summary>
        public IList<IServer> All { get; } = new ObservableCollection<IServer>();

        private IServer Create(Uri server)
        {
            IServer newItem = null;
            newItem = new ServerViewModel(server, new RelayCommand(() => All.Remove(newItem)));
            return newItem;
        }

        private readonly Property<IServer> _selectedServer;
        private readonly Property<Uri> _currentUri;
    }
}