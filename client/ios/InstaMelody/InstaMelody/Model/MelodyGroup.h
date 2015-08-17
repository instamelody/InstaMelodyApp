//
//  MelodyGroup.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 8/17/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

@class Melody;

@interface MelodyGroup : NSManagedObject

@property (nonatomic, retain) NSNumber * groupId;
@property (nonatomic, retain) NSDate * dateModified;
@property (nonatomic, retain) NSString * groupName;
@property (nonatomic, retain) NSSet *melodies;
@end

@interface MelodyGroup (CoreDataGeneratedAccessors)

- (void)addMelodiesObject:(Melody *)value;
- (void)removeMelodiesObject:(Melody *)value;
- (void)addMelodies:(NSSet *)values;
- (void)removeMelodies:(NSSet *)values;

@end
