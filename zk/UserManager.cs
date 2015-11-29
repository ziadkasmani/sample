using HeyVoteClassLibrary.Auhorization;
using HeyVoteClassLibrary.Helper;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HeyVoteClassLibrary.Managers
{
    public class UserManager
    {
        /// <summary>
        /// Adds as new user  
        /// </summary>
        /// <param name="info"></param>
        /// <param name="resourceInfo"></param>
        /// <returns></returns>
        public string AddUser(UserInfo info, ResourceInfo resourceInfo)
        {
            string directory = String.Empty;
            Guid? resourceId = null;
            bool hasImage = false;
            bool hasDirectory = false;
            try
            {
                // Create Directory
                DirectoryManager dirMgr = new DirectoryManager();
                directory = dirMgr.CreateDirectory();

                if (!String.IsNullOrWhiteSpace(directory))
                    hasDirectory = true;

                hasImage = resourceInfo.Data != null;

                if (hasImage)
                {
                    // Add Resource to that directory
                    ResourceManager mgr = new ResourceManager();
                    resourceId = mgr.AddResource(resourceInfo, directory, EnumPostType.Picture);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[AddUserInfo]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserName", info.UserName);
                    adapter.SelectCommand.Parameters.AddWithValue("@DisplayName", info.DisplayName);
                    adapter.SelectCommand.Parameters.AddWithValue("@OwnNumber", info.OwnNumber);
                    adapter.SelectCommand.Parameters.AddWithValue("@GenderId", info.GenderId);
                    adapter.SelectCommand.Parameters.AddWithValue("@AgeRangeId", info.AgeRangeId);
                    adapter.SelectCommand.Parameters.AddWithValue("@Status", info.Status);
                    adapter.SelectCommand.Parameters.AddWithValue("@ImageIdf", resourceId);

                    var nodeParam = adapter.SelectCommand.Parameters.Add("@FolderPath", SqlDbType.Udt);
                    nodeParam.Value = SqlHierarchyId.Parse(directory);
                    nodeParam.UdtTypeName = "HierarchyId";

                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", info.TerritoryId);

                    DataTable dt = new DataTable();

                    // Gets UserId of Inserted User
                    adapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var userIdf = dt.Rows[0][CodeHelper.Idf];
                        var userFolderPath = dt.Rows[0][CodeHelper.FolderPath];
                        if (userIdf != null && userFolderPath != null)
                            return JsonWebToken.Encode(new TokenInfo() { Idf = Guid.Parse(userIdf.ToString()), FolderPath = userFolderPath.ToString(), TerritoryId = info.TerritoryId }, CodeHelper.SecretAccessKey, HvHashAlgorithm.RS256);

                    }

                    if (resourceId != null)
                        DeleteFile(resourceId);
                    // Delete directory and file
                    if (hasDirectory)
                        DeleteDirectory(directory);

                    throw new Exception(CodeHelper.UnableToAddUser);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                if (resourceId != null)
                    DeleteFile(resourceId);
                // Delete directory and file
                if (hasDirectory)
                    DeleteDirectory(directory);
                throw new Exception(CodeHelper.UnableToAddUser);
            }

        }

        private static void DeleteFile(Guid? resourceId)
        {
            ResourceManager resMgr = new ResourceManager();
            resMgr.DeleteResource(resourceId);
        }

        private static void DeleteDirectory(string directory)
        {
            DirectoryManager dirMgr = new DirectoryManager();
            dirMgr.DeleteDirectory(directory);
        }

        public bool CheckIfUserExists(string ownNumber)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[CheckUserByNumber]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@OwnNumber", ownNumber);

                    // Check if user is already registered
                    //DataTable dt = new DataTable();
                    //adapter.Fill(dt);
                    //if (dt != null && dt.Rows.Count > 0)
                    //{
                    //    var obj = dt.Rows[0][0];
                    //    if (obj != null)
                    //        return Convert.ToBoolean(obj.ToString());
                    //}
                    adapter.SelectCommand.Connection.Open();
                    bool exists = Convert.ToBoolean(adapter.SelectCommand.ExecuteScalar());
                    adapter.SelectCommand.Connection.Close();

                    return exists;
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw new Exception(CodeHelper.UnableToCheckUserInfo);
            }
        }

        public BasicUserInfo GetUserInfo(Guid userIdf)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[GetBasicProfileData]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new BasicUserInfo
                            {
                                ImageIdf = Convert.ToString(row["ImageIdf"]),
                                FolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                UserName = Convert.ToString(row["UserName"]),
                                Status = Convert.ToString(row["Status"]),
                                AgeRangeId = Convert.ToInt32(row["AgeRangeId"]),
                                AgeRanges = Convert.ToString(row["AgeRanges"]),
                                GenderId = Convert.ToInt32(row["GenderId"]),
                                Gender = Convert.ToString(row["Gender"]),
                                LastLoggedIn = Convert.ToDateTime(row["LastLoggedIn"].ToString())
                            }).ToList().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetUserBasicInfo);
            }
        }

        /// <summary>
        /// Update User Status
        /// </summary>
        /// <param name="userIdf"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateStatus(Guid userIdf, string status)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(status))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[UpdateStatus]", AppConfigManager.ConnectionString))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                        adapter.SelectCommand.Parameters.AddWithValue("@Status", status);

                        // Gets postId of Inserted post
                        adapter.SelectCommand.Connection.Open();
                        int results = adapter.SelectCommand.ExecuteNonQuery();
                        adapter.SelectCommand.Connection.Close();
                        if (results > 0)
                            return true;
                    }
                    throw new Exception(CodeHelper.UnableToUpdateStatus);
                }
                else
                {
                    throw new Exception(CodeHelper.UnableToUpdateStatus);

                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUpdateStatus);
            }
        }

        /// <summary>
        /// Update User Name
        /// </summary>
        /// <param name="userIdf"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateName(Guid userIdf, string name)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[UpdateName]", AppConfigManager.ConnectionString))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                        adapter.SelectCommand.Parameters.AddWithValue("@Name", name);

                        // Gets postId of Inserted post
                        adapter.SelectCommand.Connection.Open();
                        int results = adapter.SelectCommand.ExecuteNonQuery();
                        adapter.SelectCommand.Connection.Close();
                        if (results > 0)
                            return true;

                        throw new Exception(CodeHelper.UnableToUpdateName);
                    }
                }
                else
                {
                    throw new Exception(CodeHelper.UnableToUpdateName);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUpdateName);
            }
        }

        /// <summary>
        /// Change profile picture
        /// </summary>
        /// <param name="userIdf"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ChangeProfilePicture(Guid userIdf, string folderPath, byte[] data)
        {
            Guid? oldImage = null;
            Guid resourceId = Guid.Empty;
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[GetPicture]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        object obj = dt.Rows[0][0];
                        if (obj != null && !String.IsNullOrWhiteSpace(obj.ToString()))
                            oldImage = Guid.Parse(Convert.ToString(obj));

                        ResourceManager mgr = new ResourceManager();
                        resourceId = mgr.AddResource(new ResourceInfo() { Data = data, FileExtension = CodeHelper.CustomPicFileExtension }, folderPath, EnumPostType.Picture);

                        if (resourceId != Guid.Empty)
                        {
                            adapter.SelectCommand.CommandText = "[user].[ChangePicture]";
                            adapter.SelectCommand.Parameters.AddWithValue("@ResoureId", resourceId);

                            adapter.SelectCommand.Connection.Open();
                            int results = adapter.SelectCommand.ExecuteNonQuery();
                            adapter.SelectCommand.Connection.Close();
                            if (results > 0)
                            {
                                // if file was added and ImageIdf was updated, delete old file
                                if (oldImage != null)
                                    DeleteFile(oldImage);
                                return true.ToString();
                            }
                            else
                            {
                                // if image was added but ImageIdf was not updated remove newly added file
                                if (resourceId != Guid.Empty)
                                    DeleteFile(resourceId);
                            }
                        }

                    }

                    throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
                }
            }
            catch (Exception ex)
            {
                if (resourceId != Guid.Empty)
                    DeleteFile(resourceId);

                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
        }

        public ProfileInfo ViewProfile(Guid userIdf, Guid contactIdf, int territoryId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[ViewProfile]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@ContactUserIdf", contactIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new ProfileInfo()
                            {
                                ImageIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                FolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                Status = Convert.ToString(row["Status"]),
                                FollowerCount = Convert.ToInt64(row["FollowersCount"]),
                                FollowingCount = Convert.ToInt64(row["FollowingCount"]),
                                TerritoryViewCount = Convert.ToInt64(row["TerritoryViewCount"]),
                                GlobalViewCount = Convert.ToInt64(row["GlobalViewCount"]),
                                HeyVotesCount = Convert.ToInt64(row["HeyVotesCount"]),
                                isAllowed = Convert.ToBoolean(row["isAllowed"]),
                                isFollowing = Convert.ToBoolean(row["isFollowing"]),
                                URank = Convert.ToString(row["URank"])
                            }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToViewProfile);
            }
        }

        /// <summary>
        /// Remove Profile Picture
        /// </summary>
        /// <param name="userIdf"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public string RemoveProfilePicture(Guid userIdf, string folderPath)
        {
            Guid? oldImage = null;
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[GetPicture]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        object obj = dt.Rows[0][0];
                        if (obj != null && !String.IsNullOrWhiteSpace(obj.ToString()))
                        {
                            oldImage = Guid.Parse(Convert.ToString(obj));

                            adapter.SelectCommand.CommandText = "[user].[ChangePicture]";
                            adapter.SelectCommand.Parameters.AddWithValue("@ResoureId", null);

                            adapter.SelectCommand.Connection.Open();
                            int results = adapter.SelectCommand.ExecuteNonQuery();
                            adapter.SelectCommand.Connection.Close();
                            if (results > 0)
                            {
                                if (oldImage != null)
                                    DeleteFile(oldImage);
                                return true.ToString();
                            }
                        }
                    }

                    throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUpdateProfilePicture);
            }
        }

        public bool BlockUser(Guid userIdf, Guid blockUserIdf)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[BlockUser]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@BlockUserIdf", blockUserIdf);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToBlockUser);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToBlockUser);
            }
        }

        public bool UnBlockUser(Guid userIdf, Guid blockUserIdf)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[UnBlockUser]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@BlockUserIdf", blockUserIdf);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToUnBlockUser);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUnBlockUser);
            }
        }

        public NotificationInfo FollowUser(Guid userIdf, Guid followUserIdf, int territoryId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[FollowUser]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@FollowUserIdf", followUserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new NotificationInfo
                            {
                                Id= Convert.ToInt64(row["Id"]),
                                UserIdf = Convert.ToString(row["UserIdf"]),
                                ImageIdf = Convert.ToString(row["ImageIdf"]),
                                FolderPath = Convert.ToString(row["FolderPath"]),
                                NotifyUserIdf = Convert.ToString(row["NotifyUserIdf"]),
                                Title = Convert.ToString(row["Title"]),
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                            }).FirstOrDefault();

                    throw new Exception(CodeHelper.UnableToFollowUser);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToFollowUser);
            }
        }

        public bool UnFollowUser(Guid userIdf, Guid followUserIdf)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[UnFollowUser]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@FollowUserIdf", followUserIdf);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToUnFollowUser);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUnFollowUser);
            }
        }

        public bool ChangeViewedNotificationStatus(Guid idf, string notificationIds)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[ChangeViewedNotifications]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", idf);
                    adapter.SelectCommand.Parameters.AddWithValue("@NotificationIds", notificationIds);

                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToSchedulePost);
            }
        }
    }

    public class UserInfo
    {
        public long Id { get; set; }
        public Guid Idf { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string OwnNumber { get; set; }
        public string Status { get; set; }
        public Guid? ImageIdf { get; set; }
        public string FolderIdf { get; set; }
        public int TerritoryId { get; set; }
        public int GenderId { get; set; }
        public int AgeRangeId { get; set; }
        public string DomainName { get; set; }
        public int HowOld { get; set; }
        public string CountryCode { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public bool isActive { get; set; }
        public bool isSpecial { get; set; }
        public long Rank { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class BasicUserInfo
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string ImageIdf { get; set; }
        public string FolderPath { get; set; }
        public string Status { get; set; }
        public int AgeRangeId { get; set; }
        public string AgeRanges { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public DateTime LastLoggedIn { get; set; }

    }

    public class abc
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProfileInfo
    {
        public string ContactToken { get; set; }
        public string DisplayName { get; set; }
        public Guid? ImageIdf { get; set; }
        public string FolderPath { get; set; }
        public string Status { get; set; }
        public long FollowerCount { get; set; }
        public long FollowingCount { get; set; }
        public long TerritoryViewCount { get; set; }
        public long GlobalViewCount { get; set; }
        public long HeyVotesCount { get; set; }
        public bool isAllowed { get; set; }
        public bool isFollowing { get; set; }
        public string URank { get; set; }

    }


}
