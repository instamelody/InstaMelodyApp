//
//  NetworkManager.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/14/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AFHTTPRequestOperationManager.h"
#import "constants.h"

@interface NetworkManager : NSObject

+ (id)sharedManager;

-(void)uploadUserMelody:(NSDictionary *)userDict;
-(void)uploadChatUserMelody:(NSDictionary *)userDict;
-(void)updateProfilePicture:(UIImage *)image;
-(void)uploadFile:(NSString *)filePath withFileToken:(NSString *)fileToken;

@property (nonatomic, strong) NSDateFormatter *dateFormatter;

@end
