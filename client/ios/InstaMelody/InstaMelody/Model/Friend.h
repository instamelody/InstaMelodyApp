//
//  Friend.h
//  
//
//  Created by Ahmed Bakir on 8/21/15.
//
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface Friend : NSManagedObject

@property (nonatomic, retain) NSString * displayName;
@property (nonatomic, retain) NSString * firstName;
@property (nonatomic, retain) NSString * image;
@property (nonatomic, retain) NSString * lastName;
@property (nonatomic, retain) NSString * twitterUsername;
@property (nonatomic, retain) NSString * userId;
@property (nonatomic, retain) NSString * profileFilePath;

@end
