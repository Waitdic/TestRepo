namespace iVectorOne.Interfaces
{
    using System.Collections.Generic;

    public interface IMultiSource
    {
        List<string> Sources { get; }
    }
}