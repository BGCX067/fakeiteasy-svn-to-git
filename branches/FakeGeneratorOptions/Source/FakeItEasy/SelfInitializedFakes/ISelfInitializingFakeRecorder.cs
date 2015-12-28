﻿namespace FakeItEasy.SelfInitializedFakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Api;

    /// <summary>
    /// An interface for recorders that provides stored responses for self initializing fakes.
    /// </summary>
    public interface ISelfInitializingFakeRecorder
        : IDisposable 
    {
        /// <summary>
        /// Applies the call if the call has been recorded.
        /// </summary>
        /// <param name="call">The call to apply to from recording.</param>
        void ApplyNext(IWritableFakeObjectCall call);

        bool IsRecording { get; }
        
        /// <summary>
        /// Records the specified call.
        /// </summary>
        /// <param name="call">The call to record.</param>
        void RecordCall(ICompletedFakeObjectCall call);
    }
}
