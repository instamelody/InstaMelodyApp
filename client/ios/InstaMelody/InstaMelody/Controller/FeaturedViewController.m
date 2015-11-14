//
//  FeaturedViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 10/23/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "FeaturedViewController.h"
#import "AdViewCell.h"

@interface FeaturedViewController ()

@end

@implementation FeaturedViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

#pragma mark - collection view

#pragma mark - UICollectionView Datasource
// 1
- (NSInteger)collectionView:(UICollectionView *)view numberOfItemsInSection:(NSInteger)section {
    return 3;
}

- (NSInteger)numberOfSectionsInCollectionView: (UICollectionView *)collectionView {
    return 1;
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)cv cellForItemAtIndexPath:(NSIndexPath *)indexPath {
    
    
    if (cv == self.featuredCollectionView) {
        UICollectionViewCell *cell = [cv dequeueReusableCellWithReuseIdentifier:@"FeaturedCell" forIndexPath:indexPath];
        return cell;
    }
    
    if (cv == self.currentCollectionView) {
        UICollectionViewCell *cell = [cv dequeueReusableCellWithReuseIdentifier:@"CurrentCell" forIndexPath:indexPath];
        return cell;
    }
    
    if (cv == self.adCollectionView) {
        AdViewCell *cell = (AdViewCell *)[cv dequeueReusableCellWithReuseIdentifier:@"AdViewCell" forIndexPath:indexPath];
        cell.imageView.image = [UIImage imageNamed:@"showcase1.png"];
        
        if (indexPath.row == 1) {
                cell.imageView.image = [UIImage imageNamed:@"showcase2.png"];
        } else if (indexPath.row == 2) {
                cell.imageView.image = [UIImage imageNamed:@"showcase3.png"];
        }
        
        return cell;
    }
    
    /*
    StationCell *cell = [cv dequeueReusableCellWithReuseIdentifier:@"StationCell" forIndexPath:indexPath];
    //cell.backgroundColor = [UIColor whiteColor];
    
    UserMelody *melody = (UserMelody *)[self.loopArray objectAtIndex:indexPath.row];
    
    cell.shareButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    cell.likeButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    cell.nameLabel.text = melody.userMelodyName;
    cell.dateLabel.text = [self.dateFormatter stringFromDate:melody.dateCreated];
    
    
    [cell.shareButton setTitle:[NSString fontAwesomeIconStringForEnum:FAEllipsisH] forState:UIControlStateNormal];
    [cell.likeButton setTitle:[NSString fontAwesomeIconStringForEnum:FAHeartO] forState:UIControlStateNormal];
    
    
    //load profile pic
    
    NSString *userId = melody.userId;
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *myUserId = [defaults objectForKey:@"Id"];
    
    if ([userId isEqualToString:myUserId]) {
        
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        cell.coverImage.image = [UIImage imageWithContentsOfFile:imagePath];
        
    } else {
        Friend *friend = [Friend MR_findFirstByAttribute:@"userId" withValue:userId];
     
        
        if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
            
            NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
            
            NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
            NSString *imageName = [friend.profileFilePath lastPathComponent];
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            cell.coverImage.image = [UIImage imageWithContentsOfFile:imagePath];
            
        } else {
            NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
            //[cell.coverImage setImageWithString:userName color:nil circular:YES];
        }
        
        
    }
     
     */
    
    return nil;
}

-(void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(nonnull NSIndexPath *)indexPath {
    
    /*
    
    UserMelody *melody = (UserMelody *)[self.loopArray objectAtIndex:indexPath.row];
    
    if (melody != nil)  {
        
        UIStoryboard *mainSB = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
        
        LoopViewController *loopVC = (LoopViewController *)[mainSB instantiateViewControllerWithIdentifier:@"LoopViewController"];
        loopVC.selectedUserMelody = melody;
        
        [self.navigationController pushViewController:loopVC animated:YES];
        
        
    }
     */
}

@end
