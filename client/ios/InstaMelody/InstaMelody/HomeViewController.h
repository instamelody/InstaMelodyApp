//
//  HomeViewController.h
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import <UIKit/UIKit.h>
#import "AwesomeMenu.h"

@interface HomeViewController : UIViewController <AwesomeMenuDelegate, UIImagePickerControllerDelegate>

@property (nonatomic, strong) IBOutlet UIView *profileView;
@property (nonatomic, strong) IBOutlet UIImageView *profileImageView;
@property (nonatomic, strong) IBOutlet UILabel *nameLabel;
@property (nonatomic, strong) IBOutlet UILabel *displayNameLabel;

@property (nonatomic, strong) IBOutlet UIView *pathMenuView;

-(IBAction)showSettings:(id)sender;

@end
