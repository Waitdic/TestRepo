namespace ThirdParty.CSSuppliers.Netstorming
{
    using System.Collections;

    public class NetstormingRoomConfig
    {
        public ArrayList Config = new();
        public int Adults;
        public int Children;
        public int Infants;
        public int ChildAge;
        public int Quantity;
        public int ConfigNumber;

        // for a new config store the number of each type of passenger and create a list of config codes that can be associated with it 
        public NetstormingRoomConfig(int adults, int children, int infants, int childAge, int quantity, int roomNumber)
        {
            Adults = adults;
            Children = children;
            Infants = infants;
            ChildAge = childAge;
            GenerateRoomConfig(adults, children, infants, childAge);
            Quantity += quantity;
            ConfigNumber = roomNumber;
        }

        // make a code based on the occupants. Consists of roomtype_whether an extra bed is required (can only have one)_
        // the age of the child if there is an extra bed_ whether a cot is required(can't have cots in a sinlge room)
        private void GenerateRoomConfig(int adults, int children, int infants, int childAge)
        {
            int passengerCount = adults + children;
            string infantString = infants == 1 ? "Y" : "N";

            switch (passengerCount)
            {
                // for now the cots and extra beds will be commented out as when booking them they can return a pending status which we cant have
                case 1:
                {
                    if (infants == 0)
                    {
                        Config.Add("sgl_N_0_N");
                        Config.Add("tsu_N_0_N");
                    }
                    else if (infants == 1)
                    {
                        Config.Add("tsu_N_0_Y");
                    }

                    break;
                }

                case 2:
                {
                    Config.Add($"twn_N_0_{infantString}");
                    Config.Add($"dbl_N_0_{infantString}");
                    break;
                }

                case 3:
                {
                    Config.Add($"trp_N_0_{infantString}");
                    break;
                }

                case 4:
                {
                    Config.Add($"qud_N_0_{infantString}");
                    break;
                }
            }
        }
    }
}