//
//  HomeViewController.h
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import <UIKit/UIKit.h>
#import "AwesomeMenu.h"
#import "LoopViewController.h"
#import "NetworkManager.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"

@interface HomeViewController : UIViewController <AwesomeMenuDelegate, UIImagePickerControllerDelegate, LoopDelegate>

@property (nonatomic, strong) IBOutlet UIView *profileView;
@property (nonatomic, strong) IBOutlet UIImageView *profileImageView;
@property (nonatomic, strong) IBOutlet UILabel *nameLabel;
@property (nonatomic, strong) IBOutlet UILabel *displayNameLabel;

@property (nonatomic, strong) IBOutlet UIView *pathMenuView;
@property (nonatomic, strong) NSDateFormatter *dateFormatter;

-(IBAction)showSettings:(id)sender;

@end
