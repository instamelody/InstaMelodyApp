//
//  StationCell.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/25/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface StationCell : UICollectionViewCell

@property IBOutlet UIButton* shareButton;
@property IBOutlet UIButton* joinButton;
@property IBOutlet UIButton* playButton;
@property IBOutlet UIButton* likeButton;
@property IBOutlet UIImageView* coverImage;

@property IBOutlet UILabel* nameLabel;
@property IBOutlet UILabel* dateLabel;

@end
