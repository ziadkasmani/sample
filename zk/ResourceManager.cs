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
    public class ResourceManager
    {
        /// <summary>
        /// Adds File to Database
        /// </summary>
        /// <param name="info"></param>
        /// <param name="hierarchyId">Folder Directory of user uploading file</param>
        /// <returns></returns>
        public Guid AddResource(ResourceInfo info, string hierarchyId, EnumPostType postType)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[resource].[AddFileByParentId]", AppConfigManager.ConnectionString))
                {
                    info.ResourceId = Guid.NewGuid();
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@ResourceId", info.ResourceId);
                    var nodeParam = adapter.SelectCommand.Parameters.Add("@UserHierarchyId", SqlDbType.Udt);
                    nodeParam.Value = SqlHierarchyId.Parse(hierarchyId);
                    nodeParam.UdtTypeName = "HierarchyId";
                    string extn = CodeHelper.CustomPicFileExtension;

                    switch (postType)
                    {
                        case EnumPostType.None:
                            break;
                        case EnumPostType.Picture:
                            extn = CodeHelper.CustomPicFileExtension;
                            break;
                        case EnumPostType.Video:
                            extn = CodeHelper.CustomVideoFileExtension;
                            break;
                        case EnumPostType.Audio:
                            extn = CodeHelper.CustomAudioFileExtension;
                            break;
                        default:
                            break;
                    }

                    adapter.SelectCommand.Parameters.AddWithValue("@FileExtension", extn);
                    adapter.SelectCommand.Parameters.AddWithValue("@FileData", info.Data);

                    adapter.SelectCommand.Connection.Open();
                    adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToAddFile);
            }

            return info.ResourceId;
        }

        /// <summary>
        /// Delete Resource
        /// </summary>
        /// <param name="hierarchyId"></param>
        /// <returns></returns>
        public bool DeleteResource(Guid? resourceId)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[resource].[DeleteResource]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@ResourceId", resourceId);
                    adapter.SelectCommand.Connection.Open();

                    int rows_affected = adapter.SelectCommand.ExecuteNonQuery();

                    adapter.SelectCommand.Connection.Close();

                    if (rows_affected > 0)
                        return true;

                    return false;
                }

            }
            catch (Exception)
            {
                // add entry in a table to delete it from backend and remove exception
                throw new Exception(CodeHelper.UnableToDeleteDirectory);
            }

        }
    }

    public class ResourceInfo
    {
        public Guid ResourceId { get; set; }
        public string SystemFileName { get; set; }
        public string OriginalFileName { get; set; }
        public string FileExtension { get; set; }
        public string DataUrl { get; set; }
        public byte[] Data { get; set; }

    }

}
