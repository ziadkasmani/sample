using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HeyVoteClassLibrary.Managers;
using HeyVoteClassLibrary.Helper;
using HeyVoteClassLibrary.Auhorization;
using HeyVoteWeb.Helper;
using System.ServiceModel.Web;

namespace HeyVoteWeb.WebServices
{
    public partial class HeyVoteService : IContactMethods
    {
        public List<ContactInfo> AddContacts(List<ContactInfo> lstContacts, bool isWeb)
        {
            try
            {
                ContactManager mgr = new ContactManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.AddContacts(tokenInfo.Idf, lstContacts);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<ContactInfo> SynchronizeContacts(List<ContactInfo> lstContacts, bool isWeb)
        {
            try
            {
                ContactManager mgr = new ContactManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.AddContacts(tokenInfo.Idf, lstContacts);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public List<ContactInfo> GetContactList(bool isWeb)
        {
            try
            {
                ContactManager mgr = new ContactManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetContactList(tokenInfo.Idf, true, false, false, false);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public List<ContactHeaderInfo> GetCategorizedContactList(bool isWeb)
        {
            try
            {
                ContactManager mgr = new ContactManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetCategorizedContactList(tokenInfo.Idf, true, false, false, false);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public List<ContactHeaderInfo> GetFollowerFollowingList(bool isFollowerList, bool isWeb)
        {
            try
            {
                ContactManager mgr = new ContactManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetCategorizedContactList(tokenInfo.Idf, false, isFollowerList, !isFollowerList, false);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }

        public List<ContactHeaderInfo> GetBlockedUserList(bool isBlockedList, bool isWeb)
        {
            try
            {
                ContactManager mgr = new ContactManager();
                TokenInfo tokenInfo = new HelperMethods().GetUserToken<TokenInfo>(isWeb, HttpContext.Current);
                return mgr.GetCategorizedContactList(tokenInfo.Idf, false, false, false, isBlockedList);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }

        }
    }
}