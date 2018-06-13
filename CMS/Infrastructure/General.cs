using System;
using System.Web;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace CMS.Infrastructure
{
    public class General
    {
        private static readonly string _awsAccessKey = ConfigurationManager.AppSettings["amazon.s3_access_key_id"];
        private static readonly string _awsSecretKey = ConfigurationManager.AppSettings["amazon.s3_secret_access_key"];
        private static readonly string _bucketName = ConfigurationManager.AppSettings["amazon.bucket_id"];
        private static readonly string _s3Url = ConfigurationManager.AppSettings["amazon.s3.url"];

        public static string UploadToS3(HttpPostedFileBase file, string folderName)
        {
            string retStr = string.Empty;

            try
            {
                IAmazonS3 client;

                AmazonS3Config config = new AmazonS3Config();
                config.ServiceURL = _s3Url;
                config.AuthenticationRegion = "EU-WEST-1";
                config.RegionEndpoint = RegionEndpoint.GetBySystemName("eu-west-1");

                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                delegate (
                    object sender,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                string fileName = file.FileName.Replace(' ', '-');

                var objectKey = HttpUtility.UrlPathEncode(string.Format("{0}/{1}", folderName, fileName));

                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey, config))
                {
                    var request = new PutObjectRequest()
                    {
                        BucketName = _bucketName,
                        CannedACL = S3CannedACL.Private,
                        Key = objectKey,
                        InputStream = file.InputStream
                    };

                    PutObjectResponse response = client.PutObject(request);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        retStr = string.Format("{0}{1}", _s3Url, objectKey);
                    }
                    else
                    {
                        retStr = "Error: HttpStatusCode not OK";
                    }
                }
            }
            catch (Exception ex)
            {
                retStr = "Error:" + ex.ToString();

            }

            return retStr;
        }
    }
}