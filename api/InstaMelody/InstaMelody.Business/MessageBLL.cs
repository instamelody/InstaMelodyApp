using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;

namespace InstaMelody.Business
{
    public class MessageBLL
    {
        #region Public Methods

        #region User Chats

        /// <summary>
        /// Starts the chat.
        /// </summary>
        /// <param name="friend">The friend.</param>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Only Friends of the authenticated User can be added to a Chat.</exception>
        /// <exception cref="System.Data.DataException">Failed to create Chat.</exception>
        public object StartChat(User friend, Message message, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // check requested user is friends with session user if setting is set
            var userBll = new UserBLL();
            var foundFriend = userBll.FindUser(friend);
            if (Settings.Default.UsersCanOnlyMessageFriends 
                && !Utilities.AreUsersFriends(sessionUser.Id, foundFriend.Id))
            {
                throw new UnauthorizedAccessException("Only Friends of the authenticated User can be added to a Chat.");
            }

            // create chat
            var dal = new Chats();
            var newChat = dal.CreateChat();
            if (newChat == null)
            {
                throw new DataException("Failed to create Chat.");
            }

            // add users to chat
            dal.AddUserToChat(new ChatUser
            {
                ChatId = newChat.Id,
                UserId = foundFriend.Id
            });
            dal.AddUserToChat(new ChatUser
            {
                ChatId = newChat.Id,
                UserId = sessionUser.Id
            });

            // create chat message
            var chatMessage = this.CreateChatMessage(newChat, sessionUser, message);

            // TODO: send push notification to requested user

            // return chat & file upload token (if necessary)
            if (chatMessage.Item2 != null)
            {
                return new ApiChatFileUpload
                {
                    Chat = this.GetChat(newChat),
                    FileUploadToken = chatMessage.Item2
                };
            }

            return newChat;
        }

        /// <summary>
        /// Gets the chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">No valid Chat Id provided.</exception>
        /// <exception cref="System.Data.DataException">Could not find the requested Chat, Id {0}.</exception>
        public Chat GetChat(Chat chat, Guid sessionToken)
        {
            // check token
            Utilities.GetUserBySession(sessionToken);

            if (chat.Id.Equals(default(Guid)))
            {
                throw new ArgumentException("No valid Chat Id provided.");
            }

            return this.GetChat(chat);
        }

        /// <summary>
        /// Adds the user to chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="requestedUser">The requested user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Only Friends of the authenticated User can be added to a Chat.</exception>
        /// <exception cref="System.ArgumentException">No valid Chat Id provided.</exception>
        public Chat AddUserToChat(Chat chat, User requestedUser, Guid sessionToken)
        {
            // check token
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // check requested user is friends with session user if setting is set
            var userBll = new UserBLL();
            var foundFriend = userBll.FindUser(requestedUser);
            if (Settings.Default.UsersCanOnlyMessageFriends
                && !Utilities.AreUsersFriends(sessionUser.Id, foundFriend.Id))
            {
                throw new UnauthorizedAccessException("Only Friends of the authenticated User can be added to a Chat.");
            }

            if (chat.Id.Equals(default(Guid)))
            {
                throw new ArgumentException("No valid Chat Id provided.");
            }

            var dal = new Chats();
            var foundChat = dal.GetChatById(chat.Id);

            // add requested user to chat
            dal.AddUserToChat(new ChatUser
            {
                ChatId = foundChat.Id,
                UserId = requestedUser.Id
            });

            // TODO: send push notifications to all users in chat

            return this.GetChat(foundChat);
        }

        /// <summary>
        /// Sends the chat message.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Cannot send a message to a Chat that this User is not a member of.</exception>
        public object SendChatMessage(Chat chat, Message message, Guid sessionToken)
        {
            // check token
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var dal = new Chats();

            // check user is part of chat
            var chatUsers = dal.GetUsersInChat(sessionUser.Id);
            if (!chatUsers.Any(u => u.UserId.Equals(sessionUser.Id)))
            {
                throw new UnauthorizedAccessException("Cannot send a message to a Chat that this User is not a member of.");
            }

            // create message
            var chatMessage = this.CreateChatMessage(chat, sessionUser, message);

            // TODO: send push notification to all users in chat

            // return new chat message
            if (chatMessage.Item2 != null)
            {
                return new ApiChatFileUpload
                {
                    ChatMessage = chatMessage.Item1,
                    FileUploadToken = chatMessage.Item2
                };
            }

            return chatMessage;
        }

        /// <summary>
        /// Gets all user chats.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        public IList<Chat> GetAllUserChats(Guid sessionToken)
        {
            List<Chat> results = null;

            // check token
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // retrieve all chats for user
            var dal = new Chats();
            var chats = dal.GetChatsByUserId(sessionUser.Id);
            if (chats != null && chats.Any())
            {
                results = chats.Select(this.GetChat).ToList();
            }

            // return all chats
            return results;
        }

        /// <summary>
        /// Removes the user from chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.UnauthorizedAccessException">Cannot remove User from a Chat they do not belong to.</exception>
        public void RemoveUserFromChat(Chat chat, Guid sessionToken)
        {
            // check token
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            if (chat.Id.Equals(default(Guid)))
            {
                throw new ArgumentException();
            }

            // update chat
            var dal = new Chats();
            var chatUsers = dal.GetUsersInChat(chat.Id);
            if (!chatUsers.Any(c => c.UserId.Equals(sessionUser.Id)))
            {
                throw new UnauthorizedAccessException("Cannot remove User from a Chat they do not belong to.");
            }

            dal.DeleteChatUser(chat.Id, sessionUser.Id);

            // TODO: send push notification to other chat users
        }

        #endregion User Chats

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not find the requested Chat, Id {0}.</exception>
        private Chat GetChat(Chat chat)
        {
            var dal = new Chats();

            //// check user is part of chat
            //var chatUsers = dal.GetUsersInChat(chat.Id);
            //if (!chatUsers.Any(c => c.UserId.Equals(sessionUser.Id)))
            //{
            //    throw new UnauthorizedAccessException("Cannot get a chat that this User is not a member of.");
            //}

            // get chat
            var foundChat = dal.GetChatById(chat.Id);
            if (foundChat == null)
            {
                throw new DataException("Could not find the requested Chat, Id {0}.");
            }

            // get chat users
            var users = this.GetUsersByChat(foundChat);
            foreach (var user in users)
            {
                foundChat.Users.Add(user);
            }
            
            // get chat messages
            var messages = dal.GetMessagesByChat(foundChat.Id);
            foreach (var message in messages)
            {
                var messageRecord = this.GetMessageByChatMessage(message);
                if (messageRecord != null)
                {
                    message.Message = messageRecord;
                    foundChat.Messages.Add(message);
                }
            }

            // return chat
            return foundChat;
        }

        /// <summary>
        /// Gets the users by chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <returns></returns>
        private IList<User> GetUsersByChat(Chat chat)
        {
            List<User> results = null;

            if (chat != null)
            {
                results = new List<User>();

                var dal = new Chats();
                var users = dal.GetUsersInChat(chat.Id);

                var userBll = new UserBLL();
                foreach (var chatUser in users)
                {
                    var user = userBll.FindUser(new User { Id = chatUser.UserId });
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the message by chat message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private Message GetMessageByChatMessage(ChatMessage message)
        {
            // get message by chat message
            var dal = new Messages();
            var foundMessage = dal.GetMessageById(message.MessageId);
            if (foundMessage == null)
            {
                return null;
            }

            // get attachments
            switch (foundMessage.MediaType)
            {
                case MediaTypeEnum.Image:
                    foundMessage.Image = this.GetMessageImage(foundMessage);
                    break;
                case MediaTypeEnum.Video:
                    foundMessage.Video = this.GetMessageVideo(foundMessage);
                    break;
                case MediaTypeEnum.Melody:
                    foundMessage.UserMelody = this.GetMessageMelody(foundMessage);
                    break;
            }

            // return message
            return foundMessage;
        }

        /// <summary>
        /// Gets the message image.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid Message.</exception>
        private Image GetMessageImage(Message message)
        {
            if (message == null || message.Id.Equals(default(Guid)))
            {
                throw new ArgumentException("Invalid Message.");
            }

            Image result = null;

            // get message image
            var dal = new Messages();
            var messageImage  = dal.GetImageByMessageId(message.Id);
            if (messageImage != null)
            {
                result = new Image();

                var imgDal = new Images();
                var image = imgDal.GetImageById(messageImage.Id);
                if (image == null) return null;

                image.FilePath = Utilities.GetFilePath(image.FileName, MediaTypeEnum.Image);
            }

            return result;
        }

        /// <summary>
        /// Gets the message video.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid Message.</exception>
        private Video GetMessageVideo(Message message)
        {
            if (message == null || message.Id.Equals(default(Guid)))
            {
                throw new ArgumentException("Invalid Message.");
            }
            Video result = null;

            // get message image
            var dal = new Messages();
            var messageVideo = dal.GetVideoByMessageId(message.Id);
            if (messageVideo != null)
            {
                result = new Video();

                var vidDal = new Videos();
                var video = vidDal.GetVideoById(messageVideo.Id);
                if (video == null) return null;

                video.FilePath = Utilities.GetFilePath(video.FileName, MediaTypeEnum.Video);
            }

            return result;
        }

        private UserMelody GetMessageMelody(Message message)
        {
            // TODO:

            // get message melody

            // get melody

            // get file path

            // return melody if found

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the chat message.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private Tuple<ChatMessage, FileUploadToken> CreateChatMessage(Chat chat, User sender, Message message)
        {
            var dal = new Chats();
            if (chat.Id.Equals(default(Guid)))
            {
                throw new ArgumentException("A valid Chat Id must be provided.");
            }

            var foundChat = dal.GetChatById(chat.Id);
            if (foundChat == null)
            {
                throw  new ArgumentException(string.Format("Chat Id {0} not found.", chat.Id));
            }

            // create message
            var newMessage = this.CreateMessage(message, sender.Id);

            // create chat message
            var newChatMessage = dal.CreateChatMessage(new ChatMessage
            {
                ChatId = foundChat.Id,
                MessageId = newMessage.Item1.Id,
                SenderId = sender.Id
            });
            newChatMessage.Message = newMessage.Item1;

            // return chat message and upload token
            return new Tuple<ChatMessage, FileUploadToken>(newChatMessage, newMessage.Item2);
        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="senderId">The sender identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">
        /// Cannot create a NULL Message.
        /// or
        /// UserId is not valid.
        /// </exception>
        private Tuple<Message, FileUploadToken> CreateMessage(Message message, Guid senderId)
        {
            if (message == null)
            {
                throw new DataException("Cannot create a NULL Message.");
            }
            if (senderId.Equals(default(Guid)))
            {
                throw new DataException("UserId is not valid.");
            }

            // create message
            var dal = new Messages();
            var addedMessage = dal.AddMessage(message);

            FileUploadToken addedToken = null;
            var fileUploadToken = new FileUploadToken
            {
                UserId = senderId,
                DateCreated = DateTime.UtcNow
            };

            // create attachment
            if (message.Image != null)
            {
                var newImage = this.CreateMessageImage(addedMessage.Id, message.Image);
                addedMessage.Image = newImage.Image;

                fileUploadToken.MediaType = FileUploadTypeEnum.MessageImage;
                fileUploadToken.FileName = addedMessage.Image.FileName;
            }
            else if (message.Video != null)
            {
                var newVideo = this.CreateMessageVideo(addedMessage.Id, message.Video);
                addedMessage.Video = newVideo.Video;

                fileUploadToken.MediaType = FileUploadTypeEnum.MessageVideo;
                fileUploadToken.FileName = addedMessage.Video.FileName;
            }
            else if (message.UserMelody != null)
            {
                var newMelody = this.CreateMessageMelody(addedMessage.Id, message.UserMelody);
                addedMessage.UserMelody = newMelody.UserMelody;

                // TODO: determine how to handle user melody file upload tokens
            }

            if (!fileUploadToken.MediaType.Equals(FileUploadTypeEnum.Unknown))
            {
                var fileBll = new FileBLL();
                addedToken = fileBll.CreateToken(fileUploadToken);
            }

            return new Tuple<Message, FileUploadToken>(addedMessage, addedToken);
        }

        /// <summary>
        /// Creates the message image.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a MessageImage record.</exception>
        private MessageImage CreateMessageImage(Guid messageId, Image image)
        {
            // create image
            var fileBll = new FileBLL();
            var addedImage = fileBll.AddImage(image);

            // create message image
            var dal = new Messages();
            var addedMessageImage = dal.AddMessageImage(messageId, addedImage.Id);
            if (addedMessageImage == null)
            {
                throw new DataException("Failed to create a MessageImage record.");
            }

            addedImage.FilePath = Utilities.GetFilePath(addedImage.FileName, MediaTypeEnum.Image);
            addedMessageImage.Image = addedImage;

            // return message image
            return addedMessageImage;
        }

        /// <summary>
        /// Creates the message video.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="video">The video.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a MessageVideo record.</exception>
        private MessageVideo CreateMessageVideo(Guid messageId, Video video)
        {
            // create image
            var fileBll = new FileBLL();
            var addedVideo = fileBll.AddVideo(video);

            // create message image
            var dal = new Messages();
            var addedMessageVideo = dal.AddMessageVideo(messageId, addedVideo.Id);
            if (addedMessageVideo == null)
            {
                throw new DataException("Failed to create a MessageVideo record.");
            }

            addedVideo.FilePath = Utilities.GetFilePath(addedVideo.FileName, MediaTypeEnum.Video);
            addedMessageVideo.Video = addedVideo;

            // return message image
            return addedMessageVideo;
        }

        private MessageMelody CreateMessageMelody(Guid messageId, UserMelody melody)
        {
            // TODO:

            // create melody

            // create message melody

            // get file path

            // return message melody

            throw new NotImplementedException();
        }

        #endregion Private Methods
        
        #region Internal Methods

        /// <summary>
        /// Deletes the message image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        internal void DeleteMessageImage(int imageId)
        {
            var dal = new Messages();
            dal.DeleteMessageImageByImageId(imageId);
        }

        /// <summary>
        /// Deletes the message video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        internal void DeleteMessageVideo(int videoId)
        {
            var dal = new Messages();
            dal.DeleteMessageVideoByVideoId(videoId);
        }

        internal void DeleteMessageUserMelody()
        {
            // TODO:
            throw new NotImplementedException();
        }

        #endregion Internal Methods
    }
}
