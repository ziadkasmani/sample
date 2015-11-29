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
    public class CategoryManager
    {
        public List<CategoryInfo> GetCategoryList()
        {
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("[posts].[GetCategoryList]", AppConfigManager.ConnectionString))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (from row in dt.AsEnumerable()
                            select new CategoryInfo
                            {
                                Id = Convert.ToInt32(Convert.ToString(row["Id"])),
                                Category = Convert.ToString(row["Territory"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(CodeHelper.UnableToGetCategoryList);
            }
        }
    }

    public class CategoryInfo
    {
        public int Id { get; set; }
        public string Category { get; set; }
    }
}
