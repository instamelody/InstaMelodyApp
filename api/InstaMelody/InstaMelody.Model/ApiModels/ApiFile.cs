using System.Runtime.Serialization;

namespace InstaMelody.Model.ApiModels
{
    /// <summary>
    /// The Api File.
    /// </summary>
    [DataContract]
    public class ApiFile
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public long Size { get; set; }

        public ApiFile(string n, string p, long s)
        {
            Name = n;
            Path = p;
            Size = s;
        }
    }
}
