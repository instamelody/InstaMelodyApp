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
#import "UserMelody.h"
#import "UserMelodyPart.h"
#import "MelodyGroup.h"

@interface DataManager : NSObject

+ (id)sharedManager;

- (void)fetchFriends;
- (NSArray *)friendList;

- (void)fetchMelodies;
- (NSArray *)melodyList;
- (NSArray *)melodyGroupList;

- (void)fetchUserMelodies;
- (NSArray *)userMelodyList;
-(void)fetchUserMelody:(NSString*)melodyId;

-(void)clearChats;
-(void)clearUserMelodies;
-(void)clearFriends;

-(void)fetchNewestStations;
-(void)fetchTopStations;

-(BOOL)isPremium;
-(BOOL)isMature;

@property (nonatomic, strong) NSDateFormatter *dateFormatter;
@property NSDateFormatter *melodyDateFormatter;

@end
