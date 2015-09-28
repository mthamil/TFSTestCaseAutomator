using System;
using System.Diagnostics;
using System.Windows.Input;
using SharpEssentials.Controls.Mvvm;

namespace TestCaseAutomator.ViewModels
{
    [DebuggerDisplay("{" + nameof(Uri) + "}")]
    public class ServerViewModel : ViewModelBase, IServer
    {
        public ServerViewModel(Uri uri, ICommand forget)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (forget == null)
                throw new ArgumentNullException(nameof(forget));

            Uri = uri;
            Forget = forget;
        }

        /// <summary>
        /// A server's URI.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Command that removes a server URI from the server list.
        /// </summary>
        public ICommand Forget { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Uri.Equals(((ServerViewModel)obj).Uri);
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }
    }
}