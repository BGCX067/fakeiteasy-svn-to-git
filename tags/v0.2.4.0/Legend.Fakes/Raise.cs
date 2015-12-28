using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.Configuration;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes
{
    public static class Raise
    {
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        public static Raise<TEventArgs> With<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            return new Raise<TEventArgs>(sender, e);
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        public static Raise<TEventArgs> With<TEventArgs>(TEventArgs e) where TEventArgs : EventArgs
        {
            return new Raise<TEventArgs>(null, e);
        }
    }

    public class Raise<TEventArgs>
        : IEventRaiserArguments, IHideObjectMembers where TEventArgs : EventArgs 
    {
        #region Properties
        private object sender;
        private TEventArgs eventArguments;
        #endregion

        #region Methods
        internal Raise(object sender, TEventArgs e)
        {
            this.sender = sender;
            this.eventArguments = e;
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        public void Now(object sender, TEventArgs e)
        {
            throw new NotSupportedException(ExceptionMessages.NowCalledDirectly);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public EventHandler<TEventArgs> Go
        {
            get
            {
                return new EventHandler<TEventArgs>(Now);
            }
        }

        object IEventRaiserArguments.Sender
        {
            get { return this.sender; }
        }

        EventArgs IEventRaiserArguments.EventArguments
        {
            get { return this.eventArguments; }
        }
        #endregion
    }
}