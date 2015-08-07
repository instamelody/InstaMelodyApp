//
//  CreateChatViewController.m
//  
//
//  Created by Ahmed Bakir on 8/7/15.
//
//

#import "CreateChatViewController.h"
#import "constants.h"
#import "AFHTTPRequestOperationManager.h"

@interface CreateChatViewController ()

@end

@implementation CreateChatViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

-(IBAction)submit:(id)sender {
    
    if (![self.nameField.text isEqualToString:@""] && ![self.messageField.text isEqualToString:@""] ) {
        
        
        NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
        
        NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"User": @{@"DisplayName" : self.nameField.text}, @"Message": @{@"Description" : self.messageField.text}}];
        
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat", BASE_URL];
        
        //add 64 char string
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            
            NSLog(@"JSON: %@", responseObject);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You have created a chat" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            [self dismissViewControllerAnimated:YES completion:nil];
            
        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
            NSLog(@"Error: %@", error);
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:error.description delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
        }];

        
    } else {
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please fill in all fields" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
    }

    
}

@end
