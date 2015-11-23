using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;

namespace WebApplication4.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service1 : IService1
    {
        public int GetData(int value)
        {
            return 10;
        }

        public List<UserInfo> GetUserList(int pageId, int pageSize)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter("[dbo].[AddUser_New]", Properties.Settings.Default.ConnectionString))
            {
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                adapter.SelectCommand.Parameters.AddWithValue("@PageId", pageId);
                adapter.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                var lst = (from row in dt.AsEnumerable()
                           select new UserInfo
                           {
                               Id = Convert.ToInt32(row["Id"]),
                               FirstName = Convert.ToString(row["FirstName"]),
                               LastName = Convert.ToString(row["LastName"]),
                               Status = Convert.ToBoolean(row["Status"])
                           }).ToList();

                adapter.SelectCommand.CommandText = "[dbo].[GetCountOfUsers]";

                adapter.SelectCommand.Parameters.Clear();

                adapter.SelectCommand.Connection.Open();

                int count = Convert.ToInt32(adapter.SelectCommand.ExecuteScalar());

                adapter.SelectCommand.Connection.Close();
                //Thread.Sleep(2000);
                return lst ;

            }
        }
    }
}
