using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InstaMelody.API.Constants;
using InstaMelody.API.Helpers;
using InstaMelody.Business;
using InstaMelody.Infrastructure;
using InstaMelody.Model.ApiModels;
using NLog;

namespace InstaMelody.API.Controllers
{
    [RoutePrefix(Routes.PrefixUpload01)]
    public class UploadsController : ApiController
    {
        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.RouteUploadToken)]
        public async Task<HttpResponseMessage> UploadFile(Guid sessionToken, Guid token)
        {
            if (!token.Equals(default(Guid)) && !sessionToken.Equals(default(Guid)))
            {
                try
                {
                    InstaMelodyLogger.Log(
                       string.Format(
                           "Upload file request - Session token: {0}, File upload token: {1}.",
                           sessionToken, token),
                       LogLevel.Trace);

                    var bll = new FileBll();
                    var isUploadValid = bll.CanUploadFile(sessionToken, token);

                    if (isUploadValid)
                    {
                        if (!Request.Content.IsMimeMultipartContent("form-data"))
                        {
                            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFileUpload);
                        }

                        var results = new List<ApiFile>();
                        var provider = await this.Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                        var files = provider.Files;
                        foreach (var file in files)
                        {
                            var uploadedFile = await this.TryUploadFile(file, token);
                            if (uploadedFile != null)
                            {
                                results.Add(uploadedFile);
                                bll.ExpireToken(token);
                            }
                            else
                            {
                                bll.DeleteAssociatedFileInfo(token);
                                bll.ExpireToken(token);

                                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFileUpload);
                            }
                        }

                        return this.Request.CreateResponse(HttpStatusCode.OK, results);
                    }

                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Exceptions.FailedFileUpload);
                }
                catch (Exception exc)
                {
                    InstaMelodyLogger.Log(string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace), LogLevel.Error);

                    if (exc is UnauthorizedAccessException)
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exc.Message);
                    }

                    if (exc is DataException)
                    {
                        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, exc.Message);
                    }

                    return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message, exc);
                }
            }

            InstaMelodyLogger.Log("Received NULL UploadFile request", LogLevel.Trace);
            return this.Request.CreateErrorResponse(HttpStatusCode.NoContent, Exceptions.NullFileUpload);
        }

        /// <summary>
        /// Tries the upload file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private async Task<ApiFile> TryUploadFile(HttpContent file, Guid token)
        {
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(
                            Request.RequestUri.AbsolutePath, string.Empty);

            var bll = new FileBll();
            var uploadMetadata = bll.GetTokenInfo(token);
            if (uploadMetadata == null) return null;
            
            // read filestream
            var fileStream = await file.ReadAsByteArrayAsync();
                
            // detect mime type from sniffer & get filepath
            var type = MimeSniffer.GetMime(fileStream);
            var uploadPath = Business.Utilities.GetFilePath(type, file.Headers.ContentType.MediaType);
            if (string.IsNullOrWhiteSpace(uploadPath)) return null;

            // get destination file path mapping
            var path = HttpContext.Current.Server.MapPath("~/" + uploadPath);

            // format filename and check against token
            var fileName = file.Headers.ContentDisposition.FileName;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }
            }
            if (fileName == null 
                || !fileName.Equals(uploadMetadata.FileName)) return null;

            // save the file
            var uploadPathFile = string.Format("{0}/{1}", path, uploadMetadata.FileName);
            File.WriteAllBytes(uploadPathFile, fileStream);

            // return the file info
            var fileInfo = new FileInfo(uploadPathFile);
            return new ApiFile(
                fileInfo.Name,
                string.Format("{0}/{1}", uploadPath, fileInfo.Name),
                fileInfo.Length / 1024);
        }
    }
}
