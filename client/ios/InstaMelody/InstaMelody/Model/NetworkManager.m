//
//  NetworkManager.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/14/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "NetworkManager.h"

@implementation NetworkManager

+ (id)sharedManager {
    static NetworkManager *sharedMyManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedMyManager = [[self alloc] init];
        sharedMyManager.dateFormatter =  [[NSDateFormatter alloc] init];
        
        NSLocale *enUSPOSIXLocale = [NSLocale localeWithLocaleIdentifier:@"en_US_POSIX"];
        
        [sharedMyManager.dateFormatter setLocale:enUSPOSIXLocale];
        
        [sharedMyManager.dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss"];
        [sharedMyManager.dateFormatter setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0]];
    });
    return sharedMyManager;
}


#pragma mark - network operations

-(void)uploadChatUserMelody:(NSDictionary *)userDict{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    time_t unixTime = time(NULL);
    
    NSNumber *isExplicit = [userDict objectForKey:@"IsExplicit"];
    NSNumber *isPublic = [userDict objectForKey:@"IsStationPostMelody"];
    
    NSString *recordingPath = [userDict objectForKey:@"LoopURL"];
    NSString *recordingName = [recordingPath lastPathComponent];
    //NSString *loopName = [userDict objectForKey:@"Name"];
    NSString *loopName = nil; 
    NSString *description = [userDict objectForKey:@"Description"];
    
    if (loopName == nil) {
        loopName = [NSString stringWithFormat:@"%d", (int)unixTime];
    }
    
    if (description == nil) {
        description = [NSString stringWithFormat:@"%d", (int)unixTime];
    }
    
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
    
    NSMutableArray *partArray = [NSMutableArray new];
    
    NSNumber *firstId = [userDict objectForKey:@"MelodyId1"];
    NSNumber *secondId = [userDict objectForKey:@"MelodyId2"];
    NSNumber *thirdId = [userDict objectForKey:@"MelodyId3"];
    
    if (firstId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:firstId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (secondId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:secondId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (thirdId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:thirdId forKey:@"Id"];
        [partArray addObject:entry];
        
    }
    
    NSMutableDictionary *recordingDict = [NSMutableDictionary dictionary];
    
    [recordingDict setObject:loopName forKey:@"Name"];
    [recordingDict setObject:description forKey:@"Description"];
    [recordingDict setObject:recordingName forKey:@"FileName"];
    [partArray addObject:recordingDict];
    
    //NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"UserMelody": @{@"Parts" : partArray}}];
    
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Chat": @{@"Id" : [userDict objectForKey:@"Id"]}, @"Message": @{@"Description" : @"User Melody Message", @"UserMelody": @{@"Parts" : partArray,  @"IsExplicit" : isExplicit} }}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat/Message", API_BASE_URL];
    
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
        
        if ([isPublic boolValue] == TRUE) {
            NSDictionary *melodyDict = [responseDict objectForKey:@"UserMelody"];
            [self makeLoopPublic:[responseDict objectForKey:@"Id"]];
        }
        
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


-(void)uploadUserMelody:(NSDictionary *)userDict{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    time_t unixTime = time(NULL);
    
    
    NSString *recordingPath = [userDict objectForKey:@"LoopURL"];
    NSString *recordingName = [recordingPath lastPathComponent];
    NSString *melodyName = [userDict objectForKey:@"Name"];
    NSString *description = [userDict objectForKey:@"Description"];
    
    //step 1 - get file token
    NSString *token =  [defaults objectForKey:@"authToken"];
    NSString *userName =  [defaults objectForKey:@"DisplayName"];
    
    NSMutableArray *partArray = [NSMutableArray new];
    
    NSNumber *firstId = [userDict objectForKey:@"MelodyId1"];
    NSNumber *secondId = [userDict objectForKey:@"MelodyId2"];
    NSNumber *thirdId = [userDict objectForKey:@"MelodyId3"];
    
    NSNumber *isExplicit = [userDict objectForKey:@"IsExplicit"];
    NSNumber *isPublic = [userDict objectForKey:@"IsStationPostMelody"];
    
    if (firstId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:firstId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (secondId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:secondId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    
    if (thirdId) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:thirdId forKey:@"Id"];
        [partArray addObject:entry];

    }
    
    NSMutableDictionary *recordingDict = [NSMutableDictionary dictionary];
    
    if (melodyName == nil) {
        melodyName = [NSString stringWithFormat:@"%@'s Loop", userName];
    }
    
    if (description == nil) {
        description = [NSString stringWithFormat:@"%@'s Loop", userName];
    }
    
    
    [recordingDict setObject:melodyName forKey:@"Name"];
    [recordingDict setObject:description forKey:@"Description"];
    [recordingDict setObject:recordingName forKey:@"FileName"];
    [partArray addObject:recordingDict];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"UserMelody": @{@"Parts" : partArray, @"IsExplicit" : isExplicit}}];
    
    
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
        if ([isPublic boolValue] == TRUE) {
            NSDictionary *melodyDict = [responseDict objectForKey:@"UserMelody"];
            [self makeLoopPublic:[melodyDict objectForKey:@"Id"]];
        }
        
        
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

-(void)makeLoopPublic:(NSString *)loopId {
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSString *stationId = [[[NSUserDefaults standardUserDefaults] objectForKey:@"stationId"] stringValue];
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Station": @{@"Id" : stationId}, @"Message": @{@"Description" : @"station post", @"UserMelody" : @{@"Id": loopId}}}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Station/Post", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        NSLog(@"----- loop is now public");
        
        
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
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
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
        
        //[[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Path"] forKey:@"ProfileFilePath"];
        [[NSNotificationCenter defaultCenter] postNotificationName:@"uploadDone" object:nil];
        
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

@end
