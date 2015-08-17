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

@interface DataManager : NSObject

+ (id)sharedManager;

- (void)updateFriends:(NSArray *)friendsList;
- (void)fetchFriends;
- (NSArray *)friendList;

@end
