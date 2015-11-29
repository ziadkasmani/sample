using HeyVoteClassLibrary.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Auhorization
{
    public class TokenAuthorization
    {
        public static bool CheckBasicAuthorization(string token)
        {
            try
            {
                BasicTokenInfo tokenInfo = JsonWebToken.DecodeToken<BasicTokenInfo>(token, CodeHelper.SecretAccessKey, true, false);

                if (String.Equals(tokenInfo.RandomString, CodeHelper.SecretAccessKeyPayload))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

        }

        public bool CheckUserToken(string token)
        {
            try
            {
                string payload = JsonWebToken.Decode(token, CodeHelper.SecretAccessKey);

                if (String.Equals(payload, CodeHelper.SecretAccessKeyPayload))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

        }


    }

    public class TokenInfo
    {
        public Guid Idf { get; set; }
        public string FolderPath { get; set; }
        public DateTime Expiry { get; set; }
        public int TerritoryId { get; set; }
    }

    public class BasicTokenInfo
    {
        public string RandomString { get; set; }
    }
    
}
