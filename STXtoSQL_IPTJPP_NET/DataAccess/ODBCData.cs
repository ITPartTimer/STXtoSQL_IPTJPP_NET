using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<IPTJPP> Get_IPTJPP()
        {

            List<IPTJPP> lstIPTJPP = new List<IPTJPP>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);

            try
            {
                conn.Open();

                // Try to split with verbatim literal
                OdbcCommand cmd = conn.CreateCommand();

                // NULL values don't work well when going from an ODBC recordset to POCO then to SQL Server
                // If jpp_part_cust_id is NULL, assign a value of 99999
                cmd.CommandText = @"select jpp_job_no,jpp_job_itm,jpp_job_sbitm,jpp_invt_typ,jpp_wdth,
                                    jpp_trgt_ord_info,nvl(jpp_part_cus_id, 99999) as jpp_part_cus_id,jpp_part,jpp_pcs
                                    from iptjpp_rec 
                                    where (jpp_invt_typ <> 'S' and jpp_invt_typ <> 'M')
                                    and jpp_job_no in
                                    (select psh_job_no
                                    from iptpsh_rec s
                                    inner join iptjob_rec j
                                    on j.job_job_no = s.psh_job_no
                                    where s.psh_whs = 'SW'
                                    and psh_sch_seq_no <> 99999999
                                    and (job_job_sts = 0 or job_job_sts = 1)
                                    and (job_prs_cl = 'SL' or job_prs_cl = 'CL' or job_prs_cl = 'MB'))";

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        IPTJPP b = new IPTJPP();

                        b.job_no = Convert.ToInt32(rdr["jpp_job_no"]);
                        b.itm = Convert.ToInt32(rdr["jpp_job_itm"]);
                        b.sbitm = Convert.ToInt32(rdr["jpp_job_sbitm"]);
                        b.invt_typ = rdr["jpp_invt_typ"].ToString();
                        b.wdth = Convert.ToDecimal(rdr["jpp_wdth"]);
                        b.ord_info = rdr["jpp_trgt_ord_info"].ToString();
                        b.cus_id = Convert.ToInt32(rdr["jpp_part_cus_id"]);
                        b.part = rdr["jpp_part"].ToString().TrimEnd(' ');
                        b.pcs = Convert.ToInt32(rdr["jpp_pcs"]);                      

                        lstIPTJPP.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return lstIPTJPP;
        }
    }
}
