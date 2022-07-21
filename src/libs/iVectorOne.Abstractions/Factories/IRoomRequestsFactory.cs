namespace iVectorOne.Factories
{
    using System.Collections.Generic;
    using iVectorOne.SDK.V2.PropertySearch;

    public interface IRoomRequestsFactory
    {
        List<RoomRequest> Create(string roomDetailsString);
    }
}