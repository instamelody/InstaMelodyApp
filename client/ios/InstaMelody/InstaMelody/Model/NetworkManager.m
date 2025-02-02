//
//  NetworkManager.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/14/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import "NetworkManager.h"
#import "AppDelegate.h"
#import "AFURLSessionManager.h"

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
    
    NSString *isExplicit = [NSString stringWithFormat:@"%@", [userDict objectForKey:@"IsExplicit"]];
    NSString * isExpTF = ([isExplicit isEqualToString:@"1"]) ? @"true":@"false";
    //NSNumber *isPublic = [userDict objectForKey:@"IsStationPostMelody"];
    
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
    
    if (firstId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:firstId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (secondId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:secondId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (thirdId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:thirdId forKey:@"Id"];
        [partArray addObject:entry];
        
    }
    
    NSMutableDictionary *recordingDict = [NSMutableDictionary dictionary];
    
    [recordingDict setObject:loopName forKey:@"Name"];
    [recordingDict setObject:description forKey:@"Description"];
    [recordingDict setObject:recordingName forKey:@"FileName"];
    [partArray addObject:recordingDict];
    
    //NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"UserMelody": @{@"Parts" : partArray}}];
    
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Chat": @{@"Id" : [userDict objectForKey:@"Id"]}, @"Message": @{@"Description" : @"User Melody Message", @"UserMelody": @{@"Parts" : partArray,  @"IsExplicit" : isExpTF} }}];
    
    
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
        
        
        //Workaround for issue with /Message/Chat/Message not setting isExplicit flag correctly on new loop
        
        //NSString * returningExplicitValue = [NSString stringWithFormat:@"%@", [responseDict valueForKey:@"IsExplicit"]];
        if ([isExplicit isEqualToString:@"1"])
             {
                 NSString * newLoopID = [[responseDict objectForKey:@"Chat"] valueForKey:@"ChatLoopId"];
                 
                 NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop/Update", API_BASE_URL];
                 
                 NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
                 
                 //add 64 char string
                 
                 AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
                 
                 NSDictionary *loopInfo = @{@"Id": newLoopID, @"IsExplicit": @"true"};
                 NSDictionary *parameters = @{@"Token": token, @"Loop": loopInfo};
                 
                 [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
                     NSLog(@"Explicit status updated after creating new chat loop");
                     
                 } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
                     
                     NSLog(@"Problem updating explicit status after creating new chat loop");
                 }];

                 
             }

        //[self uploadData:imageData withFileToken:fileTokenString andFileName:imageName];
        /*
        if ([isPublic boolValue] == TRUE) {
            
            NSDictionary *melodyDict = [responseDict objectForKey:@"UserMelody"];
            NSDictionary *chatMsgDict = [responseDict objectForKey:@"ChatMessage"];
            
            
            if (melodyDict != nil) {
                
                //melody part
                
                NSDictionary *loopDict = [responseDict objectForKey:@"Loop"];
                [self makeLoopPublic:loopDict withDataLoad:YES];
                
                //[self makeLoopPublic:[responseDict objectForKey:@"Id"]];
            } else if (chatMsgDict != nil) {
                
                //chat loop part
                
                NSDictionary *messageDict = [chatMsgDict objectForKey:@"Message"];
                
                if (messageDict != nil) {
                    melodyDict = [messageDict objectForKey:@"UserMelody"];
                    //[self makeLoopPublic:[melodyDict objectForKey:@"Id"]];
                    [self makeLoopPublic:melodyDict withDataLoad:YES];
                }
            }
        }
        */
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            if ([[errorDict objectForKey:@"Message"] isEqualToString:@"Cannot send a message to a Chat that User is not a member of."])
            {
                //They are not a member of this chat, so we need to make a different API call...
                [self uploadLoopPart:userDict];
                
            } else {
            
                NSString *ErrorResponse = [NSString stringWithFormat:@"Error %td: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
                
                NSLog(@"%@",ErrorResponse);
                
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
            }
        }
    }];
    
    
}


-(void)uploadUserMelody:(NSDictionary *)userDict{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    //time_t unixTime = time(NULL);
    
    
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
    
    NSString *isExplicit = [NSString stringWithFormat:@"%@", [userDict objectForKey:@"IsExplicit"]];
    //NSNumber *isPublic = [userDict objectForKey:@"IsStationPostMelody"];
    
    if (firstId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:firstId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (secondId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:secondId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    
    if (thirdId != nil) {
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
        /*
        if ([isPublic boolValue] == TRUE) {
            / *
            NSDictionary *melodyDict = [responseDict objectForKey:@"UserMelody"];
            [self makeLoopPublic:[melodyDict objectForKey:@"Id"]];
             * /
            NSDictionary *loopDict = [responseDict objectForKey:@"Loop"];
            [self makeLoopPublic:loopDict withDataLoad:YES];
        }
        */
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %d: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];

}

-(void)uploadLoop:(NSDictionary *)userDict{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    //time_t unixTime = time(NULL);
    
    
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
    
    NSString *isExpl = [NSString stringWithFormat:@"%@", [userDict objectForKey:@"IsExplicit"]];
    NSString * isExpTF = ([isExpl isEqualToString:@"1"]) ? @"true":@"false";
    //NSNumber *isPublic = [userDict objectForKey:@"IsStationPostMelody"];
    
    if (firstId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:firstId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    if (secondId != nil) {
        NSDictionary *entry = [NSDictionary dictionaryWithObject:secondId forKey:@"Id"];
        [partArray addObject:entry];
    }
    
    
    if (thirdId != nil) {
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
    
    //NSDictionary *userMelodyDict = @{ @"UserMelody": @{@"Parts" : partArray, @"IsExplicit" : isExplicit}};
    NSDictionary *userMelodyDict = @{ @"UserMelody": @{@"Parts" : partArray, @"IsExplicit" : isExpTF}};
    
    NSDictionary *loopDict = @{@"Name": melodyName, @"IsExplicit": isExpTF, @"Parts": @[userMelodyDict]};
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Loop": loopDict}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    manager.requestSerializer = [AFJSONRequestSerializer serializer];
    manager.responseSerializer = [AFJSONResponseSerializer serializer];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);
        
        //
        //step 2 - upload file
        
        NSDictionary *responseDict = (NSDictionary *)responseObject;
        NSArray *tokenArray = [responseDict objectForKey:@"FileUploadTokens"];
        
        if (tokenArray !=nil && [tokenArray isKindOfClass:[NSArray class]] ) {
            
            NSDictionary *tokenDict = tokenArray[0];
            NSString *fileTokenString = [tokenDict objectForKey:@"Token"];
            
            [self uploadFile:recordingPath withFileToken:fileTokenString];
            //[self uploadData:imageData withFileToken:fileTokenString andFileName:imageName];
            /*
            if ([isPublic boolValue] == TRUE) {
                
                NSDictionary *loopDict = [responseDict objectForKey:@"Loop"];
                [self makeLoopPublic:loopDict withDataLoad:NO];
                
                
                / *
                 
                 NSDictionary *melodyDict = [responseDict objectForKey:@"UserMelody"];
                 
                 if (melodyDict == nil) {
                 NSDictionary *loopDict = [responseDict objectForKey:@"Loop"];
                 if (loopDict != nil) {
                 NSArray * parts = [loopDict objectForKey:@"Parts"];
                 melodyDict = [parts[0] objectForKey:@"UserMelody"];
                 }
                 }
                 [self makeLoopPublic:[melodyDict objectForKey:@"Id"]];
                 * /
            }
            */
            
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

-(void)uploadLoopPart:(NSDictionary *)userDict{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    //time_t unixTime = time(NULL);
    
    
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
    
    NSString *isExplicit = [NSString stringWithFormat:@"%@", [userDict objectForKey:@"IsExplicit"]];
    NSString *isStationPostMelody = [NSString stringWithFormat:@"%@",  [userDict objectForKey:@"IsStationPostMelody"]];
    
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
    
    NSDictionary *userMelodyDict = @{ @"UserMelody": @{@"Parts" : partArray, @"IsExplicit" : isExplicit}};
    
    //NSDictionary *loopDict = @{@"Id": [userDict objectForKey:@"LoopId"]};
    
    AppDelegate *appDelegate=(AppDelegate*)[[UIApplication sharedApplication] delegate];
    NSString * loopOwnersId = (appDelegate.loopOwnersId.length>0) ? appDelegate.loopOwnersId : [defaults objectForKey:@"Id"];
    
    NSDictionary *loopDict = @{@"UserId": loopOwnersId, @"Name": melodyName };
    
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Loop": loopDict , @"LoopPart": userMelodyDict}];
    
    
    NSString *requestUrl = [NSString stringWithFormat:@"%@/Melody/Loop/Attach", API_BASE_URL];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    manager.requestSerializer = [AFJSONRequestSerializer serializer];
    manager.responseSerializer = [AFJSONResponseSerializer serializer];
    
    [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        
        NSLog(@"JSON: %@", responseObject);
        
        //
        //step 2 - upload file
        
        NSDictionary *responseDict = (NSDictionary *)responseObject;
        NSArray *tokenArray = [responseDict objectForKey:@"FileUploadTokens"];
        
        if (tokenArray != nil && tokenArray.count > 0) {
            NSDictionary *tokenDict = tokenArray[0];
            NSString *fileTokenString = [tokenDict objectForKey:@"Token"];
            
            [self uploadFile:recordingPath withFileToken:fileTokenString];
            //[self uploadData:imageData withFileToken:fileTokenString andFileName:imageName];
            if ([isStationPostMelody isEqualToString:@"1"]) {
                
                //NSDictionary *loopDict = [responseDict objectForKey:@"Loop"];
                //[self makeLoopPublic:loopDict];
                
                /*
                NSDictionary *melodyDict = [responseDict objectForKey:@"UserMelody"];
                
                if (melodyDict == nil) {
                    NSDictionary *loopDict = [responseDict objectForKey:@"Loop"];
                    if (loopDict != nil) {
                        NSArray * parts = [loopDict objectForKey:@"Parts"];
                        melodyDict = [parts[0] objectForKey:@"UserMelody"];
                    }
                }
                [self makeLoopPublic:[melodyDict objectForKey:@"Id"]];
                 */
            }
        }
        
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %td: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
    }];
    
}

/*
-(void)makeLoopPublic:(NSString *)loopId {
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSString *stationId = [[[NSUserDefaults standardUserDefaults] objectForKey:@"stationId"] stringValue];
    
    if (stationId!=nil && ![stationId isEqualToString:@""]) {
        
        if (loopId!=nil && ![loopId isEqualToString:@""]) {
            
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
    }
    
}
 */
/*
-(void)makeLoopPublic:(NSDictionary *)loopDict withDataLoad:(BOOL)withLoad {
    //withLoad parameter determines whether this call creates a new loop (YES)
    //or just ties an existing loop into the public list (NO)
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    NSString *stationId = [[[NSUserDefaults standardUserDefaults] objectForKey:@"stationId"] stringValue];
    
    if (stationId!=nil && ![stationId isEqualToString:@""]) {
        
        //NSDictionary *messageDict = @{@"Description" : @"station post", @"UserMelody" : @{@"Id": loopId}}};
        
        NSMutableArray *partsArray = [NSMutableArray new];
        
        NSArray *inputParts = [loopDict objectForKey:@"Parts"];
        NSDictionary *partDict = inputParts[0];
        NSDictionary *userMelodyDict = [partDict objectForKey:@"UserMelody"];
        NSArray *melodyParts = [userMelodyDict objectForKey:@"Parts"];
        for (NSDictionary *partDict in melodyParts) {
            if ([[partDict objectForKey:@"IsUserCreated"] boolValue] == NO) {
                NSDictionary *part = @{@"Id": [partDict objectForKey:@"Id"]};
                [partsArray addObject:part];
            } else {
                NSDictionary *part = @{@"Name": [loopDict objectForKey:@"Name"], @"FileName": [partDict objectForKey:@"FileName"]};
                [partsArray addObject:part];
            }
        }
        
        NSDictionary *partsDict = @{@"Parts": partsArray};
        //NSDictionary *userLoopDict = @{@"Name": [loopDict objectForKey:@"Name"], @"Parts": @[@{@"UserMelody": partsDict}]};
        NSDictionary *userLoopDict = @{@"Name": [loopDict objectForKey:@"Name"], @"Parts": @{@"UserMelody": partsDict}};
        NSDictionary *messageDict = @{@"Description":[loopDict objectForKey:@"Name"], @"UserLoop": userLoopDict};
        NSMutableDictionary *parameters;
        
        if (withLoad)
        {
            parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Station": @{@"Id" : stationId}, @"Message": messageDict }];
        } else {
            parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Station": @{@"Id" : stationId}, @"Message": @{@"Description" : @"station post", @"UserMelody" : @{@"Id":[loopDict objectForKey:@"Id"]}}}];
        }
        
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
    
}
*/
-(void)updateProfilePicture:(UIImage *)image{
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    //save to file
    
    time_t unixTime = time(NULL);
    
    NSString *imageName = [NSString stringWithFormat:@"%@_%@_profile_%d.jpg", [defaults objectForKey:@"FirstName"], [defaults objectForKey:@"LastName"], (int)unixTime];
    
    //try to create folder
    //NSFileManager *fileManager = [NSFileManager defaultManager];
    
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
    BOOL result = [imageData writeToFile:imagePath atomically:YES];
    if (!result)
    {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"There was a problem saving the file on the phone." delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        return;
        
    } else {
        NSError *error = nil;
        NSURL *localURL = [NSURL fileURLWithPath:imagePath];
        BOOL success = [localURL setResourceValue:[NSNumber numberWithBool:YES] forKey:NSURLIsExcludedFromBackupKey error:&error];
        if(!success){
            NSLog(@"Error excluding %@ from backup %@", [localURL lastPathComponent], error);
        }
    }
    
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
        
        //UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"File uploaded successfully" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        //[alertView show];
        
        //NSArray *responseArray = (NSArray *)responseObject;
        //NSDictionary *responseDict = responseArray[0];
        
        //[[NSUserDefaults standardUserDefaults] setObject:[responseDict objectForKey:@"Path"] forKey:@"ProfileFilePath"];
        [[NSNotificationCenter defaultCenter] postNotificationName:@"uploadDone" object:nil];
        
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        
        if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
            NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
            
            NSString *ErrorResponse = [NSString stringWithFormat:@"Error %d: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
            
            NSLog(@"%@",ErrorResponse);
            
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }
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

-(void)checkAndDownloadFile:(NSString *)fileURL ofType:(NSString *)fileType withPostDownloadCompletion:(void (^)(void))downloadCompletionBlock
{
    //fileURL should be the server extension of file, eg: Uploads/Images/__profile_1452646100.jpg
    //fileType should be the local directory name, currently one of: Melodies, Profiles, Recordings.
    
    //fileType = [NSString stringWithFormat:@"%@/", fileType];
    
    NSFileManager * fileManager = [NSFileManager defaultManager];
    NSString *documentsPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) firstObject];
    NSString *localDirectoryPath = [documentsPath stringByAppendingPathComponent:fileType];
    
    //Get the directory portion of the file URL
    NSArray * parts = [fileURL componentsSeparatedByString:@"/"];
    NSString * fileName = [parts lastObject];
    
    //NSString * directory = [
    //NSString * directory = [fileURL stringByReplacingOccurrencesOfString:[parts lastObject] withString:@""];
    
    if (![fileManager fileExistsAtPath:localDirectoryPath])
    {
        NSError* error;
        if([[NSFileManager defaultManager] createDirectoryAtPath:localDirectoryPath withIntermediateDirectories:NO attributes:nil error:&error]) {
            
            NSLog(@"success creating folder");
            
        } else {
            NSLog(@"[%@] ERROR: attempting to write create MyFolder directory", [self class]);
            NSAssert( FALSE, @"Failed to create directory maybe out of disk space?");
        }
    }
    
    if (![fileManager fileExistsAtPath:[NSString stringWithFormat:@"%@/%@", localDirectoryPath, fileName]])
    {
        NSURLSessionConfiguration *configuration = [NSURLSessionConfiguration defaultSessionConfiguration];
        AFURLSessionManager *manager = [[AFURLSessionManager alloc] initWithSessionConfiguration:configuration];
        
        NSString *APICallURL = [NSString stringWithFormat:@"%@/%@", DOWNLOAD_BASE_URL, fileURL];
        APICallURL = [APICallURL stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
        
        NSURL * destinationURL = [NSURL fileURLWithPath:[NSString stringWithFormat:@"%@/%@", localDirectoryPath, fileName]];
        
        NSURL *URL = [NSURL URLWithString:APICallURL];
        NSURLRequest *request = [NSURLRequest requestWithURL:URL];
        NSProgress *progress = nil;
        
        NSURLSessionDownloadTask *downloadTask = [manager downloadTaskWithRequest:request progress:&progress destination:^NSURL *(NSURL *targetPath, NSURLResponse *response) {
                return destinationURL;
        }
                                                  
        completionHandler:^(NSURLResponse *response, NSURL *filePath, NSError *error) {
            
            NSLog(@"Succesful download of file %@", destinationURL);
            if (downloadCompletionBlock)
                downloadCompletionBlock();
            
        }];
        [downloadTask resume];

    } else {
        NSLog(@"No need to download file %@/%@", localDirectoryPath, fileName);
    }
    
}

@end
