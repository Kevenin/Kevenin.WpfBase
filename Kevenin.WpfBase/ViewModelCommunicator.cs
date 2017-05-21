using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevenin.WpfBase
{
    public enum ViewModelMessages
    {
        //TODO make ViewModelMessages Generic
    };

    public class ViewModelCommunicator
    {
        public delegate void RegisterHandler(VMCommunicatorEventArgs v);

        public static event RegisterHandler Register = delegate { };

        public static void NotifyColleagues(ViewModelMessages message, object o = null)
        {
            OnNotifyChanged(message, o);
        }
        private static void OnNotifyChanged(ViewModelMessages message, object o)
        {
            if (Register != null)
                Register(new VMCommunicatorEventArgs(message, o));
        }
    }

    public class VMCommunicatorEventArgs : EventArgs
    {
        private ViewModelMessages message;
        private object obj;

        public VMCommunicatorEventArgs(ViewModelMessages msg, object o)
        {
            this.message = msg;
            this.obj = o;
        }

        public ViewModelMessages Message
        {
            get { return message; }
        }

        public object Obj
        {
            get { return obj; }
        }
    }
}
