    //
//  ChatViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/7/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "ChatViewController.h"
#import "AFHTTPRequestOperationManager.h"
#import "AFURLSessionManager.h"
#import "JSQMessages.h"
#import "constants.h"
#import "Friend.h"
#import <MagicalRecord/MagicalRecord.h>

@interface ChatViewController ()

@property (nonatomic, strong) NSMutableArray *messages;

@property (strong, nonatomic) JSQMessagesBubbleImage *outgoingBubbleImageData;

@property (strong, nonatomic) JSQMessagesBubbleImage *incomingBubbleImageData;

@property (strong, nonatomic) JSQMessagesBubbleImage *outgoingMelodyBubbleImageData;

@property (strong, nonatomic) JSQMessagesBubbleImage *incomingMelodyBubbleImageData;

@property (strong, nonatomic) NSDateFormatter *dateFormatter;

@end

@implementation ChatViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    //self.title = [self.chatDict objectForKey:@"Id"];

    
    //2015-08-10T20:23:13.283
    //yyyy-MM-dd
    
    self.dateFormatter = [[NSDateFormatter alloc] init];
    
    NSLocale *enUSPOSIXLocale = [NSLocale localeWithLocaleIdentifier:@"en_US_POSIX"];
    
    [self.dateFormatter setLocale:enUSPOSIXLocale];
    
    [self.dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.SSS"];
    [self.dateFormatter setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0]];
    
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    /**
     *  You MUST set your senderId and display name
     */
    self.senderId = [defaults objectForKey:@"Id"];
    self.senderDisplayName = @"Me";
    //self.senderDisplayName = [defaults objectForKey:@"DisplayName"];
    
    UIImageView *tempImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"blurBlue"]];
    [tempImageView setFrame:self.collectionView.frame];
    
    self.collectionView.backgroundView = tempImageView;
    
    self.inputToolbar.contentView.textView.placeHolder = @"Write something...";
    
    /**
     *  Create message bubble images objects.
     *
     *  Be sure to create your bubble images one time and reuse them for good performance.
     *
     */
    JSQMessagesBubbleImageFactory *bubbleFactory = [[JSQMessagesBubbleImageFactory alloc] init];
    
    //self.outgoingBubbleImageData = [bubbleFactory outgoingMessagesBubbleImageWithColor:[UIColor jsq_messageBubbleLightGrayColor]];
    //self.outgoingBubbleImageData = [bubbleFactory outgoingMessagesBubbleImageWithColor:[UIColor colorWithRed:112/255.0f green:104/255.0f blue:105/255.0f alpha:0.2f]];
    //self.incomingBubbleImageData = [bubbleFactory incomingMessagesBubbleImageWithColor:[UIColor colorWithRed:112/255.0f green:104/255.0f blue:105/255.0f alpha:0.4f]];
    
    self.incomingBubbleImageData = [bubbleFactory incomingMessagesBubbleImageWithColor:[UIColor colorWithRed:255/255.0f green:255/255.0f blue:255/255.0f alpha:0.4f]];
    self.outgoingBubbleImageData = [bubbleFactory outgoingMessagesBubbleImageWithColor:[UIColor colorWithRed:255/255.0f green:255/255.0f blue:255/255.0f alpha:0.2f]];
    
    self.incomingMelodyBubbleImageData = [bubbleFactory incomingMessagesBubbleImageWithColor:[UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:0.4f]];
    self.outgoingMelodyBubbleImageData = [bubbleFactory outgoingMessagesBubbleImageWithColor:[UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:0.2f]];
    
    [self loadMessages];
    
}

-(void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    
    [[NSNotificationCenter defaultCenter] addObserverForName:@"uploadDone" object:nil queue:[NSOperationQueue mainQueue] usingBlock:^(NSNotification *note) {
        [self loadMessages];
    }];
    
}

-(void)viewDidDisappear:(BOOL)animated {
    [super viewDidDisappear:animated];
    [[NSNotificationCenter defaultCenter] removeObserver:self name:@"uploadDone" object:nil];
}

#pragma mark - JSQMessages CollectionView DataSource

- (id<JSQMessageData>)collectionView:(JSQMessagesCollectionView *)collectionView messageDataForItemAtIndexPath:(NSIndexPath *)indexPath
{
    return [self.messages objectAtIndex:indexPath.item];
}

- (id<JSQMessageBubbleImageDataSource>)collectionView:(JSQMessagesCollectionView *)collectionView messageBubbleImageDataForItemAtIndexPath:(NSIndexPath *)indexPath
{
    /**
     *  You may return nil here if you do not want bubbles.
     *  In this case, you should set the background color of your collection view cell's textView.
     *
     *  Otherwise, return your previously created bubble image data objects.
     */
    
    JSQMessage *message = [self.messages objectAtIndex:indexPath.item];
    
    if ([message.senderId isEqualToString:self.senderId]) {
        
        if ([message.text containsString:@"added to the loop"]) {
            return self.outgoingMelodyBubbleImageData;
        }
        
        return self.outgoingBubbleImageData;
    }
    
    if ([message.text containsString:@"added to the loop"]) {
        return self.incomingMelodyBubbleImageData;
    }
    
    return self.incomingBubbleImageData;
}

- (id<JSQMessageAvatarImageDataSource>)collectionView:(JSQMessagesCollectionView *)collectionView avatarImageDataForItemAtIndexPath:(NSIndexPath *)indexPath
{
    /**
     *  Return `nil` here if you do not want avatars.
     *  If you do return `nil`, be sure to do the following in `viewDidLoad`:
     *
     *  self.collectionView.collectionViewLayout.incomingAvatarViewSize = CGSizeZero;
     *  self.collectionView.collectionViewLayout.outgoingAvatarViewSize = CGSizeZero;
     *
     *  It is possible to have only outgoing avatars or only incoming avatars, too.
     */
    
    /**
     *  Return your previously created avatar image data objects.
     *
     *  Note: these the avatars will be sized according to these values:
     *
     *  self.collectionView.collectionViewLayout.incomingAvatarViewSize
     *  self.collectionView.collectionViewLayout.outgoingAvatarViewSize
     *
     *  Override the defaults in `viewDidLoad`
     */
    
    
    /*
    JSQMessage *message = [self.demoData.messages objectAtIndex:indexPath.item];
    
    if ([message.senderId isEqualToString:self.senderId]) {
        if (![NSUserDefaults outgoingAvatarSetting]) {
            return nil;
        }
    }
    else {
        if (![NSUserDefaults incomingAvatarSetting]) {
            return nil;
        }
    }
    
    
    return [self.demoData.avatars objectForKey:message.senderId];
     */
    return nil;
}

- (NSAttributedString *)collectionView:(JSQMessagesCollectionView *)collectionView attributedTextForCellTopLabelAtIndexPath:(NSIndexPath *)indexPath
{
    /**
     *  This logic should be consistent with what you return from `heightForCellTopLabelAtIndexPath:
     *  The other label text delegate methods should follow a similar pattern.
     *
     *  Show a timestamp for every 3rd message
     */
    if (indexPath.item % 3 == 0) {
        JSQMessage *message = [self.messages objectAtIndex:indexPath.item];
        return [[JSQMessagesTimestampFormatter sharedFormatter] attributedTimestampForDate:message.date];
    }
    
    return nil;
}

- (NSAttributedString *)collectionView:(JSQMessagesCollectionView *)collectionView attributedTextForMessageBubbleTopLabelAtIndexPath:(NSIndexPath *)indexPath
{
    JSQMessage *message = [self.messages objectAtIndex:indexPath.item];
    
    /**
     *  iOS7-style sender name labels
     */
    if ([message.senderId isEqualToString:self.senderId]) {
        return nil;
    }
    
    if (indexPath.item - 1 > 0) {
        JSQMessage *previousMessage = [self.messages objectAtIndex:indexPath.item - 1];
        if ([[previousMessage senderId] isEqualToString:message.senderId]) {
            return nil;
        }
    }
    
    /**
     *  Don't specify attributes to use the defaults.
     */
    return [[NSAttributedString alloc] initWithString:message.senderDisplayName];
}

- (NSAttributedString *)collectionView:(JSQMessagesCollectionView *)collectionView attributedTextForCellBottomLabelAtIndexPath:(NSIndexPath *)indexPath
{
    return nil;
}

#pragma mark - UICollectionView DataSource

- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section
{
    return [self.messages count];
}

- (UICollectionViewCell *)collectionView:(JSQMessagesCollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    /**
     *  Override point for customizing cells
     */
    JSQMessagesCollectionViewCell *cell = (JSQMessagesCollectionViewCell *)[super collectionView:collectionView cellForItemAtIndexPath:indexPath];
    
    /**
     *  Configure almost *anything* on the cell
     *
     *  Text colors, label text, label colors, etc.
     *
     *
     *  DO NOT set `cell.textView.font` !
     *  Instead, you need to set `self.collectionView.collectionViewLayout.messageBubbleFont` to the font you want in `viewDidLoad`
     *
     *
     *  DO NOT manipulate cell layout information!
     *  Instead, override the properties you want on `self.collectionView.collectionViewLayout` from `viewDidLoad`
     */
    
    JSQMessage *msg = [self.messages objectAtIndex:indexPath.item];
    
    if (!msg.isMediaMessage) {
        
        if ([msg.senderId isEqualToString:self.senderId]) {
            cell.textView.textColor = [UIColor blackColor];
        }
        else {
            cell.textView.textColor = [UIColor whiteColor];
        }
        
        cell.textView.linkTextAttributes = @{ NSForegroundColorAttributeName : cell.textView.textColor,
                                              NSUnderlineStyleAttributeName : @(NSUnderlineStyleSingle | NSUnderlinePatternSolid) };
        
    }
    
    return cell;
}

#pragma mark - UICollectionView Delegate

#pragma mark - Custom menu items

- (BOOL)collectionView:(UICollectionView *)collectionView canPerformAction:(SEL)action forItemAtIndexPath:(NSIndexPath *)indexPath withSender:(id)sender
{
    if (action == @selector(customAction:)) {
        return YES;
    }
    
    return [super collectionView:collectionView canPerformAction:action forItemAtIndexPath:indexPath withSender:sender];
}

- (void)collectionView:(UICollectionView *)collectionView performAction:(SEL)action forItemAtIndexPath:(NSIndexPath *)indexPath withSender:(id)sender
{
    if (action == @selector(customAction:)) {
        [self customAction:sender];
        return;
    }
    
    [super collectionView:collectionView performAction:action forItemAtIndexPath:indexPath withSender:sender];
}

- (void)customAction:(id)sender
{
    NSLog(@"Custom action received! Sender: %@", sender);
    
    [[[UIAlertView alloc] initWithTitle:@"Custom Action"
                                message:nil
                               delegate:nil
                      cancelButtonTitle:@"OK"
                      otherButtonTitles:nil]
     show];
}



#pragma mark - JSQMessages collection view flow layout delegate

#pragma mark - Adjusting cell label heights

- (CGFloat)collectionView:(JSQMessagesCollectionView *)collectionView
                   layout:(JSQMessagesCollectionViewFlowLayout *)collectionViewLayout heightForCellTopLabelAtIndexPath:(NSIndexPath *)indexPath
{
    /**
     *  Each label in a cell has a `height` delegate method that corresponds to its text dataSource method
     */
    
    /**
     *  This logic should be consistent with what you return from `attributedTextForCellTopLabelAtIndexPath:`
     *  The other label height delegate methods should follow similarly
     *
     *  Show a timestamp for every 3rd message
     */
    if (indexPath.item % 3 == 0) {
        return kJSQMessagesCollectionViewCellLabelHeightDefault;
    }
    
    return 0.0f;
}

- (CGFloat)collectionView:(JSQMessagesCollectionView *)collectionView
                   layout:(JSQMessagesCollectionViewFlowLayout *)collectionViewLayout heightForMessageBubbleTopLabelAtIndexPath:(NSIndexPath *)indexPath
{
    /**
     *  iOS7-style sender name labels
     */
    JSQMessage *currentMessage = [self.messages objectAtIndex:indexPath.item];
    if ([[currentMessage senderId] isEqualToString:self.senderId]) {
        return 0.0f;
    }
    
    if (indexPath.item - 1 > 0) {
        JSQMessage *previousMessage = [self.messages objectAtIndex:indexPath.item - 1];
        if ([[previousMessage senderId] isEqualToString:[currentMessage senderId]]) {
            return 0.0f;
        }
    }
    
    return kJSQMessagesCollectionViewCellLabelHeightDefault;
}

- (CGFloat)collectionView:(JSQMessagesCollectionView *)collectionView
                   layout:(JSQMessagesCollectionViewFlowLayout *)collectionViewLayout heightForCellBottomLabelAtIndexPath:(NSIndexPath *)indexPath
{
    return 0.0f;
}

#pragma mark - Responding to collection view tap events

- (void)collectionView:(JSQMessagesCollectionView *)collectionView
                header:(JSQMessagesLoadEarlierHeaderView *)headerView didTapLoadEarlierMessagesButton:(UIButton *)sender
{
    NSLog(@"Load earlier messages!");
}

- (void)collectionView:(JSQMessagesCollectionView *)collectionView didTapAvatarImageView:(UIImageView *)avatarImageView atIndexPath:(NSIndexPath *)indexPath
{
    NSLog(@"Tapped avatar!");
}

- (void)collectionView:(JSQMessagesCollectionView *)collectionView didTapMessageBubbleAtIndexPath:(NSIndexPath *)indexPath
{
    NSLog(@"Tapped message bubble!");
    
    JSQMessage *message = (JSQMessage *)[self.messages objectAtIndex:indexPath.row];
    NSString *tag = message.tag;
    
    UserMelody *um = [UserMelody MR_findFirstByAttribute:@"userMelodyId" withValue:tag];
    
    if (tag.length > 0) {
        
        [[DataManager sharedManager] fetchUserMelody:tag];
        
        um = [UserMelody MR_findFirstByAttribute:@"userMelodyId" withValue:tag];
        
        if (um != nil){
            UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
            LoopViewController *vc = (LoopViewController *)[sb instantiateViewControllerWithIdentifier:@"LoopViewController"];
            vc.selectedUserMelody = um;
            vc.delegate = self;
            [self.navigationController pushViewController:vc animated:YES];
            
            NSLog(@"found something");
        }
        
    }
    /*
    NSArray *messageArray = [self.chatDict objectForKey:@"Messages"];
    NSDictionary *messageDict = [messageArray objectAtIndex:indexPath.row];
    NSDictionary *messageContent = [messageDict objectForKey:@"Message"];
    
    if ([[messageContent objectForKey:@"UserMelody"] isKindOfClass:[NSDictionary class]]) {
        NSDictionary *umDict = [messageContent objectForKey:@"UserMelody"];
        
        //NSString *umId = [umDict objectForKey:@""]
    }
     */
}

- (void)collectionView:(JSQMessagesCollectionView *)collectionView didTapCellAtIndexPath:(NSIndexPath *)indexPath touchLocation:(CGPoint)touchLocation
{
    NSLog(@"Tapped cell at %@!", NSStringFromCGPoint(touchLocation));
}

- (void)didPressSendButton:(UIButton *)button
           withMessageText:(NSString *)text
                  senderId:(NSString *)senderId
         senderDisplayName:(NSString *)senderDisplayName
                      date:(NSDate *)date
{
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Chat": @{@"Id" : [self.chatDict objectForKey:@"Id"]}, @"Message": @{@"Description" : text}}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat/Message", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        /**
         *  Sending a message. Your implementation of this method should do *at least* the following:
         *
         *  1. Play sound (optional)
         *  2. Add new id<JSQMessageData> object to your data source
         *  3. Call `finishSendingMessage`
         */
        [JSQSystemSoundPlayer jsq_playMessageSentSound];
        
        JSQMessage *message = [[JSQMessage alloc] initWithSenderId:senderId
                                                 senderDisplayName:senderDisplayName
                                                              date:date
                                                              text:text];
        
        [self.messages addObject:message];
        
        [self.collectionView reloadData];
        
        [self finishSendingMessageAnimated:YES];
        
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
}

#pragma mark -attach files

- (void)didPressAccessoryButton:(UIButton *)sender
{
    UIActionSheet *sheet = [[UIActionSheet alloc] initWithTitle:@"Media messages"
                                                       delegate:self
                                              cancelButtonTitle:@"Cancel"
                                         destructiveButtonTitle:nil
                                              otherButtonTitles:@"Send photo", @"Send melody", nil];
    
    [sheet showFromToolbar:self.inputToolbar];
}

- (void)actionSheet:(UIActionSheet *)actionSheet didDismissWithButtonIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == actionSheet.cancelButtonIndex) {
        return;
    }
    
    switch (buttonIndex) {
        case 0:
            //[self.demoData addPhotoMediaMessage];
            [self presentImagePicker:nil];
            break;

        case 1:
        {
            [self createLoop:nil];
            /*
            __weak UICollectionView *weakView = self.collectionView;
            
            [self.demoData addLocationMediaMessageCompletion:^{
                [weakView reloadData];
            }];
             */
        }
            break;
            /*
            
        case 2:
            [self.demoData addVideoMediaMessage];
            break;*/
    }
}



-(void)loadMessages {
    
    self.messages = [NSMutableArray new];
    NSArray *messageArray = [self.chatDict objectForKey:@"Messages"];
    
    for (NSDictionary *messageDict in messageArray) {
        NSDictionary *messageContent = [messageDict objectForKey:@"Message"];
        
        NSString *senderId = [messageDict objectForKey:@"SenderId"];
        NSString *senderName = self.senderDisplayName;
        
        if (![senderId isEqualToString:self.senderId]) {
            Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:senderId];
            senderName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
        }
        
        NSString *dateString = [messageDict objectForKey:@"DateCreated"];
        dateString = [dateString stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
        NSDate *date = [self.dateFormatter dateFromString:dateString];
        
        if (date != nil) {
            
            NSInteger mediaType = [[messageContent objectForKey:@"MediaType"] integerValue];
            if (mediaType == 2) {
                id imageComponent = [messageContent objectForKey:@"Image"];
                if (imageComponent != nil && [imageComponent isKindOfClass:[NSDictionary class]]) {
                    NSString *remoteImageURL = [imageComponent objectForKey:@"FilePath"];
                    NSString *imageName = [remoteImageURL lastPathComponent];
                    NSString *imagePath = [self getPathforImageNamed:imageName];
                    [self createPhotoMessageWithSenderId:senderId andName:senderName andPath:imagePath];
                    
                    if (imagePath == nil) {
                        [self downloadImageWithUrl:remoteImageURL];
                    }
                    
                }
            } else {
                
                if ([[messageContent objectForKey:@"UserMelody"] isKindOfClass:[NSDictionary class]]) {
                    //melody message
                    NSLog(@"i have a melody message");

                    NSString *textString = [NSString stringWithFormat:@"%@ has added to the loop!", senderName];
                    
                    if ([senderName isEqualToString:@"Me"]) {
                        textString = @"You have added to the loop!";
                    }
                    
                    JSQMessage *message = [[JSQMessage alloc] initWithSenderId:senderId
                                                             senderDisplayName:senderName
                                                                          date:date
                                                                          text:textString];
                    [self.messages addObject:message];
                    
                    
                } else {
                    //text message
                    JSQMessage *message = [[JSQMessage alloc] initWithSenderId:senderId
                                                             senderDisplayName:senderName
                                                                          date:date
                                                                          text:[messageContent objectForKey:@"Description"]];
                    
                    [self.messages addObject:message];
                }
                
            }
        }
    }
    
    NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"date" ascending:YES];
    NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
    NSArray *sortedArray = [self.messages sortedArrayUsingDescriptors:descriptors];
    
    self.messages = [NSMutableArray arrayWithArray:sortedArray];
    [self.collectionView reloadData];
}

-(void)createPhotoMessageWithSenderId:(NSString *)senderId andName:(NSString *)senderName andPath:(NSString *)path {
    UIImage *localImage = [UIImage imageWithContentsOfFile:path];
    JSQPhotoMediaItem *photoItem = [[JSQPhotoMediaItem alloc] initWithImage:localImage];
    JSQMessage *photoMessage = [JSQMessage messageWithSenderId:senderId
                                                   displayName:senderName
                                                         media:photoItem];
    [self.messages addObject:photoMessage];
}

-(void)createPlaceholderWithSenderId:(NSString *)senderId andName:(NSString *)senderName andMelodyId:(NSString *)melodyId {
    UIImage *localImage = [UIImage imageNamed:@"placeholder"];
    JSQPhotoMediaItem *photoItem = [[JSQPhotoMediaItem alloc] initWithImage:localImage];
    JSQMessage *photoMessage = [JSQMessage messageWithSenderId:senderId
                                                   displayName:senderName
                                                         media:photoItem];
    photoMessage.tag = melodyId;
    [self.messages addObject:photoMessage];
}

#pragma mark - loop delegate

-(IBAction)createLoop:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    LoopViewController *vc = (LoopViewController *)[sb instantiateViewControllerWithIdentifier:@"LoopViewController"];
    vc.delegate = self;
    [self.navigationController pushViewController:vc animated:YES];
}

-(void)didFinishWithInfo:(NSDictionary *)userDict {
    [self createPhotoMessageWithSenderId:self.senderId andName:self.senderDisplayName andPath:nil];
    
    NSMutableDictionary *mutableDict = [NSMutableDictionary dictionaryWithDictionary:userDict];
    
    [mutableDict setObject:[self.chatDict objectForKey:@"Id"] forKey:@"Id"];
    
    [[NetworkManager sharedManager] uploadChatUserMelody:mutableDict];
    
    [self.collectionView reloadData];
}

-(void)cancel {
//do nothing
}

#pragma mark - image picker


-(IBAction)presentImagePicker:(id)sender {
    
    UIImagePickerController *imagePicker = [[UIImagePickerController alloc] init];
    imagePicker.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
    imagePicker.delegate = self;
    
    [self presentViewController:imagePicker animated:YES completion:^{
        NSLog(@"Image picker presented!");
    }];
}


-(void)imagePickerController:(UIImagePickerController *)picker
didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    UIImage *selectedImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    //[self.profileImageView setImage:selectedImage];
    [picker dismissViewControllerAnimated:YES completion:^{
        NSLog(@"Image selected!");
    }];
    
    //save image
    NSString *imagePath = [self saveImage:selectedImage];
    
    //create media message bubble
    [self createPhotoMessageWithSenderId:self.senderId andName:self.senderDisplayName andPath:imagePath];
    
    //upload image
    [self attachImageWithPath:imagePath];
    
    [JSQSystemSoundPlayer jsq_playMessageSentSound];
    
    [self finishSendingMessageAnimated:YES];
}

-(void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    [picker dismissViewControllerAnimated:YES completion:^{
        NSLog(@"Picker cancelled without doing anything");
    }];
}

-(NSString *)saveImage:(UIImage *)image {
    //save to file
    
    time_t unixTime = time(NULL);
    
    NSString *chatId = [self.chatDict objectForKey:@"Id"];
    
    NSString *imageName = [NSString stringWithFormat:@"chat_%@_%d.jpg", chatId, (int)unixTime];
    
    //try to create folder
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Downloads"];
    
    if (![fileManager fileExistsAtPath:profilePath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:profilePath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    //save to folder
    NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
    NSData *imageData = UIImageJPEGRepresentation(image, 0.7);
    [imageData writeToFile:imagePath atomically:YES];
    
    return imagePath;
}

-(NSString *)getPathforImageNamed:(NSString *)imageName {
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Downloads"];
    
    //save to folder
    NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
    
    if (![fileManager fileExistsAtPath:imagePath]){
        return nil;
    }
    
    return imagePath;
}

#pragma mark - network operations

-(void)attachImageWithPath:(NSString *)imagePath{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    NSString *imageName = [imagePath lastPathComponent];
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Chat": @{@"Id" : [self.chatDict objectForKey:@"Id"]}, @"Message": @{@"Description" : @"", @"Image": @{@"FileName" : imageName}}}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat/Message", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);
        
        //
        //step 2 - upload file
        
        NSDictionary *responseDict = (NSDictionary *)responseObject;
        NSDictionary *tokenDict = [responseDict objectForKey:@"FileUploadToken"];
        NSString *fileTokenString = [tokenDict objectForKey:@"Token"];
        
        [[NetworkManager sharedManager] uploadFile:imagePath withFileToken:fileTokenString];
        //[self uploadData:imageData withFileToken:fileTokenString andFileName:imageName];
        
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
    
}

-(void)downloadImageWithUrl:(NSString *)remoteUrl {

    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *fileName = [remoteUrl lastPathComponent];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Downloads"];
    
    NSString *pathString = [profilePath stringByAppendingPathComponent:fileName];
    
    //download iamge if it doesn't exist
    if (![fileManager fileExistsAtPath:pathString]) {
        
        //make folder if it doesn't exist
        if (![fileManager fileExistsAtPath:profilePath]){
            
            NSError* error;
            if(  [[NSFileManager defaultManager] createDirectoryAtPath:profilePath withIntermediateDirectories:NO attributes:nil error:&error]) {
                
                NSLog(@"success creating folder");
                
            } else {
                NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
                NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
            }
            
        }
        
        NSURLSessionConfiguration *configuration = [NSURLSessionConfiguration defaultSessionConfiguration];
        AFURLSessionManager *manager = [[AFURLSessionManager alloc] initWithSessionConfiguration:configuration];
        
        NSString *fullUrlString = [NSString stringWithFormat:@"%@/%@", DOWNLOAD_BASE_URL, remoteUrl];
        fullUrlString = [fullUrlString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
        
        NSURL *URL = [NSURL URLWithString:fullUrlString];
        NSURLRequest *request = [NSURLRequest requestWithURL:URL];
        
        NSURLSessionDownloadTask *downloadTask = [manager downloadTaskWithRequest:request progress:nil destination:^NSURL *(NSURL *targetPath, NSURLResponse *response) {
            
            //NSString *fileString = [NSString stringWithFormat:@"file://%@", destinationFilePath];
            NSURL *fileURL = [NSURL fileURLWithPath:pathString];
            
            return fileURL;
            
            //NSURL *documentsDirectoryURL = [[NSFileManager defaultManager] URLForDirectory:NSDocumentDirectory inDomain:NSUserDomainMask appropriateForURL:nil create:NO error:nil];
            //return [documentsDirectoryURL URLByAppendingPathComponent:[response suggestedFilename]];
        } completionHandler:^(NSURLResponse *response, NSURL *filePath, NSError *error) {
            
            if (error == nil) {
                NSLog(@"File downloaded to: %@", filePath);
                
                [self loadMessages];
                
            } else {
                NSLog(@"Download error: %@", error.description);
            }
        }];
        [downloadTask resume];
        
    }
    
}



@end
