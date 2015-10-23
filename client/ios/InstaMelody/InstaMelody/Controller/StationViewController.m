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

@property NSArray *loopArray;

@end

@implementation StationViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.

    self.profileImageView.layer.cornerRadius = self.profileImageView.frame.size.height / 2;
    self.profileImageView.layer.masksToBounds = YES;
    self.filterControl.layer.cornerRadius = 4;
    self.filterControl.layer.masksToBounds = YES;
    
    self.fanButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    self.vipButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    
    [self.fanButton setTitle:[NSString fontAwesomeIconStringForEnum:FAThumbsUp] forState:UIControlStateNormal];
    [self.vipButton setTitle:[NSString fontAwesomeIconStringForEnum:FAStar] forState:UIControlStateNormal];
    
    self.liveImageView.hidden = ![[DataManager sharedManager] isPremium];
    
    [self getFirstStation];
}

-(void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    
    
    if (self.selectedFriend == nil) {
        
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        if ([defaults objectForKey:@"authToken"] !=  nil) {
            self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"]];
            
            self.stationLabel.text = @"My Station";
            
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
        self.stationLabel.text = [NSString stringWithFormat:@"%@'s Station", self.selectedFriend.displayName];
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [self.selectedFriend.profileFilePath lastPathComponent];
        
        if (imageName != nil) {
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
        }
    }
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:[NSString fontAwesomeIconStringForEnum:FACog] style:UIBarButtonItemStylePlain target:self action:@selector(showVolume:)];
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)getFirstStation {
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Station", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    NSString *userId = [[NSUserDefaults standardUserDefaults] objectForKey:@"Id"];
    
    if (self.selectedFriend != nil) {
        userId = self.selectedFriend.userId;
    }
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token, @"userId": userId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        // NSLog(@"JSON: %@", responseObject);
        NSLog(@"stations updated");
        
        NSArray *stationList = (NSArray *)responseObject;
        
        if (stationList.count > 0) {
            
            NSDictionary *selectedStation = stationList[0];
            
            //self.loopArray = stationList;
            //self.stationLabel.text = [selectedStation objectForKey:@"Name"];
            if (self.selectedFriend == nil) {
                //get all my loops
                [self fetchMyMelodies];
            } else {
                //get my friend's public loops
            }

        }
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching stations: %@", error);
        
        if (error.code == -1011 && self.selectedFriend == nil) {
            [self createStation];
        }
        
    }];
}

-(void)createStation {
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
    
    NSString *nameString = [NSString stringWithFormat:@"%@'s Station", [defaults objectForKey:@"FirstName"]];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Station": @{@"Name" : nameString}}];
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Station/New", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);
        
        //
        //step 2 - reload
        [self getFirstStation];
        
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            //TODOAHMED
            //[alertView show];
            
            [self fetchMyMelodies];
        }
    }];
}


-(void)fetchMyMelodies {
    
    
    self.loopArray = [[DataManager sharedManager] userMelodyList];
    
    [self.collectionView reloadData];
}

#pragma mark - actions

-(IBAction)showVolume:(id)sender
{
    
}

#pragma mark - collection view

#pragma mark - UICollectionView Datasource
// 1
- (NSInteger)collectionView:(UICollectionView *)view numberOfItemsInSection:(NSInteger)section {
    return [self.loopArray count];
}

- (NSInteger)numberOfSectionsInCollectionView: (UICollectionView *)collectionView {
    return 1;
}

- (UICollectionViewCell *)collectionView:(UICollectionView *)cv cellForItemAtIndexPath:(NSIndexPath *)indexPath {
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
    
    /*
    UIColor *darkestColor = [UIColor colorWithWhite:0.1f alpha:0.5f];
    UIColor *lightestColor = [UIColor colorWithWhite:0.7f alpha:0.0f];
    
    CAGradientLayer *headerGradient = [CAGradientLayer layer];
    headerGradient.colors = [NSArray arrayWithObjects:(id)darkestColor.CGColor, (id)lightestColor.CGColor, (id)darkestColor.CGColor,  nil];
    
    headerGradient.frame = cell.coverImage.bounds;
    [cell.coverImage.layer insertSublayer:headerGradient atIndex:0];
     */
    
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
        
        /*
        NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
        cell.nameLabel.text = friend.firstName;
         */
        
        if (friend.profileFilePath != nil && ![friend.profileFilePath isEqualToString:@""]) {
            
            NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
            
            NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
            NSString *imageName = [friend.profileFilePath lastPathComponent];
            
            NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
            cell.coverImage.image = [UIImage imageWithContentsOfFile:imagePath];
            
        } else {
            NSString *userName = [NSString stringWithFormat:@"%@ %@", friend.firstName, friend.lastName];
            [cell.coverImage setImageWithString:userName color:nil circular:YES];
        }
        
        
    }

    
    
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

-(IBAction)showLoops:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    LoopViewController *loopVc = (LoopViewController *)[sb instantiateViewControllerWithIdentifier:@"LoopViewController"];
    loopVc.delegate = self;
    
    [self.navigationController pushViewController:loopVc animated:YES];
}


@end
