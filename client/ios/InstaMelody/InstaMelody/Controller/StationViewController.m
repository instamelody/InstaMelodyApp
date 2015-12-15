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
@property NSArray *cleanLoopArray;
@property NSDateFormatter *fromDateFormatter;
@property NSDateFormatter *toDateFormatter;

@end

@implementation StationViewController
{
    NSIndexPath * selectedIndexPath;
    BOOL isUsersStation;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.

    self.profileImageView.layer.cornerRadius = self.profileImageView.frame.size.height / 2;
    self.profileImageView.layer.masksToBounds = YES;
    self.filterControl.layer.cornerRadius = 4;
    self.filterControl.layer.masksToBounds = YES;
    
    //self.fanButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    //self.vipButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    

    //[self.fanButton setTitle:[NSString fontAwesomeIconStringForEnum:FAThumbsUp] forState:UIControlStateNormal];
    //[self.vipButton setTitle:[NSString fontAwesomeIconStringForEnum:FAStar] forState:UIControlStateNormal];
    
    //self.liveImageView.hidden = ![[DataManager sharedManager] isPremium];
    
    self.fromDateFormatter = [[NSDateFormatter alloc] init];
    
    NSLocale *enUSPOSIXLocale = [NSLocale localeWithLocaleIdentifier:@"en_US_POSIX"];
    
    [self.fromDateFormatter setLocale:enUSPOSIXLocale];
    
    [self.fromDateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.SSS"];
    [self.fromDateFormatter setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0]];
    
    self.toDateFormatter = [[NSDateFormatter alloc] init];
    [self.toDateFormatter setDateStyle:NSDateFormatterShortStyle];
    [self.toDateFormatter setTimeStyle:NSDateFormatterShortStyle];
    
    [self getFirstStation];
    
    self.stationLabel.text = @"My Station";
    [self.fanButton setTitle:@"0 Fans" forState:UIControlStateNormal];
    [self.fanButton setTitle:@"0 VIPs" forState:UIControlStateNormal];
    [self fixButtons];
}

-(void)fixButtons {
    
    [self centerButton:self.fanButton];
    [self centerButton:self.vipButton];
}

-(void)centerButton:(UIButton *)button {
    // the space between the image and text
    CGFloat spacing = 6.0;
    
    // lower the text and push it left so it appears centered
    //  below the image
    CGSize imageSize = button.imageView.image.size;
    button.titleEdgeInsets = UIEdgeInsetsMake(
                                              0.0, - imageSize.width, - (imageSize.height + spacing), 0.0);
    
    // raise the image and push it right so it appears centered
    //  above the text
    CGSize titleSize = [button.titleLabel.text sizeWithAttributes:@{NSFontAttributeName: button.titleLabel.font}];
    button.imageEdgeInsets = UIEdgeInsetsMake(
                                              - (titleSize.height + spacing), 0.0, 0.0, - titleSize.width);
}

-(void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    
    
    if (self.selectedFriend == nil) {
        
        isUsersStation = TRUE;
        
        if (self.stationDict != nil) {
            
            
            //self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", self.selectedFriend.firstName, self.selectedFriend.lastName];
            
            self.title = [self.stationDict objectForKey:@"Name"];
            self.stationLabel.text = [self.stationDict objectForKey:@"Name"];
            
            //get station
            //[self getFirstStation];
            
        } else {
            NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
            if ([defaults objectForKey:@"authToken"] !=  nil && ![[defaults objectForKey:@"authToken"] isEqualToString:@""] ) {
                self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"]];
                
            }
            
            if ([defaults objectForKey:@"ProfileFilePath"] != nil && ![[defaults objectForKey:@"ProfileFilePath"] isEqualToString:@""]) {
                
                NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
                
                NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
                NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
                
                NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
                self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
            }
            
        }
        
    } else {

        isUsersStation = FALSE;
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
    
    //self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:[NSString fontAwesomeIconStringForEnum:FACog] style:UIBarButtonItemStylePlain target:self action:@selector(showVolume:)];
    
}

-(void)like:(id)sender {
    NSLog(@"hello");
    
    UIButton *button = (UIButton *)sender;
    
    NSDictionary *loopDict = self.loopArray[button.tag];
    
    //NSDictionary *messageDict = [loopDict objectForKey:@"Message"];
    //NSString *messageId = [messageDict objectForKey:@"Id"];
    NSString *messageId = [loopDict objectForKey:@"Id"];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"StationMessage": @{@"Id" : messageId}}];
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Station/Post/Like", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);
        
        //
        //step 2 - reload
        [button setTitleColor:INSTA_BLUE forState:UIControlStateNormal];
        [button setEnabled:false];
        
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            //TODOAHMED
            [alertView show];
            
            //[self fetchMyLoops];
            //[self fetchMyMelodies];
        }
    }];

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
    
    if (self.stationDict != nil) {
        userId = [self.stationDict objectForKey:@"UserId"];
    }
    
    [self getUserDetails:userId];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token, @"userId": userId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        // NSLog(@"JSON: %@", responseObject);
        NSLog(@"stations updated");
        
        NSArray *stationList = (NSArray *)responseObject;
        
        if (stationList.count > 0) {
            
            NSDictionary *selectedStation = stationList[0];
            
            [[NSUserDefaults standardUserDefaults] setObject:[selectedStation objectForKey:@"Id"] forKey:@"stationId"];
            
            NSInteger numFans = [[selectedStation objectForKey:@"Likes"] integerValue];
            NSInteger numVips = 0;
            id followers = [selectedStation objectForKey:@"Followers"];
            if ([followers isKindOfClass:[NSArray class]]) {
                //sd
                NSArray *followerArray = (NSArray *)followers;
                numVips = followerArray.count;
            }
            
            //self.loopArray = stationList;
            dispatch_async(dispatch_get_main_queue(), ^(void){
                //Run UI Updates
                self.stationLabel.text = [selectedStation objectForKey:@"Name"];
                
                [self.fanButton setTitle:[NSString stringWithFormat:@"%ld Fans", numFans] forState:UIControlStateNormal];
                [self.vipButton setTitle:[NSString stringWithFormat:@"%ld VIPs", numVips] forState:UIControlStateNormal];
                [self fixButtons];
                
            });
            
            if (self.stationDict == nil && self.selectedFriend == nil) {
                //get all my loops
                isUsersStation = TRUE;
                [self fetchMyLoops:nil];
                
                //[self getStationPosts:[selectedStation objectForKey:@"Id"] ];
            } else {
                //get my friend's public loops
                isUsersStation = FALSE;
                [self fetchMyLoops:userId];

                //[self getStationPosts:[selectedStation objectForKey:@"Id"] ];
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
/*
-(void)getStationPosts:(NSString *)stationId {
 
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Station/Posts", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    NSString *userId = [[NSUserDefaults standardUserDefaults] objectForKey:@"Id"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token, @"id": stationId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        // NSLog(@"JSON: %@", responseObject);
        NSLog(@"stations updated");
        
        
        //NSArray *tempArray = (NSArray *)responseObject;
        NSMutableArray * tempArray0 = [[NSMutableArray alloc] init];
        
        for (NSDictionary * thisLoop in responseObject) {
            if (![[thisLoop objectForKey:@"Parts"] isEqual:[NSNull null]])
            {
                [tempArray0 addObject:thisLoop];
            }
        }
        
        NSArray * tempArray = [[NSArray alloc] initWithArray:tempArray0];

        
        //NSArray *tempArray = [[DataManager sharedManager] userMelodyList];
        
        if (tempArray.count > 20) {
            tempArray = [tempArray subarrayWithRange:NSMakeRange(0, 20)];
        }
        
        NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
        
        if (tempArray.count > 0) {
            NSString *userId = [tempArray[0] objectForKey:@"UserId"];
            
            NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
            
            NSString *myUserId = [defaults objectForKey:@"Id"];
            
            if ([userId isEqualToString:myUserId]) {
                
                valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateModified" ascending:NO];
            }
            
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            
            if ([[DataManager sharedManager] isMature]) {
                
                self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
                self.cleanLoopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
                
            } else {
                
                self.loopArray = [self filteredArray:[tempArray sortedArrayUsingDescriptors:descriptors] ];
                self.cleanLoopArray = [self filteredArray:[tempArray sortedArrayUsingDescriptors:descriptors]];
                
            }
            

        }
        [self.collectionView reloadData];
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching stations: %@", error);
        
        if (error.code == -1011 && self.selectedFriend == nil) {
            [self createStation];
        }
        
    }];
    
}
*/
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
            isUsersStation = TRUE;
            [self fetchMyLoops:nil];
            //[self fetchMyMelodies];
        }
    }];
}


-(NSArray *)filteredArray:(NSArray *)inputArray {
    NSMutableArray *tempArray = [NSMutableArray new];
    if (inputArray != nil) {
        for (NSDictionary *itemDict in inputArray) {
            if ([[itemDict objectForKey:@"IsExplicit"] boolValue] == false) {
                [tempArray addObject:itemDict];
            }
        }

    }
    return (NSArray *)tempArray;
}


-(void)fetchMyLoops:(NSString *)forUserID {
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSDictionary *parameters;
    
    if (isUsersStation)
    {
        
        parameters = @{@"token": token};
        
    } else {
        
        parameters = @{@"userId": forUserID, @"token": token};
        
    }
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        // NSLog(@"JSON: %@", responseObject);
        NSLog(@"loop updated");
        
        //NSArray *tempArray = (NSArray *)responseObject;
        NSMutableArray * tempArray0 = [[NSMutableArray alloc] init];
        
        for (NSDictionary * thisLoop in responseObject) {
            if (![[thisLoop objectForKey:@"Parts"] isEqual:[NSNull null]])
            {
                if (isUsersStation || ![[thisLoop objectForKey:@"IsExplicit"] boolValue])
                {
                    [tempArray0 addObject:thisLoop];
                }
            }
        }
        
        NSArray * tempArray = [[NSArray alloc] initWithArray:tempArray0];
        
        //NSArray *tempArray = [[DataManager sharedManager] userMelodyList];
        
        if (tempArray.count > 20) {
            tempArray = [tempArray subarrayWithRange:NSMakeRange(0, 20)];
        }
        
        for (NSDictionary * thisDict in tempArray) {
            NSLog(@"Loop %@ isExplicit=%@, isChat=%@", [thisDict valueForKey:@"Name"], [thisDict valueForKey:@"IsExplicit"], [thisDict valueForKey:@"IsChatLoop"]);
        }
        
        NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
        
        NSString *userId = [self.loopArray[0] objectForKey:@"UserId"];
        
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        
        NSString *myUserId = [defaults objectForKey:@"Id"];
        
        if ([userId isEqualToString:myUserId]) {
            
            valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateModified" ascending:NO];
        }
        
        NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
        self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
        
        self.cleanLoopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
        
        [self.collectionView reloadData];
        
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching loops: %@", error);
        
    }];
}

    -(void)fetchLoopsContd
    {
        
    }

-(void)fetchMyMelodies {
    
    
    NSArray *tempArray = [[DataManager sharedManager] userMelodyList];
    
    if (self.loopArray.count > 20) {
        self.loopArray = [self.loopArray subarrayWithRange:NSMakeRange(0, 20)];
    }
    
    NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateModified" ascending:NO];
    NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
    self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
    
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
    
    /*
     UIColor *darkestColor = [UIColor colorWithWhite:0.1f alpha:0.5f];
     UIColor *lightestColor = [UIColor colorWithWhite:0.7f alpha:0.0f];
     
     CAGradientLayer *headerGradient = [CAGradientLayer layer];
     headerGradient.colors = [NSArray arrayWithObjects:(id)darkestColor.CGColor, (id)lightestColor.CGColor, (id)darkestColor.CGColor,  nil];
     
     headerGradient.frame = cell.coverImage.bounds;
     [cell.coverImage.layer insertSublayer:headerGradient atIndex:0];
     */
    //cell.shareButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:20.0f];
    cell.likeButton.titleLabel.font  = [UIFont fontAwesomeFontOfSize:30.0f];
    
    NSDictionary *loopDict = self.loopArray[indexPath.row];
    
    NSDictionary *messageDict = [loopDict objectForKey:@"Message"];
    
    if (messageDict != nil) {
        NSDictionary *userMelodyDict = [messageDict objectForKey:@"UserMelody"];
        
        NSDictionary *userLoopDict = [messageDict objectForKey:@"UserLoop"];
        
        if (userLoopDict != nil && [userLoopDict isKindOfClass:[NSDictionary class]]) {
            
            cell.nameLabel.text = [userLoopDict objectForKey:@"Name"];
            
            NSString *oldDateString = [userLoopDict objectForKey:@"DateModified"];
            
            if (oldDateString == nil) {
                oldDateString = [userLoopDict objectForKey:@"DateCreated"];
            }
            
            NSDate *dateObject = [self.fromDateFormatter dateFromString:oldDateString];
            
            cell.likeButton.tag = indexPath.row;
            
            cell.dateLabel.text = [self.toDateFormatter stringFromDate:dateObject];
            
            
        } else if (userMelodyDict != nil && [userMelodyDict isKindOfClass:[NSDictionary class]]) {
            
            cell.nameLabel.text = [userMelodyDict objectForKey:@"Name"];
            
            NSString *oldDateString = [userMelodyDict objectForKey:@"DateModified"];
            
            if (oldDateString == nil) {
                oldDateString = [userMelodyDict objectForKey:@"DateCreated"];
            }
            
            NSDate *dateObject = [self.fromDateFormatter dateFromString:oldDateString];

            cell.likeButton.tag = indexPath.row;
            
            cell.dateLabel.text = [self.toDateFormatter stringFromDate:dateObject];
            
        }
    } else {
        
        cell.nameLabel.text = [loopDict objectForKey:@"Name"];
        
        NSString *oldDateString = [loopDict objectForKey:@"DateModified"];
        NSDate *dateObject = [self.fromDateFormatter dateFromString:oldDateString];
        
        cell.likeButton.tag  = indexPath.row;
        
        cell.dateLabel.text = [self.toDateFormatter stringFromDate:dateObject];

        
    }
    
    
    //[cell.shareButton setTitle:[NSString fontAwesomeIconStringForEnum:FAshareAltSquare] forState:UIControlStateNormal];
    [cell.likeButton setTitle:[NSString fontAwesomeIconStringForEnum:FAHeartO] forState:UIControlStateNormal];
    [cell.likeButton addTarget:self action:@selector(like:) forControlEvents:UIControlEventTouchUpInside];
    
    [cell.shareButton addTarget:self action:@selector(share:) forControlEvents:UIControlEventTouchUpInside];
    
    //load profile pic
    

    NSString *userId = [loopDict objectForKey:@"UserId"];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *myUserId = [defaults objectForKey:@"Id"];
    
    if ([userId isEqualToString:myUserId]) {

        //if my station + public
        
        if ([loopDict objectForKey:@"Parts"] != nil && [[loopDict objectForKey:@"Parts"] isKindOfClass:[NSArray class]]) {
            //NSArray *parts = [loopDict objectForKey:@"Parts"];
            //NSDictionary *firstPart = parts[0];
            //NSDictionary *melodyDict = [firstPart objectForKey:@"UserMelody"];
        
             if ([[loopDict objectForKey:@"IsExplicit"] boolValue] == NO) {
                 [cell.joinButton setTitle:@"Public" forState:UIControlStateNormal];
             } else {
                 [cell.joinButton setTitle:@"Explicit" forState:UIControlStateNormal];
             }
            
            
        }
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        cell.coverImage.image = [UIImage imageWithContentsOfFile:imagePath];
        
    } else {
        
        cell.joinButton.hidden = TRUE;
        
        if (self.selectedFriend != nil) {
            userId = self.selectedFriend.userId;
        }
        
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
            //[cell.coverImage setImageWithString:userName color:nil circular:YES];
        }
        
        
    }

    
    
    return cell;
}

-(void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(nonnull NSIndexPath *)indexPath {
    
    selectedIndexPath = indexPath;
    
    NSDictionary *loopDict = [self.loopArray objectAtIndex:indexPath.row];
    
    UIStoryboard *mainSB = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    
    LoopViewController *loopVC = (LoopViewController *)[mainSB instantiateViewControllerWithIdentifier:@"LoopViewController"];
    //loopVC.selectedUserMelody = melody;
    
    NSString *userId = [loopDict objectForKey:@"UserId"];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *myUserId = [defaults objectForKey:@"Id"];
    
    
    NSDictionary *messageDict = [loopDict objectForKey:@"Message"];
    
    if (messageDict != nil) {
        
        
        NSDictionary *userMelodyDict = [messageDict objectForKey:@"UserMelody"];
        
        NSDictionary *userLoopDict = [messageDict objectForKey:@"UserLoop"];
        
        if (userLoopDict != nil && [userLoopDict isKindOfClass:[NSDictionary class]]) {
            
            NSArray *partArray = [userLoopDict objectForKey:@"Parts"];
            
            if (partArray!= nil && [partArray isKindOfClass:[NSArray class]]) {
                loopVC.selectedLoop = userLoopDict;
                loopVC.delegate = self;
            } else {
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Invalid loop" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
                loopVC = nil;
            }
            
            
        } else if (userMelodyDict != nil && [userMelodyDict isKindOfClass:[NSDictionary class]])  {
            UserMelody *tempMelody = [[DataManager sharedManager] createUserMelodyWithDict:userMelodyDict];
            
            //loopVC.selectedLoop = userMelodyDict;
            loopVC.selectedUserMelody = tempMelody;
            loopVC.delegate = self;
        }
        
        
    } else {
        
        NSArray *partArray = [loopDict objectForKey:@"Parts"];
        
        if (partArray!= nil && [partArray isKindOfClass:[NSArray class]]) {
            loopVC.selectedLoop = loopDict;
            loopVC.delegate = self;
        } else {
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Invalid loop" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            loopVC = nil;
        }
        
    }
    
    if (loopVC != nil) {
            [self.navigationController pushViewController:loopVC animated:YES];
    }
    
}

/* Method not called?
-(void)getLoop:(NSString *)loopId {
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token, @"id": loopId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        if ([responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *responseDict = (NSDictionary *)responseObject;
            
            
            UIStoryboard *mainSB = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
            
            LoopViewController *loopVC = (LoopViewController *)[mainSB instantiateViewControllerWithIdentifier:@"LoopViewController"];
            //loopVC.selectedUserMelody = melody;
            loopVC.loopDict = responseDict;
            loopVC.delegate = self;
            
            [self.navigationController pushViewController:loopVC animated:YES];
            
            
        }
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
}
*/
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    
    if ([segue.identifier isEqualToString:@"showStationLoop"]) {
        

        
    }
    
}

-(IBAction)showLoops:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    LoopViewController *loopVc = (LoopViewController *)[sb instantiateViewControllerWithIdentifier:@"LoopViewController"];
    loopVc.delegate = self;
    
    [self.navigationController pushViewController:loopVc animated:YES];
}

-(IBAction)change:(id)sender {
    UISegmentedControl *control = _filterControl; // (UISegmentedControl *)sender;
    
    self.loopArray = self.cleanLoopArray;
    
    NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateCreated" ascending:NO];
    
    NSString *userId = [self.loopArray[0] objectForKey:@"UserId"];
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *myUserId = [defaults objectForKey:@"Id"];
    
    if ([userId isEqualToString:myUserId]) {

        valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"DateModified" ascending:NO];
    }
    
    switch (control.selectedSegmentIndex) {
        case 0: {
            
            NSArray *tempArray = self.loopArray;
           
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
            
            break;
        }
        case 1: {
            
            NSMutableArray *tempArray = [NSMutableArray new];
            
            for (NSDictionary *itemDict in self.loopArray) {
                NSInteger explicit = [[itemDict valueForKey:@"IsExplicit"] integerValue];
                if (!explicit) {
                    [tempArray addObject:itemDict];
                }
            }
            
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
            
            
            break;
        }
        case 2: {
            NSMutableArray *tempArray = [NSMutableArray new];
            
            for (NSDictionary *itemDict in self.loopArray) {
                if ([[itemDict objectForKey:@"IsChatLoop"] boolValue] == TRUE
                    && ![[itemDict valueForKey:@"IsExplicit"] integerValue]) {
                    [tempArray addObject:itemDict];
                }
            }
            
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
            
            break;
        }
        case 3: {
            
            NSMutableArray *tempArray = [NSMutableArray new];
            for (NSDictionary *itemDict in self.loopArray) {
                NSInteger explicit = [[itemDict valueForKey:@"IsExplicit"] integerValue];
                if (explicit) {
                    [tempArray addObject:itemDict];
                }
            }
            
            NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
            self.loopArray = [tempArray sortedArrayUsingDescriptors:descriptors];
            
            break;
        }
            
        default:
            break;
    }
    
    [self.collectionView reloadData];
}


-(void)getUserDetails:(NSString*)userId {
    
    //https://api.instamelody.com/v1.0/User?token=9d0ab021-fcf8-4ec3-b6e3-bb1d0d03b12e&displayName=testeraccount
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User", API_BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    
    NSDictionary *parameters = @{@"token": token, @"id": userId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        
        NSDictionary *responseDict = (NSDictionary *)responseObject;
        
        
        dispatch_async(dispatch_get_main_queue(), ^(void){
            self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [responseDict objectForKey:@"FirstName"], [responseDict objectForKey:@"LastName"]];
            
        });

        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            //[alertView show];
        }
    }];
    
    requestUrl = [NSString stringWithFormat:@"%@/User/Friends", API_BASE_URL];
    
    parameters = @{@"id": userId};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"results %@", responseObject);
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
    
        NSLog(@"failed!");
    }];
    
}

#pragma mark - loop delegate

-(void)didFinishWithInfo:(NSDictionary *)userDict
{
    //sdfsdf
    /*
    if ([userDict objectForKey:@"Id"] != nil) {
        [[NetworkManager sharedManager] uploadChatUserMelody:userDict];
    } else if ([userDict objectForKey:@"LoopId"] != nil) {
        [[NetworkManager sharedManager] uploadLoopPart:userDict];
    } else {
         [[NetworkManager sharedManager] uploadUserMelody:userDict];
    } */
    
    //copying from HomeViewController which works
    
    if ([userDict objectForKey:@"Id"] != nil) {
        [[NetworkManager sharedManager] uploadChatUserMelody:userDict];
    } else {
        [[NetworkManager sharedManager] uploadLoop:userDict];
    }
    
}

-(void)setExplicit:(BOOL)isExplicit
{
    NSMutableArray *newLoopArray = [[NSMutableArray alloc] initWithArray:self.cleanLoopArray];
    NSMutableDictionary *loopDict = [[NSMutableDictionary alloc] initWithDictionary:[self.loopArray objectAtIndex:selectedIndexPath.row]];
    [loopDict setValue:[NSNumber numberWithBool:isExplicit] forKey:@"IsExplicit"];

    int index = 0;
    for (NSDictionary * element in newLoopArray) {
        if ([[element valueForKey:@"Id"] isEqualToString:[loopDict valueForKey:@"Id"]] )
        {
            break;
        } else {
            index = index + 1;
        }
    }
    [newLoopArray replaceObjectAtIndex:index withObject:loopDict];
    
    self.loopArray = newLoopArray;
    self.cleanLoopArray = newLoopArray;
    //[self.collectionView reloadData];
    [self change:nil];
    
}
@end
