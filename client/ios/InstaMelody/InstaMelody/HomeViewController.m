//
//  HomeViewController.m
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import "HomeViewController.h"
#import "constants.h"
#import "LoopViewController.h"
#import "DataManager.h"
#import "DAAlertController.h"

@interface HomeViewController ()

@end

@implementation HomeViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    self.profileView.layer.cornerRadius = self.profileView.frame.size.height / 2;
    self.profileView.layer.masksToBounds = YES;
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *authToken = [defaults objectForKey:@"authToken"];
    //NSString *deviceToken = [defaults objectForKey:@"deviceToken"];
    
    self.dateFormatter = [[NSDateFormatter alloc] init];
    
    if ( authToken ==  nil || [authToken isEqualToString:@""]) {
        
        [self signIn:nil];

    } else {
        //validate token
        
        //sd
        
    }
    
    //to make nav bar clear
    
    //self.navigationController.navigationBar.translucent = YES;
    [(UIView*)[self.navigationController.navigationBar.subviews objectAtIndex:0] setAlpha:0.2f];
    
    NSDictionary *navbarTitleTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                               [UIColor whiteColor],
                                               NSForegroundColorAttributeName,
                                               [UIFont fontWithName:@"Century Gothic" size:18.0],
                                               NSFontAttributeName,
                                               nil];
    
    
    NSDictionary *buttonTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                          [UIColor whiteColor],
                                          NSForegroundColorAttributeName,
                                          [UIFont fontWithName:@"FontAwesome" size:20.0],
                                          NSFontAttributeName,
                                          nil];
    
    /*
    NSDictionary *buttonTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                               [UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:1.0f],
                                               NSForegroundColorAttributeName,
                                               [UIFont fontWithName:@"FontAwesome" size:20.0],
                                               NSFontAttributeName,
                                               nil];
     */
    
    
    [self.navigationController.navigationBar setTitleTextAttributes:navbarTitleTextAttributes];
    //[self.navigationController.navigationBar setTintColor:[UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:1.0f]];
    [self.navigationController.navigationBar setTintColor:[UIColor whiteColor]];
    
    [[UIBarButtonItem appearance] setTitleTextAttributes:buttonTextAttributes forState:UIControlStateNormal];
    
    if ([defaults objectForKey:@"micVolume"] == nil) {
        [defaults setObject:[NSNumber numberWithFloat:0.5] forKey:@"micVolume"];
        [defaults setObject:[NSNumber numberWithFloat:0.5] forKey:@"melodyVolume"];
        [defaults synchronize];
    }
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:[NSString fontAwesomeIconStringForEnum:FAEllipsisH] style:UIBarButtonItemStylePlain target:self action:@selector(showOptions:)];
    
    [self fixButtons];
    
    [[NSNotificationCenter defaultCenter] addObserverForName:@"downloadedProfile" object:nil queue:[NSOperationQueue mainQueue] usingBlock:^(NSNotification * _Nonnull note) {
        [self loadProfileImage];
    }];
    
    [[NSNotificationCenter defaultCenter] addObserverForName:@"infoUpdated" object:nil queue:[NSOperationQueue mainQueue] usingBlock:^(NSNotification * _Nonnull note) {
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        if ([defaults objectForKey:@"authToken"] !=  nil) {
            self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"]];
            
            self.displayNameLabel.text = [NSString stringWithFormat:@"@%@", [defaults objectForKey:@"DisplayName"]];
            
        }
    }];
}

-(void)viewDidAppear:(BOOL)animated {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    if ([defaults objectForKey:@"authToken"] !=  nil) {
        self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"]];
        
        self.displayNameLabel.text = [NSString stringWithFormat:@"@%@", [defaults objectForKey:@"DisplayName"]];

        [self loadProfileImage];
    }
    
    NSString *authToken = [defaults objectForKey:@"authToken"];
    
    if ( authToken ==  nil || [authToken isEqualToString:@""]) {
        
    } else {
        [[DataManager sharedManager] fetchFriends];
        [[DataManager sharedManager] fetchMelodies];
        [[DataManager sharedManager] fetchUserMelodies];
    }
    
    if ([defaults objectForKey:@"ProfileFilePath"] != nil) {
        
        NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
        
        NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
        NSString *imageName = [[defaults objectForKey:@"ProfileFilePath"] lastPathComponent];
        
        NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
        self.profileImageView.image = [UIImage imageWithContentsOfFile:imagePath];
    }
    
    self.liveImageView.hidden = ![[DataManager sharedManager] isPremium];
}

-(void)loadProfileImage {
    
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

-(void)fixButtons {
    
    [self centerButton:self.stationButton];
    [self centerButton:self.feedButton];
    [self centerButton:self.messengerButton];
    [self centerButton:self.studioButton];
    [self centerButton:self.storeButton];
    [self centerButton:self.libraryButton];
    [self centerButton:self.showcaseButton];
    [self centerButton:self.friendsButton];
    [self centerButton:self.followingButton];
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

-(IBAction)showOptions:(id)sender
{
    
    DAAlertAction *cancelAction = [DAAlertAction actionWithTitle:@"Cancel" style:DAAlertActionStyleCancel handler:^{
        NSLog(@"\"Cancel\" button tapped");
    }];
    
    DAAlertAction *profileAction = [DAAlertAction actionWithTitle:@"Edit Profile" style:DAAlertActionStyleDefault handler:^{
        [self changeProfilePic:nil];
    }];
    
    DAAlertAction *logoutAction = [DAAlertAction actionWithTitle:@"Sign out" style:DAAlertActionStyleDefault handler:^{
                [self signOut:nil];
    }];
    
    NSArray *actions = @[profileAction, logoutAction, cancelAction];
    
    [DAAlertController showActionSheetInViewController:self fromBarButtonItem:sender withTitle:@"More options" message:@"" actions:actions permittedArrowDirections:UIPopoverArrowDirectionDown];
    
}

-(IBAction)showSettings:(id)sender {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
}

-(IBAction)showLoops:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    LoopViewController *loopVc = (LoopViewController *)[sb instantiateViewControllerWithIdentifier:@"LoopViewController"];
    loopVc.delegate = self;
    
    [self.navigationController pushViewController:loopVc animated:YES];
}

-(IBAction)signOut:(id)sender {
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    if ([defaults objectForKey:@"authToken"]== nil || [[defaults objectForKey:@"authToken"] isEqualToString:@""]) {
        [self signIn:nil];
    } else {
        
        [defaults setObject:@"" forKey:@"authToken"];
        [defaults synchronize];
    }
}

-(IBAction)signIn:(id)sender {
    UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
    UINavigationController *nc = [sb instantiateViewControllerWithIdentifier:@"SignInNavController"];
    [self presentViewController:nc animated:NO completion:nil];
}

-(IBAction)changeProfilePic:(id)sender {
    
    UIImagePickerController *imagePicker = [[UIImagePickerController alloc] init];
    imagePicker.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
    imagePicker.delegate = self;
    
    [self presentViewController:imagePicker animated:YES completion:^{
        NSLog(@"Image picker presented!");
    }];
}

#pragma mark - loop delegate

-(void)didFinishWithInfo:(NSDictionary *)userDict
{
    //sdfsdf
    [[NetworkManager sharedManager] uploadUserMelody:userDict];
}

#pragma mark - image picker delegate


-(void)imagePickerController:(UIImagePickerController *)picker
didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    UIImage *selectedImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    [self.profileImageView setImage:selectedImage];
    [picker dismissViewControllerAnimated:YES completion:^{
         NSLog(@"Image selected!");
     }];
    
    [self prepareImage:selectedImage];
}

-(void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    [picker dismissViewControllerAnimated:YES completion:^{
        NSLog(@"Picker cancelled without doing anything");
    }];
}


-(void)prepareImage:(UIImage *)image {

    UIImage *resizedImage = nil;
    CGSize originalImageSize = image.size;
    CGSize targetImageSize = CGSizeMake(150.0f, 150.0f);
    float scaleFactor, tempImageHeight, tempImageWidth;
    CGRect croppingRect;
    BOOL favorsX = NO;
    if (originalImageSize.width > originalImageSize.height) {
        scaleFactor = targetImageSize.height / originalImageSize.height;
        favorsX = YES;
    } else {
        scaleFactor = targetImageSize.width / originalImageSize.width;
        favorsX = NO;
    }
    
    tempImageHeight = originalImageSize.height * scaleFactor;
    tempImageWidth = originalImageSize.width * scaleFactor;
    if (favorsX) {
        float delta = (tempImageWidth - targetImageSize.width) / 2;
        croppingRect = CGRectMake(-1.0f * delta, 0, tempImageWidth, tempImageHeight);
    } else {
        float delta = (tempImageHeight - targetImageSize.height) / 2;
        croppingRect = CGRectMake(0, -1.0f * delta, tempImageWidth, tempImageHeight);
    }
    UIGraphicsBeginImageContext(targetImageSize);
    [image drawInRect:croppingRect];
    resizedImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    [[NetworkManager sharedManager] updateProfilePicture:resizedImage];
}


@end
