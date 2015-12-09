using HeyVoteClassLibrary.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteWeb.WebServices
{
    [ServiceContract]
    interface IContactMethods
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetContactList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ContactInfo> GetContactList(bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategorizedContactList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ContactHeaderInfo> GetCategorizedContactList(bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetFollowerFollowingList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ContactHeaderInfo> GetFollowerFollowingList(bool isFollowerList, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetBlockedUserList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ContactHeaderInfo> GetBlockedUserList(bool isBlockedList, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddContacts", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ContactInfo> AddContacts(List<ContactInfo> lstContacts, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SynchronizeContacts", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ContactInfo> SynchronizeContacts(List<ContactInfo> lstContacts, bool isWeb);
    }

}
