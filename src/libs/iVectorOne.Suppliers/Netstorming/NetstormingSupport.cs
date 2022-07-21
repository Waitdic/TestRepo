namespace iVectorOne.Suppliers.Netstorming
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using iVector.Search.Property;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Netstorming.Models.Common;

    public class NetstormingSupport
    {
        public static List<string> NetstormingSources = new()
        {
            ThirdParties.WHL,
        };

        public static Header Header(string actor, string user, string password, string version)
        {
            return new Header
            {
                Actor = actor,
                User = user,
                Password = password,
                Version = version,
                Timestamp = GenerateTimeStamp()
            };
        }

        private static string GenerateTimeStamp()
        {
            string sTimeStamp = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            return sTimeStamp;
        }

        // build a list of the possible room configs 
        // if a room has the same combination of passengers then increase the quantity of the existing config
        internal static List<NetstormingRoomConfig> BuildListOfConfigs(RoomDetails rooms)
        {
            var roomConfigs = new List<NetstormingRoomConfig>();
            int roomNumber = 1;

            foreach (var room in rooms)
            {
                int childAge = string.IsNullOrEmpty(room.ChildAgeCSV) ? 0 : room.ChildAgeCSV.Split(',')[0].ToSafeInt();
                string sRoomConfig = $"{room.Adults}_{room.Children}_{room.Infants}_{childAge}";

                // if the config does not already exist then create a new one, if it matches then update the quantity of the config
                int iConfigNumber = RoomConfigExists(roomConfigs, sRoomConfig);
                if (iConfigNumber == 0)
                {
                    roomConfigs.Add(new NetstormingRoomConfig(room.Adults, room.Children, room.Infants, childAge, 1, roomNumber));
                    roomNumber += 1;
                }
                else
                {
                    roomConfigs[iConfigNumber - 1].Quantity += 1;
                }
            }

            return roomConfigs;
        }

        // check to see if the combination of occupants already exists
        internal static int RoomConfigExists(List<NetstormingRoomConfig> roomConfigs, string roomConfig)
        {
            foreach (var roomConfigInRoomConfigs in roomConfigs)
            {
                string[] split = roomConfig.Split('_');
                if (roomConfigInRoomConfigs.Adults == split[0].ToSafeInt() &&
                    roomConfigInRoomConfigs.Children == split[1].ToSafeInt() &&
                    roomConfigInRoomConfigs.Infants == split[2].ToSafeInt() &&
                    roomConfigInRoomConfigs.ChildAge == split[3].ToSafeInt())
                {
                    // if it already exists return the room it corresponds to 
                    return roomConfigInRoomConfigs.ConfigNumber;
                }
            }

            return 0;
        }

        // build a list of all the request combinations 
        // if the room configs are the same don't build combinations of the room types just the largest amount for each room request, 
        // eg if there are two rooms requested with the same number of people in each and multiple room types, ie twin and double for 2 adults then we request
        // 2 twins and 2 doubles and don't need to ask for a twin and a double as it is presumed these will now be available
        internal static List<RoomCombo> GenerateRoomCombos(List<NetstormingRoomConfig> roomConfigs)
        {

            // build a list of the strings that will actually be used in the request 
            // add each of the types to it then move on to the next room and for each of those types add each of the strings already in the list 
            // set the new list as the old list so that it works for the next rooms
            var exitingRequests = new List<RoomCombo>();

            // go through each room config
            foreach (var roomConfig in roomConfigs)
            {
                var currentRequests = new List<RoomCombo>();

                // go through each type that exists for each config, eg double, twin etc
                foreach (string configType in roomConfig.Config)
                {
                    var currentRequest = CreateRoom(configType, roomConfig.Quantity);
                    if (exitingRequests.Count == 0)
                    {
                        currentRequests.Add(new RoomCombo(currentRequest));
                    }
                    else
                    {
                        // add each new request to each of the existing request which makes the combinations
                        foreach (var aExitingRequest in exitingRequests)
                            currentRequests.Add(new RoomCombo(aExitingRequest, currentRequest));
                    }
                }

                // make the current set of strings the existing ones so that is ready for the next room config
                exitingRequests = currentRequests;
            }

            return exitingRequests;
        }

        public class RoomCombo
        {
            private List<Room> Rooms { get; }

            public RoomCombo(Room room)
            {
                Rooms = new List<Room> { room };
            }

            public RoomCombo(RoomCombo combo, Room room)
            {
                // copy rooms, not object reference
                Rooms = new List<Room>(combo.Rooms) { room };
            }

            public Room[] ToArray()
            {
                return Rooms.ToArray();
            }
        }

        internal static Room CreateRoom(string configCode, int quantity)
        {

            // split up the string that was made earlier
            string roomCode = configCode.Split('_')[0];
            bool extraBed = configCode.Split('_')[1] == "Y";
            bool cot = configCode.Split('_')[3] == "Y";

            // build a string to be used in the xml request based on the components
            var room = new Room
            {
                Type = roomCode,
                Required = quantity
            };

            if (extraBed)
            {
                int childAge = configCode.Split('_')[2].ToSafeInt();

                room.Extrabed = "true";
                room.Age = childAge;
            }

            if (cot)
            {
                room.Cot = "true";
            }

            return room;
        }

        // a function so that the whole thing works from the search 
        // first it gets a list of all the possible room configurations eg twin and a double for one room of two people
        // then it generates a list containing all the combinations returned in the format needed for the xml request
        internal static List<RoomCombo> GetRoomRequests(RoomDetails roomDetails)
        {
            var roomConfigs = BuildListOfConfigs(roomDetails);
            return GenerateRoomCombos(roomConfigs);
        }

        public static XmlDocument Serialize(EnvelopeBase request, ISerializer serializer)
        {
            var xmlRequest = serializer.Serialize(request);
            var xmlDeclaration = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
            xmlRequest.InsertBefore(xmlDeclaration, xmlRequest.DocumentElement);
            return xmlRequest;
        }

        public static T DeSerialize<T>(XmlDocument response, ISerializer serializer) where T : EnvelopeBase
        {
            return serializer.DeSerialize<T>(response);
        }
    }
}