//
//  LoopViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/17/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "DACircularProgressView.h"
#import "VolumeViewController.h"
#import <AVFoundation/AVFoundation.h>
#import "DataManager.h"
#import "MelodyGroup.h"
#import "UserMelody.h"
#import "UserCell.h"
#import "MelodyCell.h"
#import "TopicCell.h"
#import "StatusCell.h"
#import "UsersCell.h"
#import "EZAudio.h"
#import "UIImageView+Letters.h"
#import "CLTokenInputView.h"
#import "BEMCheckBox.h"

@protocol LoopDelegate <NSObject>

-(void)didFinishWithInfo:(NSDictionary *)userDict;
-(void)cancel;
-(void)setExplicit:(NSString *)isExplicit;

@end

@interface LoopViewController : UIViewController <AVAudioPlayerDelegate, EZMicrophoneDelegate, UITableViewDataSource, UITableViewDelegate, CLTokenInputViewDelegate, UITextFieldDelegate, BEMCheckBoxDelegate>

@property BOOL isNotMyStudio;
@property BOOL isForeignChatLoop;

@property (nonatomic, strong) NSDictionary *selectedLoop;
//@property (nonatomic, strong) UserMelody *selectedUserMelody;
//Should comment out above

@property (nonatomic, strong) IBOutlet UIBarButtonItem *volumeBarButtonItem;

//@property (nonatomic, strong) IBOutlet UIButton *chooseLoopButton;
//@property (nonatomic, strong) IBOutlet UIButton *chooseLoop2Button;
//@property (nonatomic, strong) IBOutlet UIButton *playLoopButton;
//@property (nonatomic, strong) IBOutlet UIButton *playLoop2Button;
@property (nonatomic, strong) IBOutlet UIButton *shareButton;
@property (nonatomic, strong) IBOutlet UIButton *volumeButton;

@property (nonatomic, strong) IBOutlet UIButton *recordButton;
@property (nonatomic, strong) IBOutlet UIButton *playButton;

@property (nonatomic, strong) IBOutlet UIButton *forwardButton;
@property (nonatomic, strong) IBOutlet UIButton *backwardButton;

@property (nonatomic, strong) IBOutlet UILabel *loopStatusLabel;
@property (nonatomic, strong) IBOutlet UILabel *progressLabel;

@property (nonatomic, strong) IBOutlet UILabel *saveBarTopicLabel;
@property (nonatomic, strong) IBOutlet UILabel *saveBarStationLabel;
@property (nonatomic, strong) IBOutlet UILabel *joinBarTopicLabel;
@property (nonatomic, strong) IBOutlet UILabel *joinBarStationLabel;
@property (nonatomic, strong) IBOutlet UILabel *joinBarModLabel;
@property (nonatomic, strong) IBOutlet UIImageView *joinBarProfile;

@property IBOutlet UIButton *saveBarDelete;
@property IBOutlet UIButton *saveBarSave;
@property IBOutlet UIButton *joinButton;


@property (nonatomic, strong) IBOutlet UIView *publicView;
@property (nonatomic, strong) IBOutlet UIView *explicitView;
@property IBOutlet BEMCheckBox *publicCheckbox;
@property IBOutlet BEMCheckBox *explicitCheckbox;




@property (nonatomic, strong) IBOutlet UIView *saveBar;
@property (nonatomic, strong) IBOutlet UIView *joinBar;
@property (nonatomic, strong) IBOutlet UIView *consoleView;
@property (nonatomic, strong) IBOutlet UIView *centerConsoleView;
@property (nonatomic, strong) IBOutlet DACircularProgressView *progressView;
@property (nonatomic, strong) IBOutlet UIImageView *profileImageView;

@property (nonatomic, strong) IBOutlet UIScrollView *scrollView;
@property (nonatomic, strong) IBOutlet UICollectionView *participantsView;

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, strong) id <LoopDelegate> delegate;

@property (nonatomic, strong) NSDictionary *loopDict;

@property (nonatomic, strong) NSString *topicString;
@property BOOL isFromChat;

/**
 The CoreGraphics based audio plot
 */
@property (nonatomic, weak) IBOutlet EZAudioPlot *audioPlot;

/**
 The microphone component
 */
@property (nonatomic, strong) EZMicrophone *microphone;

-(IBAction)showVolumeSettings:(id)sender;
-(IBAction)share:(id)sender;
//-(IBAction)chooseLoop:(id)sender;
//-(IBAction)chooseLoop2:(id)sender;
//-(IBAction)playLoop:(id)sender;
//-(IBAction)playLoop2:(id)sender;
-(IBAction)toggleRecording:(id)sender;
-(IBAction)togglePlayback:(id)sender;
-(IBAction)save:(id)sender;

//-(IBAction)toggleLoop:(id)sender;
//-(IBAction)toggleLoop2:(id)sender;

-(IBAction)back:(id)sender;
-(IBAction)forward:(id)sender;


@end
