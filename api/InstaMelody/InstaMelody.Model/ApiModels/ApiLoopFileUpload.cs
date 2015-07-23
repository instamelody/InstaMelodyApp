using System.Collections.Generic;

namespace InstaMelody.Model.ApiModels
{
    public class ApiLoopFileUpload
    {
        public UserLoop Loop { get; set; }

        public IList<FileUploadToken> FileUploadTokens { get; set; } 
    }
}
