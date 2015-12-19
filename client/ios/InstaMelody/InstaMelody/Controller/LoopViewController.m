//
//  LoopViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/17/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "LoopViewController.h"
#import "MelodyGroupController.h"
#import "NSString+FontAwesome.h"
#import "UIFont+FontAwesome.h"
#import "AFURLSessionManager.h"
#import "AFHTTPRequestOperationManager.h"
#import "constants.h"
#import "CustomActivityProvider.h"

@interface LoopViewController ()

@property (nonatomic, strong) NSArray *groupArray;
@property (nonatomic, strong) Melody *selectedMelody;
@property (nonatomic, strong) Melody *selectedMelody2;
@property (nonatomic, strong) Melody *selectedMelody3;
@property (nonatomic, strong) NSURL *currentRecordingURL;

@property (nonatomic, strong) AVAudioPlayer *bgPlayer;
@property (nonatomic, strong) AVAudioPlayer *bgPlayer2;
@property (nonatomic, strong) AVAudioPlayer *bgPlayer3;
@property (nonatomic, strong) AVAudioPlayer *fgPlayer;

@property (nonatomic, strong) NSTimer *timer;
@property (nonatomic, strong)  NSDate *startTime;
@property NSNumber *savedGroupId;

@property (nonatomic, strong) AVAudioRecorder *recorder;
@property (nonatomic, strong) NSMutableArray *partArray;
@property NSInteger currentPartIndex;
@property BOOL goBack;
@property BOOL isNewPart;

@property (nonatomic, strong) NSArray *inputs;

@property NSUserDefaults *defaults;

@end

@implementation LoopViewController
{
    NSNumber * initialIsExplicitStatus;
}

-(void)viewDidLoad {
    [super viewDidLoad];
    //_isFromChat = TRUE;
    self.goBack = NO;
    self.isNewPart = YES;
    self.defaults = [NSUserDefaults standardUserDefaults];
    
    self.groupArray = [[DataManager sharedManager] melodyGroupList];
    [self roundView:self.profileImageView];
    [self roundView:self.centerConsoleView];
    [self roundView:self.consoleView];
    
    [self applyFontAwesome];
    
    self.explicitCheckbox.delegate = self;
    self.explicitCheckbox.tag = 1;
    self.publicCheckbox.delegate = self;
    self.publicCheckbox.tag = 2;
    
    self.progressView.roundedCorners = YES;
    self.progressView.trackTintColor = [UIColor clearColor];
    self.progressView.progressTintColor = INSTA_BLUE;
    self.progressView.thicknessRatio = 0.1f;
    
    if(_isNotMyStudio)
    {
        self.explicitView.userInteractionEnabled = NO;
        self.publicView.userInteractionEnabled = NO;
    } else {
        self.explicitView.userInteractionEnabled = YES;
        self.publicView.userInteractionEnabled = YES;
    }
    
    NSString *myUserId = [self.defaults objectForKey:@"Id"];
    
     if (self.selectedLoop !=nil) {
         [self getLoop:[self.selectedLoop objectForKey:@"Id"]];
         
        //self.isNewPart = NO;
         
     } else if (self.selectedUserMelody != nil) {
        
        int count = 0;
        NSLog(@"loaded with a melody");
         
         if ([self.selectedUserMelody.userId isEqualToString:myUserId]) {
             self.isNotMyStudio = NO;
         }
                           
        if ([self.selectedUserMelody.isExplicit boolValue]) {
            self.explicitCheckbox.on = YES;
            self.publicCheckbox.on = NO;
        } else {
            self.explicitCheckbox.on = NO;
            self.publicCheckbox.on = YES;
        }
         
         initialIsExplicitStatus = self.selectedUserMelody.isExplicit;
         
         //if ([self.selectedUserMelody.isStationPostMelody boolValue]) {
           //  self.publicCheckbox.on = YES;
         //}
         //Dropping use of this flag. If explicit is off, it is public.
         
        for (UserMelodyPart *part in [self.selectedUserMelody parts]) {
            if ([part.isUserCreated boolValue] == true) {
                //get and set recording
                
                //part.fileName
                
                NSArray *paths =
                NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                                    NSUserDomainMask, YES);
                NSString *documentsPath = [paths objectAtIndex:0];
                
                NSString *recordingPath = [documentsPath stringByAppendingPathComponent:@"Recordings"];
                
                NSString *filePath = [recordingPath
                                      stringByAppendingPathComponent:part.fileName];
                
                if ([[NSFileManager defaultManager] fileExistsAtPath:filePath]){
                    self.currentRecordingURL = [NSURL URLWithString:filePath];
                    self.playButton.hidden = NO;
                } else {
                    [self downloadRecording:part.filePath toPath:filePath];
                }
                
                
                
            } else if (count == 0) {
                Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:part.partId];
                
                //get and set system melodies
                [self didSelectMelody:melody];
                count++;
            } else if (count == 1) {
                //
                
                Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:part.partId];
                
                [self didSelectMelody2:melody];
                count++;
                
            } else if (count == 2) {
                //
                
                Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:part.partId];
                
                [self didSelectMelody3:melody];
                count++;
                
            }
        }
         
        //self.isNewPart = NO;
     } else {
         //it's just me now
         
         NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
         
         NSString *myUserId = [defaults objectForKey:@"Id"];
         
         NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
         
         NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
         NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
         
         if (imageName != nil) {
            
             NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
             self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
             
         }
         
         self.playButton.hidden = YES;
         self.progressLabel.text = @"Press Record to Start";
         self.backwardButton.hidden = YES;
         self.forwardButton.hidden = YES;
         self.isNewPart = YES;
         self.isNotMyStudio = NO;
         
         self.explicitCheckbox.on = NO;
         self.publicCheckbox.on = YES;
     }
    
    [[NSNotificationCenter defaultCenter] addObserverForName:@"pickedMelody" object:nil queue:[NSOperationQueue mainQueue] usingBlock:^(NSNotification * _Nonnull note) {
        //
        
        if (note.userInfo != nil) {
            Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:[note.userInfo objectForKey:@"melodyId"]];
            
            NSString *name = melody.melodyName;
            CLToken *token = [[CLToken alloc] initWithDisplayText:name context:nil];
            CLTokenInputView *tokenInputView = (CLTokenInputView *)[self.tableView viewWithTag:99];
            
            BOOL isToken1 = [name isEqualToString:self.selectedMelody.melodyName];
            BOOL isToken2 = [name isEqualToString:self.selectedMelody2.melodyName];
            BOOL isToken3 = [name isEqualToString:self.selectedMelody3.melodyName];
            
            if (tokenInputView.allTokens.count < 4 && !isToken1 && !isToken2 && !isToken3) {
                
                switch (tokenInputView.allTokens.count) {
                    case 0:
                        [self didSelectMelody:melody];
                        break;
                    case 1:
                        [self didSelectMelody2:melody];
                        break;
                    case 2:
                        [self didSelectMelody3:melody];
                        break;
                    default:
                        break;
                }
                
                [tokenInputView addToken:token];
                
            }
            
            //add melody
            self.savedGroupId = [note.userInfo objectForKey:@"groupId"];
            
        }
    }];
    
    //[[NSNotificationCenter defaultCenter] postNotificationName:@"pickedMelody" object:nil userInfo:userDict];
    
    [self initializeAudio];
    
    [self updateBarStatus];
    
}

-(void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    
    //to make nav bar clear
    
    //self.navigationController.navigationBar.translucent = YES;
    [(UIView*)[self.navigationController.navigationBar.subviews objectAtIndex:0] setAlpha:0.2f];
    
    NSDictionary *navbarTitleTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                               [UIColor whiteColor],
                                               NSForegroundColorAttributeName,
                                               [UIFont fontWithName:@"Century Gothic" size:18.0],
                                               NSFontAttributeName,
                                               nil];
    
    
    /*
     NSDictionary *buttonTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
     [UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:1.0f],
     NSForegroundColorAttributeName,
     [UIFont fontWithName:@"FontAwesome" size:20.0],
     NSFontAttributeName,
     nil];
     */
    
    
    [self.navigationController.navigationBar setTitleTextAttributes:navbarTitleTextAttributes];
    //[self.navigationController.navigationBar setTintColor:[UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:1.0f]];
    [self.navigationController.navigationBar setTintColor:[UIColor whiteColor]];
    
    
    NSDictionary *buttonTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                          [UIColor whiteColor],
                                          NSForegroundColorAttributeName,
                                          [UIFont fontWithName:@"FontAwesome" size:20.0],
                                          NSFontAttributeName,
                                          nil];
    
    
    [[UIBarButtonItem appearance] setTitleTextAttributes:buttonTextAttributes forState:UIControlStateNormal];
    
}

-(void)viewDidDisappear:(BOOL)animated {
    [super viewDidDisappear:animated];
    [self stopEverything:nil];
}

-(void)updateBarStatus {
    /*
    if (self.isMyStudio) {
        self.saveBar.hidden = NO;
        self.recordButton.hidden = NO;
        
        self.publicView.hidden = NO;
        self.explicitView.hidden = NO;
        
        self.saveBarStationLabel.text = [NSString stringWithFormat:@"@%@", [[NSUserDefaults standardUserDefaults] objectForKey:@"DisplayName"]];
        
        UITextField *topicField = (UITextField *)[self.tableView viewWithTag:98];
        
        self.joinBar.hidden = YES;
        self.joinBarModLabel.hidden = YES;
    } else {
        self.saveBar.hidden = YES;
        self.recordButton.hidden = YES;
        
        self.publicView.hidden = YES;
        self.explicitView.hidden = YES;
        
        self.joinBar.hidden = NO;
        self.joinBarModLabel.hidden = NO;
        
        
    }
     */
    
    self.saveBar.hidden = NO;
    self.recordButton.hidden = NO;
    
    self.publicView.hidden = NO;
    self.explicitView.hidden = NO;
    
    self.saveBarStationLabel.text = [NSString stringWithFormat:@"@%@", [[NSUserDefaults standardUserDefaults] objectForKey:@"DisplayName"]];
    
    UITextField *topicField = (UITextField *)[self.tableView viewWithTag:98];
    topicField.delegate = self;
    
    self.joinBar.hidden = YES;
    self.joinBarModLabel.hidden = YES;

}

-(void)initializeAudio {
    // Background color
    self.audioPlot.backgroundColor = [UIColor blackColor];
    
    // Waveform color
    self.audioPlot.color = INSTA_BLUE;
    
    // Plot type
    self.audioPlot.plotType = EZPlotTypeBuffer;
    //
    // Create the microphone
    //
    self.microphone = [EZMicrophone microphoneWithDelegate:self];
    
    //
    // Set up the microphone input UIPickerView items to select
    // between different microphone inputs. Here what we're doing behind the hood
    // is enumerating the available inputs provided by the AVAudioSession.
    //
    self.inputs = [EZAudioDevice inputDevices];
    self.audioPlot.hidden = YES;
}

-(void)getLoop:(NSString *)loopId {
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token, @"id": loopId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        if ([responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *responseDict = (NSDictionary *)responseObject;
            
            NSArray *parts = [responseDict objectForKey:@"Parts"];
            
            if (parts != nil && [parts isKindOfClass:[NSArray class]]) {
                if (parts.count > 0) {
                    [self.playButton setHidden:NO];
                } else {
                    [self.playButton setHidden:YES];
                }

                NSString * isExp = [NSString stringWithFormat:@"%@", [responseDict valueForKey:@"IsExplicit"]];
                if ([isExp isEqualToString:@"1"]) {
                    self.explicitCheckbox.on = YES;
                    self.publicCheckbox.on = NO;
                } else {
                    self.explicitCheckbox.on = NO;
                    self.publicCheckbox.on = YES;
                }
                
                initialIsExplicitStatus = [NSNumber numberWithInt:[isExp intValue]];
                
            }
            

            /*
            [self.statusButton setTitle:@"Ready to play" forState:UIControlStateNormal];
            [self.loopTitleButton setTitle:[responseDict objectForKey:@"Name"] forState:UIControlStateNormal];
            
             */
            
            self.loopDict = responseDict;
            
            [self buildPartArray];
        }
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
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

-(void)buildPartArray {
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSArray *rawParts = [self.loopDict objectForKey:@"Parts"];
    
    self.partArray = [NSMutableArray new];
    
    for (NSDictionary *part in rawParts) {
        
        NSLog(@"got here");
        NSMutableDictionary *partDict = [NSMutableDictionary dictionary];
        
        NSDictionary *userMelodyDict = [part objectForKey:@"UserMelody"];
        
        NSString *userId = [userMelodyDict objectForKey:@"UserId"];
        /*
        BOOL isExplicit =[[userMelodyDict objectForKey:@"IsExplicit"] boolValue];
        
        BOOL isPublic = [[userMelodyDict objectForKey:@"IsStationPostMelody"] boolValue];
        
        if (isExplicit) {
            self.explicitCheckbox.on = YES;
            self.publicCheckbox.on = NO;
        } else {
            self.explicitCheckbox.on = NO;
            self.publicCheckbox.on = YES;
        }
        */
        //if (isPublic) {
        //    self.publicCheckbox.on = YES;
        //}
        //Using isExplicit only now
        
        [partDict setObject:userId forKey:@"UserId"];
        
        NSString *myUserId = [defaults objectForKey:@"Id"];
        
        if ([userId isEqualToString:myUserId]) {
            [partDict setObject:@"My part" forKey:@"PartName"];
        } else {
            Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:userId];
            if (friend !=nil) {
                [partDict setObject:[NSString stringWithFormat:@"%@'s part", friend.firstName] forKey:@"PartName"];
            } else {
                [partDict setObject:@"Friend's part" forKey:@"PartName"];
            }
        }
        
        //get user name
        NSArray *subPartArray = [userMelodyDict objectForKey:@"Parts"];
        
        NSMutableArray *fileArray = [NSMutableArray new];
        NSMutableArray *partIdArray = [NSMutableArray new];
        
        for (NSDictionary *subPart in subPartArray) {
            [fileArray addObject:[subPart objectForKey:@"FilePath"]];
            
            BOOL isUserCreated = [[subPart objectForKey:@"IsUserCreated"] boolValue];
            
            if (!isUserCreated) {
                [partIdArray addObject:[subPart objectForKey:@"Id"]];
            }
        }
        
        [partDict setObject:fileArray forKey:@"Files"];
        [partDict setObject:partIdArray forKey:@"Ids"];
        [self.partArray addObject:partDict];
        
    }
    
    [self downloadAllFiles];
    
    UICollectionView *collectionView = (UICollectionView *)[self.tableView viewWithTag:97];
    [collectionView reloadData];
    
}

-(void)downloadAllFiles {
    for (NSDictionary *part in self.partArray) {
        NSArray *files = [part objectForKey:@"Files"];
        
        for (NSString *filePath in files) {
            
            NSArray *paths =
            NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                                NSUserDomainMask, YES);
            NSString *documentsPath = [paths objectAtIndex:0];
            
            if ([filePath containsString:@"recording"]) {
                
                NSString *recordingPath = [documentsPath stringByAppendingPathComponent:@"Recordings"];
                
                if (![[NSFileManager defaultManager] fileExistsAtPath:recordingPath]){
                    
                    NSError* error;
                    if(  [[NSFileManager defaultManager] createDirectoryAtPath:recordingPath withIntermediateDirectories:NO attributes:nil error:&error]) {
                        
                        NSLog(@"success creating folder");
                        
                    } else {
                        NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
                        NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
                    }
                    
                }
                
                NSString *localFilePath = [recordingPath stringByAppendingPathComponent:[filePath lastPathComponent]];
                
                if ([[NSFileManager defaultManager] fileExistsAtPath:localFilePath]){
                    self.currentRecordingURL = [NSURL URLWithString:localFilePath];
                } else {
                    [self downloadRecording:filePath toPath:localFilePath];
                }
                
            } else {
                
                NSString *melodyPath = [documentsPath stringByAppendingPathComponent:@"Melodies"];
                
                NSString *localFilePath = [melodyPath stringByAppendingPathComponent:[filePath lastPathComponent]];
                
                if (![[NSFileManager defaultManager] fileExistsAtPath:melodyPath]){
                    
                    NSError* error;
                    if(  [[NSFileManager defaultManager] createDirectoryAtPath:melodyPath withIntermediateDirectories:NO attributes:nil error:&error]) {
                        
                        NSLog(@"success creating folder");
                        
                    } else {
                        NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
                        NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
                    }
                    
                }
                
                if ([[NSFileManager defaultManager] fileExistsAtPath:localFilePath]){
                    NSLog(@"file already downloaded");
                } else {
                    [self downloadFile:filePath toPath:localFilePath];
                }
                
            }
        }
    }
}

-(void)roundView:(UIView *)view {
    view.layer.cornerRadius = view.frame.size.height / 2;
    view.layer.masksToBounds = YES;
}

-(void)updatePlaybackProgress {
    
    float oldProgress = self.progressView.progress;
    float newProgress =self.fgPlayer.currentTime/self.fgPlayer.duration;
    
    if (newProgress - oldProgress > 0.02) {
        [self.progressView setProgress:newProgress  animated:YES];
    }
    //NSLog(@"playback progress: %f", newProgress);
}

-(void)updateRecordProgress {
    NSDate *now = [NSDate date];
    
    float RECORDING_LIMIT = [[DataManager sharedManager] isPremium] ? PREM_RECORDING_LIMIT :  FREE_RECORDING_LIMIT;
    NSTimeInterval interval = [now timeIntervalSinceDate:self.startTime];
    
    if (interval > RECORDING_LIMIT) {
        self.startTime = [NSDate date];
        interval = 0;
    }
    
    float oldProgress = self.progressView.progress;
    float newProgress =interval/RECORDING_LIMIT;
    
    if (newProgress - oldProgress > 0.02) {
            [self.progressView setProgress:newProgress animated:YES];
    }
}

-(void)applyFontAwesome {
    //self.playButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:50.0f];
    
    if (self.selectedUserMelody != nil) {
        self.playButton.hidden = YES;
    }
    
    self.playLoopButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:25.0f];
    self.playLoop2Button.titleLabel.font = [UIFont fontAwesomeFontOfSize:25.0f];
    
    self.forwardButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:40.0f];
    self.backwardButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:40.0f];
    
    self.saveBarDelete.titleLabel.font = [UIFont fontAwesomeFontOfSize:25.0f];
    self.saveBarSave.titleLabel.font = [UIFont fontAwesomeFontOfSize:25.0f];
    
    [self.saveBarDelete setTitle:[NSString fontAwesomeIconStringForEnum:FAtrash] forState:UIControlStateNormal];
    [self.saveBarSave setTitle:[NSString fontAwesomeIconStringForEnum:FAFloppyO] forState:UIControlStateNormal];

    
    
    [self.playButton setImage:[UIImage imageNamed:@"play"] forState:UIControlStateNormal];
    [self.playLoopButton setTitle:[NSString fontAwesomeIconStringForEnum:FARefresh] forState:UIControlStateNormal];
    
    [self.playLoop2Button setTitle:[NSString fontAwesomeIconStringForEnum:FARefresh] forState:UIControlStateNormal];
    
    
    [self.forwardButton setTitle:[NSString fontAwesomeIconStringForEnum:FAFastForward] forState:UIControlStateNormal];
    [self.backwardButton setTitle:[NSString fontAwesomeIconStringForEnum:FAFastBackward] forState:UIControlStateNormal];
    
    
}

-(IBAction)join:(id)sender {
    self.isNotMyStudio = NO;
    [self updateBarStatus];
}

-(IBAction)chooseLoop:(id)sender {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Loops" message:@"Choose a loop" preferredStyle:UIAlertControllerStyleActionSheet];
    
    MelodyGroup *group = self.groupArray[0];
    
    if (group != nil) {
        
        for (Melody *melody in group.melodies) {
            UIAlertAction *action = [UIAlertAction actionWithTitle:melody.melodyName style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
                //make the button do something
                [self didSelectMelody:melody];
                
            }];
            [alert addAction:action];
            
        }
        
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
        [alert addAction:cancelAction];
    }
    
    [self presentViewController:alert animated:YES completion:nil];
}

-(IBAction)chooseLoop2:(id)sender {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Loops" message:@"Choose a loop" preferredStyle:UIAlertControllerStyleActionSheet];
    
    MelodyGroup *group = self.groupArray[0];
    
    if (group != nil) {
        
        for (Melody *melody in group.melodies) {
            UIAlertAction *action = [UIAlertAction actionWithTitle:melody.melodyName style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
                //make the button do something
                [self didSelectMelody2:melody];
                
            }];
            [alert addAction:action];
            
        }
        
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
        [alert addAction:cancelAction];
    }
    
    [self presentViewController:alert animated:YES completion:nil];
}


-(IBAction)chooseLoop3:(id)sender {
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Loops" message:@"Choose a loop" preferredStyle:UIAlertControllerStyleActionSheet];
    
    MelodyGroup *group = self.groupArray[0];
    
    if (group != nil) {
        
        for (Melody *melody in group.melodies) {
            UIAlertAction *action = [UIAlertAction actionWithTitle:melody.melodyName style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
                //make the button do something
                [self didSelectMelody3:melody];
                
            }];
            [alert addAction:action];
            
        }
        
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDestructive handler:nil];
        [alert addAction:cancelAction];
    }
    
    [self presentViewController:alert animated:YES completion:nil];
}

-(IBAction)share:(id)sender {

    //CustomActivityProvider *ActivityProvider = [[CustomActivityProvider alloc] initWithPlaceholderItem:@""];
    NSArray *itemsToShare = @[@"Check out InstaMelody in the App Store! https://itunes.apple.com/us/app/instamelody/id897451088"];
    UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:itemsToShare applicationActivities:nil];
    //activityVC.excludedActivityTypes = @[UIActivityTypePrint, UIActivityTypeAssignToContact, UIActivityTypeSaveToCameraRoll]; //or whichever you don't need
    [activityVC setValue:@"InstaMelody" forKey:@"subject"];
    
    activityVC.completionWithItemsHandler = ^(NSString *activityType, BOOL completed, NSArray *returnedItems, NSError *activityError) {
        NSLog(@"Completed successfully...");
    };
    
    [self presentViewController:activityVC animated:YES completion:nil];
    
}

-(IBAction)showVolumeSettings:(id)sender {
    
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    VolumeViewController *vc = [sb instantiateViewControllerWithIdentifier:@"VolumeViewController"];
    [self presentViewController:vc animated:YES completion:nil];

}

-(IBAction)toggleLoop:(id)sender {
    
    UIButton *toggleBtn = (UIButton *)[self.view viewWithTag:5];
    
    if ([self.bgPlayer isPlaying]) {
        [self.bgPlayer stop];
        
    } else {
        
        if (self.selectedMelody != nil) {
            [self playLoop:nil];
        }

    }
}


-(IBAction)toggleLoop2:(id)sender {
    if ([self.bgPlayer2 isPlaying]) {
        [self.bgPlayer2 stop];
    } else {
        
        if (self.selectedMelody2 != nil) {
            [self playLoop2:nil];
        }
    }
}


-(IBAction)toggleLoop3:(id)sender {
    if ([self.bgPlayer3 isPlaying]) {
        [self.bgPlayer3 stop];
    } else {
        if (self.selectedMelody3 != nil) {
            [self playLoop3:nil];
        }
    }
}

-(IBAction)previewMelodies:(id)sender {
    
    UIButton *toggleBtn = (UIButton *)[self.view viewWithTag:5];
    
    if ([self.bgPlayer isPlaying]) {
        [toggleBtn setTitle:@"Preview melodies" forState:UIControlStateNormal];
        [self stopEverything:nil];
    } else {
        [toggleBtn setTitle:@"Stop melodies" forState:UIControlStateNormal];
        [self toggleMelodies:nil];
    }
}

-(IBAction)playLoop:(id)sender {
    
    NSNumber *volume = [self.defaults objectForKey:@"melodyVolume"];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *pathString = [NSString stringWithFormat:@"%@/Melodies/%@", documentsPath, self.selectedMelody.fileName];
    
    //pathString = [pathString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
    
    NSURL *docURL = [NSURL fileURLWithPath:pathString];
    
    NSError *error = nil;
    
    self.bgPlayer = [[AVAudioPlayer alloc] initWithContentsOfURL:docURL error:&error];
    self.bgPlayer.delegate = self;
    
    if (error == nil) {
        
        [self.playButton setImage:[UIImage imageNamed:@"play"] forState:UIControlStateNormal];
        [self.bgPlayer setNumberOfLoops:-1];
        [self.bgPlayer setVolume:[volume floatValue]];
        [self.bgPlayer play];
        
    } else {
        NSLog(@"Error loading file: %@", [error description]);
        
    }

}

-(IBAction)playLoop2:(id)sender {
    NSNumber *volume = [self.defaults objectForKey:@"melodyVolume"];
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *pathString = [NSString stringWithFormat:@"%@/Melodies/%@", documentsPath, self.selectedMelody2.fileName];
    
    //pathString = [pathString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
    
    NSURL *docURL = [NSURL fileURLWithPath:pathString];
    
    NSError *error = nil;
    
    self.bgPlayer2 = [[AVAudioPlayer alloc] initWithContentsOfURL:docURL error:&error];
    self.bgPlayer2.delegate = self;
    
    if (error == nil) {
        
        [self.playButton setImage:[UIImage imageNamed:@"play"] forState:UIControlStateNormal];
        [self.bgPlayer2 setNumberOfLoops:-1];
        [self.bgPlayer2 setVolume:[volume floatValue]];
        [self.bgPlayer2 play];
        
    } else {
        NSLog(@"Error loading file: %@", [error description]);
        
    }
    
}

-(IBAction)playLoop3:(id)sender {
    NSNumber *volume = [self.defaults objectForKey:@"melodyVolume"];
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *pathString = [NSString stringWithFormat:@"%@/Melodies/%@", documentsPath, self.selectedMelody3.fileName];
    
    //pathString = [pathString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
    
    NSURL *docURL = [NSURL fileURLWithPath:pathString];
    
    NSError *error = nil;
    
    self.bgPlayer3 = [[AVAudioPlayer alloc] initWithContentsOfURL:docURL error:&error];
    self.bgPlayer3.delegate = self;
    
    if (error == nil) {
        
        [self.playButton setImage:[UIImage imageNamed:@"play"] forState:UIControlStateNormal];
        
        [self.bgPlayer3 setNumberOfLoops:-1];
        [self.bgPlayer3 setVolume:[volume floatValue]];
        [self.bgPlayer3 play];
        

    } else {
        NSLog(@"Error loading file: %@", [error description]);
        
    }
    
}

-(IBAction)playRecording:(id)sender {
    
    NSNumber *volume = [self.defaults objectForKey:@"micVolume"];
    //pathString = [pathString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
    
    NSURL *docURL = self.currentRecordingURL;
    
    NSError *error = nil;
    
    self.fgPlayer = [[AVAudioPlayer alloc] initWithContentsOfURL:docURL error:&error];
    self.fgPlayer.delegate = self;
    
    if (error == nil) {
        
        [self.playButton setImage:[UIImage imageNamed:@"pause"] forState:UIControlStateNormal];
        [self.fgPlayer setVolume:volume.floatValue];
        
        [self.fgPlayer play];
        
        NSTimeInterval interval = 0.15;
        
        /*
        if (self.fgPlayer.duration < 5.0) {
            interval = 0.2;
        }*/
        
        if (self.timer !=nil) {
            [self.timer invalidate];
        }
        
        dispatch_async(dispatch_get_main_queue(), ^{
            self.progressView.progress = 0.0;
        });
        
        self.timer = [NSTimer scheduledTimerWithTimeInterval:interval target:self selector:@selector(updatePlaybackProgress) userInfo:nil repeats:YES];
    } else {
        NSLog(@"Error loading file: %@", [error description]);
        
    }
    
}

-(IBAction)save:(id)sender {

    UITextField *topicField = (UITextField *)[self.tableView viewWithTag:98];
    BOOL isPremium = [[DataManager sharedManager] isPremium];
    
    if (self.explicitCheckbox.on != [initialIsExplicitStatus intValue] && !self.recorder) // && !self.isNewPart)
    {
        //need to save the Explicit state
        NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop/Update", API_BASE_URL];
        
        NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
        
        //add 64 char string
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
        
        NSString * boolValueAsString = (self.explicitCheckbox.on) ? @"true":@"false";
        
        NSDictionary *loopInfo = @{@"Id": [self.loopDict objectForKey:@"Id"], @"IsExplicit": boolValueAsString};
        NSDictionary *parameters = @{@"Token": token, @"Loop": loopInfo};
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            NSLog(@"Explicit status updated");
            initialIsExplicitStatus = (initialIsExplicitStatus) ? [NSNumber numberWithInt:0] : [NSNumber numberWithInt:1] ;
            if ([self.delegate respondsToSelector:@selector(setExplicit:)])
            {
                [self.delegate setExplicit:[NSNumber numberWithInt:self.explicitCheckbox.on]];
            }
            
        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
         
            NSLog(@"Problem updating explicit status");
        }];
        
    }
    
    
    if (![topicField.text isEqualToString:@""] && self.currentRecordingURL && self.recorder) {
        
        NSMutableDictionary *userDict = [NSMutableDictionary new];
        [userDict setObject:[self.currentRecordingURL path] forKey:@"LoopURL"];
        //[userDict setObject:@"dev topic" forKey:@"Description"];
        [userDict setObject:topicField.text forKey:@"Name"];
        
        if (self.selectedMelody != nil) {
            [userDict setObject:self.selectedMelody.melodyId forKey:@"MelodyId1"];
        }
        
        if (self.selectedMelody2 != nil) {
            [userDict setObject:self.selectedMelody2.melodyId forKey:@"MelodyId2"];
        }
        
        if (self.selectedMelody3 != nil) {
            [userDict setObject:self.selectedMelody3.melodyId forKey:@"MelodyId3"];
        }
        
        [userDict setObject:[NSNumber numberWithBool:self.explicitCheckbox.on ] forKey:@"IsExplicit"];
        [userDict setObject:[NSNumber numberWithBool:self.publicCheckbox.on ] forKey:@"IsStationPostMelody"];
        
        if (!isPremium && self.selectedMelody2) {
            UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Please upgrade" message:@"You must go premium to select more than 1 melody." preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *action = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:nil];
            [alert addAction:action];
            [self presentViewController:alert animated:YES completion:nil];
        } else {
            if (self.isNewPart) {
                
                NSMutableDictionary *mutableDict = [NSMutableDictionary dictionaryWithDictionary:userDict];
                
                BOOL isChatLoop = [[self.loopDict objectForKey:@"IsChatLoop"] boolValue];
                
                if (isChatLoop == true && [userDict objectForKey:@"Id"] == nil) {
                    
                    [mutableDict setObject:[self.loopDict objectForKey:@"ChatId"] forKey:@"Id"];
                    
                } else if ([self.loopDict objectForKey:@"Id"] != nil) {
                    [mutableDict setObject:[self.loopDict objectForKey:@"Id"] forKey:@"LoopId"];
                }
                [self.delegate didFinishWithInfo:mutableDict];
                
            }
            
            
            [self.navigationController popViewControllerAnimated:YES];
        }
    } else if (self.recorder) {
        UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Error" message:@"Please make sure you have selected a loop topic and recording" preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *action = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:nil];
        [alert addAction:action];
        [self presentViewController:alert animated:YES completion:nil];
    }
    
}


-(IBAction)back:(id)sender {
    //check if part > 0
    
    if (self.currentPartIndex > 0) {
         //skip to prv
        self.goBack = YES;
        [self audioPlayerDidFinishPlaying:self.fgPlayer successfully:YES];
    }
}

-(IBAction)forward:(id)sender {
    //check if part < max
    //skip to next
    if (self.currentPartIndex < self.partArray.count) {
        //self.currentPartIndex++;
        [self audioPlayerDidFinishPlaying:self.fgPlayer successfully:YES];
    }
}

-(IBAction)toggleRecording:(id)sender {
    if (self.recorder.isRecording) {
        
        [self.timer invalidate];
        self.progressView.progress = 0;
        
        [self stopRecording];
        
        [self.microphone stopFetchingAudio];
        self.profileImageView.hidden = NO;
        self.audioPlot.hidden = YES;
        self.playButton.hidden = NO;
        
    } else {
        NSError *error = nil;
        
        AVAudioSession *audioSession = [AVAudioSession sharedInstance];
        
        if ([self isHeadsetPluggedIn]) {
        
            [audioSession setCategory:AVAudioSessionCategoryPlayAndRecord
                            error:&error];
        } else {
        
            [audioSession setCategory:AVAudioSessionCategoryPlayAndRecord withOptions:AVAudioSessionCategoryOptionDefaultToSpeaker error:&error];
        }
        
        /*
        [[AudioSessionManager sharedInstance] changeMode:@"kAudioSessionManagerMode_Record"];
        [[AudioSessionManager sharedInstance] start];
         */
        
        [self.microphone startFetchingAudio];
        [self.microphone setDevice:self.inputs[0]];
        self.profileImageView.hidden = YES;
        self.audioPlot.hidden = NO;
        self.playButton.hidden = YES;
        
        if (error == nil) {
            NSArray *paths =
            NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                                NSUserDomainMask, YES);
            NSString *documentsPath = [paths objectAtIndex:0];
            
            NSString *recordingPath = [documentsPath stringByAppendingPathComponent:@"Recordings"];
            
            if (![[NSFileManager defaultManager] fileExistsAtPath:recordingPath]){
                
                NSError* error;
                if(  [[NSFileManager defaultManager] createDirectoryAtPath:recordingPath withIntermediateDirectories:NO attributes:nil error:&error]) {
                    
                    NSLog(@"success creating folder");
                    
                } else {
                    NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
                    NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
                }
                
            }
            
            time_t unixTime = time(NULL);
            
            NSString *fileName = [NSString stringWithFormat:@"recording_%d.wav", (int)unixTime];
            
            NSString *filePath = [recordingPath
                                  stringByAppendingPathComponent:fileName];
            NSURL *fileURL = [NSURL fileURLWithPath:filePath];
            NSMutableDictionary *settingsDict = [NSMutableDictionary new];
            [settingsDict setObject:[NSNumber numberWithInt:44100.0]
                             forKey:AVSampleRateKey];
            [settingsDict setObject:[NSNumber numberWithInt:2]
                             forKey:AVNumberOfChannelsKey];
            [settingsDict setObject:[NSNumber
                                     numberWithInt:AVAudioQualityMedium]
                             forKey:AVEncoderAudioQualityKey];
            
            [settingsDict setObject:[NSNumber numberWithInt:kAudioFormatLinearPCM] forKey:AVFormatIDKey];
            
            [settingsDict setObject:[NSNumber numberWithInt:16]
                             forKey:AVEncoderBitRateKey];
            self.recorder = [[AVAudioRecorder alloc]
                             initWithURL:fileURL
                             settings:settingsDict error:&error];
            if (error == nil) {
                NSLog(@"audio recorder initialized successfully!");
                
                self.currentRecordingURL = fileURL;
                
                if (self.selectedLoop || self.selectedUserMelody) {
                    self.isNewPart = YES;
                }
                
                [self.recorder record];
                
                [self toggleMelodies:nil];
                
                self.startTime = [NSDate date];
                self.timer = [NSTimer scheduledTimerWithTimeInterval:0.05 target:self selector:@selector(updateRecordProgress) userInfo:nil repeats:YES];
                
                [self.recordButton setImage:[UIImage imageNamed:@"stop"] forState:UIControlStateNormal];
                
                float RECORDING_LIMIT = [[DataManager sharedManager] isPremium] ? PREM_RECORDING_LIMIT :  FREE_RECORDING_LIMIT;
                
                [self performSelector:@selector(stopRecording) withObject:self afterDelay:RECORDING_LIMIT];
                
                /*
                if (self.selectedMelody != nil) {
                    [self playLoop:nil];
                }
                if (self.selectedMelody2 != nil) {
                    [self playLoop2:nil];
                }*/
            } else {
                NSLog(@"error initializing audio recorder: %@",
                      [error description]);
            }
        } else {
            NSLog(@"error initializing audio session: %@",
                  [error description]);
        }
        
    }
}

-(void)stopRecording {
    
    if ([self.recorder isRecording]) {
        [self.recorder stop];
        
        if ([self.bgPlayer isPlaying]) {
            [self.bgPlayer stop];
        }
        
        if ([self.bgPlayer2 isPlaying]) {
            [self.bgPlayer2 stop];
        }
        
        
        if ([self.bgPlayer3 isPlaying]) {
            [self.bgPlayer3 stop];
        }
        
        [self.recordButton setImage:[UIImage imageNamed:@"redo"] forState:UIControlStateNormal];
        
        [self.timer invalidate];
        
        self.progressView.progress = 0;
        
        self.playButton.hidden = NO;
    }

}

-(IBAction)toggleMelodies:(id)sender {
    
    [self toggleLoop:nil];
    [self toggleLoop2:nil];
    [self toggleLoop3:nil];
    
    
}

-(IBAction)stopEverything:(id)sender {
    
    if (self.timer != nil) {
        [self.timer invalidate];
        
    }
    dispatch_async(dispatch_get_main_queue(), ^{
        self.progressView.progress = 0;
    });
    [self.fgPlayer stop];
    [self.bgPlayer stop];
    [self.bgPlayer2 stop];
    [self.bgPlayer3 stop];
    
}

-(IBAction)togglePlayback:(id)sender {
    //sdf

    UIButton *toggleBtn = (UIButton *)[self.view viewWithTag:5];
    self.currentPartIndex = 0;
    
    
    if ([self.fgPlayer isPlaying] || [self.bgPlayer isPlaying]) {
    
        [self stopEverything:nil];
        
        
        [self.playButton setImage:[UIImage imageNamed:@"play"] forState:UIControlStateNormal];
        
        [self.profileImageView setImage:[UIImage imageNamed:@"Profile"]];
        [self.progressLabel setText:@"Press Play to Start"];
        
        //if (!self.isNewPart) {
            self.selectedMelody = nil;
            self.selectedMelody2 = nil;
            self.selectedMelody3 = nil;
        //}
        
        [toggleBtn setTitle:@"Preview melodies" forState:UIControlStateNormal];
        

    } else {
        
        NSError *error = nil;
        
        if (self.recorder != nil)
        {
            //there's something the user has just recorded; let's play just that.
            [self playRecording:nil];
            if (self.selectedMelody != nil) {
                [self playLoop:nil];
            }
            
            if (self.selectedMelody2 != nil) {
                [self playLoop2:nil];
            }
            
            if (self.selectedMelody3 != nil) {
                [self playLoop3:nil];
            }
            
        } else {

            AVAudioSession *audioSession = [AVAudioSession sharedInstance];
            [audioSession setCategory:AVAudioSessionCategoryPlayback
                                error:&error];
            /*
            [[AudioSessionManager sharedInstance] changeMode:@"kAudioSessionManagerMode_Playback"];
            [[AudioSessionManager sharedInstance] start];
             */
            
            if (error == nil) {
                
                /*
                
                if ([self isHeadsetPluggedIn]) {
                    
                    [self playEverything];
                } else {
                    UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Headphones not detected" message:@"For the best results, please plug in your headphones" preferredStyle:UIAlertControllerStyleAlert];
                    UIAlertAction *okAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
                        [self playEverything];
                    }];
                    [alert addAction:okAction];
                    [self presentViewController:alert animated:YES completion:nil];
                }
                 
                 */
                
                [self playEverything];
            } else {
                UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"Error" message:@"Error setting audio" preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *okAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
                    [self playEverything];
                }];
                [alert addAction:okAction];
            }
        
        }
        
    }
}

-(void)preload {
    
    
    NSArray *paths =
    NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                        NSUserDomainMask, YES);
    NSString *documentsPath = [paths objectAtIndex:0];
    
    NSArray *files = [[self.partArray objectAtIndex:self.currentPartIndex] objectForKey:@"Files"];
    NSArray *partIds = [[self.partArray objectAtIndex:self.currentPartIndex] objectForKey:@"Ids"];
    for (NSString *filePath in files) {
        if ([filePath containsString:@"recording"]) {
            
            NSString *recordingPath = [documentsPath stringByAppendingPathComponent:@"Recordings"];
            
            if (![[NSFileManager defaultManager] fileExistsAtPath:recordingPath]){
                
                NSError* error;
                if(  [[NSFileManager defaultManager] createDirectoryAtPath:recordingPath withIntermediateDirectories:NO attributes:nil error:&error]) {
                    
                    NSLog(@"success creating folder");
                    
                } else {
                    NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
                    NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
                }
                
            }
            
            NSString *localFilePath = [recordingPath stringByAppendingPathComponent:[filePath lastPathComponent]];
            
            if ([[NSFileManager defaultManager] fileExistsAtPath:localFilePath]){
                self.currentRecordingURL = [NSURL URLWithString:localFilePath];
            } else {
                [self downloadRecording:filePath toPath:localFilePath];
            }
            
        }
        
    }
    
    int count = 0;
    
    self.progressView.progress = 0.0;
    for (NSString *partId in partIds) {
        
        //get and set recording
        
        //part.fileName
        
        if (count == 0) {
            Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:partId];
            
            //get and set system melodies
            [self didSelectMelody:melody];
        } else if (count == 1) {
            //
            
            Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:partId];
            
            [self didSelectMelody2:melody];
            
        } else if (count == 2) {
            //
            
            Melody *melody = [Melody MR_findFirstByAttribute:@"melodyId" withValue:partId];
            
            [self didSelectMelody3:melody];
            
        }
        count = count+ 1;
    }
    
    
    NSString *stringText = [NSString stringWithFormat:@"%@ (%ld/%ld)", [[self.partArray objectAtIndex:self.currentPartIndex] objectForKey:@"PartName"], (self.currentPartIndex+1), self.partArray.count];
    
    [self.progressLabel setText:stringText];
    
    //load friend pic here
    NSString *userId = [[self.partArray objectAtIndex:self.currentPartIndex] objectForKey:@"UserId"];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *myUserId = [defaults objectForKey:@"Id"];
    
    if ([userId isEqualToString:myUserId]) {
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
        
    } else {
        Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:userId];
        
        NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
        
        if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
            
            NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
            
            NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
            NSString *imageName = [friend.profileFilePath lastPathComponent];
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
            
        } else if (friend != nil) {
            NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
            
            if (friend == nil) {
                userName = @"User";
            }
            
            [self.profileImageView setImageWithString:userName color:nil circular:YES];
        }
        
    }
    
    
}

-(void)playEverything {
    
    [self.fgPlayer stop];
    [self.bgPlayer stop];
    [self.bgPlayer2 stop];
    [self.bgPlayer3 stop];
    
    if (self.selectedLoop != nil) {
        
        //if (!self.isNewPart) {
            [self preload];
        //}
    }
    
    [self playRecording:nil];
    
    if (self.selectedMelody != nil) {
        [self playLoop:nil];
    }
    
    if (self.selectedMelody2 != nil) {
        [self playLoop2:nil];
    }
    
    if (self.selectedMelody3 != nil) {
        [self playLoop3:nil];
    }
    
    [self.playButton setImage:[UIImage imageNamed:@"pause"] forState:UIControlStateNormal];
}

-(void)didSelectMelody:(Melody *)melody {
    [self.chooseLoopButton setTitle:melody.melodyName forState:UIControlStateNormal];
    
    [self loadMelody:melody];
}

-(void)didSelectMelody2:(Melody *)melody {
    [self.chooseLoop2Button setTitle:melody.melodyName forState:UIControlStateNormal];
    
    [self loadMelody2:melody];
}

-(void)didSelectMelody3:(Melody *)melody {
    
    [self loadMelody3:melody];
}

-(void)loadMelody:(Melody *)melody {
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *melodyPath = [documentsPath stringByAppendingPathComponent:@"Melodies"];
    
    if (![[NSFileManager defaultManager] fileExistsAtPath:melodyPath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:melodyPath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    
    NSURL *docURL = [NSURL fileURLWithPath:documentsPath];
    NSArray *contents = [fileManager contentsOfDirectoryAtURL:docURL
                                   includingPropertiesForKeys:@[]
                                                      options:NSDirectoryEnumerationSkipsHiddenFiles
                                                        error:nil];
    
    
    
    NSString *pathString = [melodyPath stringByAppendingPathComponent:melody.fileName];
    
    //NSString *filePath = [documentsPath stringByAppendingPathComponent:pathString];
    
    self.selectedMelody = melody;
    
    //if file exists, load, set status = loaded,
    if ([fileManager fileExistsAtPath:pathString]) {
        //
        
        self.loopStatusLabel.text = @"Melody loaded!";
        
        //self.playButton.hidden = NO;
        
    } else {
        //else, download, show progress, set loaded, set play button
        
        self.loopStatusLabel.text = @"Melody downloading (0%)";
        
        [self downloadFile:melody.filePathUrlString toPath:pathString];
    }
}

-(void)loadMelody2:(Melody *)melody {
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *melodyPath = [documentsPath stringByAppendingPathComponent:@"Melodies"];
    
    if (![[NSFileManager defaultManager] fileExistsAtPath:melodyPath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:melodyPath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    
    NSURL *docURL = [NSURL fileURLWithPath:documentsPath];
    NSArray *contents = [fileManager contentsOfDirectoryAtURL:docURL
                                   includingPropertiesForKeys:@[]
                                                      options:NSDirectoryEnumerationSkipsHiddenFiles
                                                        error:nil];
    
    
    
    NSString *pathString = [melodyPath stringByAppendingPathComponent:melody.fileName];
    
    //NSString *filePath = [documentsPath stringByAppendingPathComponent:pathString];
    
    self.selectedMelody2 = melody;
    
    //if file exists, load, set status = loaded,
    if ([fileManager fileExistsAtPath:pathString]) {
        //
        
        self.loopStatusLabel.text = @"Melody loaded!";
        
        //self.playButton.hidden = NO;
        
    } else {
        //else, download, show progress, set loaded, set play button
        
        self.loopStatusLabel.text = @"Melody downloading (0%)";
        
        [self downloadFile:melody.filePathUrlString toPath:pathString];
    }
}

-(void)loadMelody3:(Melody *)melody {
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *melodyPath = [documentsPath stringByAppendingPathComponent:@"Melodies"];
    
    if (![[NSFileManager defaultManager] fileExistsAtPath:melodyPath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:melodyPath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    
    NSURL *docURL = [NSURL fileURLWithPath:documentsPath];
    NSArray *contents = [fileManager contentsOfDirectoryAtURL:docURL
                                   includingPropertiesForKeys:@[]
                                                      options:NSDirectoryEnumerationSkipsHiddenFiles
                                                        error:nil];
    
    
    
    NSString *pathString = [melodyPath stringByAppendingPathComponent:melody.fileName];
    
    self.selectedMelody3 = melody;
    
    //if file exists, load, set status = loaded,
    if ([fileManager fileExistsAtPath:pathString]) {
        //
        
        self.loopStatusLabel.text = @"Melody loaded!";
        
        //self.playButton.hidden = NO;
        
    } else {
        //else, download, show progress, set loaded, set play button
        
        self.loopStatusLabel.text = @"Melody downloading (0%)";
        
        [self downloadFile:melody.filePathUrlString toPath:pathString];
    }
}

-(void)downloadFile:(NSString *)sourceFilePath toPath:(NSString *)destinationFilePath {
    NSURLSessionConfiguration *configuration = [NSURLSessionConfiguration defaultSessionConfiguration];
    AFURLSessionManager *manager = [[AFURLSessionManager alloc] initWithSessionConfiguration:configuration];
    
    NSString *fullUrlString = [NSString stringWithFormat:@"%@/%@", DOWNLOAD_BASE_URL, sourceFilePath];
    fullUrlString = [fullUrlString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
    
    NSURL *URL = [NSURL URLWithString:fullUrlString];
    NSURLRequest *request = [NSURLRequest requestWithURL:URL];
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    NSString *melodyPath = [documentsPath stringByAppendingPathComponent:@"Melodies"];
    
    if (![fileManager fileExistsAtPath:melodyPath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:melodyPath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    
    NSProgress *progress = nil;
    
    NSURLSessionDownloadTask *downloadTask = [manager downloadTaskWithRequest:request progress:&progress destination:^NSURL *(NSURL *targetPath, NSURLResponse *response) {
        
        //NSString *fileString = [NSString stringWithFormat:@"file://%@", destinationFilePath];
        NSURL *fileURL = [NSURL fileURLWithPath:destinationFilePath];
        
        return fileURL;
         
        //NSURL *documentsDirectoryURL = [[NSFileManager defaultManager] URLForDirectory:NSDocumentDirectory inDomain:NSUserDomainMask appropriateForURL:nil create:NO error:nil];
        //return [documentsDirectoryURL URLByAppendingPathComponent:[response suggestedFilename]];
    } completionHandler:^(NSURLResponse *response, NSURL *filePath, NSError *error) {
        
        if (error == nil) {
            NSLog(@"File downloaded to: %@", filePath);
            self.loopStatusLabel.text = @"Melody loaded!";
            
            //self.playButton.hidden = NO;
        } else {
            NSLog(@"Download error: %@", error.description);
            self.loopStatusLabel.text = @"Error loading melody";
        }
        [progress removeObserver:self forKeyPath:@"fractionCompleted" context:NULL];
    }];
    [downloadTask resume];
    
    [progress addObserver:self
               forKeyPath:@"fractionCompleted"
                  options:NSKeyValueObservingOptionNew
                  context:NULL];
}

-(void)downloadRecording:(NSString *)sourceFilePath toPath:(NSString *)destinationFilePath {
    NSURLSessionConfiguration *configuration = [NSURLSessionConfiguration defaultSessionConfiguration];
    AFURLSessionManager *manager = [[AFURLSessionManager alloc] initWithSessionConfiguration:configuration];
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    NSString *recordingPath = [documentsPath stringByAppendingPathComponent:@"Recordings"];
    
    if (![fileManager fileExistsAtPath:recordingPath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:recordingPath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    
    NSString *fullUrlString = [NSString stringWithFormat:@"%@/%@", DOWNLOAD_BASE_URL, sourceFilePath];
    fullUrlString = [fullUrlString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
    
    NSURL *URL = [NSURL URLWithString:fullUrlString];
    NSURLRequest *request = [NSURLRequest requestWithURL:URL];
    
    NSProgress *progress = nil;
    
    NSURLSessionDownloadTask *downloadTask = [manager downloadTaskWithRequest:request progress:&progress destination:^NSURL *(NSURL *targetPath, NSURLResponse *response) {
        
        //NSString *fileString = [NSString stringWithFormat:@"file://%@", destinationFilePath];
        NSURL *fileURL = [NSURL fileURLWithPath:destinationFilePath];
        
        return fileURL;
        
        //NSURL *documentsDirectoryURL = [[NSFileManager defaultManager] URLForDirectory:NSDocumentDirectory inDomain:NSUserDomainMask appropriateForURL:nil create:NO error:nil];
        //return [documentsDirectoryURL URLByAppendingPathComponent:[response suggestedFilename]];
    } completionHandler:^(NSURLResponse *response, NSURL *filePath, NSError *error) {
        
        if (error == nil) {
            NSLog(@"File downloaded to: %@", filePath);
            self.loopStatusLabel.text = @"Recording loaded!";
            self.progressView.progress = 0.0;
            [self.timer invalidate];
            
            self.currentRecordingURL = filePath;
            
            self.playButton.hidden = NO;
        } else {
            NSLog(@"Download error: %@", error.description);
            self.loopStatusLabel.text = @"Error loading recording";
        }
        [progress removeObserver:self forKeyPath:@"fractionCompleted" context:NULL];
    }];
    [downloadTask resume];
    
    [progress addObserver:self
               forKeyPath:@"fractionCompleted"
                  options:NSKeyValueObservingOptionNew
                  context:NULL];
}

- (void)observeValueForKeyPath:(NSString *)keyPath ofObject:(id)object change:(NSDictionary *)change context:(void *)context
{
    if ([keyPath isEqualToString:@"fractionCompleted"]) {
        NSProgress *progress = (NSProgress *)object;
        
        dispatch_async(dispatch_get_main_queue(), ^{
            //Wants to update UI or perform any task on main thread.
            double percent = progress.fractionCompleted * 100.0;
            self.loopStatusLabel.text = [NSString stringWithFormat:@"Downloading (%.0f%%)", percent];
        });
        
        NSLog(@"Progress… %f", progress.fractionCompleted);
    } else {
        [super observeValueForKeyPath:keyPath ofObject:object change:change context:context];
    }
}

- (BOOL)isHeadsetPluggedIn {
    AVAudioSessionRouteDescription* route = [[AVAudioSession sharedInstance] currentRoute];
    for (AVAudioSessionPortDescription* desc in [route outputs]) {
        if ([[desc portType] isEqualToString:AVAudioSessionPortHeadphones])
            return YES;
    }
    return NO;
}

-(void)audioPlayerDidFinishPlaying: (AVAudioPlayer *)player successfully:(BOOL)flag
{
    
    [self.timer invalidate];
    self.progressView.progress = 0;
    UIButton *toggleBtn = (UIButton *)[self.view viewWithTag:5];
    [toggleBtn setTitle:@"Preview melodies" forState:UIControlStateNormal];
    
    
    if (flag && player == self.fgPlayer) {
        [self.playButton setImage:[UIImage imageNamed:@"play"] forState:UIControlStateNormal];

        //if (player == self.fgPlayer) {
            [self.bgPlayer stop];
            [self.bgPlayer2 stop];
            [self.bgPlayer3 stop];
        //}
        
        if (self.selectedLoop && !self.recorder) { // && !self.isNewPart) {
            
            NSInteger partCount = MAX(0, self.partArray.count -1);
            
            if (self.currentPartIndex < partCount) {
                
                if (self.goBack) {
                    self.currentPartIndex--;
                    //[self preload];
                    self.goBack = NO;
                } else {
                    self.currentPartIndex++;
                }
                //[self preload];
                [self performSelector:@selector(playEverything) withObject:nil afterDelay:0.1];
                //[self playEverything];
            } else {
                [self.profileImageView setImage:[UIImage imageNamed:@"Profile"]];
                self.currentPartIndex = 0;
            }
            
        } else {
            [self.profileImageView setImage:[UIImage imageNamed:@"Profile"]];
            self.currentPartIndex = 0;
        }
    }
}




#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    
    if (self.selectedLoop != nil) {
        return 4;
    }
    return 3;
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    CGFloat height = 40.0f;

    if (self.selectedLoop != nil) {
        switch (indexPath.row) {
            case 0:
                //
                height = 111.0f;
                break;
            case 1:
                height = 87.0f;
                break;
            case 2:
                height = 151.0f;
                break;
            case 3:
                height = 59.0f;
                break;
            default:
                break;
        }
    } else {
        switch (indexPath.row) {
            case 0:
                //
                height = 87.0f;
                break;
            case 1:
                height = 151.0f;
                break;
            case 2:
                height = 59.0f;
                break;
            default:
                break;
        }
    }
    
    return height;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    if (self.selectedLoop != nil) {
        
        if (indexPath.row == 0) {
            UsersCell *cell = (UsersCell *)[tableView dequeueReusableCellWithIdentifier:@"UserCollectionCell" forIndexPath:indexPath];
            cell.collectionView.tag = 97;
            return cell;
        } else if (indexPath.row == 1) {
            TopicCell *cell = (TopicCell *)[tableView dequeueReusableCellWithIdentifier:@"TopicCell" forIndexPath:indexPath];
            cell.topicField.tag = 98;
            
            if (self.selectedLoop !=nil) {
                cell.topicField.text = [self.selectedLoop objectForKey:@"Name"];
                
                //NSDictionary *melodyDict = [partDict objectForKey:@"UserMelody"];
                
                NSString *userId = [self.selectedLoop objectForKey:@"UserId"];
                
                Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:userId];
                
                NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
                
                if (friend == nil) {
                    userName = @"User";
                }
                
                dispatch_async(dispatch_get_main_queue(), ^{
                    self.saveBarTopicLabel.text = [self.selectedLoop objectForKey:@"Name"];
                    //self.saveBarStationLabel.text = userName;
                });

                [cell.topicField setEnabled:NO];
                [cell.topicField setBackgroundColor:[UIColor lightGrayColor]];
            } else if (self.topicString != nil) {
                cell.topicField.text = self.topicString;
            }
            
            if (self.selectedUserMelody != nil) {
                cell.topicField.text = self.selectedUserMelody.userMelodyName;
                
                dispatch_async(dispatch_get_main_queue(), ^{
                    self.saveBarTopicLabel.text = self.selectedUserMelody.userMelodyName;
                });
                [cell.topicField setEnabled:NO];
                [cell.topicField setBackgroundColor:[UIColor lightGrayColor]];
            } else if (self.topicString != nil) {
                cell.topicField.text = self.topicString;
            }
            return cell;
        } else if (indexPath.row == 2) {
            
            MelodyCell *cell = (MelodyCell *)[tableView dequeueReusableCellWithIdentifier:@"MelodyCell" forIndexPath:indexPath];
            //UILabel *melodyLabel = [cell viewWithTag:911];
            
            cell.tokenInputView.tag = 99;
            
            cell.tokenInputView.layer.cornerRadius = 4.0f;
            cell.tokenInputView.layer.masksToBounds = YES;
            
            cell.tokenInputView.placeholderText = @"Add melodies";
            
            cell.tokenInputView.accessoryView = [self contactAddButton];

            return cell;
        } else {
            StatusCell *cell = (StatusCell *)[tableView dequeueReusableCellWithIdentifier:@"StatusCell" forIndexPath:indexPath];
            self.loopStatusLabel = cell.statusLabel;
            [cell.playButton setTag:5];
            [cell.playButton addTarget:self action:@selector(previewMelodies:) forControlEvents:UIControlEventTouchUpInside];
            [cell.playButton setTitle:@"Preview melodies" forState:UIControlStateNormal];
            return cell;
            
        }
        
    } else {
        
        if (indexPath.row == 0) {
            TopicCell *cell = (TopicCell *)[tableView dequeueReusableCellWithIdentifier:@"TopicCell" forIndexPath:indexPath];
            cell.topicField.tag = 98;
            if (self.selectedUserMelody != nil) {
                cell.topicField.text = self.selectedUserMelody.userMelodyName;
                [cell.topicField setEnabled:NO];
                [cell.topicField setBackgroundColor:[UIColor lightGrayColor]];
                dispatch_async(dispatch_get_main_queue(), ^{
                    self.saveBarTopicLabel.text = self.selectedUserMelody.userMelodyName;
                });
            } else if (self.topicString != nil) {
                cell.topicField.text = self.topicString;
            }
            
            return cell;
        } else if (indexPath.row == 1) {
            MelodyCell *cell = (MelodyCell *)[tableView dequeueReusableCellWithIdentifier:@"MelodyCell" forIndexPath:indexPath];
            
            cell.tokenInputView.tag = 99;
            
            
            cell.tokenInputView.layer.cornerRadius = 4.0f;
            cell.tokenInputView.layer.masksToBounds = YES;
            
            cell.tokenInputView.placeholderText = @"Add melodies";
            
            cell.tokenInputView.accessoryView = [self contactAddButton];
            
            return cell;
        } else {
            
            StatusCell *cell = (StatusCell *)[tableView dequeueReusableCellWithIdentifier:@"StatusCell" forIndexPath:indexPath];
            self.loopStatusLabel = cell.statusLabel;
            [cell.playButton setTag:5];
            [cell.playButton addTarget:self action:@selector(previewMelodies:) forControlEvents:UIControlEventTouchUpInside];
            return cell;
            
        }

    }
    
    /*
    UserMelody *userMelody = (UserMelody *)[self.melodyList objectAtIndex:indexPath.row];
    
    //NSDictionary *friendDict = [self.friendsList objectAtIndex:indexPath.row];
    
    cell.textLabel.text = userMelody.userMelodyName;
    cell.detailTextLabel.text = [NSString stringWithFormat:@"%ld Parts", userMelody.parts.count];
    cell.backgroundColor = [UIColor clearColor];*/
    
    return nil;
}

#pragma mark - UICollectionView Datasource
// 1
- (NSInteger)collectionView:(UICollectionView *)view numberOfItemsInSection:(NSInteger)section {
    
    if (self.partArray != nil) {
        return [self.partArray count];
    }
    return 0;
}

- (NSInteger)numberOfSectionsInCollectionView: (UICollectionView *)collectionView {
    if (self.partArray != nil) {
        return 1;
    }
    return 0;
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)cv cellForItemAtIndexPath:(NSIndexPath *)indexPath {
    UserCell *cell = [cv dequeueReusableCellWithReuseIdentifier:@"UserCell" forIndexPath:indexPath];
    cell.imageView.layer.cornerRadius = cell.imageView.frame.size.height / 2;
    cell.imageView.layer.masksToBounds = YES;
    
    NSDictionary *partDict = [self.partArray objectAtIndex:indexPath.row];
    
    //NSDictionary *melodyDict = [partDict objectForKey:@"UserMelody"];
    
    NSString *userId = [partDict objectForKey:@"UserId"];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *myUserId = [defaults objectForKey:@"Id"];
    
    if ([userId isEqualToString:myUserId]) {
        cell.nameLabel.text = @"Me";
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        cell.imageView.image = [UIImage imageWithContentsOfFile:imagePath];
        
    } else {
        Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:userId];
        
        NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
        cell.nameLabel.text = friend.firstName;
        
        if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
            
            NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
            
            NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
            NSString *imageName = [friend.profileFilePath lastPathComponent];
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            cell.imageView.image = [UIImage imageWithContentsOfFile:imagePath];
            
        } else if (friend !=nil ){
            NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
            if (friend.firstName == nil) {
                userName = @"User";
            }
            
            [cell.imageView setImageWithString:userName color:nil circular:YES];
        }
        

    }

    return cell;
}

-(void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath  {
    
    if (indexPath.row != self.currentPartIndex) {
        


    }
    
    //self.isNewPart = NO;
    
    self.currentPartIndex = indexPath.row - 1 ;
    [self audioPlayerDidFinishPlaying:self.fgPlayer successfully:YES];
    
    //if final item, add user to loop
    
}

#pragma mark - token delegate

#pragma mark - CLTokenInputViewDelegate

/*
- (void)tokenInputView:(CLTokenInputView *)view didChangeText:(NSString *)text
{
    if ([text isEqualToString:@""]){
        self.filteredNames = nil;
        self.tableView.hidden = YES;
    } else {
        NSPredicate *predicate = [NSPredicate predicateWithFormat:@"self contains[cd] %@", text];
        self.filteredNames = [self.names filteredArrayUsingPredicate:predicate];
        self.tableView.hidden = NO;
    }
    [self.tableView reloadData];
}
 */

/*
- (void)tokenInputView:(CLTokenInputView *)view didAddToken:(CLToken *)token
{
    NSString *name = token.displayText;
    //add to list
}*/

- (void)tokenInputView:(CLTokenInputView *)view didRemoveToken:(CLToken *)token
{
    NSString *name = token.displayText;
    if ([self.selectedMelody.melodyName isEqualToString:name]) {
        self.selectedMelody = nil;
    } else if ([self.selectedMelody2.melodyName isEqualToString:name]) {
        self.selectedMelody2 = nil;
    }  else if ([self.selectedMelody3.melodyName isEqualToString:name]) {
        self.selectedMelody3 = nil;
    }
    
    [self.view endEditing:YES];
    
}

/*

- (CLToken *)tokenInputView:(CLTokenInputView *)view tokenForText:(NSString *)text
{
    if (self.filteredNames.count > 0) {
        NSString *matchingName = self.filteredNames[0];
        CLToken *match = [[CLToken alloc] initWithDisplayText:matchingName context:nil];
        return match;
    }
    // TODO: Perhaps if the text is a valid phone number, or email address, create a token
    // to "accept" it.
    return nil;
}

- (void)tokenInputViewDidEndEditing:(CLTokenInputView *)view
{
    NSLog(@"token input view did end editing: %@", view);
    //view.accessoryView = nil;
}

- (void)tokenInputViewDidBeginEditing:(CLTokenInputView *)view
{
    
    NSLog(@"token input view did begin editing: %@", view);
    //view.accessoryView = [self contactAddButton];
    [self.view removeConstraint:self.tableViewTopLayoutConstraint];
    self.tableViewTopLayoutConstraint = [NSLayoutConstraint constraintWithItem:self.tableView attribute:NSLayoutAttributeTop relatedBy:NSLayoutRelationEqual toItem:view attribute:NSLayoutAttributeBottom multiplier:1.0 constant:0];
    [self.view addConstraint:self.tableViewTopLayoutConstraint];
    [self.view layoutIfNeeded];
}
*/

#pragma mark - Demo Buttons
- (UIButton *)contactAddButton
{
    UIButton *contactAddButton = [UIButton buttonWithType:UIButtonTypeContactAdd];
    [contactAddButton addTarget:self action:@selector(onAccessoryContactAddButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    return contactAddButton;
}

- (void)onAccessoryContactAddButtonTapped:(id)sender
{
    
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    UINavigationController *vc = (UINavigationController *)[sb instantiateViewControllerWithIdentifier:@"MelodyGroupNavController"];
    //vc.delegate = self;
    MelodyGroupController *groupVC = vc.topViewController;
    groupVC.groupId = self.savedGroupId;
    [self presentViewController:vc animated:YES completion:nil];
    
}

#pragma mark - text view delegate

- (BOOL)textFieldShouldReturn:(UITextField *)textField {
    
    /*
    if (textField == self.inputText) {
        [textField resignFirstResponder];
        return NO;
    }*/
    [textField resignFirstResponder];
    return YES;
}

- (void)didTapCheckBox:(BEMCheckBox *)checkBox
{
    if (checkBox.tag == 1)
    {
        //Explicit
        self.publicCheckbox.on = NO;
        
    } else {
        //Public
        self.explicitCheckbox.on = NO;

    }
    
}

/*
- (BOOL)textView:(UITextView *)textView shouldChangeTextInRange:(NSRange)range replacementText:(NSString *)text {
    NSRange resultRange = [text rangeOfCharacterFromSet:[NSCharacterSet newlineCharacterSet] options:NSBackwardsSearch];
    if ([text length] == 1 && resultRange.location != NSNotFound) {
        [textView resignFirstResponder];
        return NO;
    }
    
    return YES;
}*/


//------------------------------------------------------------------------------
#pragma mark - Utility
//------------------------------------------------------------------------------

/*
 Give the visualization of the current buffer (this is almost exactly the openFrameworks audio input eample)
 */
- (void)drawBufferPlot
{
    self.audioPlot.plotType = EZPlotTypeBuffer;
    self.audioPlot.shouldMirror = NO;
    self.audioPlot.shouldFill = NO;
}

//------------------------------------------------------------------------------

/*
 Give the classic mirrored, rolling waveform look
 */
-(void)drawRollingPlot
{
    self.audioPlot.plotType = EZPlotTypeRolling;
    self.audioPlot.shouldFill = YES;
    self.audioPlot.shouldMirror = YES;
}

#pragma mark - EZMicrophoneDelegate
#warning Thread Safety
// Note that any callback that provides streamed audio data (like streaming
// microphone input) happens on a separate audio thread that should not be
// blocked. When we feed audio data into any of the UI components we need to
// explicity create a GCD block on the main thread to properly get the UI
// to work.
- (void)microphone:(EZMicrophone *)microphone
  hasAudioReceived:(float **)buffer
    withBufferSize:(UInt32)bufferSize
withNumberOfChannels:(UInt32)numberOfChannels
{
    // Getting audio data as an array of float buffer arrays. What does that mean?
    // Because the audio is coming in as a stereo signal the data is split into
    // a left and right channel. So buffer[0] corresponds to the float* data
    // for the left channel while buffer[1] corresponds to the float* data
    // for the right channel.
    
    // See the Thread Safety warning above, but in a nutshell these callbacks
    // happen on a separate audio thread. We wrap any UI updating in a GCD block
    // on the main thread to avoid blocking that audio flow.
    __weak typeof (self) weakSelf = self;
    dispatch_async(dispatch_get_main_queue(), ^{
        // All the audio plot needs is the buffer data (float*) and the size.
        // Internally the audio plot will handle all the drawing related code,
        // history management, and freeing its own resources.
        // Hence, one badass line of code gets you a pretty plot :)
        [weakSelf.audioPlot updateBuffer:buffer[0] withBufferSize:bufferSize];
    });
}

//------------------------------------------------------------------------------

- (void)microphone:(EZMicrophone *)microphone hasAudioStreamBasicDescription:(AudioStreamBasicDescription)audioStreamBasicDescription
{
    // The AudioStreamBasicDescription of the microphone stream. This is useful
    // when configuring the EZRecorder or telling another component what
    // audio format type to expect.
    [EZAudioUtilities printASBD:audioStreamBasicDescription];
}

//------------------------------------------------------------------------------

- (void)microphone:(EZMicrophone *)microphone
     hasBufferList:(AudioBufferList *)bufferList
    withBufferSize:(UInt32)bufferSize
withNumberOfChannels:(UInt32)numberOfChannels
{
    // Getting audio data as a buffer list that can be directly fed into the
    // EZRecorder or EZOutput. Say whattt...
}

@end
