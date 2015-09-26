//
//  StationViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/25/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface StationViewController : UIViewController <UICollectionViewDataSource, UICollectionViewDelegate>

@property IBOutlet UIView *profileView;
@property IBOutlet UIImageView *profileImageView;
@property IBOutlet UISegmentedControl *filterControl;
@property IBOutlet UICollectionView *collectionView;
@property IBOutlet UILabel *nameLabel;

@property IBOutlet UIBarButtonItem *settingsButton;
@property IBOutlet UIButton *vipButton;
@property IBOutlet UIButton *fanButton;



@end
