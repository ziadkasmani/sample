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
    interface IUserMethods
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddUser", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string AddUser(UserInfo info, ResourceInfo resourceInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/CheckIfUserExists", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CheckIfUserExists(string number);

        [OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "/GetData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [WebGet(UriTemplate = "/GetData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool GetData();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/PostData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string PostData(string signalR, string userIdf);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/PostDataNew", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string PostDataNew(MyUser info, MyUser info1);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetBasicProfileData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[WebGet(UriTemplate = "/GetBasicProfileData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        BasicUserInfo GetBasicProfileData(bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateName", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool UpdateName(string name, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateStatus", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool UpdateStatus(string status, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ChangePicture", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string ChangeProfilePicture(string data, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveProfilePicture", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string RemoveProfilePicture(bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/RegisterSignalR", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string RegisterSignalR(string signalRIdf);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/LoginWeb", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string LoginWeb(string token, string signalRIdf);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/BlockUnBlockUser", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool BlockUnBlockUser(string contactToken, bool isWeb, bool allowed);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/FollowUnFollowUser", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool FollowUnFollowUser(string contactToken, bool isWeb, bool follow);

        #region Comments

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddComment", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CommentInfo AddComment(long postId, CommentInfo info, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetComments", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CommentInfo> GetComments(long postId, int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOwnComments", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CommentInfo> GetOwnComments(long postId, int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteComment", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool DeleteComment(long postId, CommentInfo info, bool isWeb);

        #endregion

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeViewedNotificationStatus", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool ChangeViewedNotificationStatus(string notificationIds, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ViewProfile", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ProfileInfo ViewProfile(string contactToken, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ViewProfileExternal", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ProfileInfo ViewProfileExternal(string contactToken, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SearchUsers", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<SearchInfo> SearchUsers(string searchString, int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/RequestForOTP", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        long RequestForOTP(string Number, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ValidateOTP", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool ValidateOTP(long id, string otp, bool isWeb);
    }

    public class MyUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    
}
