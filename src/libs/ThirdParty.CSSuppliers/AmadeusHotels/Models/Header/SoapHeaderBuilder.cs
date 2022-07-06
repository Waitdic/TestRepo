namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Header
{
    using System;
    using Support;
    using Intuitive.Helpers.Extensions;

    public class SoapHeaderBuilder
    {
        public static SoapHeader BuildSoapHeader(
            IAmadeusHotelsSettings settings,
            IThirdPartyAttributeSearch searchDetails,
            string soapAction,
            bool isStateful = false,
            string sessionId = "")
        {
            var header = new SoapHeader
            {
                Session = isStateful && string.IsNullOrEmpty(sessionId)
                    ? new SessionRequest { TransactionStatusCode = "Start" }
                    : isStateful ? new SessionRequest
                    {
                        SessionId = sessionId.Split('|')[0],
                        SequenceNumber = sessionId.Split('|')[1].ToSafeInt(),
                        SecurityToken = sessionId.Split('|')[2]
                    } : null,
                MessageID = { Message = Guid.NewGuid().ToString() },
                Action = { Message = soapAction },
                To = { Message = settings.URL(searchDetails) },
                TransactionFlowLink =
                {
                    Consumer = { UniqueID = Guid.NewGuid().ToString() }
                },
            };

            if (string.IsNullOrEmpty(sessionId))
            {
                var passwordDigest = new PasswordDigest(settings.Password(searchDetails));
                header.Security = new Security
                {
                    UsernameToken =
                    {
                        Id = "UsernameToken-1",
                        Username = settings.UserID(searchDetails),
                        Created = passwordDigest.TimeStamp,
                        Nonce =
                        {
                            EncodingType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary",
                            Base64Nonce = passwordDigest.Base64Nonce
                        },
                        Password =
                        {
                            Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest",
                            DigestedPassword = passwordDigest.DigestedPassword
                        }
                    }
                };

                header.AmaSecurityHostedUser = new AmaSecurityHostedUser
                {
                    UserID = new UserID
                    {
                        PosType = "1",
                        PseudoCityCode = settings.OfficeID(searchDetails),
                        AgentSign = settings.AgentDutyCode(searchDetails),
                        RequestorType = "U"
                    }
                };

            }

            return header;
        }
    }
}
