using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebApplication4.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<UserInfo> GetUserList(int pageId, int pageSize);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        int GetData(int value);
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }   
        public string LastName { get; set; }
        public bool Status { get; set; }
    }

}
