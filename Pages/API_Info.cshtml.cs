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
using System.Data;
using System.IO;
using System.Data.SqlClient;

using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Razor.Tokenizer;
using System.Security.Cryptography.X509Certificates;

namespace s3cr3tx.Pages
{
    public class API_InfoModel : PageModel
    { 
        public s3cr3tx.Models.S3cr3tx S3Cr3Tx;
        public s3cr3tx.Models.S3cr3txDbContext _s3cr3tx;
        public s3cr3tx.Models.memberSessionDbContext _memberSessionDB;
        public s3cr3tx.Models.MemberDbContext _memberDB;
        private s3cr3tx.Models.Member _member;
        private s3cr3tx.Models.MemberSession _msession;
        public string _output;
        public string _passcode;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public API_InfoModel(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,s3cr3tx.Models.S3cr3txDbContext s3Cr3tx, s3cr3tx.Models.memberSessionDbContext memberSessionDb, s3cr3tx.Models.MemberDbContext memberDb)
        {
            _hostingEnvironment = hostingEnvironment;
            _s3cr3tx = s3Cr3tx;
            S3Cr3Tx = new S3cr3tx();
            _output = S3Cr3Tx.Output;
            _memberSessionDB = memberSessionDb;
            _memberDB = memberDb;

        }
        //public s3cr3tx.S3cr3tx S3Cr3Tx;

        public void OnGet([Bind("Email", "AuthCode", "Token", "EoD", "Input", "Output")] s3cr3tx.Models.S3cr3tx _S3Cr3Tx, [Bind("id","confirmed","email","enabled","confirmed")]s3cr3tx.Models.Member member, [Bind("id","IsActive","SessionExpires")] s3cr3tx.Models.MemberSession session)
        {
            try
            {
                S3Cr3Tx = _S3Cr3Tx;
                if (Request.Cookies.TryGetValue(@"s3cr3tx", out string? strValue))
                {
                    //decrypt the cookie value
                     string strSessionID = Controllers.uspEncDec.Dec(@"support@gratitech.com", strValue);
                    //get the session object
                    string strConnection = "Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx"; // = @"Data Source=.;User Id=sa;Password=yourStrong!Password;Initial Catalog=s3cr3tx";

                    SqlConnection sql = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                    SqlCommand command = new SqlCommand();
                    command.CommandText = @"usp_tbl_member_sessions_selCode";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p5 = new SqlParameter(@"p_ID", strSessionID);
                    //SqlParameter p4 = new SqlParameter(@"p_member_code", strResult);
                    command.Parameters.Add(p5);
                    //command2.Parameters.Add(p4);
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter();
                    using (sql)
                    {
                        sql.Open();
                        command.Connection = sql;
                        da.SelectCommand = command;
                        da.Fill(dataSet);
                    }

                    MemberSession ms = new MemberSession()
                    {
                        id = (long)dataSet.Tables[0].Rows[0].ItemArray[0],
                        member_id = (long)dataSet.Tables[0].Rows[0].ItemArray[1],
                        member_email = dataSet.Tables[0].Rows[0].ItemArray[2].ToString(),
                        member_token = dataSet.Tables[0].Rows[0].ItemArray[3].ToString(),
                        FirstName = dataSet.Tables[0].Rows[0].ItemArray[4].ToString(),
                        session_start = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[5],
                        IsActive = (bool)dataSet.Tables[0].Rows[0].ItemArray[6],
                        LastActive = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[7],
                        SessionExpires = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[8],
                        member_ip = dataSet.Tables[0].Rows[0].ItemArray[9].ToString(),
                    };
                    //get the member object

                    SqlConnection sql3 = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                    SqlCommand command3 = new SqlCommand();
                    command3.CommandText = @"usp_tbl_member_sel";
                    command3.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter p6 = new SqlParameter(@"p_member_id", ms.member_id);
                    //SqlParameter p4 = new SqlParameter(@"p_member_code", strResult);
                    command3.Parameters.Add(p6);
                    //command2.Parameters.Add(p4);
                    DataSet dataSet2 = new DataSet();
                    SqlDataAdapter da2 = new SqlDataAdapter();
                    using (sql3)
                    {
                        sql3.Open();
                        command3.Connection = sql3;
                        da2.SelectCommand = command3;
                        da2.Fill(dataSet2);
                    }
                    //create member object
                    Member memberCurrent = new Member()
                    {
                        id = (long)dataSet2.Tables[0].Rows[0].ItemArray[0],
                        email = dataSet2.Tables[0].Rows[0].ItemArray[1].ToString(),
                        Code = dataSet2.Tables[0].Rows[0].ItemArray[2].ToString(),
                        ConfirmCode = @"",
                        FirstName = dataSet2.Tables[0].Rows[0].ItemArray[3].ToString(),
                        LastName = dataSet2.Tables[0].Rows[0].ItemArray[4].ToString(),
                        regcode = dataSet2.Tables[0].Rows[0].ItemArray[5].ToString(),
                        created = (DateTime)dataSet2.Tables[0].Rows[0].ItemArray[6],
                        updated = (DateTime)dataSet2.Tables[0].Rows[0].ItemArray[7],
                        enabled = (bool)dataSet2.Tables[0].Rows[0].ItemArray[8],
                        confirmed = (bool)dataSet2.Tables[0].Rows[0].ItemArray[9],
                        country = dataSet2.Tables[0].Rows[0].ItemArray[10].ToString(),
                        state = dataSet2.Tables[0].Rows[0].ItemArray[11].ToString(),
                        gender = dataSet2.Tables[0].Rows[0].ItemArray[12].ToString(),
                        mobile = dataSet2.Tables[0].Rows[0].ItemArray[13].ToString(),
                        MobileCarrier = dataSet2.Tables[0].Rows[0].ItemArray[14].ToString(),
                        city = dataSet2.Tables[0].Rows[0].ItemArray[15].ToString(),
                        zipcode = dataSet2.Tables[0].Rows[0].ItemArray[16].ToString(),
                        address = dataSet2.Tables[0].Rows[0].ItemArray[17].ToString(),
                        address2 = dataSet2.Tables[0].Rows[0].ItemArray[18].ToString()
                    };

                    if ((memberCurrent.enabled.Equals(1)) && (memberCurrent.confirmed.Equals(1)) && (ms.IsActive.Equals(1)) && (DateTime.Now < ms.SessionExpires))
                    {
                    S3Cr3Tx = _S3Cr3Tx;
                        var memberBundle = Controllers.ebundle.GetEbundle(ms.member_email);
                        S3Cr3Tx.Email = ms.member_email;
                        S3Cr3Tx.Token = Controllers.uspEncDec.Enc(ms.member_email, memberBundle.strapikey);
                        S3Cr3Tx.AuthCode = Controllers.uspEncDec.Enc(ms.member_email, memberBundle.strauth);
                        MemoryStream mStream1 = new MemoryStream();
                        MemoryStream mStream2 = new MemoryStream();
                        TextWriter tpr = new Utf8StringWriter();
                        TextWriter tpu = new Utf8StringWriter();

                        RSACryptoServiceProvider rsaPr = new RSACryptoServiceProvider(4096);
                        rsaPr.FromXmlString(memberBundle.ky);
                        RSACryptoServiceProvider rsaPu = new RSACryptoServiceProvider(4096);
                        rsaPu.FromXmlString(memberBundle.kyp);
                        ExportPrivateKey(rsaPr, tpr);
                        if (tpr is not null) { 
                        S3Cr3Tx.PEMpri = tpr.ToString();
                        }
                        ExportPublicKey(rsaPu, tpu);
                        if (tpu is not null)
                        {
                            S3Cr3Tx.PEMpub = tpu.ToString();
                        }

                       
            }
                    else
            {
                _output = @"Please login again";
                Response.Redirect(@"http://localhost:8001/Login");
            }
        }
                else
                {
                    _output = @"Please login again";
                    Response.Redirect(@"http://localhost:8001/Login");
                }
            }
            catch(Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnPostView";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
            }
        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
        public string Message { get; set; } = "";

        public class NewK
        {
            public string email { get; set; }
            public string pd { get; set; }
            public string pd2 { get; set; }
        }
       
        private void Download_File(string strFile,string strName)
        {
           // Sending file to user.
            Response.ContentType = "application/pfx";
            
            Response.Headers.Add("Content-Disposition", "attachment; filename="+Path.GetFileName(strName));
            Response.SendFileAsync(strFile);
        }
        private string pkcs12File(Member member)
        {
            try
            {
                var memberBundle = Controllers.ebundle.GetEbundle(member.email);
                
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096);
                rsa.FromXmlString(memberBundle.ky);

                var certRequest = new CertificateRequest(new X500DistinguishedName(@"cn="+member.FirstName), rsa,HashAlgorithmName.SHA256,RSASignaturePadding.Pkcs1);
                var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
                //certificate.PrivateKey = rsa;
                //X509Certificate2 cert2 = certificate.CopyWithPrivateKey(rsa);
                byte[] p12 = certificate.Export(X509ContentType.Pkcs12, Controllers.uspEncDec.Dec(@"support@gratitech.com",member.regcode));
                string strFileName = _hostingEnvironment.WebRootPath + @"\"+ member.FirstName + member.LastName + @"-" + Guid.NewGuid().ToString() + @".p12";
                System.IO.File.WriteAllBytes(strFileName, p12);
                return strFileName;
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.apiPageCS.genfile";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
                return @"";
            }
        }
        private X509Certificate2 buildSelfSignedServerCertificate(Member member, string passcode)
        {
            SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName("localhost");
            sanBuilder.AddDnsName(Environment.MachineName);

            string CertName = @"s3cr3tx.com";

            X500DistinguishedName distinguishedName = new X500DistinguishedName(@"CN=" + CertName);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096))
            {
                rsa.FromXmlString(Controllers.ebundle.GetEbundle(member.email).ky);
                
                var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));


                request.CertificateExtensions.Add(
                   new X509EnhancedKeyUsageExtension(
                       new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

                request.CertificateExtensions.Add(sanBuilder.Build());

                var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
                //certificate.FriendlyName = @"";

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, passcode), passcode, X509KeyStorageFlags.UserKeySet|X509KeyStorageFlags.Exportable);
            }
        }
        private string pfxFile(Member member)
        {
            try
            {
                var memberBundle = Controllers.ebundle.GetEbundle(member.email);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096);
                rsa.FromXmlString(memberBundle.ky);

                var certRequest = new CertificateRequest(new X500DistinguishedName(@"cn=" + member.FirstName), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
                
                //certificate.PrivateKey = rsa;
                //X509Certificate2 cert2 = certificate.CopyWithPrivateKey(rsa);
                byte[] pfx = certificate.Export(X509ContentType.Pfx, Controllers.uspEncDec.Dec(@"support@gratitech.com", member.regcode));
                string strFileName = _hostingEnvironment.WebRootPath + @"\" + member.FirstName + member.LastName + @"-" + Guid.NewGuid().ToString() + @".pfx";
                System.IO.File.WriteAllBytes(strFileName,(pfx));
                return strFileName;
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.apiPageCS.genfile";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
                return @"";
            }
        }

        public void OnPostView([Bind("PEMpri", "PEMpub", "passcode")] s3cr3tx.Models.S3cr3tx _S3Cr3Tx)
        {
            try
            {
                if (Request.Cookies.TryGetValue(@"s3cr3tx", out string? strValue))
                {
                    if (Request.Form.TryGetValue("S3Cr3Tx.passcode", out Microsoft.Extensions.Primitives.StringValues Code))
                    {
                        S3Cr3Tx.passcode = Code[0];
                        //decrypt the cookie value
                        string strSessionID = Controllers.uspEncDec.Dec(@"support@gratitech.com", strValue);
                        //get the session object
                        string strConnection = "Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx"; // = @"Data Source=.;User Id=sa;Password=yourStrong!Password;Initial Catalog=s3cr3tx";

                        SqlConnection sql = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                        SqlCommand command = new SqlCommand();
                        command.CommandText = @"usp_tbl_member_sessions_selCode";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter p5 = new SqlParameter(@"p_ID", strSessionID);
                        //SqlParameter p4 = new SqlParameter(@"p_member_code", strResult);
                        command.Parameters.Add(p5);
                        //command2.Parameters.Add(p4);
                        DataSet dataSet = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter();
                        using (sql)
                        {
                            sql.Open();
                            command.Connection = sql;
                            da.SelectCommand = command;
                            da.Fill(dataSet);
                        }

                        MemberSession ms = new MemberSession()
                        {
                            id = (long)dataSet.Tables[0].Rows[0].ItemArray[0],
                            member_id = (long)dataSet.Tables[0].Rows[0].ItemArray[1],
                            member_email = dataSet.Tables[0].Rows[0].ItemArray[2].ToString(),
                            member_token = dataSet.Tables[0].Rows[0].ItemArray[3].ToString(),
                            FirstName = dataSet.Tables[0].Rows[0].ItemArray[4].ToString(),
                            session_start = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[5],
                            IsActive = (bool)dataSet.Tables[0].Rows[0].ItemArray[6],
                            LastActive = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[7],
                            SessionExpires = (DateTime)dataSet.Tables[0].Rows[0].ItemArray[8],
                            member_ip = dataSet.Tables[0].Rows[0].ItemArray[9].ToString(),
                        };
                        //get the member object

                        SqlConnection sql3 = new SqlConnection("Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx");//
                        SqlCommand command3 = new SqlCommand();
                        command3.CommandText = @"usp_tbl_member_sel";
                        command3.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter p6 = new SqlParameter(@"p_member_id", ms.member_id);
                        //SqlParameter p4 = new SqlParameter(@"p_member_code", strResult);
                        command3.Parameters.Add(p6);
                        //command2.Parameters.Add(p4);
                        DataSet dataSet2 = new DataSet();
                        SqlDataAdapter da2 = new SqlDataAdapter();
                        using (sql3)
                        {
                            sql3.Open();
                            command3.Connection = sql3;
                            da2.SelectCommand = command3;
                            da2.Fill(dataSet2);
                        }
                        //create member object
                        Member memberCurrent = new Member()
                        {
                            id = (long)dataSet2.Tables[0].Rows[0].ItemArray[0],
                            email = dataSet2.Tables[0].Rows[0].ItemArray[1].ToString(),
                            Code = dataSet2.Tables[0].Rows[0].ItemArray[2].ToString(),
                            ConfirmCode = @"",
                            FirstName = dataSet2.Tables[0].Rows[0].ItemArray[3].ToString(),
                            LastName = dataSet2.Tables[0].Rows[0].ItemArray[4].ToString(),
                            regcode = dataSet2.Tables[0].Rows[0].ItemArray[5].ToString(),
                            created = (DateTime)dataSet2.Tables[0].Rows[0].ItemArray[6],
                            updated = (DateTime)dataSet2.Tables[0].Rows[0].ItemArray[7],
                            enabled = (bool)dataSet2.Tables[0].Rows[0].ItemArray[8],
                            confirmed = (bool)dataSet2.Tables[0].Rows[0].ItemArray[9],
                            country = dataSet2.Tables[0].Rows[0].ItemArray[10].ToString(),
                            state = dataSet2.Tables[0].Rows[0].ItemArray[11].ToString(),
                            gender = dataSet2.Tables[0].Rows[0].ItemArray[12].ToString(),
                            mobile = dataSet2.Tables[0].Rows[0].ItemArray[13].ToString(),
                            MobileCarrier = dataSet2.Tables[0].Rows[0].ItemArray[14].ToString(),
                            city = dataSet2.Tables[0].Rows[0].ItemArray[15].ToString(),
                            zipcode = dataSet2.Tables[0].Rows[0].ItemArray[16].ToString(),
                            address = dataSet2.Tables[0].Rows[0].ItemArray[17].ToString(),
                            address2 = dataSet2.Tables[0].Rows[0].ItemArray[18].ToString()
                        };

                        if ((memberCurrent.enabled.Equals(1)) && (memberCurrent.confirmed.Equals(1)) && (ms.IsActive.Equals(1)) && (DateTime.Now < ms.SessionExpires))
                        {
                            //S3Cr3Tx = _S3Cr3Tx;
                            var memberBundle = Controllers.ebundle.GetEbundle(ms.member_email);
                            S3Cr3Tx.Email = ms.member_email;
                            S3Cr3Tx.Token = memberBundle.strapikey;
                            S3Cr3Tx.AuthCode = memberBundle.strauth;
                            MemoryStream mStream1 = new MemoryStream();
                            MemoryStream mStream2 = new MemoryStream();
                            TextWriter tpr = new Utf8StringWriter();
                            TextWriter tpu = new Utf8StringWriter();

                            RSACryptoServiceProvider rsaPr = new RSACryptoServiceProvider(4096);
                            rsaPr.FromXmlString(memberBundle.ky);
                            RSACryptoServiceProvider rsaPu = new RSACryptoServiceProvider(4096);
                            rsaPu.FromXmlString(memberBundle.kyp);
                            ExportPrivateKey(rsaPr, tpr);
                            if (tpr is not null)
                            {
                                S3Cr3Tx.PEMpri = tpr.ToString();
                            }
                            ExportPublicKey(rsaPu, tpu);
                            if (tpu is not null)
                            {
                                S3Cr3Tx.PEMpub = tpu.ToString();
                            }
                            // application's content root path
                            string contentRootPath = _hostingEnvironment.ContentRootPath;

                            // application's publishing path
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string strNewFile = @"s3cr3tx_" + memberCurrent.FirstName + memberCurrent.LastName + @"-" + Guid.NewGuid().ToString();
                            string strNewZipFile = @"\" + strNewFile + ".zip";
                            System.IO.File.Copy(webRootPath + @"/s3cr3tx.zip", webRootPath + strNewZipFile);
                            var zipFile = ZipFile.Open(webRootPath + strNewZipFile, ZipArchiveMode.Update);
                            //string strFileName = pkcs12File(memberCurrent);
                            X509Certificate2 cert2 = buildSelfSignedServerCertificate(memberCurrent, S3Cr3Tx.passcode);
                            Byte[] certbytes = cert2.Export(X509ContentType.Pfx, S3Cr3Tx.passcode);
                            string strFileName2 = memberCurrent.FirstName + @"-" + Guid.NewGuid().ToString() + @".pfx";
                            System.IO.File.WriteAllBytes(_hostingEnvironment.WebRootPath + @"\" + strFileName2, certbytes);


                            //string strFileName = pfxFile(memberCurrent); 
                            zipFile.CreateEntryFromFile(_hostingEnvironment.WebRootPath + @"\" + strFileName2, Path.GetFileName(strFileName2));
                            //using (FileStream zipToOpen = new FileStream(webRootPath + strNewZipFile, FileMode.Open))
                            //{
                            //    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            //    {
                            //        ZipArchiveEntry readmeEntry = archive.CreateEntry(strNewFile + @".pem");
                            //        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                            //        {
                            //            writer.Write(S3Cr3Tx.PEMpri);
                            //            writer.Flush();
                            //        }
                            //    }
                            //}
                            //string strPEM = S3Cr3Tx.PEMpub + System.Environment.NewLine + S3Cr3Tx.PEMpri;
                            Download_File(webRootPath + "/" + strFileName2, strFileName2);

                        }
                        else
                        {
                            _output = @"Please login again";
                            Response.Redirect(@"http://localhost:8001/Login");
                        }
                    }
                }
                else
                {
                    _output = @"Please login again";
                    Response.Redirect(@"http://localhost:8001/Login");
                }
            }
            catch (Exception ex)
            {
                //string strResult = @"";
                string strSource = @"s3cr3tx.api.RegisterPageCS.OnPostView";
                s3cr3tx.Controllers.ValuesController.LogIt(ex.GetBaseException().ToString(), strSource);
                Redirect(@"http://localhost:8001/Login");
            }
        }
         
                            
                    

              

            

        
        private static void ExportPrivateKey(RSACryptoServiceProvider csp, TextWriter outputStream)
        {
            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
            }
        }
        private static void ExportPublicKey(RSACryptoServiceProvider csp, TextWriter outputStream)
        {
            var parameters = csp.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // SEQUENCE
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // BIT STRING
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // # of unused bits
                        bitStringWriter.Write((byte)0x30); // SEQUENCE
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                            var paramsLength = (int)paramsStream.Length;
                            EncodeLength(bitStringWriter, paramsLength);
                            bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                        }
                        var bitStringLength = (int)bitStringStream.Length;
                        EncodeLength(innerWriter, bitStringLength);
                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                    }
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END PUBLIC KEY-----");
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }
    }
}
