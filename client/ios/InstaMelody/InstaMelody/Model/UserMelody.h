//
//  UserMelody.h
//  
//
//  Created by Ahmed Bakir on 9/18/15.
//
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

@class NSManagedObject;

@interface UserMelody : NSManagedObject

@property (nonatomic, retain) NSString * userMelodyId;
@property (nonatomic, retain) NSString * userId;
@property (nonatomic, retain) NSString * userMelodyName;
@property (nonatomic, retain) NSDate * dateCreated;
@property (nonatomic, retain) NSSet *parts;
@end

@interface UserMelody (CoreDataGeneratedAccessors)

- (void)addPartsObject:(NSManagedObject *)value;
- (void)removePartsObject:(NSManagedObject *)value;
- (void)addParts:(NSSet *)values;
- (void)removeParts:(NSSet *)values;

@end
