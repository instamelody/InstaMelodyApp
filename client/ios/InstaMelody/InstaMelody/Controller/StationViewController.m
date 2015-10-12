//
//  StationViewController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/25/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "StationViewController.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"
#import "StationCell.h"

@interface StationViewController ()

@end

@implementation StationViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.

    self.profileView.layer.cornerRadius = self.profileView.frame.size.height / 2;
    self.profileView.layer.masksToBounds = YES;
    self.filterControl.layer.cornerRadius = 4;
    self.filterControl.layer.masksToBounds = YES;
    
    self.fanButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    self.vipButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    
    [self.fanButton setTitle:[NSString fontAwesomeIconStringForEnum:FAThumbsUp] forState:UIControlStateNormal];
    [self.vipButton setTitle:[NSString fontAwesomeIconStringForEnum:FAStar] forState:UIControlStateNormal];
    [self createMenu];
    
}

-(void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    
    
    if (self.selectedFriend == nil) {
        
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        if ([defaults objectForKey:@"authToken"] !=  nil) {
            self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"]];
            
            //self.displayNameLabel.text = [NSString stringWithFormat:@"@%@", [defaults objectForKey:@"DisplayName"]];
            
        }
        
        if ([defaults objectForKey:@"ProfileFilePath"] != nil) {
            
            NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
            
            NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
            NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
        }
        
        
    } else {

        self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", self.selectedFriend.firstName, self.selectedFriend.lastName];
        
        self.title = [NSString stringWithFormat:@"%@'s Station", self.selectedFriend.displayName];
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [self.selectedFriend.profileFilePath lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
    }
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:[NSString fontAwesomeIconStringForEnum:FACog] style:UIBarButtonItemStylePlain target:self action:@selector(showVolume:)];
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - actions

-(IBAction)showVolume:(id)sender
{
    
}

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
    StationCell *cell = [cv dequeueReusableCellWithReuseIdentifier:@"StationCell" forIndexPath:indexPath];
    //cell.backgroundColor = [UIColor whiteColor];
    
    cell.shareButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    cell.likeButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    
    [cell.shareButton setTitle:[NSString fontAwesomeIconStringForEnum:FAShareSquareO] forState:UIControlStateNormal];
    [cell.likeButton setTitle:[NSString fontAwesomeIconStringForEnum:FAHeartO] forState:UIControlStateNormal];
    return cell;
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

- (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void)createMenu {
    UIImage *micImage = [UIImage imageNamed:@"mic"];
    
    UIImage *soloImage = [UIImage imageNamed:@"solo"];
    UIImage *chatImage = [UIImage imageNamed:@"chat"];
    UIImage *loopImage = [UIImage imageNamed:@"loop"];
    
    micImage = [self imageWithImage:micImage scaledToSize:CGSizeMake(70, 70)];
    soloImage = [self imageWithImage:soloImage scaledToSize:CGSizeMake(70, 70)];
    chatImage = [self imageWithImage:chatImage scaledToSize:CGSizeMake(70, 70)];
    loopImage = [self imageWithImage:loopImage scaledToSize:CGSizeMake(70, 70)];
    
    // Default Menu
    
    AwesomeMenuItem *starMenuItem1 = [[AwesomeMenuItem alloc] initWithImage:soloImage
                                                           highlightedImage:soloImage
                                                               ContentImage:nil
                                                    highlightedContentImage:nil];
    AwesomeMenuItem *starMenuItem2 = [[AwesomeMenuItem alloc] initWithImage:chatImage
                                                           highlightedImage:chatImage
                                                               ContentImage:nil
                                                    highlightedContentImage:nil];
    AwesomeMenuItem *starMenuItem3 = [[AwesomeMenuItem alloc] initWithImage:loopImage
                                                           highlightedImage:loopImage
                                                               ContentImage:nil
                                                    highlightedContentImage:nil];
    
    NSArray *menuItems = [NSArray arrayWithObjects:starMenuItem1, starMenuItem2, starMenuItem3, nil];
    
    AwesomeMenuItem *startItem = [[AwesomeMenuItem alloc] initWithImage:micImage
                                                       highlightedImage:micImage
                                                           ContentImage:micImage
                                                highlightedContentImage:micImage];
    
    AwesomeMenu *menu = [[AwesomeMenu alloc] initWithFrame:self.view.bounds startItem:startItem menuItems:menuItems];
    menu.delegate = self;
    menu.startPoint = CGPointMake(self.view.frame.size.width - 50, self.view.frame.size.height - 50);
    menu.menuWholeAngle = -1 * M_PI / 2;
    
    [self.view addSubview:menu];
    
}


- (void)awesomeMenu:(AwesomeMenu *)menu didSelectIndex:(NSInteger)idx
{
    NSLog(@"Select the index : %ld",(long)idx);
    
    switch (idx) {
        case 1:
            [self showChats:nil];
            break;
        case 2:
            [self showLoops:nil];
            break;
        default:
            break;
    }
    
}
- (void)awesomeMenuDidFinishAnimationClose:(AwesomeMenu *)menu {
    NSLog(@"Menu was closed!");
}
- (void)awesomeMenuDidFinishAnimationOpen:(AwesomeMenu *)menu {
    NSLog(@"Menu is open!");
}

-(IBAction)showChats:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    UIViewController *vc = [sb instantiateViewControllerWithIdentifier:@"ChatsTableViewController"];
    [self.navigationController pushViewController:vc animated:YES];
}

-(IBAction)showLoops:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    LoopViewController *loopVc = (LoopViewController *)[sb instantiateViewControllerWithIdentifier:@"LoopViewController"];
    loopVc.delegate = self;
    
    [self.navigationController pushViewController:loopVc animated:YES];
}


@end
