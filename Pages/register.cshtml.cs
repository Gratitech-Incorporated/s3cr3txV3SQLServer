using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http;
using s3cr3tx.Models;
using System.Text.Json;
using s3cr3tx.Controllers;
using System.Data.SqlClient;
using System.Security;
using System.Security.Cryptography;
using System.Net.Mail;


    namespace s3cr3tx.Pages
{
    public class RegisterModel : PageModel
    {
        // private readonly ILogger<IndexModel> _logger;

        public s3cr3tx.Models.Member _member;
        public s3cr3tx.Models.MemberDbContext _members;
        public string _output;

        //public s3cr3tx.Models.MemberSession memberSession;
        //public string _output;
        public RegisterModel(s3cr3tx.Models.MemberDbContext members)
        {
            _members = members;
            _member = new Member();
            _output = _member.message;
        }

        //public s3cr3tx.S3cr3tx S3Cr3Tx;

        public void OnGet([Bind("FirstName", "LastName", "email", "Code", "ConfirmCode", "mobile", "MobileCarrier", "country", "state", "city", "zipcode", "address", "address2", "gender", "message")] s3cr3tx.Models.Member member)
        {
            try
            {

                _member = member;
                _member.message = _output;
            }
            catch (Exception ex)
            {
                string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnGet";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

            }
        }
        public string Message { get; set; } = "";

        public class NewK
        {
            public string email { get; set; }
            public string pd { get; set; }
            public string pd2 { get; set; }
        }
        public void OnPostView([Bind("FirstName", "LastName", "email", "Code", "ConfirmCode", "mobile", "MobileCarrier", "country", "state", "city", "zipcode", "address", "address2", "gender", "message")] s3cr3tx.Models.Member member)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return;
                //}
                string email = _member.email; //= member;

                HttpRequest Request = HttpContext.Request;
                if (Request.Form.TryGetValue("_member.email", out Microsoft.Extensions.Primitives.StringValues Email))
                {
                    if (Request.Form.TryGetValue("_member.FirstName", out Microsoft.Extensions.Primitives.StringValues FirstName))
                    {
                        if (Request.Form.TryGetValue("_member.LastName", out Microsoft.Extensions.Primitives.StringValues LastName))
                        {
                            if (Request.Form.TryGetValue("_member.Code", out Microsoft.Extensions.Primitives.StringValues Code))
                            {
                                if (Request.Form.TryGetValue("_member.ConfirmCode", out Microsoft.Extensions.Primitives.StringValues Code2))
                                {
                                    if (Request.Form.TryGetValue("_member.mobile", out Microsoft.Extensions.Primitives.StringValues Mobile))
                                    {
                                        if (Request.Form.TryGetValue("_member.MobileCarrier", out Microsoft.Extensions.Primitives.StringValues MobileCarrier))
                                        {
                                            if (Request.Form.TryGetValue("_member.country", out Microsoft.Extensions.Primitives.StringValues Country))
                                            {
                                                _member.country = Country[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.state", out Microsoft.Extensions.Primitives.StringValues State))
                                            {
                                                _member.state = State[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.city", out Microsoft.Extensions.Primitives.StringValues City))
                                            {
                                                _member.city = City[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.zipcode", out Microsoft.Extensions.Primitives.StringValues zip))
                                            {
                                                _member.zipcode = zip[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.address", out Microsoft.Extensions.Primitives.StringValues Address))
                                            {
                                                _member.address = Address[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.Country", out Microsoft.Extensions.Primitives.StringValues address2))
                                            {
                                                _member.address2 = address2[0];
                                            }
                                            if (Request.Form.TryGetValue("_member.Gender", out Microsoft.Extensions.Primitives.StringValues Gender))
                                            {
                                                _member.gender = Gender[0];
                                            }

                                            _member.email = Email[0].ToLower();
                                            _member.FirstName = FirstName[0];
                                            _member.LastName = LastName[0];
                                            string strResult = @"";
                                            string strRslt = @"";
                                            strResult = uspEncDec.EncDec(@"support@gratitech.com", Code[0], true, false, out strRslt);

                                            // _member.Code = Controllers.uspEncDec.Enc(@"support@gratitech.com", Code[0])//_member.Code = System.Convert.ToBase64String(System.Security.Cryptography.SHA512.HashData(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(Code[0] + _member.regcode))));
                                            strRslt = Controllers.uspEncDec.Enc(@"support@gratitech.com", Code[0]);
                                            _member.Code = strResult;
                                            _member.mobile = Mobile[0];
                                            _member.MobileCarrier = MobileCarrier[0];
                                            //now insert new member into the database
                                            DateTime dtTimeStamp = DateTime.Now;
                                            //string strResult = @"";
                                            string strConnection = "Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx"; // = @"Data Source=.;User Id=sa;Password=yourStrong!Password;Initial Catalog=s3cr3tx";
                                            SqlConnection sql = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                                            SqlCommand command = new SqlCommand();
                                            command.CommandText = @"usp_tbl_member_ins";
                                            command.CommandType = System.Data.CommandType.StoredProcedure;
                                            SqlParameter p1 = new SqlParameter(@"p_member_email", _member.email.Trim());
                                            SqlParameter p2 = new SqlParameter(@"p_member_code", _member.Code);
                                            SqlParameter p3 = new SqlParameter(@"p_member_first_name", _member.FirstName.Trim());
                                            SqlParameter p4 = new SqlParameter(@"p_member_last_name", _member.LastName.Trim());
                                            SqlParameter p5 = new SqlParameter(@"p_member_reg_number", _member.regcode);
                                            SqlParameter p6 = new SqlParameter(@"p_member_create_date", dtTimeStamp);
                                            SqlParameter p7 = new SqlParameter(@"p_member_country", _member.country);
                                            SqlParameter p8 = new SqlParameter(@"p_member_state", _member.state);
                                            SqlParameter p9 = new SqlParameter(@"p_member_city", _member.city);
                                            SqlParameter p10 = new SqlParameter(@"p_member_zip", _member.zipcode);
                                            SqlParameter p11 = new SqlParameter(@"p_member_address", _member.address);
                                            SqlParameter p12 = new SqlParameter(@"p_member_address2", _member.address2);
                                            SqlParameter p13 = new SqlParameter(@"p_member_gender", _member.gender);
                                            SqlParameter p14 = new SqlParameter(@"p_member_update_date", dtTimeStamp);
                                            SqlParameter p15 = new SqlParameter(@"p_member_mobile_phone", _member.mobile);
                                            SqlParameter p16 = new SqlParameter(@"p_member_mobile_carrier", _member.MobileCarrier);

                                            command.Parameters.Add(p1);
                                            command.Parameters.Add(p2);
                                            command.Parameters.Add(p3);
                                            command.Parameters.Add(p4);
                                            command.Parameters.Add(p5);
                                            command.Parameters.Add(p6);
                                            command.Parameters.Add(p7);
                                            command.Parameters.Add(p8);
                                            command.Parameters.Add(p9);
                                            command.Parameters.Add(p10);
                                            command.Parameters.Add(p11);
                                            command.Parameters.Add(p12);
                                            command.Parameters.Add(p13);
                                            command.Parameters.Add(p14);
                                            command.Parameters.Add(p15);
                                            command.Parameters.Add(p16);
                                            long result = 0;
                                            using (sql)
                                            {
                                                sql.Open();
                                                command.Connection = sql;
                                                result = (long)command.ExecuteScalar();
                                            }
                                            //handle result if valid
                                            if (result > 0)
                                            {
                                                //send verification email
                                                MailMessage mail = new MailMessage();
                                                mail.From = new MailAddress(@"support@s3cr3tx.com");
                                                mail.Sender = new MailAddress(@"support@s3cr3tx.com");
                                                mail.Subject = @"s3cr3tx account verification";
                                                mail.To.Add(new MailAddress(_member.email));
                                                mail.IsBodyHtml = true;
                                                mail.Body = @"Dear " + _member.FirstName + @",<br/><br/>Welcome to s3cr3tx! Please click the following link to verify your account: <a href='http://localhost:8001/Validate?=" + System.Web.HttpUtility.UrlEncode(_member.email) + @"_" + _member.regcode + "'>http://localhost:8001/Validate?=" + System.Web.HttpUtility.UrlEncode(_member.email) + @"_" + _member.regcode + @"</a><br/><br/>Thank you!<br/><br/>s3cr3tx support@gratitech.com";
                                                SmtpClient smtp = new SmtpClient();
                                                smtp.EnableSsl = true;
                                                smtp.Host = GetS3cr3txD(@"aqD7WcnkYIZ5v8jXjmvp/qESkh9oVa/2t+lTgsp3NqdU67vRRRNUWqh/GhWSD7bvaqMbQE1/lKT1Isi1h3jnRp4aYr8gnShqzESJ7Fq18cwSODIAMd/rRtR8MyKb9TbwLc7vjEcIEELi5OaRPVThgEtSEA7r4TYu5Cb49F6pvvb4dgE5ArPp0EQdG6j1tvCNujWWF/gBE2T64NxVoZji+jjyMEA51StN8cWmxnYI/E5yB4WdVhW1vUnHu5Z9XB7TSCQJuZcQnQQ3GzQ6YjZIX6LA28hV/jm1DUguN4cAw09O1JSy2K9nMGNgtzEo+hi63APzELJFvCic8N3XL7j115RcvlxeG7bx8iFrfTa3lo2b4WqOfqbflTmr/sDpq3BhAUNwsnBwhap92SMuXX2KW5F+cZ5JYVg7VF+KhpKRwwhgttXlho/O/6HGsco5zePTlXnyFl1mzLHT9R5SfCIwVyQP7Kx3odwUebTgKdtIwo2tnWViFKUfCUDSTDEFucz6jCyh+TtGxqED89IY6x7TUCXDQ2pAYRKG8qhGIi65RD2qTxx+4EpYfQqiGV1WHLfAreh6YiOUoqdIaK5Ton/rDwxytWkYhNdUNlOp5QtNs2ZFWotJhPhii2njJlOnBdg+bPAiwYtonvb8Slo9YZwDBv3GI6GA4Qzy8ru6eYvl648=");
                                                smtp.Credentials = new NetworkCredential(GetS3cr3txD(@"3GPH+iO6fmAzhIeeos8a9caXN7LLPsZHZLrhJEPlzvxVVDQBt9k4wyfWq8X6LLqB1nw2InJMgZ1U/vH7lVvJWg/H4jqlAKX+p1wFJ/F3RazqXex11gOSqgp6Zsmyh+vI7kaQIo3bUiZhwD4Wp9LajnxbkeKjh6SwHgJMpqFCESgM4WMN8JTIMoaC+BqSMgnrUZiOo+cZNuSzBI6CSe+hSJWwoBrDMFMlWI04dQUtQVr3xNfL/vcDrTSvPPV+NlvMIO3frrRZ+wHqerIP28pbiYT5cINy5wtJRTUiGfnx2iYR6kvNac3tECgcIL16eQxMeVtMOWExJSi/2TiaX0zD87eSXvdroZPrRo8tJV5Sbhrrsyh4aTDH4y20mDkmXTLAy8fpmGkrIYonb4ORjYvNqh5JcKpmLcmEso3XTDjZB0C/e4Ttplh0+RiFxS+KSa5Jn+GTYM/thLMx9f4V9Zoti7f8i7SD5TlFvRTEI/FWv/ZxNpocas7xDb8gI1+FOWSs2dgNwa89Arg6U15r6OUyEd2COfTzlgQZvBjockEJlQ+LohmNI3jFI7HgVsAPQXWaR4YLi5fGn8vXZ2A5Ri2yopSycTNs2ZPnJJ9TyMtMqCmtXX34cYtCAARcTyqlmNcAs5ajhQZ9LVTb5h821dBHE1blHc1O4OaeBcZIdyu33xw="), GetS3cr3txD(@"shVhMLUPkgtiXNoyb2ad22YfCNIsWEsMrhh8x94hgBPFDhEatSRLmdQ+4pJTyJEGnRHY41+5uDIsrcKnewn/6vsXJG2rMEaPKr8cFOlyl6rLS+LDl+zKymYT5oPOaaLf7+7k9eImeQSorLDbnqZgqyJMQ5xAW7zbPQiFFjCavtkPKLw8XePwcfSwkYl5uw+VbPCe9jBiq46FvJssbkqEVKq4FFzvMt5J0Gf0vPDMWBjy+cBV/TaIi21u5eTjjw7048DM6quCtTI7sPapcpQwqqcbO/iJXJIs3lHDXHMU3heek7a2DbVMMF34cLPe33sOVKbQZFez5Hag2R/l89Oi92as5dGUTJCVQFpruChHqwmEgjhsWU4gLlForupsQUb7POJbW1O7ALyIo0ajprImUKkZFvOCeujyvu8N8Cz2UXYZteKDHs5p0+dtMbXuXz29n6vh+kpq0ZFWuqdW+4xeslW3kalmLQZxhoLskUtVWxN6/ZdvXlkCtj3ZVxuL1C/UxZ2qpod8X559jgINEAqqEebQHWHcoAI7jRSr9mpBLL5YW9IdOHXMenaUBSs8xEpURYAwIRLkjMiw3Bv6tUcfNyoqsO0N5zw/lEhQNIsdpyOY2TSKq79kWODMjifE+6UdFzfbysUMQfUWRj5xdakelX9W8n6nZpgFUCPj44ms2DM="));
                                                smtp.Port = 587;
                                                smtp.Send(mail);
                                                strResult = @"Registration Success: Please check for a verification email from us.";

                                                _member.message = strResult;
                                                _output = strResult;

                                                //string strRslt = @"";
                                                //strResult = @"Registration Success: Please check for a verification email from us.";

                                            }
                                            else
                                            {
                                                strResult = @"An account exists with the email address entered.  Please check for a verification email from us and then login or contact us at support@gratitech.com";
                                            };

                                           

                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnPostView";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);

            }
        }
        private string GetS3cr3txD(string strS3cr3tx)
        {
            try
            {
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
    }

}
        // include function to encrypt
        //
    //    public string Enc(string strInput)
    //    {
    //        WebClient wc = new WebClient();
    //        //wc.Credentials.GetCredential();


    //        wc.BaseAddress = @"http://localhost:8001/Values";
    //        WebHeaderCollection webHeader = new WebHeaderCollection();
    //        webHeader.Add(@"Email:" + S3Cr3Tx.Email);
    //        webHeader.Add(@"AuthCode:" + S3Cr3Tx.AuthCode);
    //        webHeader.Add(@"APIToken:" + S3Cr3Tx.Token);
    //        webHeader.Add(@"Input:" + S3Cr3Tx.Input);
    //        webHeader.Add(@"EorD:" + S3Cr3Tx.EoD);
    //        webHeader.Add(@"Def:" + @"z");

    //        wc.Headers = webHeader;
    //        string result = @"";
            
    //            result = wc.DownloadString(@"http://localhost:8001/Values");
           
    //        return result;
    //    }
    

