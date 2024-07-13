namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// This entity returns one (or zero, in this case null will be returned) segment that is chosen
    /// from possible active segments by priority
    /// </summary>
    interface ISegmentsSelector
    {
        [CanBeNull]
        string GetActiveSegment(List<string> variants);
    }
}