namespace Unibrics.Configuration.General
{
    using System.Collections.Generic;

    public interface IActiveSegmentsProvider
    {
        IEnumerable<(string segment, string description)> GetAllActiveSegments();
    }
}