using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HeyVoteClassLibrary.Managers;
using HeyVoteClassLibrary.Auhorization;
using HeyVoteClassLibrary.Helper;
using System.ServiceModel.Activation;
using HeyVoteWeb.Helper;
using System.ServiceModel.Web;
using Microsoft.AspNet.SignalR;
using HeyVoteWeb.Hubs;

namespace HeyVoteWeb.WebServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class HeyVoteService : IUserMethods
    {
        public string AddUser(UserInfo info, ResourceInfo resourceInfo)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(resourceInfo.DataUrl))
                    resourceInfo.Data = JsonWebToken.Base64UrlDecode(resourceInfo.DataUrl);

                string reqHeader = HttpContext.Current.Request.Headers[CodeHelper.HeaderAccessKey];
                if (!String.IsNullOrEmpty(reqHeader))
                {
                    // If token is valid then add user
                    if (TokenAuthorization.CheckBasicAuthorization(reqHeader))
                    {
                        UserManager mgr = new UserManager();
                        // gets territory id by country code
                        TerritoryManager trMgr = new TerritoryManager();
                        info.TerritoryId = trMgr.GetTerritoryIdByCountryCode(info.CountryCode, trMgr.GetTerritoryList()).Id;
                        if (!String.IsNullOrWhiteSpace(info.UserName) && !String.IsNullOrWhiteSpace(info.DisplayName) && !String.IsNullOrWhiteSpace(info.OwnNumber) 
                            && !String.IsNullOrWhiteSpace(info.Status) && !String.IsNullOrWhiteSpace(info.SerialNum) && !String.IsNullOrWhiteSpace(info.ComId))
                            return mgr.AddUser(info, resourceInfo);
                        throw new Exception(CodeHelper.UnableToAddUser);
                    }

                    // throw exception stating token is invalid
                    throw new Exception(CodeHelper.InvalidToken);
                }

                throw new Exception(CodeHelper.InvalidHeader);
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //return ex.Message;
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public string CheckIfUserExists(string number)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(number))
                {
                    string reqHeader = HttpContext.Current.Request.Headers[CodeHelper.HeaderAccessKey];
                    if (!String.IsNullOrEmpty(reqHeader))
                    {
                        //check basic token validation required for accessing webservice
                        if (TokenAuthorization.CheckBasicAuthorization(reqHeader))
                        {
                            UserManager mgr = new UserManager();
                            return mgr.CheckIfUserExists(number).ToString();
                        }

                        // throw exception stating token is invalid
                        throw new Exception(CodeHelper.InvalidToken);

                    }
                }
                // throw exception stating header is invalid
                throw new Exception(CodeHelper.InvalidHeader);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Returns basic profile picture and name by using token
        /// </summary>
        /// <returns></returns>
        public BasicUserInfo GetBasicProfileData(bool isWeb)
        {
            try
            {
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return new UserManager().GetUserInfo(tokenInfo.Idf);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool GetData()
        {
            return true;
        }

        public string PostData(string signalR, string userIdf)
        {
            return String.Format("Hello - {0}----{1}", signalR, userIdf);
        }

        public string PostDataNew(MyUser info, MyUser info1)
        {
            return String.Format("Hello - {0}{1}", info.Name, info1.Name);
        }

        public bool UpdateStatus(string status, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                Guid userIdf = new HelperMethods().GetUserIdf(isWeb, HttpContext.Current);
                return mgr.UpdateStatus(userIdf, status);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool UpdateName(string name, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                Guid userIdf = new HelperMethods().GetUserIdf(isWeb, HttpContext.Current);
                return mgr.UpdateName(userIdf, name);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public string ChangeProfilePicture(string data, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                if (!String.IsNullOrWhiteSpace(data))
                    return mgr.ChangeProfilePicture(tokenInfo.Idf, tokenInfo.FolderPath, JsonWebToken.Base64UrlDecode(data));
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public string RemoveProfilePicture(bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.RemoveProfilePicture(tokenInfo.Idf, tokenInfo.FolderPath);
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public string RegisterSignalR(string signalRIdf)
        {
            try
            {
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(true, HttpContext.Current);
                HeyVoteHub.MyUsers.TryAdd(signalRIdf, tokenInfo.Idf.ToString());
                return true.ToString();
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public string LoginWeb(string token, string signalRIdf)
        {
            var tokenInfo = JsonWebToken.DecodeToken<TokenInfo>(token, CodeHelper.SecretAccessKey, true, false);

            UserManager mgr = new UserManager();

            var sampleTokenInfo = new TokenInfo() { Idf = tokenInfo.Idf, Expiry = DateTime.Now.AddMinutes(2), FolderPath = tokenInfo.FolderPath, TerritoryId = tokenInfo.TerritoryId };

            var sampleToken = JsonWebToken.Encode(sampleTokenInfo, CodeHelper.SecretAccessKey, HvHashAlgorithm.RS256);

            var obj = GlobalHost.ConnectionManager.GetHubContext<PushHub>();

            obj.Clients.Client(PushHub.loginUsers.Where(x => x.Key == signalRIdf).FirstOrDefault().Value).notifyUsers(sampleToken);

            return true.ToString();
        }


        #region Comments

        public CommentInfo AddComment(long postId, CommentInfo info, bool isWeb)
        {
            try
            {
                CommentManager mgr = new CommentManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                info.PostId = postId;
                info.UserIdf = tokenInfo.Idf;
                return mgr.AddComment(info, tokenInfo.TerritoryId);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<CommentInfo> GetComments(long postId, int pageId, int pageSize, bool isWeb)
        {
            try
            {
                CommentManager mgr = new CommentManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                PostInfo post = new PostManager().GetPostById(tokenInfo.Idf, 0, tokenInfo.FolderPath, postId);
                if (post.isDone == true)
                    return mgr.GetComments(tokenInfo.Idf, postId, pageId, helperMgr.SetPageSize(pageSize, isWeb));
                else
                    return mgr.GetOwnComments(tokenInfo.Idf, postId, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<CommentInfo> GetOwnComments(long postId, int pageId, int pageSize, bool isWeb)
        {
            try
            {
                CommentManager mgr = new CommentManager();
                HelperMethods helperMgr = new HelperMethods();
                Guid userIdf = helperMgr.GetUserIdf(isWeb, HttpContext.Current);
                return mgr.GetOwnComments(userIdf, postId, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool DeleteComment(long postId, CommentInfo info, bool isWeb)
        {
            try
            {
                CommentManager mgr = new CommentManager();
                Guid userIdf = new HelperMethods().GetUserIdf(isWeb, HttpContext.Current);
                info.PostId = postId;
                info.UserIdf = userIdf;
                return mgr.DeleteComment(info);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
        

        #endregion

        public bool BlockUnBlockUser(string contactToken, bool isWeb, bool allowed)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                if (!String.IsNullOrWhiteSpace(contactToken))
                {
                    if (allowed)
                        return mgr.UnBlockUser(tokenInfo.Idf, JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false).Idf);
                    return mgr.BlockUser(tokenInfo.Idf, JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false).Idf);
                }
                    
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool FollowUnFollowUser(string contactToken, bool isWeb, bool follow)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                if (!String.IsNullOrWhiteSpace(contactToken))
                {
                    if (follow)
                    {
                        TokenInfo contact = JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false);
                        NotificationInfo info = mgr.FollowUser(tokenInfo.Idf, contact.Idf, tokenInfo.TerritoryId);
                        var obj = GlobalHost.ConnectionManager.GetHubContext<HeyVoteHub>();
                        obj.Clients.Client(contact.Idf.ToString()).notifyUsers(new
                        {
                            Id = info.Id,
                            Title = info.Title,
                            UserIdf = info.UserIdf,
                            ImageIdf = info.ImageIdf,
                            FolderPath = info.FolderPath,
                            CreatedOn = info.CreatedOn,
                            DisplayName = info.DisplayName,
                        });
                        return true;
                    }
                    return mgr.UnFollowUser(tokenInfo.Idf, JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false).Idf);
                }
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public bool ChangeViewedNotificationStatus(string notificationIds, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.ChangeViewedNotificationStatus(tokenInfo.Idf, notificationIds);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public ProfileInfo ViewProfile(string contactToken, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                if (!String.IsNullOrWhiteSpace(contactToken))
                {
                    TokenInfo contactTokenInfo = JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false);
                    return mgr.ViewProfile(tokenInfo.Idf, contactTokenInfo.Idf, Convert.ToInt32(contactTokenInfo.TerritoryId));
                }
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public ProfileInfo ViewProfileExternal(string contactToken, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                if (!String.IsNullOrWhiteSpace(contactToken))
                {
                    TokenInfo contactTokenInfo = JsonWebToken.DecodeToken<TokenInfo>(contactToken, CodeHelper.SecretAccessKey, true, false);
                    return mgr.ViewProfileExternal(tokenInfo.Idf, contactTokenInfo.Idf, Convert.ToInt32(contactTokenInfo.TerritoryId));
                }
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<SearchInfo> SearchUsers(string searchString,int pageId, int pageSize, bool isWeb)
        {
            try
            {
                UserManager mgr = new UserManager();
                HelperMethods helperMgr = new HelperMethods();
                TokenInfo tokenInfo = helperMgr.GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.SearchUsers(tokenInfo.Idf, searchString, pageId, helperMgr.SetPageSize(pageSize, isWeb));
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public long RequestForOTP(string Number, bool isWeb)
        {
            try
            {
                string reqHeader = HttpContext.Current.Request.Headers[CodeHelper.HeaderAccessKey];
                if (!String.IsNullOrEmpty(reqHeader))
                {
                    // If token is valid then add user
                    if (TokenAuthorization.CheckBasicAuthorization(reqHeader))
                    {
                        UserManager mgr = new UserManager();
                        // gets territory id by country code
                        
                        throw new Exception(CodeHelper.UnableToAddUser);
                    }

                    // throw exception stating token is invalid
                    throw new Exception(CodeHelper.InvalidToken);
                }

                throw new Exception(CodeHelper.InvalidHeader);
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //return ex.Message;
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public bool ValidateOTP(long id, string otp, bool isWeb)
        {
            try
            {
                string reqHeader = HttpContext.Current.Request.Headers[CodeHelper.HeaderAccessKey];
                if (!String.IsNullOrEmpty(reqHeader))
                {
                    // If token is valid then add user
                    if (TokenAuthorization.CheckBasicAuthorization(reqHeader))
                    {
                        UserManager mgr = new UserManager();
                        // gets territory id by country code
                        return mgr.ValidateOTP(id, otp);
                        throw new Exception(CodeHelper.UnableToAddUser);
                    }

                    // throw exception stating token is invalid
                    throw new Exception(CodeHelper.InvalidToken);
                }

                throw new Exception(CodeHelper.InvalidHeader);
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //return ex.Message;
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}