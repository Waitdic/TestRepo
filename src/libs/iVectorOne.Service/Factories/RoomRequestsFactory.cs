namespace iVectorOne.Factories
{
    using System.Collections.Generic;
    using System.Linq;
    using iVectorOne.SDK.V2.PropertySearch;

    public class RoomRequestsFactory : IRoomRequestsFactory
    {
        public List<RoomRequest> Create(string roomDetailsString)
        {
            var rooms = new List<RoomRequest>();

            if (!string.IsNullOrWhiteSpace(roomDetailsString))
            {
                //First get rid of brackets
                roomDetailsString = roomDetailsString.Replace("(","").Replace(")", "");

                //second split the rooms on pipes
                var roomStrings = roomDetailsString.Split('|').ToList();

                foreach (var roomstring in roomStrings)
                {
                    var room = new RoomRequest() { ChildAges = new List<int>() };

                    //next split child ages from pax totals on tilde
                    var roomsplit = roomstring.Split('~').ToList();

                    //Finally split pax totals on comma
                    var paxNumbers = roomsplit[0].Split(',')
                       .Where(m => int.TryParse(m, out _))
                       .Select(m => int.Parse(m))
                       .ToList();

                    if(paxNumbers.Count == 3)
                    {
                        room.Adults = paxNumbers[0];
                        room.Children = paxNumbers[1];
                        room.Infants = paxNumbers[2];
                    }

                    if (roomsplit.Count > 1)
                    {
                        var childages = roomsplit[1].Split(',')
                           .Where(m => int.TryParse(m, out _))
                           .Select(m => int.Parse(m))
                           .ToList();

                        room.ChildAges.AddRange(childages);
                    }
                    rooms.Add(room);
                }
            }

            return rooms;
        }
    }
}