using HeyVoteClassLibrary.Auhorization;
using HeyVoteClassLibrary.Helper;
using HeyVoteWeb.Helper;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace HeyVoteWeb.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HeyVoteService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select HeyVoteService.svc or HeyVoteService.svc.cs at the Solution Explorer and start debugging.
    public partial class HeyVoteService : IHeyVoteService
    {

        static HeyVoteService()
        {
        }

        public string DoWork()
        {
            return "ziadabcd";
        }

        public string SetCredentials(string tempToken)
        {
            try
            {
                var obj = JsonWebToken.DecodeToken<TokenInfo>(tempToken, CodeHelper.SecretAccessKey, true, true);

                string token = JsonWebToken.Encode(new TokenInfo() { Idf = obj.Idf, FolderPath = obj.FolderPath, Expiry = DateTime.UtcNow.AddMinutes(40), TerritoryId = obj.TerritoryId }, CodeHelper.SecretAccessKey, HvHashAlgorithm.RS256);

                HttpContext.Current.Response.SetCookie(new HttpCookie(AppConfigManager.CookieKey, token) { Path = "/", Secure = true });

                return true.ToString();
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

    }
}
