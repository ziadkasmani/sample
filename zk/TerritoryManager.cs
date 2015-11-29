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
    public class TerritoryManager
    {
        /// <summary>
        /// Get Territory List
        /// </summary>
        /// <returns></returns>
        public List<TerritoryInfo> GetTerritoryList()
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[territory].[GetTerritoryList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new TerritoryInfo
                            {
                                Id =Convert.ToInt32(Convert.ToString(row["Id"])),
                                Territory = Convert.ToString(row["Territory"]),
                                CountryCode = Convert.ToString(row["CountryCode"]),
                                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                                ModifiedOn = Convert.ToDateTime(row["ModifiedOn"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetTerritoryList);
            }

        }

        /// <summary>
        /// Get territory by country code
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public TerritoryInfo GetTerritoryIdByCountryCode(string countryCode, List<TerritoryInfo> lstTerritories)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(countryCode))
                {
                    TerritoryInfo info = lstTerritories.Where(x => x.CountryCode.Equals(countryCode)).FirstOrDefault();
                    if (info != null)
                        return info;
                }

                throw new Exception(CodeHelper.UnableToFindTerritory);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToFindTerritory);
            }
        }

        public Tuple<int, string> GetTerritoryIdByUserIdf(Guid userIdf)
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[territory].[GetTerritorByUserIdf]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.Parameters.AddWithValue("@UserIdf", userIdf);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        object territoryId = dt.Rows[0][CodeHelper.TerritoryId];
                        object countryCode = dt.Rows[0][CodeHelper.CountryCode];
                        if (territoryId != null && countryCode != null && !String.IsNullOrWhiteSpace(countryCode.ToString()))
                            return new Tuple<int, string>(Convert.ToInt32(territoryId), countryCode.ToString());
                    }

                    throw new Exception(CodeHelper.UnableToFindUserTerritory);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToFindUserTerritory);
            }
        }
    }

    public class TerritoryInfo
    {
        public int Id { get; set; }
        public string Territory { get; set; }
        public string CountryCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}
