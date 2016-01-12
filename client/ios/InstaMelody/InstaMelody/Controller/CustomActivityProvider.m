//
//  CustomActivityProvider.m
//

#import "CustomActivityProvider.h"
#import "AppDelegate.h"

@implementation CustomActivityProvider

- (id) activityViewController:(UIActivityViewController *)activityViewController
          itemForActivityType:(NSString *)activityType
{
    //AppDelegate *delegate=(AppDelegate*)[[UIApplication sharedApplication]delegate];
    //delegate.selectedNSName = [delegate.selectedNSName stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceCharacterSet]];;
    
    if ( [activityType isEqualToString:UIActivityTypePostToTwitter] )
        return [NSString stringWithFormat:@"Custom Twitter message here"];// @"This is a #twitter post!";
    
    if ( [activityType isEqualToString:UIActivityTypePostToFacebook] )
        return [NSString stringWithFormat:@"You won't see this message..."]; //Facebook doesn't allow message
    
    if ( [activityType isEqualToString:UIActivityTypeMessage] )
        return [NSString stringWithFormat:@"Custom SMS message here"]; //SMS

    if ( [activityType isEqualToString:UIActivityTypeMail] )
        return [NSString stringWithFormat:@"Custom Email message here"]; //Email
    
    return nil;
}

- (id) activityViewControllerPlaceholderItem:(UIActivityViewController *)activityViewController { return @""; }

@end
