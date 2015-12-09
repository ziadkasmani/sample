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
    interface IPostMethods
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddPost", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string AddPost(PostInfo info, ResourceInfo resource1Info, ResourceInfo resource2Info, List<ContactInfo> lstContacts, bool isPicture, bool isVideo, bool isAudio, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/DeletePost", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool DeletePost(long postId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPostList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PostInfo> GetPostList(int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetContactPostsList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PostInfo> GetContactPostsList(string contactToken, int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPostListByTerritoryId", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PostInfo> GetPostListByTerritoryId(int pageId, int pageSize, int territoryId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOngoingHistoryPostList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PostInfo> GetOngoingHistoryPostList(int pageId, int pageSize, bool isWeb);

        [OperationContract(IsOneWay = true, Action = "POST", Name = "SendPostResults")]//(IsOneWay = true, Name = "SendPostResults", Action ="POST")
        [WebInvoke(Method = "POST", UriTemplate = "/SendPostResults", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SendPostResults(string postIds);

        [OperationContract(IsOneWay = true, Action = "POST", Name = "SendNotificationsForNewlyAddedUser")]
        [WebInvoke(Method = "POST", UriTemplate = "/SendNotificationsForNewlyAddedUser", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SendNotificationsForNewlyAddedUser(string userIdf, string ownNumber);

        [OperationContract(IsOneWay = true, Action = "POST", Name = "SchedulePost")]
        [WebInvoke(Method = "POST", UriTemplate = "/SchedulePost", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SchedulePost(string postIds);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Vote", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool Vote(long postId, bool voteOption, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetNotificationByUser", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<NotificationInfo> GetNotificationByUser(int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPostById", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        PostInfo GetPostById(long postId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetMyVoteList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PostInfo> GetMyVoteList(int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetMyPostList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<PostInfo> GetMyPostList(int pageId, int pageSize, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SpamPost", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool SpamPost(long postId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ReHeyVote", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool ReHeyVote(long postId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SubscribePost", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool SubscribePost(long postId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeNotificationStatus", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool ChangeNotificationStatus(long notificationId, bool isWeb);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryList", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CategoryInfo> GetCategoryList(bool isWeb);

    }
}
