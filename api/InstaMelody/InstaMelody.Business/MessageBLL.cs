using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Infrastructure.Enums;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;
using NLog;

namespace InstaMelody.Business
{
    public class MessageBll
    {
        #region Public Methods

        #region User Chats

        /// <summary>
        /// Starts the chat.
        /// </summary>
        /// <param name="friend">The friend.</param>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="chatName">Name of the chat.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Only Friends of the authenticated User can be added to a Chat.</exception>
        /// <exception cref="System.Data.DataException">Failed to create Chat.</exception>
        public object StartChat(User friend, Message message, Guid sessionToken, string chatName = null)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // check requested user is friends with session user if setting is set
            var userBll = new UserBll();
            var foundFriend = userBll.FindUser(friend);
            if (Settings.Default.UsersCanOnlyMessageFriends 
                && !Utilities.AreUsersFriends(sessionUser.Id, foundFriend.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Only Friends of the authenticated User can be added to a Chat. User Id: {0}, Requested User Id: {1}, Token: {2}",
                        sessionUser.Id, foundFriend.Id, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Only Friends of the authenticated User can be added to a Chat.");
            }

            // create chat
            var dal = new Chats();
            var newChat = dal.CreateChat(chatName);
            if (newChat == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create Chat. User Id: {0}, Requested User Id: {1}, Token: {2}", 
                        sessionUser.Id, foundFriend.Id, sessionToken), LogLevel.Error);
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

            if (message == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot start a Chat with a NULL message. User Id: {0}, Requested User Id: {1}, Token: {2}, Chat Id: {3}",
                        sessionUser.Id, foundFriend.Id, sessionToken, newChat.Id), LogLevel.Error);
                throw new DataException("Cannot start a Chat with a NULL message.");
            }

            // create chat message
            var chatMessage = CreateChatMessage(newChat, sessionUser, message);

            InstaMelodyLogger.Log(
                        string.Format("Chat Created for User: {0}, Requestor Id: {1}",
                            foundFriend.Id, sessionUser.Id), LogLevel.Error);

            // send push notification to requested user
            Utilities.SendPushNotification(foundFriend.Id, sessionUser.DisplayName, APNSTypeEnum.ChatCreated, newChat.Id);

            // return chat & file upload token (if necessary)
            if (chatMessage.Item2 != null)
            {
                return new ApiChatFileUpload
                {
                    Chat = GetChat(newChat),
                    FileUploadToken = chatMessage.Item2
                };
            }

            return GetChat(newChat);
        }

        /// <summary>
        /// Starts the chat.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="chatName">Name of the chat.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create Chat.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Only Friends of the authenticated User can be added to a Chat.</exception>
        public object StartChat(IList<User> users, Message message, Guid sessionToken, string chatName = null)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            var userBll = new UserBll();

            // create chat
            var dal = new Chats();
            var newChat = dal.CreateChat(chatName);
            if (newChat == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create Chat. User Id: {0}, Token: {1}",
                        sessionUser.Id, sessionToken), LogLevel.Error);
                throw new DataException("Failed to create Chat.");
            }

            InstaMelodyLogger.Log(
                        string.Format("Creating Chat for {0} total Users.",
                            users.Count + 1), LogLevel.Error);

            // add users to chat
            var foundUserIds = new List<Guid>();
            foreach (var user in users)
            {
                var foundUser = userBll.FindUser(user);
                if (Settings.Default.UsersCanOnlyMessageFriends
                    && !Utilities.AreUsersFriends(sessionUser.Id, foundUser.Id))
                {
                    dal.DeleteChat(newChat.Id);
                    InstaMelodyLogger.Log(
                        string.Format("Only Friends of the authenticated User can be added to a Chat. User Id: {0}, Requested User Id: {1}, Token: {2}",
                            sessionUser.Id, foundUser.Id, sessionToken), LogLevel.Error);
                    throw new UnauthorizedAccessException("Only Friends of the authenticated User can be added to a Chat.");
                }
                foundUserIds.Add(foundUser.Id);
                dal.AddUserToChat(new ChatUser
                {
                    ChatId = newChat.Id,
                    UserId = foundUser.Id
                });
            }
            dal.AddUserToChat(new ChatUser
            {
                ChatId = newChat.Id,
                UserId = sessionUser.Id
            });

            // create chat message
            var chatMessage = CreateChatMessage(newChat, sessionUser, message);

            InstaMelodyLogger.Log(
                        string.Format("Chat Created for Users: {0}, Requestor Id: {1}",
                            string.Join(",", foundUserIds), sessionUser.Id), LogLevel.Error);

            // send push notification to requested user
            Utilities.SendPushNotification(foundUserIds, sessionUser.DisplayName, APNSTypeEnum.ChatCreated, newChat.Id);

            // return chat & file upload token (if necessary)
            if (chatMessage.Item2 != null)
            {
                return new ApiChatFileUpload
                {
                    Chat = GetChat(newChat),
                    FileUploadToken = chatMessage.Item2
                };
            }

            return GetChat(newChat);
        }

        /// <summary>
        /// Gets the chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="limitRecords">The limit records.</param>
        /// <param name="fromId">From identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">No valid Chat Id provided.</exception>
        /// <exception cref="System.Data.DataException">Could not find the requested Chat, Id {0}.</exception>
        public Chat GetChat(Chat chat, Guid sessionToken, int? limitRecords = null, int? fromId = null)
        {
            // check token
            Utilities.GetUserBySession(sessionToken);

            if (chat.Id.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("No valid Chat Id provided.", LogLevel.Error);
                throw new ArgumentException("No valid Chat Id provided.");
            }

            return GetChat(chat, limitRecords, fromId);
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
            var userBll = new UserBll();
            var foundUser = userBll.FindUser(requestedUser);
            if (Settings.Default.UsersCanOnlyMessageFriends
                && !Utilities.AreUsersFriends(sessionUser.Id, foundUser.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Only Friends of the authenticated User can be added to a Chat. User Id:{0}, Requested User Id:{1}, Token: {2}",
                        sessionUser.Id, foundUser.Id, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Only Friends of the authenticated User can be added to a Chat.");
            }

            if (chat.Id.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("No valid Chat Id provided.", LogLevel.Error);
                throw new ArgumentException("No valid Chat Id provided.");
            }

            var dal = new Chats();
            var foundChat = dal.GetChatById(chat.Id);

            // add requested user to chat
            dal.AddUserToChat(new ChatUser
            {
                ChatId = foundChat.Id,
                UserId = foundUser.Id
            });

            // send push notifications to all users in chat
            var users = dal.GetUsersInChat(chat.Id);
            var userIds = users.Select(u => u.UserId);
            Utilities.SendPushNotification(userIds.ToList(), APNSTypeEnum.ChatNewUser, foundChat.Id);

            return GetChat(foundChat);
        }

        /// <summary>
        /// Sends the chat message.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Cannot find requested Chat.</exception>
        /// <exception cref="UnauthorizedAccessException">Cannot send a message to a Chat that User is not a member of.</exception>
        public object SendChatMessage(Chat chat, Message message, Guid sessionToken)
        {
            // check token
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var dal = new Chats();
            var foundChat = dal.GetChatById(chat.Id);
            if (foundChat == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot find requested Chat. Chat Id: {0}, User Id: {1}, Token: {2}",
                        chat.Id, sessionUser.Id, sessionToken), LogLevel.Error);
                throw new ArgumentException("Cannot find requested Chat.");
            }

            // check user is part of chat
            var chatUsers = dal.GetUsersInChat(foundChat.Id);
            if (!chatUsers.Any(u => u.UserId.Equals(sessionUser.Id)))
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot send a message to a Chat that User is not a member of. Chat Id: {0}, User Id: {1}, Token: {2}",
                        foundChat.Id, sessionUser.Id, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Cannot send a message to a Chat that User is not a member of.");
            }

            // create message
            var chatMessage = CreateChatMessage(chat, sessionUser, message);

            // send push notification to all users in chat
            var userIds = chatUsers.Select(u => u.UserId).Where(i => !i.Equals(sessionUser.Id));
            Utilities.SendPushNotification(userIds.ToList(), sessionUser.DisplayName, 
                APNSTypeEnum.ChatNewMessage, chatMessage.Item1.ChatId, chatMessage.Item1.Id);

            // return new chat message
            if (chatMessage.Item2 != null)
            {
                return new ApiChatFileUpload
                {
                    Chat = dal.GetChatById(foundChat.Id),
                    ChatMessage = chatMessage.Item1,
                    FileUploadToken = chatMessage.Item2
                };
            }

            return chatMessage.Item1;
        }

        /// <summary>
        /// Gets the chat message.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="chatMessage">The chat message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Cannot find requested Chat.
        /// or
        /// Cannot find requested Chat Message.
        /// </exception>
        public ChatMessage GetChatMessage(Chat chat, ChatMessage chatMessage, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var dal = new Chats();
            var foundChat = dal.GetChatById(chat.Id);
            if (foundChat == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot find requested Chat. Chat Id: {0}, User Id: {1}, Token: {2}",
                        chat.Id, sessionUser.Id, sessionToken), LogLevel.Error);
                throw new ArgumentException("Cannot find requested Chat.");
            }

            var message = dal.GetChatMessageById(chatMessage.Id);
            if (message == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot find requested Chat Message. Chat Id: {0}, Message Id: {1}, User Id: {2}, Token: {3}",
                        chat.Id, chatMessage.Id, sessionUser.Id, sessionToken), LogLevel.Error);
                throw new ArgumentException("Cannot find requested Chat Message.");
            }

            var messageRecord = GetMessageByChatMessage(message);
            if (messageRecord != null)
            {
                message.Message = messageRecord;
                foundChat.Messages.Add(message);
            }

            return message;
        }

        /// <summary>
        /// Gets all user chats.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        public IList<Chat> GetAllUserChats(Guid sessionToken)
        {
            List<Chat> results = new List<Chat>();

            // check token
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            // retrieve all chats for user
            var dal = new Chats();
            var chats = dal.GetChatsByUserId(sessionUser.Id);
            if (chats != null && chats.Any())
            {
                results = chats.Select(chat => GetChat(chat)).ToList();
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
                InstaMelodyLogger.Log(
                    string.Format("Cannot remove User from a Chat they do not belong to. Chat Id: {0}, User Id: {1}, Token: {2}", 
                        chat.Id, sessionUser.Id, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Cannot remove User from a Chat they do not belong to.");
            }

            dal.DeleteChatUser(chat.Id, sessionUser.Id);

            // send push notification to all users in chat
            var userIds = chatUsers.Select(u => u.UserId).Where(i => !i.Equals(sessionUser.Id));
            Utilities.SendPushNotification(userIds.ToList(), APNSTypeEnum.ChatRemoveUser, chat.Id);
        }

        #endregion User Chats

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="limitRecords">The limit records.</param>
        /// <param name="fromId">From identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not find the requested Chat, Id {0}.</exception>
        private Chat GetChat(Chat chat, int? limitRecords = null, int? fromId = null)
        {
            var dal = new Chats();

            // get chat
            var foundChat = dal.GetChatById(chat.Id);
            if (foundChat == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find the requested Chat. Chat Id: {0}", 
                        chat.Id), LogLevel.Error);
                throw new DataException(string.Format("Could not find the requested Chat, Id {0}.", chat.Id));
            }

            foundChat.Users = new List<User>();
            foundChat.Messages = new List<ChatMessage>();

            // get chat users
            var users = GetUsersByChat(foundChat);
            if (users != null)
            {
                foreach (var user in users)
                {
                    if (user == null)
                    {
                        continue;
                    }
                    foundChat.Users.Add(user.StripSensitiveInfoForFriends());
                }
            }
            
            // get chat messages
            IList<ChatMessage> messages;
            if (limitRecords != null && limitRecords > 0)
            {
                messages = fromId != null && fromId > 0
                    ? dal.GetMessagesByChat(foundChat.Id, (int) limitRecords, (int) fromId)
                    : dal.GetMessagesByChat(foundChat.Id, (int) limitRecords);
            }
            else
            {
                messages = dal.GetMessagesByChat(foundChat.Id);
            }

            if (messages == null)
            {
                return foundChat;
            }

            foreach (var message in messages)
            {
                var messageRecord = GetMessageByChatMessage(message);
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

                var userBll = new UserBll();
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
                    foundMessage.Image = GetMessageImage(foundMessage);
                    break;
                case MediaTypeEnum.Video:
                    foundMessage.Video = GetMessageVideo(foundMessage);
                    break;
                case MediaTypeEnum.Melody:
                    foundMessage.UserMelody = GetMessageMelody(foundMessage);
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
                InstaMelodyLogger.Log("Invalid Message - NULL or Default Id.", LogLevel.Error);
                throw new ArgumentException("Invalid Message.");
            }

            Image result = null;

            // get message image
            var dal = new Messages();
            var messageImage  = dal.GetImageByMessageId(message.Id);
            if (messageImage != null)
            {
                var imgDal = new Images();
                var image = imgDal.GetImageById(messageImage.ImageId);
                if (image == null) return null;

                image.FilePath = Utilities.GetFilePath(image.FileName, MediaTypeEnum.Image);

                result = image;
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
                InstaMelodyLogger.Log("Invalid Message - NULL or Default Id.", LogLevel.Error);
                throw new ArgumentException("Invalid Message.");
            }
            Video result = null;

            // get message video
            var dal = new Messages();
            var messageVideo = dal.GetVideoByMessageId(message.Id);
            if (messageVideo != null)
            {
                var vidDal = new Videos();
                var video = vidDal.GetVideoById(messageVideo.Id);
                if (video == null) return null;

                video.FilePath = Utilities.GetFilePath(video.FileName, MediaTypeEnum.Video);

                result = video;
            }

            return result;
        }

        /// <summary>
        /// Gets the message melody.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid Message.</exception>
        private UserMelody GetMessageMelody(Message message)
        {
            if (message == null || message.Id.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log("Invalid Message - NULL or Default Id.", LogLevel.Error);
                throw new ArgumentException("Invalid Message.");
            }
            UserMelody result = null;

            // get message melody
            var dal = new Messages();
            var messageMelody = dal.GetMessageMelodyByMessageId(message.Id);
            if (messageMelody != null)
            {
                var melodyBll = new MelodyBll();
                var melody = melodyBll.GetUserMelody(new UserMelody { Id = messageMelody.UserMelodyId });
                if (melody == null) return null;
                
                result = melody;
            }

            return result;
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
                InstaMelodyLogger.Log("A valid Chat Id must be provided - Default Chat Guid.", LogLevel.Error);
                throw new ArgumentException("A valid Chat Id must be provided.");
            }

            var foundChat = dal.GetChatById(chat.Id);
            if (foundChat == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Chat not found. Chat Id: {0}, User Id: {1}", 
                        chat.Id, sender.Id), LogLevel.Error);
                throw new ArgumentException(string.Format("Chat Id {0} not found.", chat.Id));
            }

            // create message
            var newMessage = CreateMessage(message, sender);

            // create chat message
            var newChatMessage = dal.CreateChatMessage(new ChatMessage
            {
                ChatId = foundChat.Id,
                MessageId = newMessage.Item1.Id,
                SenderId = sender.Id
            });
            newChatMessage.Message = newMessage.Item1;

            // Create Chat Loop
            if (newMessage.Item1.UserMelody != null)
            {
                CreateOrUpdateChatLoop(foundChat, newChatMessage.Message.UserMelody, sender);
            }

            // return chat message and upload token
            return new Tuple<ChatMessage, FileUploadToken>(newChatMessage, newMessage.Item2);
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
            var fileBll = new FileBll();
            var addedImage = fileBll.AddImage(image);

            // create message image
            var dal = new Messages();
            var addedMessageImage = dal.AddMessageImage(messageId, addedImage.Id);
            if (addedMessageImage == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create a MessageImage record. Message Id: {0}, Image Id: {1}", 
                        messageId, addedImage.Id), LogLevel.Error);
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
            var fileBll = new FileBll();
            var addedVideo = fileBll.AddVideo(video);

            // create message image
            var dal = new Messages();
            var addedMessageVideo = dal.AddMessageVideo(messageId, addedVideo.Id);
            if (addedMessageVideo == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to create a MessageVideo record. Message Id: {0}, Video Id: {1}",
                        messageId, addedVideo.Id), LogLevel.Error);
                throw new DataException("Failed to create a MessageVideo record.");
            }

            addedVideo.FilePath = Utilities.GetFilePath(addedVideo.FileName, MediaTypeEnum.Video);
            addedMessageVideo.Video = addedVideo;

            // return message image
            return addedMessageVideo;
        }
        /// <summary>
        /// Creates the or update chat loop.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <param name="melody">The melody.</param>
        /// <param name="creator">The creator.</param>
        private void CreateOrUpdateChatLoop(Chat chat, UserMelody melody, User creator)
        {
            var melodyBll = new MelodyBll();
            if (chat.ChatLoopId != null)
            {
                // Append to ChatLoop
                melodyBll.AttachPartToLoop(
                    new UserLoop
                    {
                        Id = (Guid)chat.ChatLoopId
                    },
                    new UserLoopPart
                    {
                        UserMelody = new UserMelody
                        {
                            Id = melody.Id
                        }
                    },
                    creator);
            }
            else
            {
                // Create new ChatLoop
                var createdLoop = melodyBll.CreateLoop(
                    new UserLoop
                    {
                        Name = string.Format("ChatLoop_{0}", chat.Id),
                        Parts = new List<UserLoopPart>
                        {
                            new UserLoopPart
                            {
                                UserMelody = new UserMelody
                                {
                                    Id = melody.Id
                                }
                            }
                        }
                    },
                    creator);

                // Update Chat.ChatLoopId
                chat.ChatLoopId = ((UserLoop)createdLoop).Id;
                var dal = new Chats();
                dal.UpdateChat(chat);
            }
        }

        #endregion Private Methods
        
        #region Internal Methods

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// UserId is not valid.
        /// or
        /// Cannot create a NULL Message.
        /// </exception>
        internal Tuple<Message, FileUploadToken> CreateMessage(Message message, User sender)
        {
            if (sender.Id.Equals(default(Guid)))
            {
                InstaMelodyLogger.Log(
                    string.Format("UserId is not valid. User Id: {0}",
                        sender.Id), LogLevel.Error);
                throw new ArgumentException("UserId is not valid.");
            }

            if (message == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot create a NULL Message. User Id: {0}", 
                        sender.Id), LogLevel.Error);
                throw new ArgumentException("Cannot create a NULL Message.");
            }

            // create message
            var dal = new Messages();

            if (message.Image != null)
                message.MediaType = MediaTypeEnum.Image;
            else if (message.Video != null)
                message.MediaType = MediaTypeEnum.Video;
            else if (message.UserMelody != null)
                message.MediaType = MediaTypeEnum.Melody;

            var addedMessage = dal.AddMessage(message);

            FileUploadToken addedToken = null;
            var fileUploadToken = new FileUploadToken
            {
                UserId = sender.Id,
                DateCreated = DateTime.UtcNow
            };

            // create attachment
            if (message.Image != null)
            {
                var newImage = CreateMessageImage(addedMessage.Id, message.Image);
                addedMessage.Image = newImage.Image;

                fileUploadToken.MediaType = FileUploadTypeEnum.MessageImage;
                fileUploadToken.FileName = addedMessage.Image.FileName;
            }
            else if (message.Video != null)
            {
                var newVideo = CreateMessageVideo(addedMessage.Id, message.Video);
                addedMessage.Video = newVideo.Video;

                fileUploadToken.MediaType = FileUploadTypeEnum.MessageVideo;
                fileUploadToken.FileName = addedMessage.Video.FileName;
            }
            else if (message.UserMelody != null)
            {
                var melodyBll = new MelodyBll();
                try
                {
                    var foundMelody = melodyBll.GetUserMelody(message.UserMelody);
                    addedMessage.UserMelody = foundMelody;

                    dal.AddMessageMelody(addedMessage.Id, foundMelody.Id);
                }
                catch (Exception)
                {
                    var createdFileUpload = melodyBll.CreateUserMelody(message.UserMelody, sender);

                    dal.AddMessageMelody(addedMessage.Id, createdFileUpload.UserMelody.Id);

                    addedMessage.UserMelody = createdFileUpload.UserMelody;
                    return new Tuple<Message, FileUploadToken>(addedMessage, createdFileUpload.FileUploadToken);
                }
            }

            if (!fileUploadToken.MediaType.Equals(FileUploadTypeEnum.Unknown))
            {
                var fileBll = new FileBll();
                addedToken = fileBll.CreateToken(fileUploadToken);
            }

            return new Tuple<Message, FileUploadToken>(addedMessage, addedToken);
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        internal Message GetMessage(Message message)
        {
            var dal = new Messages();
            var foundMessage = dal.GetMessageById(message.Id);

            return foundMessage;
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void DeleteMessage(Message message)
        {
            var dal = new Messages();
            dal.DeleteMessage(message.Id);
        }

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

        /// <summary>
        /// Deletes the message user melody.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        internal void DeleteMessageUserMelody(Guid userMelodyId)
        {
            var dal = new Messages();
            dal.DeleteMessageMelodyByMelodyId(userMelodyId);
        }

        #endregion Internal Methods
    }
}
