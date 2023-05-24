using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using s3cr3tx.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Net.Mail;
//using Microsoft.Data.SqlClient;


namespace s3cr3tx.Pages
{
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ForgotModel : PageModel
    {
        
        public string _message = @"";
        //private readonly ILogger<ForgotModel> _logger;
        public s3cr3tx.Models.Forgot forgotCurrent;
        public s3cr3tx.Models.ForgotDbContext ForgotContext;
        public string _output;

        public ForgotModel(s3cr3tx.Models.ForgotDbContext forgotDbContext)
        {
            ForgotContext = forgotDbContext;
            forgotCurrent = new Forgot();
        }

        public void OnPostView()
        {
            try
            {
                HttpRequest Request = HttpContext.Request;
                if (Request.Form.TryGetValue("forgotCurrent.member_email", out Microsoft.Extensions.Primitives.StringValues Email))
                {

                    forgotCurrent.member_email = Email[0].ToLower();
                    string strConnection = "Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx"; // = @"Data Source=.;User Id=sa;Password=yourStrong!Password;Initial Catalog=s3cr3tx";

                    SqlConnection sql3 = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                    SqlCommand command3 = new SqlCommand();
                    command3.CommandText = @"usp_tbl_member_sel_email";
                    command3.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p5 = new SqlParameter(@"p_email", forgotCurrent.member_email);
                    //SqlParameter p4 = new SqlParameter(@"p_member_code", strResult);
                    command3.Parameters.Add(p5);
                    //command2.Parameters.Add(p4);
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter();
                    using (sql3)
                    {
                        sql3.Open();
                        command3.Connection = sql3;
                        da.SelectCommand = command3;
                        da.Fill(dataSet);
                    }
                    if (dataSet.Tables.Count >0)
                    { 
                    //create member object
                    Member member = new Member()
                    {
                        id = (long)dataSet.Tables[0].Rows[0].ItemArray[0],
                        email = dataSet.Tables[0].Rows[0].ItemArray[1].ToString(),
                        Code = dataSet.Tables[0].Rows[0].ItemArray[2].ToString(),
                        ConfirmCode = @"",
                        FirstName = dataSet.Tables[0].Rows[0].ItemArray[3].ToString(),
                        LastName = dataSet.Tables[0].Rows[0].ItemArray[4].ToString(),
                        regcode = dataSet.Tables[0].Rows[0].ItemArray[5].ToString(),
                        created = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[6],
                        updated = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[7],
                        enabled = (bool)dataSet.Tables[0].Rows[0].ItemArray[8],
                        confirmed = (bool)dataSet.Tables[0].Rows[0].ItemArray[9],
                        country = dataSet.Tables[0].Rows[0].ItemArray[10].ToString(),
                        state = dataSet.Tables[0].Rows[0].ItemArray[11].ToString(),
                        gender = dataSet.Tables[0].Rows[0].ItemArray[12].ToString(),
                        mobile = dataSet.Tables[0].Rows[0].ItemArray[13].ToString(),
                        MobileCarrier = dataSet.Tables[0].Rows[0].ItemArray[14].ToString(),
                        city = dataSet.Tables[0].Rows[0].ItemArray[15].ToString(),
                        zipcode = dataSet.Tables[0].Rows[0].ItemArray[16].ToString(),
                        address = dataSet.Tables[0].Rows[0].ItemArray[17].ToString(),
                        address2 = dataSet.Tables[0].Rows[0].ItemArray[18].ToString()
                    };
                    string strIP = @"";
                    if (HttpContext.Connection.RemoteIpAddress is not null)
                    {
                        IPAddress iP = HttpContext.Connection.RemoteIpAddress;
                        strIP = iP.ToString();
                    }
                    if (member.confirmed.Equals(1) && (member.enabled.Equals(1)))
                    {
                        //store the forgot data
                        SqlConnection sql4 = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                        SqlCommand command4 = new SqlCommand();
                        command4.CommandText = @"usp_tbl_forgot_ins";
                        command4.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter p73 = new SqlParameter(@"p_member_id", member.id);
                        SqlParameter p71 = new SqlParameter(@"p_member_email", member.email);
                        SqlParameter p72 = new SqlParameter(@"p_member_token", forgotCurrent.member_code);
                        SqlParameter p74 = new SqlParameter(@"p_session_start", forgotCurrent.session_start);
                        SqlParameter p77 = new SqlParameter(@"p_session_end", forgotCurrent.code_expires);
                        SqlParameter p78 = new SqlParameter(@"p_member_ip", strIP);
                        command4.Parameters.Add(p71);
                        command4.Parameters.Add(p73);
                        command4.Parameters.Add(p72);
                        command4.Parameters.Add(p74);
                        command4.Parameters.Add(p77);
                        command4.Parameters.Add(p78);
                        int lngResult4 ;
                        using (sql4)
                        {
                            sql4.Open();
                            command4.Connection = sql4;
                            lngResult4 = (int)command4.ExecuteNonQuery();
                        }
                        if (lngResult4.Equals(0))
                        {
                            forgotCurrent.Output = @"Something went wrong.";
                            return;
                        }
                        else
                        {


                            Member _member = new Member();
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress(@"support@s3cr3tx.com");
                            mail.Sender = new MailAddress(@"support@s3cr3tx.com");
                            mail.Subject = @"s3cr3tx account verification";
                            mail.To.Add(new MailAddress(forgotCurrent.member_email));
                            mail.IsBodyHtml = true;
                            mail.Body = @"Dear " + member.FirstName + @",<br/><br/>Please click the following link to reset your password: <a href='http://localhost:8001/Freset?=" + System.Web.HttpUtility.UrlEncode(forgotCurrent.member_email) + @"_" + forgotCurrent.member_code + "'>http://localhost:8001/Freset?=" + System.Web.HttpUtility.UrlEncode(forgotCurrent.member_email) + @"_" + forgotCurrent.member_code + @"</a><br/><br/>For your account protection, the link will only be active for 20 minutes. <br/><br/>Thank you!<br/><br/>s3cr3tx support@gratitech.com";
                            SmtpClient smtp = new SmtpClient();
                            smtp.EnableSsl = true;
                                smtp.Host = GetS3cr3txD(@"aqD7WcnkYIZ5v8jXjmvp/qESkh9oVa/2t+lTgsp3NqdU67vRRRNUWqh/GhWSD7bvaqMbQE1/lKT1Isi1h3jnRp4aYr8gnShqzESJ7Fq18cwSODIAMd/rRtR8MyKb9TbwLc7vjEcIEELi5OaRPVThgEtSEA7r4TYu5Cb49F6pvvb4dgE5ArPp0EQdG6j1tvCNujWWF/gBE2T64NxVoZji+jjyMEA51StN8cWmxnYI/E5yB4WdVhW1vUnHu5Z9XB7TSCQJuZcQnQQ3GzQ6YjZIX6LA28hV/jm1DUguN4cAw09O1JSy2K9nMGNgtzEo+hi63APzELJFvCic8N3XL7j115RcvlxeG7bx8iFrfTa3lo2b4WqOfqbflTmr/sDpq3BhAUNwsnBwhap92SMuXX2KW5F+cZ5JYVg7VF+KhpKRwwhgttXlho/O/6HGsco5zePTlXnyFl1mzLHT9R5SfCIwVyQP7Kx3odwUebTgKdtIwo2tnWViFKUfCUDSTDEFucz6jCyh+TtGxqED89IY6x7TUCXDQ2pAYRKG8qhGIi65RD2qTxx+4EpYfQqiGV1WHLfAreh6YiOUoqdIaK5Ton/rDwxytWkYhNdUNlOp5QtNs2ZFWotJhPhii2njJlOnBdg+bPAiwYtonvb8Slo9YZwDBv3GI6GA4Qzy8ru6eYvl648=");
                                smtp.Credentials = new NetworkCredential(GetS3cr3txD(@"3GPH+iO6fmAzhIeeos8a9caXN7LLPsZHZLrhJEPlzvxVVDQBt9k4wyfWq8X6LLqB1nw2InJMgZ1U/vH7lVvJWg/H4jqlAKX+p1wFJ/F3RazqXex11gOSqgp6Zsmyh+vI7kaQIo3bUiZhwD4Wp9LajnxbkeKjh6SwHgJMpqFCESgM4WMN8JTIMoaC+BqSMgnrUZiOo+cZNuSzBI6CSe+hSJWwoBrDMFMlWI04dQUtQVr3xNfL/vcDrTSvPPV+NlvMIO3frrRZ+wHqerIP28pbiYT5cINy5wtJRTUiGfnx2iYR6kvNac3tECgcIL16eQxMeVtMOWExJSi/2TiaX0zD87eSXvdroZPrRo8tJV5Sbhrrsyh4aTDH4y20mDkmXTLAy8fpmGkrIYonb4ORjYvNqh5JcKpmLcmEso3XTDjZB0C/e4Ttplh0+RiFxS+KSa5Jn+GTYM/thLMx9f4V9Zoti7f8i7SD5TlFvRTEI/FWv/ZxNpocas7xDb8gI1+FOWSs2dgNwa89Arg6U15r6OUyEd2COfTzlgQZvBjockEJlQ+LohmNI3jFI7HgVsAPQXWaR4YLi5fGn8vXZ2A5Ri2yopSycTNs2ZPnJJ9TyMtMqCmtXX34cYtCAARcTyqlmNcAs5ajhQZ9LVTb5h821dBHE1blHc1O4OaeBcZIdyu33xw="), GetS3cr3txD(@"shVhMLUPkgtiXNoyb2ad22YfCNIsWEsMrhh8x94hgBPFDhEatSRLmdQ+4pJTyJEGnRHY41+5uDIsrcKnewn/6vsXJG2rMEaPKr8cFOlyl6rLS+LDl+zKymYT5oPOaaLf7+7k9eImeQSorLDbnqZgqyJMQ5xAW7zbPQiFFjCavtkPKLw8XePwcfSwkYl5uw+VbPCe9jBiq46FvJssbkqEVKq4FFzvMt5J0Gf0vPDMWBjy+cBV/TaIi21u5eTjjw7048DM6quCtTI7sPapcpQwqqcbO/iJXJIs3lHDXHMU3heek7a2DbVMMF34cLPe33sOVKbQZFez5Hag2R/l89Oi92as5dGUTJCVQFpruChHqwmEgjhsWU4gLlForupsQUb7POJbW1O7ALyIo0ajprImUKkZFvOCeujyvu8N8Cz2UXYZteKDHs5p0+dtMbXuXz29n6vh+kpq0ZFWuqdW+4xeslW3kalmLQZxhoLskUtVWxN6/ZdvXlkCtj3ZVxuL1C/UxZ2qpod8X559jgINEAqqEebQHWHcoAI7jRSr9mpBLL5YW9IdOHXMenaUBSs8xEpURYAwIRLkjMiw3Bv6tUcfNyoqsO0N5zw/lEhQNIsdpyOY2TSKq79kWODMjifE+6UdFzfbysUMQfUWRj5xdakelX9W8n6nZpgFUCPj44ms2DM="));
                                smtp.Port = 587;
                            smtp.Send(mail);
                            //_member.message = strResult;
                            _output = @"Please check your email for password reset instructions.";
                            //Response.Redirect(@"http://localhost:8001/Login");
                        }
                    }
                    else
                    {
                        _output = @"Please check for an email from us to confirm your account or contact us at support@gratitech.com";
                    }
                }
                else
                {
                    _output = @"Something went wrong please contact us at support@gratitech.com";
                }
            }
                else
            {
                _output = @"Something went wrong please contact us at support@gratitech.com";
            }
        }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
            }
        }

        private string GetS3cr3txD(string strS3cr3tx)
        {
            try { 
            string strEmail = @"support@gratitech.com";
                string strAuth = @"77+977+9dBfvv73vv70iSysv77+977+9RlwN77+9cm3vv73vv73vv70icFLvv73vv70Wf8el77+977+977+9dBjvv71JQ++/ve+/vSEY77+9M2Xvv71Qd++/ve+/ve+/vXNANhXvv71+N3Pvv71uSO+/ve+/vQ==";
                string strCode = @"77+977+9Qu+/ve+/vTjvv73vv71bXzbvv70VDsWS77+977+9TEnvv71mVlHRoe+/vS0SdO+/vQAI77+9Nu+/vSnvv71F77+9Xu+/vSY077+977+9Ne+/vWjvv70/Fm94Ru+/ve+/ve+/vSbvv70/Bg==";
                WebClient wc = new WebClient();
            wc.BaseAddress = @"http://localhost:8001/Values";
            WebHeaderCollection webHeader = new WebHeaderCollection();
            webHeader.Add(@"Email:" + strEmail);
            webHeader.Add(@"AuthCode:" + strCode);
            webHeader.Add(@"APIToken:" + strAuth);
            webHeader.Add(@"Input:" + strS3cr3tx);
            webHeader.Add(@"EorD:" + @"d");
            webHeader.Add(@"Def:" + @"z");
            wc.Headers = webHeader;
            string result = @"";
            result = wc.DownloadString(@"http://localhost:8001/Values");
            return result;
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
                return @"";
            }
        }
        public class NewK
        {
            [Required]
            public string email { get; set; }
            [Required]
            public string pd { get; set; }
            [Required]
            public string pd2 { get; set; }
        }
        private string CreateBundle(string strEmail)
        {
            try { 
            NewK newK = new NewK();
            newK.pd = @"1";
            newK.pd2 = @"1";
            newK.email = strEmail;
            WebClient wc = new WebClient();
            //wc.Credentials.GetCredential();
            wc.BaseAddress = @"http://localhost:8001/Values";
            WebHeaderCollection webHeader = new WebHeaderCollection();
            wc.Headers = webHeader;
            string result = @"";
            webHeader.Add(@"content-type:application/json");
            NewK nk = new NewK();
            nk.email = strEmail;
            nk.pd = @"1";
            nk.pd2 = @"1";
            string strNk = JsonSerializer.Serialize<NewK>(nk);
            result = wc.UploadString(@"http://localhost:8001/Values", strNk);
            return result;
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.ConfirmPage.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
                return @"";
            }
        }


    }
    }
