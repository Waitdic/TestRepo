namespace iVectorOne.CSSuppliers.AmadeusHotels.Support
{
    using Models.Header;

    public class AmadeusSessionToken
    {
        private string Session;
        private int Sequence;
        private string SecurityToken;

        public AmadeusSessionToken(Session session)
        {
            Session = session.SessionId;
            Sequence = session.SequenceNumber.GetValueOrDefault();
            SecurityToken = session.SecurityToken;
        }

        public override string ToString()
        {
            return Session + "|" + Sequence + "|" + SecurityToken;
        }
    }
}