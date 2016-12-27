using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Reflection;
using NaughtyKid.Error;

namespace NaughtyKid.DataBase
{
    /// <summary>
    /// OracleHepler
    /// </summary>
    public class OracleHepler
    {
        private readonly OracleConnection OracleConnection;
     
        public OracleHepler()
        {
            var oraclestring = ConfigurationManager.AppSettings["OrcaleConnectionString"];
            OracleConnection = new OracleConnection(oraclestring);
        }
        public DataSet GetOracleDatable(string sql, OracleParameter[] myPar)
        {
            DataSet ds = new DataSet();
            try
            {
                OracleConnection.Open();
                OracleCommand cmd = new OracleCommand(sql, OracleConnection);
                OracleDataAdapter da = new OracleDataAdapter(cmd);

                if (myPar != null)
                {
                    foreach (OracleParameter spar in myPar)
                    {
                        cmd.Parameters.Add(spar);
                    }

                }
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                OracleConnection.Close();
            }
        }
        /// <summary>
        /// 执行增删查改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="myPar"></param>
        /// <returns></returns>
        public int OracleExecteNonQuery(string sql, OracleParameter[] myPar)
        {
            int val = -1;
            try
            {
                OracleConnection.Open();
                var cmd = new OracleCommand(sql, OracleConnection);
                if (myPar != null)
                {
                    foreach (OracleParameter spar in myPar)
                    {
                        cmd.Parameters.Add(spar);
                    }
                    val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                OracleConnection.Close();
            }
            return val;
        }
        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="myPar"></param>
        /// <returns></returns>
        public string OracleFirstUnit(string sql, OracleParameter[] myPar)
        {
            string values = string.Empty;
            try
            {
                OracleConnection.Open();
                OracleCommand cmd = new OracleCommand(sql, OracleConnection);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                if (myPar != null)
                {
                    foreach (OracleParameter spar in myPar)
                    {
                        cmd.Parameters.Add(spar);
                    }
                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    values = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                OracleConnection.Close();
            }
            return values;
        }
    }
}
