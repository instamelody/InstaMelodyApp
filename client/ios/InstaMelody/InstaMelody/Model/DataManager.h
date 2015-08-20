//
//  DataManager.h
//  
//
//  Created by Ahmed Bakir on 8/17/15.
//
//

#import <Foundation/Foundation.h>
#import <MagicalRecord/MagicalRecord.h>
#import "Friend.h"
#import "Melody.h"
#import "MelodyGroup.h"

@interface DataManager : NSObject

+ (id)sharedManager;

- (void)fetchFriends;
- (NSArray *)friendList;

- (void)fetchMelodies;
- (NSArray *)melodyList;

- (NSArray *)melodyGroupList;

@property (nonatomic, strong) NSDateFormatter *dateFormatter;

@end
