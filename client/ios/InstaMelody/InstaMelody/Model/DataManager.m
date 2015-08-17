//
//  DataManager.m
//  
//
//  Created by Ahmed Bakir on 8/17/15.
//
//

#import "DataManager.h"

@implementation DataManager

+ (id)sharedManager {
    static DataManager *sharedMyManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedMyManager = [[self alloc] init];
    });
    return sharedMyManager;
}

- (void)updateFriends:(NSArray *)friendsList {
    [Friend MR_truncateAll];
    
    for (NSDictionary *friendDict in friendsList) {
        Friend *newFriend = [Friend MR_createEntity];
        newFriend.firstName = [friendDict objectForKey:@"FirstName"];
        newFriend.lastName = [friendDict objectForKey:@"LastName"];
        newFriend.userId = [friendDict objectForKey:@"Id"];
        newFriend.displayName = [friendDict objectForKey:@"DisplayName"];
    }
    
    [[NSManagedObjectContext MR_defaultContext] MR_saveToPersistentStoreWithCompletion:^(BOOL contextDidSave, NSError *error) {
        if (error == nil) {
            NSLog(@"CORE DATA save - successful");
        } else {
            NSLog(@"CORE DATA error - %@", error.description);
        }
        //
    }];
}

@end
