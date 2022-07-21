namespace ThirdParty.Factories
{
    using System.Collections.Generic;
    using ThirdParty.SDK.V2.PropertySearch;

    public interface IRoomRequestsFactory
    {
        List<RoomRequest> Create(string roomDetailsString);
    }
}