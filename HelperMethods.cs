using HeyVoteClassLibrary.Auhorization;
using HeyVoteClassLibrary.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace HeyVoteWeb.Helper
{
    public class HelperMethods
    {
        public int SetPageSize(int pageSize, bool isWeb)
        {
            if (isWeb)
                return AppConfigManager.WebPageSize;
            return AppConfigManager.MobilePageSize;
        }

        public T GetToken<T>(HttpContext context)
        {
            context.Response.AppendHeader("Access-Control-Allow-Origin", "https://localhost");
            context.Response.AppendHeader("Access-Control-Allow-Credentials", "true");

            var obj = context.Request.Cookies[AppConfigManager.CookieKey];

            if (obj != null)
                return JsonWebToken.DecodeToken<T>(obj.Value, CodeHelper.SecretAccessKey, true, true);

            throw new Exception(CodeHelper.UnableToGetCookie);
        }

        public T GetHeaderIdentifier<T>(HttpContext context)
        {
            try
            {
                var token = context.Request.Headers[CodeHelper.HeaderAccessKey];
                if (!String.IsNullOrWhiteSpace(token))
                    return JsonWebToken.DecodeToken<T>(token, CodeHelper.SecretAccessKey, true, false);
                throw new Exception(CodeHelper.InvalidLoginToken);
            }
            catch (Exception)
            {
                throw new Exception(CodeHelper.InvalidLoginToken);
            }
           
        }

        public Guid GetUserIdf(bool isWeb, HttpContext context)
        {
            Guid userIdf;
            if (isWeb)
            {
                var tokenInfo = GetToken<TokenInfo>(context);
                userIdf = tokenInfo.Idf;
                //if ((Convert.ToDateTime(tokenInfo.Expiry) - DateTime.Now).Minutes <= 2)
                //{
                //    string token = JsonWebToken.Encode(new TokenInfo() { Idf = tokenInfo.Idf, FolderPath = tokenInfo.FolderPath, Expiry = DateTime.UtcNow.AddMinutes(AppConfigManager.KeepAliveTime), TerritoryId = tokenInfo.TerritoryId }, CodeHelper.SecretAccessKey, HvHashAlgorithm.RS256);
                //    context.Response.SetCookie(new HttpCookie(AppConfigManager.CookieKey, token) { Path = "/HeyVoteWeb", Secure = true });
                //}
            }
            else
                userIdf = GetHeaderIdentifier<TokenInfo>(context).Idf;
            return userIdf;
        }

        public T GetUserToken<T>(bool isWeb, HttpContext context)
        {
            T userIdf;
            if (isWeb)
                userIdf = GetToken<T>(context);
            else
                userIdf = GetHeaderIdentifier<T>(context);

            if (isWeb)
            {
                PropertyInfo ExpiryInfo = userIdf.GetType().GetProperty("Expiry");
                var date = ExpiryInfo.GetValue(userIdf);
                TokenInfo user = (TokenInfo)(object)userIdf;
                //if ((Convert.ToDateTime(date) - DateTime.Now).Minutes <= 2)
                //{
                //    string token = JsonWebToken.Encode(new TokenInfo() { Idf = user.Idf, FolderPath = user.FolderPath, Expiry = DateTime.UtcNow.AddMinutes(AppConfigManager.KeepAliveTime), TerritoryId = user.TerritoryId }, CodeHelper.SecretAccessKey, HvHashAlgorithm.RS256);
                //    context.Response.SetCookie(new HttpCookie(AppConfigManager.CookieKey, token) { Path = "/HeyVoteWeb", Secure = true });
                //}
            }

            Guid idf = Guid.Empty;
            PropertyInfo info = userIdf.GetType().GetProperty("Idf");

            if (info.GetValue(userIdf) !=null)
                idf = Guid.Parse(info.GetValue(userIdf).ToString());

            if (idf == Guid.Empty)
                throw new Exception(CodeHelper.InvalidLoginToken);

            return userIdf;
        }

    }
    
}