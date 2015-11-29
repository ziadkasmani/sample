using HeyVoteClassLibrary.Auhorization;
using HeyVoteClassLibrary.Helper;
using HeyVoteClassLibrary.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Managers
{
    public class ContactManager
    {
        public List<ContactInfo> SynchronizeContacts(Guid userIdf, List<ContactInfo> lstContacts)
        {
            return null;
        }

        /// <summary>
        /// Gets Contact list of selected User
        /// </summary>
        /// <param name="userIdf"></param>
        /// <returns></returns>
        public List<ContactInfo> GetContactList(Guid userIdf, bool contact, bool follower, bool following, bool blocked)
        {
            try
            {
                string query = String.Empty;

                if (contact)
                    query = "[contact].[GetContactList]";
                else if (follower)
                    query = "[contact].[GetFollowers]";
                if (following)
                    query = "[contact].[GetFollowing]";
                if (blocked)
                    query = "[contact].[GetBlockedUsers]";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new ContactInfo
                            {
                                ContactToken = DBNull.Value.Equals(row["ContactUserIdf"]) ? null : JsonWebToken.Encode(new TokenInfo()
                                {
                                    Idf = Guid.Parse(Convert.ToString(row["ContactUserIdf"])),
                                    TerritoryId = Convert.ToInt32(row["TerritoryId"])
                                }, CodeHelper.SecretAccessKey, HvHashAlgorithm.RS256),
                                ImageIdf = DBNull.Value.Equals(row["ImageIdf"]) ? String.Empty : Convert.ToString(row["ImageIdf"]),
                                FolderPath = row["FolderPath"].ToString().Equals("NULL") ? String.Empty : new FolderHelper().GetFolderName(Convert.ToString(row["FolderPath"])),
                                Name = Convert.ToString(row["Name"]),
                                Number = Convert.ToString(row["Number"]),
                                Status = DBNull.Value.Equals(row["Status"]) ? String.Empty : Convert.ToString(row["Status"]),
                                postId = DBNull.Value.Equals(row["PostId"]) ? 0 : Convert.ToInt64(row["PostId"]),
                                VoteEndTime = DBNull.Value.Equals(row["VoteEndDate"]) ? (DateTime?)null : Convert.ToDateTime(row["VoteEndDate"]),
                                TerritoryId = Convert.ToInt32(row["TerritoryId"]),
                                Alias = Convert.ToString(row["Alias"]),
                                isContact = contact,
                                isFollower = follower,
                                isFollowing = following,
                                isBlocked = blocked
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToFetchContacts);
            }
        }

        public List<ContactHeaderInfo> GetCategorizedContactList(Guid userIdf, bool contact, bool follower, bool following, bool isBlockedList)
        {
            try
            {
                List<ContactHeaderInfo> lstCatContacts = new List<ContactHeaderInfo>();
                List<ContactInfo> lstContacts = GetContactList(userIdf, contact, follower, following, isBlockedList);

                var alias = lstContacts.Select(x => x.Alias).Distinct().ToList<string>();

                alias.ForEach(x =>
                {
                    var contacts = lstContacts.Where(y => y.Alias.Equals(x)).ToList();
                    if (contacts.Count > 0)
                        lstCatContacts.Add(new ContactHeaderInfo() { Alias = x, lstContacts = contacts });
                });

                return lstCatContacts.OrderBy(x => x.Alias).ToList();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToFetchContacts);
            }
        }

        /// <summary>
        /// Refresh users contact list
        /// </summary>
        /// <param name="userIdf"></param>
        /// <returns></returns>
        public bool RefreshContactList(Guid userIdf)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[contact].[RefreshContacts]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Connection.Open();
                    int result = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (result > 0)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToRefreshContacts);
            }
        }

        /// <summary>
        /// Attach Audience to post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="lstContacts"></param>
        /// <returns></returns>
        public bool SavePostContacts(Guid? userIdf, int? territoryId, bool isGlobal, long postId, List<ContactInfo> lstContacts)
        {
            try
            {
                //DataTable dt = new DataTable();
                //dt.Columns.Add("PostId");
                //dt.Columns.Add("ContactUserIdf", typeof(Guid));

                //lstContacts.ForEach(x => { dt.Rows.Add(postId, x.ContactIdf); });

                //SqlBulkCopy sbcopy = new SqlBulkCopy(AppConfigManager.ConnectionString);
                //sbcopy.DestinationTableName = AppConfigManager.DestinationTableName;

                //sbcopy.ColumnMappings.Add("PostId", "PostId");
                //sbcopy.ColumnMappings.Add("ContactUserIdf", "ContactUserIdf");

                //sbcopy.WriteToServer(dt);
                //sbcopy.Close();

                //return true;

                DataTable dtContacts = new DataTable("ContactType");
                dtContacts.Columns.Add("ContactUserIdf");
                dtContacts.Columns.Add("Name");
                dtContacts.Columns.Add("Number");
                dtContacts.Columns.Add("TerritoryId");

                lstContacts.ForEach(x =>
                {
                    dtContacts.Rows.Add(x.ContactIdf, null, null, null);
                });

                using (SqlDataAdapter adapter = new SqlDataAdapter("[contact].[SavePostContacts]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Parameters.AddWithValue("@TerritoryId", territoryId);
                    adapter.SelectCommand.Parameters.AddWithValue("@isGlobal", isGlobal);

                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@postContacts";
                    parameter.SqlDbType = System.Data.SqlDbType.Structured;
                    parameter.Value = dtContacts;
                    adapter.SelectCommand.Parameters.AddWithValue("@PostId", postId);
                    adapter.SelectCommand.Parameters.Add(parameter);

                    adapter.SelectCommand.Connection.Open();
                    int result = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    if (result > 0)
                        return true;
                    return false;
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToAddPostContacts);
            }
        }

        /// <summary>
        /// Add Set of Contacts
        /// </summary>
        /// <param name="userIdf"></param>
        /// <param name="lstContacts"></param>
        /// <returns></returns>
        public List<ContactInfo> AddContacts(Guid userIdf, List<ContactInfo> lstContacts)
        {
            try
            {
                DataTable dtContacts = new DataTable("ContactType");
                dtContacts.Columns.Add("ContactUserIdf", typeof(Guid));
                dtContacts.Columns.Add("Name");
                dtContacts.Columns.Add("Number");
                dtContacts.Columns.Add("TerritoryId");

                TerritoryManager mgr = new TerritoryManager();

                var lstTerritory = mgr.GetTerritoryList();

                Tuple<int, string> territory = mgr.GetTerritoryIdByUserIdf(userIdf);

                lstContacts = new HeyVoteUtil().ValidatePhoneNumbers(lstContacts, territory.Item2, lstTerritory);

                lstContacts.ForEach(x =>
                {
                    dtContacts.Rows.Add(Guid.Empty, x.Name, x.Number, x.TerritoryId);
                });

                using (SqlDataAdapter adapter = new SqlDataAdapter("[contact].[SyncContacts]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@syncContacts";
                    parameter.SqlDbType = System.Data.SqlDbType.Structured;
                    parameter.Value = dtContacts;
                    adapter.SelectCommand.Parameters.Add(parameter);
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    adapter.SelectCommand.Connection.Open();
                    int result = adapter.SelectCommand.ExecuteNonQuery();
                    adapter.SelectCommand.Connection.Close();
                    return GetContactList(userIdf, true, false, false, false);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToAddContact);
            }


        }

    }

    public class ContactInfo
    {
        public Guid? ContactIdf { get; set; }
        public string ContactToken { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string CountryCode { get; set; }
        public int? TerritoryId { get; set; }
        public string ImageIdf { get; set; }
        public string FolderPath { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? VoteEndTime { get; set; }
        public long postId { get; set; }
        public string Alias { get; set; }
        public bool isContact { get; set; }
        public bool isFollower { get; set; }
        public bool isFollowing { get; set; }
        public bool isBlocked { get; set; }

    }

    public class ContactHeaderInfo
    {
        public string Alias { get; set; }
        public List<ContactInfo> lstContacts { get; set; }
    }
}
