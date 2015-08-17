//
//  DataManager.m
//  
//
//  Created by Ahmed Bakir on 8/17/15.
//
//

#import "DataManager.h"
#import "Constants.h"
#import "AFHTTPRequestOperationManager.h"

@implementation DataManager

+ (id)sharedManager {
    static DataManager *sharedMyManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedMyManager = [[self alloc] init];
    });
    return sharedMyManager;
}

- (void)fetchFriends {
    NSString *requestUrl = [NSString stringWithFormat:@"%@/User/Friends", BASE_URL];
    
    NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
    
    //add 64 char string
    
    AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
    
    NSDictionary *parameters = @{@"token": token};
    
    [manager GET:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
        NSLog(@"JSON: %@", responseObject);
        
        NSArray *friendsList = (NSArray *)responseObject;
        
        [self updateFriends:friendsList];
        
        
        //NSDictionary *responseDict = (NSDictionary *)responseObject;
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        NSLog(@"Error fetching friends: %@", error);
        
    }];
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

- (NSArray *)friendList {
    return [Friend MR_findAll];
}

@end
