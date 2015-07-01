using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;

namespace InstaMelody.Business
{
    public class MessageBLL
    {
        #region Public Methods

        #region User Messages

        /// <summary>
        /// Sends the message to user.
        /// </summary>
        /// <param name="recipientId">The recipient identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="enforceFriendship">if set to <c>true</c> [enforce friendship].</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException">
        /// Cannot send a NULL message
        /// or
        /// or
        /// </exception>
        public object SendMessageToUser(Guid recipientId, Message message, Guid sessionToken, bool enforceFriendship = false)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            if (message == null 
                || (string.IsNullOrWhiteSpace(message.Description)
                    && message.Image == null && message.Melody == null))
            {
                throw new DataException("Cannot send a NULL message");
            }

            if (recipientId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException("User cannot send a message to themself.");
            }

            // check to see if users are friends
            var userBll = new UserBLL();
            if (enforceFriendship && !userBll.AreUsersFriends(recipientId, sessionUser.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("Cannot send a message to User {0} becuase they are not added as a friend.", recipientId));
            }

            var msg = this.CreateMessage(message);
            if (msg == null)
            {
                throw new DataException(string.Format("Could not create Message to Send to User: {0}", recipientId));
            }

            var dal = new UserMessages();

            var usrMsg = dal.AddUserMessage(sessionUser.Id, recipientId, msg.Id);
            if (usrMsg == null)
            {
                throw new DataException(
                    string.Format("Error creating user message. Sender: {0}, Recipient: {1}, Message: {2}",
                        sessionUser.Id, recipientId, msg.Id));
            }

            // attach message to user message
            usrMsg.Message = msg;


            // TODO: check message images or melodies
            if (msg.Image != null || msg.Melody != null)
            {
                return this.GetUserMessageFileUpload(usrMsg, sessionUser.Id);
            }

            return usrMsg;
        }

        /// <summary>
        /// Reads the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// Could not validate session.
        /// or
        /// Only the recipient of a message can mark a message as read.
        /// </exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        public UserMessage ReadMessage(UserMessage message, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            var usrMsg = this.GetUserMessage(message);
            if (usrMsg == null)
            {
                throw new DataException(string.Format("Failed to retrieve User Message record with Id: {0}", message.Id));
            }

            if (!usrMsg.RecipientId.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException("Only the recipient of a message can mark a message as read.");
            }

            var dal = new Messages();
            dal.MarkMessagAsRead(usrMsg.MessageId);

            // mark message as read
            var readMessage = dal.GetMessageById(usrMsg.MessageId);
            if (!readMessage.IsRead)
            {
                throw new DataException(string.Format("Failed to mark Message: {0} as read.", readMessage.Id));
            }

            // clone the user message and add read message to cloned
            var clonedMessage = usrMsg.Clone();
            clonedMessage.Message = readMessage;

            return clonedMessage;
        }

        /// <summary>
        /// Replies to message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="replyMessage">The reply message.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="enforceFriendship">if set to <c>true</c> [enforce friendship].</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public object ReplyToMessage(UserMessage message, Message replyMessage, Guid sessionToken, bool enforceFriendship = false)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            var usrMsg = this.GetUserMessage(message);
            if (usrMsg == null)
            {
                throw new DataException(string.Format("Failed to retrieve User Message record with Id: {0}", message.Id));
            }

            if (!sessionUser.Id.Equals(usrMsg.RecipientId) && !sessionUser.Id.Equals(usrMsg.UserId))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User {0} is not allowed to reply to a message of which they are not the sender or recipient.",
                        sessionUser.Id));
            }

            // prepare & send reply message
            replyMessage.ParentId = usrMsg.MessageId;
            var msgRecipient = sessionUser.Id.Equals(usrMsg.RecipientId) ? usrMsg.UserId : usrMsg.RecipientId;

            return this.SendMessageToUser(msgRecipient, replyMessage, sessionToken, enforceFriendship);
        }

        /// <summary>
        /// Gets the messages by user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="threaded">if set to <c>true</c> [threaded].</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.
        /// or
        /// User Messages can only be requested by the User the messages belong to.</exception>
        public IList<UserMessage> GetMessagesByUser(User user, Guid sessionToken, bool threaded = false)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            var userBll = new UserBLL();
            var foundUser = userBll.GetUser(user, sessionToken);
            if (foundUser == null)
            {
                throw new UnauthorizedAccessException("Could not find a user record witht the credentials provided.");
            }

            if (!foundUser.Id.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException("User Messages can only be requested by the User to whom the messages belong.");
            }

            var dal = new UserMessages();
            var messages = dal.GetUserMessagesByUser(sessionUser.Id);
            foreach (var userMessage in messages)
            {
                userMessage.Message = this.GetMessageById(userMessage.MessageId);
            }

            return (threaded) ? this.ThreadMessages(messages) : messages;
        }

        /// <summary>
        /// Deletes the user message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionToken">The session token.</param>
        public void DeleteUserMessage(UserMessage message, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            var usrMsg = this.GetUserMessage(message);
            if (usrMsg == null)
            {
                throw new DataException(string.Format("Failed to retrieve User Message record with Id: {0}", message.Id));
            }

            if (!sessionUser.Id.Equals(usrMsg.RecipientId) && !sessionUser.Id.Equals(usrMsg.UserId))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User {0} is not allowed to delete a message of which they are not the sender or recipient.",
                        sessionUser.Id));
            }

            var dal = new UserMessages();
            if (sessionUser.Id.Equals(usrMsg.RecipientId))
            {
                dal.DeleteUserMessageForRecipient(usrMsg.Id);
            }
            else
            {
                dal.DeleteUserMessageForSender(usrMsg.Id);
            }
        }

        #endregion User Messages

        #region Station Messages

        // TODO: Create Station message BLL functions

        #endregion Station Messages

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Threads the messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        private IList<UserMessage> ThreadMessages(IList<UserMessage> messages)
        {
            if (messages == null || !messages.Any())
            {
                return null;
            }

            var threadedMessages = new List<UserMessage>();

            foreach (var userMessage in messages)
            {
                if (userMessage.Message == null)
                {
                    continue;
                }

                if (userMessage.Message.ParentId == null
                    || userMessage.Message.ParentId.Equals(default(Guid)))
                {
                    threadedMessages.Add(userMessage);
                }

                userMessage.ReplyMessages = this.NestMessages(userMessage.MessageId, messages);
            }

            return threadedMessages;
        }

        /// <summary>
        /// Nests the messages.
        /// </summary>
        /// <param name="parentMessageId">The parent message identifier.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        private IList<UserMessage> NestMessages(Guid parentMessageId, IList<UserMessage> messages)
        {
            if (messages == null || !messages.Any())
            {
                return null;
            }

            var nestedMessages = new List<UserMessage>();

            nestedMessages.AddRange(messages.Where(m => m.Message.ParentId.Equals(parentMessageId)));

            return nestedMessages;
        }

        /// <summary>
        /// Gets the user message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Cannot retrieve NULL UserMessage</exception>
        private UserMessage GetUserMessage(UserMessage message)
        {
            UserMessage userMessage = null;

            if (message.MessageId.Equals(default(Guid)) && message.Id.Equals(default(int))
                && (message.Message == null || message.Message.Id.Equals(default(Guid))))
            {
                throw new DataException("Cannot retrieve NULL UserMessage");
            }

            var dal = new UserMessages();
            var usrMsg = dal.GetUserMessageById(message.Id);
            if (usrMsg == null)
            {
                var msgId = message.MessageId.Equals(default(Guid)) ? message.Message.Id : message.MessageId;
                usrMsg = dal.GetUserMessageByMessageId(msgId);
                if (usrMsg != null)
                {
                    userMessage = usrMsg;
                }
            }
            else
            {
                userMessage = usrMsg;
            }

            return userMessage;
        }

        /// <summary>
        /// Gets the message by identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        private Message GetMessageById(Guid messageId)
        {
            var dal = new Messages();

            var message = dal.GetMessageById(messageId);
            var msgImg = this.GetMessageImageByMessageId(message.Id);
            if (msgImg != null)
            {
                message.Image = msgImg.Image;
            }
            // TODO: Uncomment once GetMessageMelodyByMessageId is implemented
            //message.Melody = this.GetMessageMelodyByMessageId(message.Id);

            return message;
        }


        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not create message</exception>
        private Message CreateMessage(Message message)
        {
            var dal = new Messages();

            var addedMessage = dal.AddMessage(message);
            if (addedMessage == null)
            {
                throw new DataException("Could not create Message.");
            }

            if (message.Image != null)
            {
                message.MediaType = MediaTypeEnum.Image;

                var createdMessageImage = this.CreateMessageImage(addedMessage.Id, message.Image);
                if (createdMessageImage != null && createdMessageImage.Image != null)
                {
                    addedMessage.Image = createdMessageImage.Image;
                }
            }
            else if (message.Melody != null)
            {
                // TODO:
            }

            return addedMessage;
        }

        /// <summary>
        /// Gets the message image by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        private MessageImage GetMessageImageByMessageId(Guid messageId)
        {
            MessageImage result = null;

            var dal = new MessageImages();
            result = dal.GetImageByMessageId(messageId);
            if (result == null)
            {
                throw new DataException("Failed to retrieve Message Image.");
            }

            var imageBll = new ImageBLL();
            var image = imageBll.GetImage(new Image
            {
                Id = result.ImageId
            });
            if (image == null)
            {
                throw new DataException("Failed to retrieve Image.");
            }

            result.Image = image;

            return result;
        }

        private MessageMelody GetMessageMelodyByMessageId(Guid messageId)
        {
            // TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the message image.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No MessageId provided to create Message Image.
        /// or
        /// Could not create Message Image.</exception>
        private MessageImage CreateMessageImage(Guid messageId, Image image)
        {
            if (messageId.Equals(default(Guid)))
            {
                throw new DataException("No MessageId provided to create Message Image.");
            }

            var imageBll = new ImageBLL();
            var addedImage = imageBll.AddImage(image);
            if (addedImage == null)
            {
                throw new DataException("Failed to add Image.");
            }

            var dal = new MessageImages();
            var addedMsgImg = dal.AddMessageImage(messageId, addedImage.Id);
            if (addedMsgImg == null)
            {
                throw new DataException("Could not create Message Image.");
            }
            else
            {
                addedMsgImg.Image = addedImage;
            }

            return addedMsgImg;
        }

        private MessageMelody CreateMessageMelody(MessageMelody melody)
        {
            // TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the user message file upload.
        /// </summary>
        /// <param name="usrMsg">The usr MSG.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">User Message cannot be null.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        private ApiUserMessageFileUpload GetUserMessageFileUpload(UserMessage usrMsg, Guid userId)
        {
            if (usrMsg.Message == null) throw new ArgumentException("User Message cannot be null.");

            if (usrMsg.Message.Image != null || usrMsg.Message.Melody != null)
            {
                // create file upload token
                var uploadToken = new FileUploadToken
                {
                    UserId = userId,
                    MediaType = FileUploadTypeEnum.Unknown
                };

                // determine if uploading image or melody
                if (usrMsg.Message.Image != null)
                {
                    uploadToken.MediaType = FileUploadTypeEnum.MessageImage;
                    uploadToken.FileName = usrMsg.Message.Image.FileName;
                }
                else if (usrMsg.Message.Melody != null)
                {
                    uploadToken.MediaType = FileUploadTypeEnum.MessageMelody;
                    uploadToken.FileName = usrMsg.Message.Melody.FileName;
                }

                var uploadBll = new FileUploadBLL();
                var createdToken = uploadBll.CreateToken(uploadToken);

                return new ApiUserMessageFileUpload
                {
                    UserMessage = usrMsg,
                    FileUploadToken = createdToken
                };
            }

            throw new DataException();
        }

        #endregion Private Methods

        #region Internal Methods

        /// <summary>
        /// Deletes the user message image.
        /// </summary>
        /// <param name="imgId">The img identifier.</param>
        internal void DeleteUserMessageImage(int imgId)
        {
            // delete message image record
            var dal = new MessageImages();
            dal.DeleteMessageImageByImageId(imgId);
        }

        internal void DeleteUserMessageMelody(int melodyId)
        {
            // TODO:
            throw new NotImplementedException();


            // delete message melody record
            var dal = new MessageMelodies();
            dal.DeleteMessageMelodyByMelodyId(melodyId);
        }

        #endregion Internal Methods
    }
}
