using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    class SQLData : Helpers
    {
        // Insert list of IPTJPP from STRATIX into IMPORT
        public int Write_IPTJPP_IMPORT(List<IPTJPP> lstIPTJPP)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from ST_IMPORT_tbl_IPTJPP";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into IMPORT
                    cmd.CommandText = "INSERT INTO ST_IMPORT_tbl_IPTJPP (jpp_job_no,jpp_job_itm,jpp_job_sbitm,jpp_invt_typ,jpp_wdth,jpp_trgt_ord_info,jpp_part_cus_id,jpp_part,jpp_pcs) " +
                                        "VALUES (@arg1,@arg2,@arg3,@arg4,@arg5,@arg6,@arg7,@arg8,@arg9)";

                    cmd.Parameters.Add("@arg1", SqlDbType.Int);
                    cmd.Parameters.Add("@arg2", SqlDbType.Int);
                    cmd.Parameters.Add("@arg3", SqlDbType.Int);
                    cmd.Parameters.Add("@arg4", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg5", SqlDbType.Decimal);
                    cmd.Parameters.Add("@arg6", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg7", SqlDbType.Int);
                    cmd.Parameters.Add("@arg8", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg9", SqlDbType.Int);

                    foreach (IPTJPP s in lstIPTJPP)
                    {                       
                        cmd.Parameters[0].Value = Convert.ToInt32(s.job_no);
                        cmd.Parameters[1].Value = Convert.ToInt32(s.itm);
                        cmd.Parameters[2].Value = Convert.ToInt32(s.sbitm);
                        cmd.Parameters[3].Value = s.invt_typ;
                        cmd.Parameters[4].Value = Convert.ToDecimal(s.wdth);
                        cmd.Parameters[5].Value = s.ord_info;                      
                        cmd.Parameters[6].Value = Convert.ToInt32(s.cus_id);
                        cmd.Parameters[7].Value = s.part.ToString();
                        cmd.Parameters[8].Value = Convert.ToInt32(s.pcs);                     

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't have a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into IMPORT
                    cmd.CommandText = "SELECT COUNT(jpp_job_no) from ST_IMPORT_tbl_IPTJPP";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        // Insert values from IMPORT into WIP IPTJPP
        public int Write_IMPORT_to_IPTJPP()
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy IMPORT to IPTJPP table.  Return rows inserted.
                cmd.CommandText = "ST_proc_IMPORT_to_IPTJPP";
              
                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        } 
    }
}
