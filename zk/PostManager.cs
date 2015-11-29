using HeyVoteClassLibrary.Helper;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Managers
{
    public class PostManager
    {
        // Add new post
        public long AddPost(PostInfo info, Guid? Image1Idf, Guid? Image2Idf, EnumPostType postType, string hierarchyId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[AddPost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", info.UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@Title", info.Title);
                    adapter.SelectCommand.Parameters.AddWithValue("@EndDate", info.EndDate);
                    adapter.SelectCommand.Parameters.AddWithValue("@Caption1", info.Caption1);
                    adapter.SelectCommand.Parameters.AddWithValue("@Caption2", info.Caption2);
                    adapter.SelectCommand.Parameters.AddWithValue("@Img1Idf", Image1Idf);
                    adapter.SelectCommand.Parameters.AddWithValue("@Img2Idf", Image2Idf);
                    adapter.SelectCommand.Parameters.AddWithValue("@NumberOfSubscribers", info.NumberOfSubscribers);
                    adapter.SelectCommand.Parameters.AddWithValue("@isPublic", info.IsPublic);
                    adapter.SelectCommand.Parameters.AddWithValue("@toContacts", info.ToContacts);
                    adapter.SelectCommand.Parameters.AddWithValue("@toSelectedContacts", info.ToSelectedContacts);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", info.IsGlobal ? (int?)null : info.TerritoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@isGlobal", info.IsGlobal);
                    adapter.SelectCommand.Parameters.AddWithValue("@TypeId", (int)postType);
                    adapter.SelectCommand.Parameters.AddWithValue("@CategoryId", info.CategoryId);
                    var nodeParam = adapter.SelectCommand.Parameters.Add("@FolderPath", SqlDbType.Udt);
                    nodeParam.Value = SqlHierarchyId.Parse(hierarchyId);
                    nodeParam.UdtTypeName = "HierarchyId";

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    var postId = adapter.SelectCommand.ExecuteScalar();
                    adapter.SelectCommand.Connection.Close();
                    return Convert.ToInt64(postId);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToAddPost);
            }
        }

        public bool DeletePost(Guid userId, long postId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[DeletePost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                    
                    adapter.SelectCommand.Connection.Open();
                    var result = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToDeletePost);
            }
        }

        public Guid?[] AddPostImages(ResourceInfo resourceInfo1, ResourceInfo resourceInfo2, string hierarchyId, EnumPostType postType)
        {
            try
            {
                ResourceManager mgr = new ResourceManager();
                Guid resourceId1 = mgr.AddResource(resourceInfo1, hierarchyId, postType);
                Guid? resourceId2 = null;
                if (resourceInfo2 != null && !String.IsNullOrWhiteSpace(resourceInfo2.DataUrl))
                    resourceId2 = mgr.AddResource(resourceInfo2, hierarchyId, postType);

                return new Guid?[] { resourceId1, resourceId2 };
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw new Exception(CodeHelper.UnableToAddFile);
            }

        }

        public List<PostInfo> GetOngoingHistoryPostList(Guid UserIdf, int territoryId, string folderPath, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetOngoingHistoryPostsList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    FolderHelper fHelper = new FolderHelper();

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = fHelper.GetFolderName(Convert.ToString(row["PostFolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                Vote1Result = DBNull.Value.Equals(row["Vote1Result"]) ? 0 : Convert.ToInt64(row["Vote1Result"]),
                                Vote2Result = DBNull.Value.Equals(row["Vote2Result"]) ? 0 : Convert.ToInt64(row["Vote2Result"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                FolderPath = fHelper.GetFolderName(Convert.ToString(row["FolderPath"])),
                                PostType = (EnumPostType)Enum.Parse(typeof(EnumPostType), Convert.ToString(row["TypeId"])),
                                VoteOption = DBNull.Value.Equals(row["VoteOption"]) ? (bool?)null : Convert.ToBoolean(row["VoteOption"]),
                                commentInfo = new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5)
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }

        }

        public List<PostInfo> GetPostList(Guid UserIdf, int territoryId, string folderPath, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetPostsList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    FolderHelper fHelper = new FolderHelper();

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = fHelper.GetFolderName(Convert.ToString(row["PostFolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                VoteCount = Convert.ToInt64(row["VoteCount"]),
                                CommentCount = Convert.ToInt64(row["CommentCount"]),
                                PostType = (EnumPostType)Enum.Parse(typeof(EnumPostType), Convert.ToString(row["TypeId"])),
                                VoteOption = DBNull.Value.Equals(row["VoteOption"]) ? (bool?)null : Convert.ToBoolean(row["VoteOption"]),
                                hasVoted = DBNull.Value.Equals(row["hasVoted"]) ? false : Convert.ToBoolean(row["hasVoted"]),
                                FolderPath = fHelper.GetFolderName(Convert.ToString(row["FolderPath"])),
                                isDone = Convert.ToBoolean(row["isDone"]),
                                commentInfo = Convert.ToBoolean(row["isDone"]) == true ? new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5) : new CommentManager().GetOwnComments(UserIdf, Convert.ToInt64(row["Id"]), 0, 5)
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }

            throw new Exception("abc");
        }
        public List<PostInfo> GetContactPostsList(Guid UserIdf, Guid ContactUserIdf, int territoryId, string folderPath, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetContactPostsList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@ContactUserIdf", ContactUserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    FolderHelper fHelper = new FolderHelper();

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = fHelper.GetFolderName(Convert.ToString(row["PostFolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                VoteCount = Convert.ToInt64(row["VoteCount"]),
                                PostType = (EnumPostType)Enum.Parse(typeof(EnumPostType), Convert.ToString(row["TypeId"])),
                                Vote1Result = DBNull.Value.Equals(row["Vote1Result"]) ? 0 : Convert.ToInt64(row["Vote1Result"]),
                                Vote2Result = DBNull.Value.Equals(row["Vote2Result"]) ? 0 : Convert.ToInt64(row["Vote2Result"]),
                                VoteOption = DBNull.Value.Equals(row["VoteOption"]) ? (bool?)null : Convert.ToBoolean(row["VoteOption"]),
                                CommentCount = Convert.ToInt64(row["CommentCount"]),
                                hasVoted = DBNull.Value.Equals(row["hasVoted"]) ? false : Convert.ToBoolean(row["hasVoted"]),
                                FolderPath = fHelper.GetFolderName(Convert.ToString(row["FolderPath"])),
                                isDone = Convert.ToBoolean(row["isDone"]),
                                commentInfo = Convert.ToBoolean(row["isDone"]) == true ? new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5) : new CommentManager().GetOwnComments(UserIdf, Convert.ToInt64(row["Id"]), 0, 5)
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }

            throw new Exception("abc");
        }


        public List<PostInfo> GetPostListByTerritoryId(Guid UserIdf, string folderPath, int pageId, int pageSize, int territoryId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetPostsListByTerritoryId]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    FolderHelper fHelper = new FolderHelper();

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = fHelper.GetFolderName(Convert.ToString(row["PostFolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                PostType = (EnumPostType)Enum.Parse(typeof(EnumPostType), Convert.ToString(row["TypeId"])),
                                Vote1Result = DBNull.Value.Equals(row["Vote1Result"]) ? 0 : Convert.ToInt64(row["Vote1Result"]),
                                Vote2Result = DBNull.Value.Equals(row["Vote2Result"]) ? 0 : Convert.ToInt64(row["Vote2Result"]),
                                VoteOption = DBNull.Value.Equals(row["VoteOption"]) ? (bool?)null : Convert.ToBoolean(row["VoteOption"]),
                                VoteCount = Convert.ToInt64(row["VoteCount"]),
                                CommentCount = Convert.ToInt64(row["CommentCount"]),
                                hasVoted = DBNull.Value.Equals(row["hasVoted"]) ? false : Convert.ToBoolean(row["hasVoted"]),
                                FolderPath = fHelper.GetFolderName(Convert.ToString(row["FolderPath"])),
                                isDone = Convert.ToBoolean(row["isDone"]),
                                commentInfo = Convert.ToBoolean(row["isDone"]) == true ? new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5) : new CommentManager().GetOwnComments(UserIdf, Convert.ToInt64(row["Id"]), 0, 5)
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }

            throw new Exception("abc");
        }

        public PostInfo GetPostById(Guid UserIdf, int territoryId, string folderPath, long PostId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetPostById]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", PostId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    FolderHelper fHelper = new FolderHelper();

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = fHelper.GetFolderName(Convert.ToString(row["PostFolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                //UserName = Convert.ToString(row["UserName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                Vote1Result = DBNull.Value.Equals(row["Vote1Result"]) ? 0 : Convert.ToInt64(row["Vote1Result"]),
                                Vote2Result = DBNull.Value.Equals(row["Vote2Result"]) ? 0 : Convert.ToInt64(row["Vote2Result"]),
                                isDone = DBNull.Value.Equals(row["isDone"]) ? false : Convert.ToBoolean(row["isDone"]),
                                FolderPath = fHelper.GetFolderName(Convert.ToString(row["FolderPath"])),
                                PostType = (EnumPostType)Enum.Parse(typeof(EnumPostType), Convert.ToString(row["TypeId"])),
                                VoteOption = DBNull.Value.Equals(row["VoteOption"]) ? (bool?)null : Convert.ToBoolean(row["VoteOption"]),
                                VoteCount = Convert.ToInt64(row["VoteCount"]),
                                CommentCount = Convert.ToInt64(row["CommentCount"]),
                                hasVoted = DBNull.Value.Equals(row["hasVoted"]) ? false : Convert.ToBoolean(row["hasVoted"]),
                                commentInfo = Convert.ToBoolean(row["isDone"]) == true ? new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5) : new CommentManager().GetOwnComments(UserIdf, Convert.ToInt64(row["Id"]), 0, 5)
                            }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostDetails);
            }

            throw new Exception("abc");
        }

        public bool ChangeNotificationStatus(Guid idf, long notificationId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[ChangeNotificationStatus]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", idf);
                    adapter.SelectCommand.Parameters.AddWithValue("@NotificationId", notificationId);

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

        public bool SchedulePost(long postId, DateTime? ScheduledDate)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[ScheduleResultForPost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                    adapter.SelectCommand.Parameters.AddWithValue("@ScheduledDate", ScheduledDate);

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

        public bool SpamPost(Guid userIdf, long postId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[AddPostSpam]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToSpamPost);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToSchedulePost);
            }
        }

        public bool ReHeyVote(Guid userIdf, long postId, int territoryId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[ReHeyVotePost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToReHeyVotePost);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToReHeyVotePost);
            }
        }

        public bool SubscribePost(Guid userIdf, long postId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[SubscribePost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToSubscribePost);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToSubscribePost);
            }
        }

        public bool UnSubscribePost(Guid userIdf, long postId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[UnSubscribePost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;
                    throw new Exception(CodeHelper.UnableToUnSubscribePost);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToUnSubscribePost);
            }
        }

        public bool Vote(long postId, Guid userIdf, bool voteOption, int territoryId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[Vote]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@voteOption", voteOption);

                    // Gets postId of Inserted post
                    adapter.SelectCommand.Connection.Open();
                    int results = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (results > 0)
                        return true;

                    throw new Exception(CodeHelper.UnableToVote);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (msg.Equals(CodeHelper.PostResultsDeclared))
                    throw ex;
                throw new Exception(CodeHelper.UnableToVote);
            }
        }

        public List<NotificationInfo> GetNotificationUserList(string postIds)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[GetUsersToNotifyByPost]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@PostIds", postIds);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new NotificationInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                PostId = Convert.ToInt64(row["PostId"]),
                                UserIdf = Convert.ToString(row["UserIdf"]),
                                ImageIdf = Convert.ToString(row["ImageIdf"]),
                                Image1Idf = Convert.ToString(row["Image1Idf"]),
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                Image2Idf = !DBNull.Value.Equals(row["Image2Idf"]) ? Convert.ToString(row["Image2Idf"]) : null,
                                NotifyUserIdf = Convert.ToString(row["NotifyUserIdf"]),
                                FolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                Title = Convert.ToString(row["Title"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                isDone = Convert.ToBoolean(row["isDone"]),
                                Vote1Result = Convert.ToString(row["Vote1Result"]),
                                Vote2Result = Convert.ToString(row["Vote2Result"]),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                isPost = Convert.ToBoolean(row["isPost"]),
                                isFollow = Convert.ToBoolean(row["isFollow"]),
                                isContact = Convert.ToBoolean(row["isContact"]),
                                hasRead = false,
                                isViewed = false
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }
        }

        public List<NotificationInfo> GetNotificationByUser(Guid userIdf, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[user].[GetNotifications]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new NotificationInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                PostId = !DBNull.Value.Equals(row["PostId"]) ? Convert.ToInt64(row["PostId"]) : (long?)null,
                                UserIdf = Convert.ToString(row["UserIdf"]),
                                ImageIdf = Convert.ToString(row["ImageIdf"]),
                                Image1Idf = !DBNull.Value.Equals(row["Image1Idf"]) ? Convert.ToString(row["Image1Idf"]) : null,
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                Image2Idf = !DBNull.Value.Equals(row["Image2Idf"]) ? Convert.ToString(row["Image2Idf"]) : null,
                                FolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                Title = Convert.ToString(row["Title"]),
                                Caption1 = !DBNull.Value.Equals(row["Image1Idf"]) ? Convert.ToString(row["Image1Idf"]) : null,
                                Caption2 = !DBNull.Value.Equals(row["Image1Idf"]) ? Convert.ToString(row["Image1Idf"]) : null,
                                EndDate = !DBNull.Value.Equals(row["Image1Idf"]) ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null,
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                isDone = !DBNull.Value.Equals(row["Image1Idf"]) ? Convert.ToBoolean(row["isDone"]) : false,
                                hasRead = Convert.ToBoolean(row["hasRead"]),
                                isViewed = Convert.ToBoolean(row["isViewed"]),
                                Vote1Result = Convert.ToString(row["Vote1Result"]),
                                Vote2Result = Convert.ToString(row["Vote2Result"]),
                                isPost = Convert.ToBoolean(row["isPost"]),
                                isFollow = Convert.ToBoolean(row["isFollow"]),
                                isContact = Convert.ToBoolean(row["isContact"]),
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetNotifications);
            }
        }

        public List<PostInfo> GetMyPostList(Guid UserIdf, string folderPath, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetMyPostsList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    folderPath = new FolderHelper().GetFolderName(folderPath);

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                PostType = (EnumPostType)Enum.Parse(typeof(EnumPostType), Convert.ToString(row["TypeId"])),
                                Vote1Result = DBNull.Value.Equals(row["Vote1Result"]) ? 0 : Convert.ToInt64(row["Vote1Result"]),
                                Vote2Result = DBNull.Value.Equals(row["Vote2Result"]) ? 0 : Convert.ToInt64(row["Vote2Result"]),
                                isDone = DBNull.Value.Equals(row["isDone"]) ? false : Convert.ToBoolean(row["isDone"]),
                                VoteCount = Convert.ToInt64(row["VoteCount"]),
                                CommentCount = Convert.ToInt64(row["CommentCount"]),
                                hasVoted = DBNull.Value.Equals(row["hasVoted"]) ? false : Convert.ToBoolean(row["hasVoted"]),
                                FolderPath = folderPath,
                                commentInfo = Convert.ToBoolean(row["isDone"]) == true ? new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5) : new CommentManager().GetOwnComments(UserIdf, Convert.ToInt64(row["Id"]), 0, 5)
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }

            throw new Exception("abc");
        }

        public List<PostInfo> GetMyVoteList(Guid UserIdf, string folderPath, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetMyVotesList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    folderPath = new FolderHelper().GetFolderName(folderPath);

                    return (from row in dt.AsEnumerable()
                            select new PostInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImgIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                PostFolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                UserDisplayName = Convert.ToString(row["DisplayName"]),
                                Title = Convert.ToString(row["Title"]),
                                Image1Idf = Guid.Parse(Convert.ToString(row["Img1Idf"])),
                                Image2Idf = DBNull.Value.Equals(row["Img2Idf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["Img2Idf"])),
                                Caption1 = Convert.ToString(row["Caption1"]),
                                Caption2 = Convert.ToString(row["Caption2"]),
                                EndDate = Convert.ToDateTime(row["EndDate"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                Vote1Result = DBNull.Value.Equals(row["Vote1Result"]) ? 0 : Convert.ToInt64(row["Vote1Result"]),
                                Vote2Result = DBNull.Value.Equals(row["Vote2Result"]) ? 0 : Convert.ToInt64(row["Vote2Result"]),
                                isDone = DBNull.Value.Equals(row["isDone"]) ? false : Convert.ToBoolean(row["isDone"]),
                                VoteCount = Convert.ToInt64(row["VoteCount"]),
                                FolderPath = folderPath,
                                commentInfo = Convert.ToBoolean(row["isDone"]) == true ? new CommentManager().GetComments(Guid.Parse(Convert.ToString(row["UserIdf"])), Convert.ToInt64(row["Id"]), 0, 5) : new CommentManager().GetOwnComments(UserIdf, Convert.ToInt64(row["Id"]), 0, 5)
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetPostList);
            }

            throw new Exception("abc");
        }

    }

    public class PostInfo
    {
        public long Id { get; set; }
        public Guid? UserIdf { get; set; }
        public Guid? ImgIdf { get; set; }
        public string PostFolderPath { get; set; }
        public string UserDisplayName { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public DateTime? EndDate { get; set; }
        public string Caption1 { get; set; }
        public string Caption2 { get; set; }
        public Guid? Image1Idf { get; set; }
        public Guid? Image2Idf { get; set; }
        public long VoteCount1 { get; set; }
        public long VoteCount2 { get; set; }
        public long? Vote1Result { get; set; }
        public long? Vote2Result { get; set; }
        public bool? Option1 { get; set; }
        public bool? isDone { get; set; }
        public long? NumberOfSubscribers { get; set; }
        public bool IsPublic { get; set; }
        public bool? VoteOption { get; set; }
        public bool ToContacts { get; set; }
        public bool ToSelectedContacts { get; set; }
        public int? TerritoryId { get; set; }
        public int? CategoryId { get; set; }
        public bool IsGlobal { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Duration { get; set; }
        public string FolderPath { get; set; }
        public long VoteCount { get; set; }
        public long CommentCount { get; set; }
        public bool? hasVoted { get; set; }
        public EnumPostType PostType { get; set; }
        public List<CommentInfo> commentInfo { get; set; }

    }

    public enum EnumPostType
    {
        None = 0,
        Picture = 1,
        Video = 2,
        Audio = 3
    }

    public class NotificationInfo
    {
        public long Id { get; set; }
        public long? PostId { get; set; }
        public string UserIdf { get; set; }
        public string DisplayName { get; set; }
        public string ImageIdf { get; set; }
        public string Image1Idf { get; set; }
        public string Image2Idf { get; set; }
        public string NotifyUserIdf { get; set; }
        public string FolderPath { get; set; }
        public string Title { get; set; }
        public string Caption1 { get; set; }
        public string Caption2 { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool isDone { get; set; }
        public string Vote1Result { get; set; }
        public string Vote2Result { get; set; }
        public bool hasRead { get; set; }
        public bool isViewed { get; set; }
        public bool isPost { get; set; }
        public bool isFollow { get; set; }
        public bool isContact { get; set; }


    }

}


