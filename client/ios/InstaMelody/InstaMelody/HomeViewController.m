//
//  HomeViewController.m
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import "HomeViewController.h"
#import "constants.h"
#import "AFHTTPRequestOperationManager.h"
#import "LoopViewController.h"
#import "DataManager.h"

@interface HomeViewController ()

@end

@implementation HomeViewController

+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

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
    /*
    [self.navigationController.navigationBar setBackgroundImage:[UIImage new]
                                                  forBarMetrics:UIBarMetricsDefault];
    self.navigationController.navigationBar.shadowImage = [UIImage new];
    self.navigationController.navigationBar.translucent = YES;
    self.navigationController.view.backgroundColor = [UIColor clearColor];
    */
    
    [self createMenu];
}

-(void)viewDidAppear:(BOOL)animated {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
     if ([defaults objectForKey:@"authToken"] !=  nil) {
         self.nameLabel.text = [NSString stringWithFormat:@"%@ %@", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"]];
         
         self.displayNameLabel.text = [NSString stringWithFormat:@"@%@", [defaults objectForKey:@"DisplayName"]];

         //image path
         
         NSFileManager *fileManager = [NSFileManager defaultManager];
         
         NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
         
         NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
         
         NSString *remotePath =  [defaults objectForKey:@"ProfileFilePath"];
         
         NSString *fileName = [remotePath lastPathComponent];
         NSString *pathString = [profilePath stringByAppendingPathComponent:fileName];
         
         UIImage *remoteImage = [UIImage imageWithContentsOfFile:pathString];
         
         if ([fileManager fileExistsAtPath:pathString]) {
             
             self.profileImageView.image = remoteImage;
         }
         
     }
    
    NSString *authToken = [defaults objectForKey:@"authToken"];
    
    if ( authToken ==  nil || [authToken isEqualToString:@""]) {
        
    } else {
        [[DataManager sharedManager] fetchFriends];
        [[DataManager sharedManager] fetchMelodies];
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

-(IBAction)showSettings:(id)sender {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
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

-(void)createMenu {
    UIImage *micImage = [UIImage imageNamed:@"mic"];
    
    UIImage *soloImage = [UIImage imageNamed:@"solo"];
    UIImage *chatImage = [UIImage imageNamed:@"chat"];
    UIImage *loopImage = [UIImage imageNamed:@"loop"];
    
    micImage = [HomeViewController imageWithImage:micImage scaledToSize:CGSizeMake(75, 75)];
    soloImage = [HomeViewController imageWithImage:soloImage scaledToSize:CGSizeMake(50, 50)];
    chatImage = [HomeViewController imageWithImage:chatImage scaledToSize:CGSizeMake(50, 50)];
    loopImage = [HomeViewController imageWithImage:loopImage scaledToSize:CGSizeMake(50, 50)];
    
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
    menu.startPoint = CGPointMake(self.view.frame.size.width - 60, self.view.frame.size.height - 60);
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

#pragma mark - loop delegate

-(void)didFinishWithInfo:(NSDictionary *)userDict
{
    //sdfsdf
    [self uploadUserMelody:userDict];
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
    
    [self updateProfilePicture:resizedImage];
}

#pragma mark - network operations

-(void)uploadUserMelody:(NSDictionary *)userDict{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    time_t unixTime = time(NULL);
    
    
    NSString *recordingPath = [userDict objectForKey:@"LoopURL"];
    NSString *recordingName = [recordingPath lastPathComponent];
    
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
    
    NSMutableArray *partArray = [NSMutableArray new];
    
    NSNumber *firstId = [userDict objectForKey:@"MelodyId1"];
    NSNumber *secondId = [userDict objectForKey:@"MelodyId2"];
    
    if (firstId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:firstId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (secondId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:secondId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    NSMutableDictionary *recordingDict = [NSMutableDictionary dictionary];
    
    [recordingDict setObject:[NSString stringWithFormat:@"%d", (int)unixTime] forKey:@"Name"];
    [recordingDict setObject:[userDict objectForKey:@"Description"] forKey:@"Description"];
    [recordingDict setObject:recordingName forKey:@"FileName"];
    [partArray addObject:recordingDict];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"UserMelody": @{@"Parts" : partArray}}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/New", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    manager.requestSerializer = [AFJSONRequestSerializer serializer];
    manager.responseSerializer = [AFJSONResponseSerializer serializer];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);
        
        //
        //step 2 - upload file
        
        NSDictionary *responseDict = (NSDictionary *)responseObject;
        NSDictionary *tokenDict = [responseDict objectForKey:@"FileUploadToken"];
        NSString *fileTokenString = [tokenDict objectForKey:@"Token"];
        
        [self uploadFile:recordingPath withFileToken:fileTokenString];
        //[self uploadData:imageData withFileToken:fileTokenString andFileName:imageName];
        
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSString *errorString = [operation.responseObject objectForKey:@"Message"];
            NSLog(@"Error: %@", errorString);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:errorString delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
    
}


-(void)updateProfilePicture:(UIImage *)image{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    time_t unixTime = time(NULL);
    
    NSString *imageName = [NSString stringWithFormat:@"%@_%@_profile_%d.jpg", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"], (int)unixTime];
    
    //try to create folder
    NSFileManager *fileManager = [NSFileManager defaultManager];
    
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    
    NSString *profilePath = [documentsPath stringByAppendingPathComponent:@"Profiles"];
    
    if (![[NSFileManager defaultManager] fileExistsAtPath:profilePath]){
        
        NSError* error;
        if(  [[NSFileManager defaultManager] createDirectoryAtPath:profilePath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
        
    }
    
    //save to folder
    NSString *imagePath = [profilePath stringByAppendingPathComponent:imageName];
    NSData *imageData = UIImageJPEGRepresentation(image, 0.7);
    [imageData writeToFile:imagePath atomically:YES];
    
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
                         
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Image": @{@"FileName" : imageName}}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Update/Image", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);

        //
        //step 2 - upload file
        
        NSDictionary *responseDict = (NSDictionary *)responseObject;
        NSDictionary *tokenDict = [responseDict objectForKey:@"FileUploadToken"];
        NSString *fileTokenString = [tokenDict objectForKey:@"Token"];
        
        [self uploadFile:imagePath withFileToken:fileTokenString];
        //[self uploadData:imageData withFileToken:fileTokenString andFileName:imageName];
         
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSString *errorString = [operation.responseObject objectForKey:@"Message"];
            NSLog(@"Error: %@", errorString);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:errorString delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
    
}

-(void)uploadFile:(NSString *)filePath withFileToken:(NSString *)fileToken {
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *sessionToken =  [defaults objectForKey:@"authToken"];
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Upload/%@/%@", API_BASE_URL, sessionToken, fileToken];
    
    NSURL *fileURL = [NSURL fileURLWithPath:filePath];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:nil constructingBodyWithBlock:^(id<AFMultipartFormData> formData) {
        //[formData appendPartWithFormData:data name:[filePath last]];
        [formData appendPartWithFileURL:fileURL name:[filePath lastPathComponent] error:nil];
    } success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"Success: %@", responseObject);
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"File uploaded successfully" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
        NSArray *responseArray = (NSArray *)responseObject;
        NSDictionary *responseDict = responseArray[0];
        
        [[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Path"] forKey:@"ProfileFilePath"];
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSString *errorString = [operation.responseObject objectForKey:@"Message"];
            NSLog(@"Error: %@", errorString);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:errorString delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
}

@end
