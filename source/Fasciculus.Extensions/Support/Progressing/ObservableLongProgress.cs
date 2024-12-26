namespace Fasciculus.Support.Progressing
{
    /// <summary>
    /// Accumulating progress of type <see cref="long"/>
    /// </summary>
    public partial class ObservableLongProgress : ObservableAccumulatingProgress<long>
    {
        /// <summary>
        /// Initializes a new accumulating progress.
        /// </summary>
        public ObservableLongProgress()
            : base(null, 0) { }

        /// <summary>
        /// Accumulates the new value.
        /// </summary>
        protected override long Accumulate(long value)
            => Current + value;
    }
}
