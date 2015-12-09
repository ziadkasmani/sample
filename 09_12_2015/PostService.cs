using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HeyVoteClassLibrary.Managers;
using HeyVoteClassLibrary.Auhorization;
using HeyVoteClassLibrary.Helper;
using HeyVoteWeb.Helper;
using System.Threading;
using System.ServiceModel.Web;
using HeyVoteWeb.Hubs;
using Microsoft.AspNet.SignalR;

namespace HeyVoteWeb.WebServices
{
    public partial class HeyVoteService : IPostMethods
    {
        public string AddPost(PostInfo info, ResourceInfo resource1Info, ResourceInfo resource2Info, List<ContactInfo> lstContacts, bool isPicture, bool isVideo, bool isAudio, bool isWeb)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(info.Title) && !String.IsNullOrWhiteSpace(info.Caption1) && !String.IsNullOrWhiteSpace(info.Caption2)
                     && !String.IsNullOrWhiteSpace(resource1Info.DataUrl) && info.Duration >= 5 && info.Duration <= 60
                     && (info.IsPublic == true || info.ToContacts == true || (info.ToSelectedContacts == true && lstContacts.Count > 0))
                     && (info.TerritoryId != null || info.IsGlobal == true) && (isPicture || isVideo || isAudio) && (info.CategoryId == 1 || info.CategoryId == 2 || info.CategoryId == 3))
                {
                    PostManager mgr = new PostManager();

                    TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);

                    resource1Info.Data = JsonWebToken.Base64UrlDecode(resource1Info.DataUrl);

                    if (resource2Info != null && !String.IsNullOrWhiteSpace(resource2Info.DataUrl))
                        resource2Info.Data = JsonWebToken.Base64UrlDecode(resource2Info.DataUrl);

                    // Sets user token from cookie
                    info.UserIdf = tokenInfo.Idf;
                    info.EndDate = DateTime.UtcNow.AddMinutes(info.Duration);

                    if (info.IsPublic)
                    {
                        info.ToContacts = false;
                        info.ToSelectedContacts = false;
                    }
                    else if (info.ToContacts)
                    {
                        info.ToSelectedContacts = false;
                        info.IsPublic = false;
                    }
                    else if (info.ToSelectedContacts)
                    {
                        info.IsPublic = false;
                        info.ToContacts = false;
                    }

                    if (isPicture)
                    {
                        isVideo = false;
                        isAudio = false;
                    }
                    else if (isVideo)
                    {
                        isPicture = false;
                        isAudio = false;
                    }
                    else if (isAudio)
                    {
                        isVideo = false;
                        isPicture = false;
                    }

                    EnumPostType postType = isPicture ? EnumPostType.Picture : (isVideo ? EnumPostType.Video : (isAudio ? EnumPostType.Audio : EnumPostType.Picture));

                    var guids = mgr.AddPostImages(resource1Info, resource2Info, tokenInfo.FolderPath, postType);
                    long postId = mgr.AddPost(info, guids[0], guids[1], postType, tokenInfo.FolderPath);
                    if (info.ToSelectedContacts)
                    {
                        List<ContactInfo> lstFilteredContacts = new List<ContactInfo>();
                        ContactManager conMgr = new ContactManager();
                        lstContacts.ForEach(x =>
                        {
                            if (!String.IsNullOrWhiteSpace(x.ContactToken))
                            {
                                try
                                {
                                    TokenInfo contactToken = JsonWebToken.DecodeToken<TokenInfo>(x.ContactToken, CodeHelper.SecretAccessKey, true, false);
                                    x.ContactIdf = contactToken.Idf;
                                    lstFilteredContacts.Add(x);
                                }
                                catch (Exception)
                                {
                                    // if contact does not have all details, ignore it
                                }
                            }

                        });
                        conMgr.SavePostContacts(info.UserIdf, info.TerritoryId, info.IsGlobal, postId, lstFilteredContacts);
                    }

                    // schedule job
                    mgr.SchedulePost(postId, info.EndDate);

                    // send notification to all users either GCM or SignalR stating your contact has posted something
                    var obj = GlobalHost.ConnectionManager.GetHubContext<HeyVoteHub>();
                    new PostManager().GetNotificationUserList(postId.ToString()).ForEach(x =>
                    {
                        var signalR = HeyVoteHub.MyUsers.Where(y => y.Value.Equals(x.NotifyUserIdf)).FirstOrDefault();
                        if (!signalR.Equals(default(KeyValuePair<string, string>)))
                            obj.Clients.Client(signalR.Key).notifyUsers(new
                            {
                                Id = x.Id,
                                Title = x.Title,
                                UserIdf = x.UserIdf,
                                ImageIdf = x.ImageIdf,
                                Image1Idf = x.Image1Idf,
                                FolderPath = x.FolderPath,
                                CreatedOn = x.CreatedOn,
                                Caption1 = x.Caption1,
                                Caption2 = x.Caption2,
                                DisplayName = x.DisplayName,
                                PostId = x.PostId,
                                EndDate = x.EndDate,
                                isPost = x.isPost,
                                isFollow = x.isFollow,
                                isContact = x.isContact,
                                hasRead = false,
                                isViewed = false
                            });
                    });

                    return true.ToString();
                }
                return false.ToString();
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public bool DeletePost(long postId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.DeletePost(tokenInfo.Idf, postId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<NotificationInfo> GetNotificationByUser(int pageId, int pageSize, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                var userIdf = helperMgr.GetUserIdf(isWeb, HttpContext.Current);
                return mgr.GetNotificationByUser(userIdf, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<PostInfo> GetOngoingHistoryPostList(int pageId, int pageSize, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetOngoingHistoryPostList(tokenInfo.Idf, tokenInfo.TerritoryId, tokenInfo.FolderPath, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public List<PostInfo> GetPostList(int pageId, int pageSize, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetPostList(tokenInfo.Idf, tokenInfo.TerritoryId, tokenInfo.FolderPath, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<PostInfo> GetContactPostsList(string contactToken, int pageId, int pageSize, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetContactPostsList(tokenInfo.Idf, JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false).Idf, tokenInfo.TerritoryId, tokenInfo.FolderPath, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<PostInfo> GetPostListByTerritoryId(int pageId, int pageSize, int territoryId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetPostListByTerritoryId(tokenInfo.Idf, tokenInfo.FolderPath, pageId, helperMgr.SetPageSize(pageSize, isWeb), territoryId == 0 ? tokenInfo.TerritoryId : territoryId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public void SendPostResults(string postIds)
        {
            var obj = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<HeyVoteHub>();
            new PostManager().GetNotificationUserList(postIds).ForEach(x =>
            {
                var signalR = HeyVoteHub.MyUsers.Where(y => y.Value.Equals(x.NotifyUserIdf)).FirstOrDefault();
                if (!signalR.Equals(default(KeyValuePair<string, string>)))
                    obj.Clients.Client(signalR.Key).notifyUsers(new
                    {
                        Id = x.Id,
                        Title = x.Title,
                        UserIdf = x.UserIdf,
                        ImageIdf = x.ImageIdf,
                        Image1Idf = x.Image1Idf,
                        FolderPath = x.FolderPath,
                        CreatedOn = x.CreatedOn,
                        Caption1 = x.Caption1,
                        Caption2 = x.Caption2,
                        DisplayName = x.DisplayName,
                        PostId = x.PostId,
                        EndDate = x.EndDate
                    });
            });
        }

        public void SendNotificationsForNewlyAddedUser(string userIdf, string ownNumber)
        {
            NotificationManager mgr = new NotificationManager();
            string result = mgr.AndroidPush(mgr.GetIdsToNotifyForNewUser(userIdf), String.Format(AppConfigManager.NewlyAddesUserMessage, ownNumber));
            // if result is false then log exception
        }

        public void SchedulePost(string postIds)
        {
            PostManager mgr = new PostManager();
            mgr.SchedulePost(Convert.ToInt64(postIds), null);

            var obj = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<HeyVoteHub>();
            new PostManager().GetNotificationUserList(postIds).ForEach(x =>
            {
                var signalR = HeyVoteHub.MyUsers.Where(y => y.Value.Equals(x.NotifyUserIdf)).FirstOrDefault();
                if (!signalR.Equals(default(KeyValuePair<string, string>)))
                    obj.Clients.Client(signalR.Key).notifyUsers(new
                    {
                        Id = x.Id,
                        Title = x.Title,
                        UserIdf = x.UserIdf,
                        ImageIdf = x.ImageIdf,
                        Image1Idf = x.Image1Idf,
                        FolderPath = x.FolderPath,
                        CreatedOn = x.CreatedOn,
                        Caption1 = x.Caption1,
                        Caption2 = x.Caption2,
                        DisplayName = x.DisplayName,
                        PostId = x.PostId,
                        EndDate = x.EndDate
                    });
            });
        }

        public bool Vote(long postId, bool voteOption, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.Vote(postId, tokenInfo.Idf, voteOption, tokenInfo.TerritoryId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public PostInfo GetPostById(long postId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetPostById(tokenInfo.Idf, tokenInfo.TerritoryId, tokenInfo.FolderPath, postId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<PostInfo> GetMyVoteList(int pageId, int pageSize, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetMyVoteList(tokenInfo.Idf, tokenInfo.FolderPath, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<PostInfo> GetMyPostList(int pageId, int pageSize, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetMyPostList(tokenInfo.Idf, tokenInfo.FolderPath, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool SpamPost(long postId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.SpamPost(tokenInfo.Idf, postId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool ReHeyVote(long postId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.ReHeyVote(tokenInfo.Idf, postId, tokenInfo.TerritoryId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool SubscribePost(long postId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.SubscribePost(tokenInfo.Idf, postId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool ChangeNotificationStatus(long notificationId, bool isWeb)
        {
            try
            {
                PostManager mgr = new PostManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.ChangeNotificationStatus(tokenInfo.Idf, notificationId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<CategoryInfo> GetCategoryList(bool isWeb)
        {
            try
            {
                CategoryManager mgr = new CategoryManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetCategoryList();
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}