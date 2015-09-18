//
//  UserMelodyPart.h
//  
//
//  Created by Ahmed Bakir on 9/18/15.
//
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

@class UserMelody;

@interface UserMelodyPart : NSManagedObject

@property (nonatomic, retain) NSNumber * partId;
@property (nonatomic, retain) NSNumber * isUserCreated;
@property (nonatomic, retain) NSString * partName;
@property (nonatomic, retain) NSString * partDesc;
@property (nonatomic, retain) NSString * filePath;
@property (nonatomic, retain) NSDate * dateModified;
@property (nonatomic, retain) NSString * fileName;
@property (nonatomic, retain) UserMelody *userMelody;

@end
