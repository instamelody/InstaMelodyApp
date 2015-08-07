//
//  ChatCell.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/7/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ChatCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UIImageView *profileImageView;
@property (nonatomic, strong) IBOutlet UILabel *nameLabel;
@property (nonatomic, strong) IBOutlet UILabel *descriptionLabel;
@property (nonatomic, strong) IBOutlet UILabel *timeLabel;

@end
