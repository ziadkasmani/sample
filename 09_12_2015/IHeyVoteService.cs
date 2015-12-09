using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HeyVoteWeb.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHeyVoteService" in both code and config file together.
    [ServiceContract]
    public interface IHeyVoteService
    {
        [OperationContract]
        string DoWork();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SetCredentials", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string SetCredentials(string tempToken);

        

    }
}
