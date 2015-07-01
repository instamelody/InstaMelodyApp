using System;
using System.Data.SqlClient;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class FileUploadToken
    {
        public Guid Token { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public FileUploadTypeEnum MediaType { get; set; }

        public DateTime DateExpires { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsExpired
        {
            get { return DateExpires < DateTime.UtcNow; }
        }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public FileUploadToken ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Token = (Guid)dataReader["Token"];
            this.UserId = (Guid)dataReader["UserId"];
            this.FileName = Convert.ToString(dataReader["FileName"]);
            this.MediaType = (FileUploadTypeEnum)Enum.Parse(typeof(FileUploadTypeEnum), Convert.ToString(dataReader["MediaType"]));
            this.DateExpires = Convert.ToDateTime(dataReader["DateExpires"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
