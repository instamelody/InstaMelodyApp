//
//  Melody.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/17/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

@class MelodyGroup;

@interface Melody : NSManagedObject

@property (nonatomic, retain) NSNumber * melodyId;
@property (nonatomic, retain) NSNumber * isUserCreated;
@property (nonatomic, retain) NSNumber * isPremiumContent;
@property (nonatomic, retain) NSString * fileName;
@property (nonatomic, retain) NSString * filePathUrlString;
@property (nonatomic, retain) NSDate * dateModified;
@property (nonatomic, retain) NSString * melodyName;
@property (nonatomic, retain) NSString * melodyDesc;
@property (nonatomic, retain) MelodyGroup *melodyGroup;

@end
