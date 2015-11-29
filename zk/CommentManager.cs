using HeyVoteClassLibrary.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Managers
{
    public class CommentManager
    {
        public List<CommentInfo> GetComments(Guid UserIdf, long postId, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetComments]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new CommentInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImageIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                FolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                Comment = Convert.ToString(row["Comment"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                ModifiedOn = Convert.ToDateTime(row["ModifiedOn"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetComment);
            }

        }

        public List<CommentInfo> GetOwnComments(Guid UserIdf, long postId, int pageId, int pageSize)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetOwnComments]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", UserIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                    adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new CommentInfo()
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                ImageIdf = DBNull.Value.Equals(row["ImageIdf"]) ? (Guid?)null : Guid.Parse(Convert.ToString(row["ImageIdf"])),
                                FolderPath = new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                DisplayName = Convert.ToString(row["DisplayName"]),
                                Comment = Convert.ToString(row["Comment"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                ModifiedOn = Convert.ToDateTime(row["ModifiedOn"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetComment);
            }

        }

        public bool EditComment(CommentInfo info)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(info.Comment))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[EditComment]", AppConfigManager.ConnectionString))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        adapter.SelectCommand.Parameters.AddWithValue("@Id", info.Id);
                        adapter.SelectCommand.Parameters.AddWithValue("@PostId", info.PostId);
                        adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", info.UserIdf);
                        adapter.SelectCommand.Parameters.AddWithValue("@Comment", info.Comment);

                        adapter.SelectCommand.Connection.Open();
                        int results = adapter.SelectCommand.ExecuteNonQuery();
                        adapter.SelectCommand.Connection.Close();
                        if (results > 0)
                            return true;
                    }
                }
                throw new Exception(CodeHelper.UnableToEditComment);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToEditComment);
            }
        }

        public CommentInfo AddComment(CommentInfo info, int territoryId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(info.Comment))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[AddComment]", AppConfigManager.ConnectionString))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        adapter.SelectCommand.Parameters.AddWithValue("@PostId", info.PostId);
                        adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", info.UserIdf);
                        adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                        adapter.SelectCommand.Parameters.AddWithValue("@Comment", info.Comment);

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        return (from row in dt.AsEnumerable()
                                select new CommentInfo
                                {
                                    Id = Convert.ToInt64(row["Id"]),
                                    PostId = Convert.ToInt64(row["PostId"]),
                                    UserIdf = Guid.Parse(Convert.ToString(row["UserIdf"])),
                                    CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                    ModifiedOn = Convert.ToDateTime(row["CreatedOn"]),
                                }).FirstOrDefault();
                    }
                }
                throw new Exception(CodeHelper.UnableToEditComment);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToComment);
            }
        }

        public bool DeleteComment(CommentInfo info)
        {
            try
            {
                // Modified by yahya
                if (info.Id != 0)
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[DeleteComment]", AppConfigManager.ConnectionString))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                        adapter.SelectCommand.Parameters.AddWithValue("Id", info.Id);
                        adapter.SelectCommand.Parameters.AddWithValue("@PostId", info.PostId);
                        adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", info.UserIdf);

                        adapter.SelectCommand.Connection.Open();
                        int results = adapter.SelectCommand.ExecuteNonQuery();
                        adapter.SelectCommand.Connection.Close();
                        if (results > 0)
                            return true;
                    }
                }
                throw new Exception(CodeHelper.UnableToEditComment);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToComment);
            }
        }

    }

    public class CommentInfo
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public Guid UserIdf { get; set; }
        public string DisplayName { get; set; }
        public Guid? ImageIdf { get; set; }
        public string FolderPath { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
