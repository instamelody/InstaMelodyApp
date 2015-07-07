namespace InstaMelody.Model.ApiModels
{
    public class ApiChatFileUpload
    {
        public Chat Chat { get; set; }

        public ChatMessage ChatMessage { get; set; }

        public FileUploadToken FileUploadToken { get; set; }
    }
}
