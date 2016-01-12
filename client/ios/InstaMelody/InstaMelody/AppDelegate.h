//
//  AppDelegate.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 7/1/15.
//  Copyright (c) 2015 InstaMelody. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;


@property (strong, nonatomic) NSString *loopOwnersId;
//This is a bad hack, but loopVC is difficult to follow
//and the app needs to get to the app store asap.
//Need to go through loopVC and streamline code later.

@end

