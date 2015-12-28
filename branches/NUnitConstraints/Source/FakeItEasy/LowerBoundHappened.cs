namespace FakeItEasy
{
    /// <summary>
    /// Provides syntax for specifying the number repeat when asserting on 
    /// fake object calls.
    /// </summary>
    public class LowerBoundHappened
        : Happened
    {
        private int expectedRepeat;

        /// <summary>
        /// Initializes a new instance of the <see cref="HappenedNoUpperBound"/> class.
        /// </summary>
        /// <param name="expectedRepeat">The expected repeat.</param>
        internal LowerBoundHappened(int expectedRepeat)
        {
            this.expectedRepeat = expectedRepeat;
        }

        /// <summary>
        /// Restricts the repeat specification so that the call must have
        /// happened exactly the number of times previously specified, no more
        /// and no less.
        /// </summary>
        public Happened Exactly
        {
            get
            {
                return new HappenedNTimesExactly() { Parent = this };
            }
        }

        /// <summary>
        /// Restricts the repeat specification so that the call must have happened
        /// no more than the times previously specified.
        /// </summary>
        public Happened OrLess
        {
            get
            {
                return new HappenedNTimesOrLess() { Parent = this };
            }
        }

        /// <summary>
        /// Gets whether the specified repeat is the expected repeat or higher.
        /// </summary>
        /// <param name="repeat">The repeat of a call.</param>
        /// <returns>True if the repeat is a match.</returns>
        internal override bool Matches(int repeat)
        {
            return repeat >= this.expectedRepeat;
        }

        /// <summary>
        /// Gets a description of the repeat.
        /// </summary>
        /// <returns>A description of the repeat.</returns>
        public override string ToString()
        {
            if (this.expectedRepeat < 3)
            {
                return this.GetDescriptionForRepeatThatsLessThanThree();
            }

            return "#{0} times".FormatInvariant(this.expectedRepeat);
        }

        private string GetDescriptionForRepeatThatsLessThanThree()
        {
            if (this.expectedRepeat == 1)
            {
                return "once";
            }

            if (this.expectedRepeat == 2)
            {
                return "twice";
            }

            return "never";
        }


        private class HappenedNTimesExactly
            : Happened
        {
            public LowerBoundHappened Parent;

            internal override bool Matches(int repeat)
            {
                return this.Parent.expectedRepeat == repeat;
            }

            public override string ToString()
            {
                return "exactly {0}".FormatInvariant(this.Parent.ToString());
            }
        }

        private class HappenedNTimesOrLess
            : Happened
        {
            public LowerBoundHappened Parent;

            internal override bool Matches(int repeat)
            {
                return repeat <= this.Parent.expectedRepeat;
            }

            public override string ToString()
            {
                return "{0} or less".FormatInvariant(this.Parent.ToString());
            }
        }
    }
}
