//
//  HomeViewController.h
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import <UIKit/UIKit.h>
#import "LoopViewController.h"
#import "NetworkManager.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "InstamelodyViewController.h"

@interface HomeViewController : InstamelodyViewController <UIImagePickerControllerDelegate, LoopDelegate>

@property (nonatomic, strong) IBOutlet UIView *profileView;
@property (nonatomic, strong) IBOutlet UIImageView *profileImageView;
@property (nonatomic, strong) IBOutlet UILabel *nameLabel;
@property (nonatomic, strong) IBOutlet UILabel *displayNameLabel;

@property (nonatomic, strong) IBOutlet UIButton *stationButton;
@property (nonatomic, strong) IBOutlet UIButton *feedButton;
@property (nonatomic, strong) IBOutlet UIButton *messengerButton;
@property (nonatomic, strong) IBOutlet UIButton *studioButton;
@property (nonatomic, strong) IBOutlet UIButton *storeButton;
@property (nonatomic, strong) IBOutlet UIButton *libraryButton;
@property (nonatomic, strong) IBOutlet UIButton *showcaseButton;
@property (nonatomic, strong) IBOutlet UIButton *friendsButton;
@property (nonatomic, strong) IBOutlet UIButton *followingButton;
@property (nonatomic, strong) IBOutlet UIImageView *liveImageView;

@property (nonatomic, strong) IBOutlet UIView *pathMenuView;
@property (nonatomic, strong) NSDateFormatter *dateFormatter;

-(IBAction)showSettings:(id)sender;

@end
