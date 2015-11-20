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
#import "Friend.h"
#import "DataManager.h"

@interface CreateChatViewController ()

@property (nonatomic, strong) NSArray *friendsList;
@property (strong, nonatomic) NSArray *names;
@property (strong, nonatomic) NSArray *filteredNames;

@property (strong, nonatomic) NSMutableArray *selectedNames;

@end

@implementation CreateChatViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    //populate names with 'friends'
    
    [self initNames];
    
    self.tokenInputView.fieldName = @"To:";
    //self.tokenInputView.fieldView = infoButton;
    self.tokenInputView.placeholderText = @"Enter a name";
    //self.tokenInputView.accessoryView = [self contactAddButton];
    self.tokenInputView.drawBottomBorder = YES;
    
    [self.tableView registerClass:[UITableViewCell class] forCellReuseIdentifier:@"Cell"];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Navigation

/*
// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    
}
 */

-(void)initNames {
    self.friendsList = [[DataManager sharedManager] friendList];
    
    NSMutableArray *tempFriends = [NSMutableArray new];
    for (Friend *friend in self.friendsList) {
        [tempFriends addObject:friend.displayName];
    }
    
    self.names = (NSArray *)tempFriends;
    
    //self.names = ;
    self.filteredNames = nil;
    self.selectedNames = [NSMutableArray arrayWithCapacity:self.names.count];
    
}

-(IBAction)submit:(id)sender {
    
    if (![self.nameField.text isEqualToString:@""] && ![self.messageField.text isEqualToString:@""] && ![self.topicField.text isEqualToString:@""] ) {
        
        
        NSString *token =  [[NSUserDefaults standardUserDefaults] objectForKey:@"authToken"];
        
        NSInteger tokenCount = self.tokenInputView.allTokens.count;
        NSMutableDictionary *parameters;
        
        if ( tokenCount == 1) {
            CLToken *cltoken = (CLToken *)[self.tokenInputView.allTokens objectAtIndex:0];
            
            parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"User": @{@"DisplayName" : cltoken.displayText}, @"Message": @{@"Description" : self.messageField.text}, @"Chat": @{@"Name" : self.topicField.text}}];
        } else if  (tokenCount > 1){
            
            NSMutableArray *tempUserArray = [NSMutableArray new];
            
            for (CLToken *cltoken in self.tokenInputView.allTokens) {
                NSDictionary *tempDict = [NSDictionary dictionaryWithObject:cltoken.displayText forKey:@"DisplayName"];
                [tempUserArray addObject:tempDict];
            }
            
            
            parameters = [NSMutableDictionary dictionaryWithDictionary:@{@"Token": token, @"Users": tempUserArray, @"Message": @{@"Description" : self.messageField.text}, @"Chat": @{@"Name" : self.topicField.text}}];
        }
        
        
        NSString *requestUrl = [NSString stringWithFormat:@"%@/Message/Chat", API_BASE_URL];
        
        //add 64 char string
        
        AFHTTPRequestOperationManager *manager = [AFHTTPRequestOperationManager manager];

        manager.requestSerializer = [AFJSONRequestSerializer serializer];
        
        [manager POST:requestUrl parameters:parameters success:^(AFHTTPRequestOperation *operation, id responseObject) {
            
            NSLog(@"JSON: %@", responseObject);
            
            NSDictionary *responseDict = (NSDictionary *)responseObject;
            UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Success" message:@"You have created a chat" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
            [alertView show];
            
            [self.delegate didCreateChatWithId:[responseDict objectForKey:@"Id"]];
            
            [self dismissViewControllerAnimated:YES completion:nil];
            
        } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
            if ([operation.responseObject isKindOfClass:[NSDictionary class]]) {
                NSDictionary *errorDict = [NSJSONSerialization JSONObjectWithData:(NSData *)error.userInfo[AFNetworkingOperationFailingURLResponseDataErrorKey] options:0 error:nil];
                
                NSString *ErrorResponse = [NSString stringWithFormat:@"Error %ld: %@", operation.response.statusCode, [errorDict objectForKey:@"Message"]];
                
                NSLog(@"%@",ErrorResponse);
                
                UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:ErrorResponse delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
                [alertView show];
            }
        }];

        
    } else {
        
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please fill in all fields" delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil];
        [alertView show];
        
    }

    
}

#pragma mark - CLTokenInputViewDelegate

- (void)tokenInputView:(CLTokenInputView *)view didChangeText:(NSString *)text
{
    if ([text isEqualToString:@""]){
        self.filteredNames = nil;
        self.tableView.hidden = YES;
    } else {
        NSPredicate *predicate = [NSPredicate predicateWithFormat:@"self contains[cd] %@", text];
        self.filteredNames = [self.names filteredArrayUsingPredicate:predicate];
        self.tableView.hidden = NO;
    }
    [self.tableView reloadData];
}

- (void)tokenInputView:(CLTokenInputView *)view didAddToken:(CLToken *)token
{
    NSString *name = token.displayText;
    [self.selectedNames addObject:name];
}

- (void)tokenInputView:(CLTokenInputView *)view didRemoveToken:(CLToken *)token
{
    NSString *name = token.displayText;
    [self.selectedNames removeObject:name];
}

- (CLToken *)tokenInputView:(CLTokenInputView *)view tokenForText:(NSString *)text
{
    if (self.filteredNames.count > 0) {
        NSString *matchingName = self.filteredNames[0];
        CLToken *match = [[CLToken alloc] initWithDisplayText:matchingName context:nil];
        return match;
    }
    // TODO: Perhaps if the text is a valid phone number, or email address, create a token
    // to "accept" it.
    return nil;
}

- (void)tokenInputViewDidEndEditing:(CLTokenInputView *)view
{
    NSLog(@"token input view did end editing: %@", view);
    view.accessoryView = nil;
}

- (void)tokenInputViewDidBeginEditing:(CLTokenInputView *)view
{
    
    NSLog(@"token input view did begin editing: %@", view);
    //view.accessoryView = [self contactAddButton];
    [self.view removeConstraint:self.tableViewTopLayoutConstraint];
    self.tableViewTopLayoutConstraint = [NSLayoutConstraint constraintWithItem:self.tableView attribute:NSLayoutAttributeTop relatedBy:NSLayoutRelationEqual toItem:view attribute:NSLayoutAttributeBottom multiplier:1.0 constant:0];
    [self.view addConstraint:self.tableViewTopLayoutConstraint];
    [self.view layoutIfNeeded];
}


#pragma mark - UITableViewDataSource

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return self.filteredNames.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"Cell" forIndexPath:indexPath];
    NSString *name = self.filteredNames[indexPath.row];
    cell.textLabel.text = name;
    if ([self.selectedNames containsObject:name]) {
        cell.accessoryType = UITableViewCellAccessoryCheckmark;
    } else {
        cell.accessoryType = UITableViewCellAccessoryNone;
    }
    return cell;
}


#pragma mark - UITableViewDelegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    NSString *name = self.filteredNames[indexPath.row];
    CLToken *token = [[CLToken alloc] initWithDisplayText:name context:nil];
    if (self.tokenInputView.isEditing) {
        [self.tokenInputView addToken:token];
    }
}

#pragma mark - Demo Buttons
- (UIButton *)contactAddButton
{
    UIButton *contactAddButton = [UIButton buttonWithType:UIButtonTypeContactAdd];
    [contactAddButton addTarget:self action:@selector(onAccessoryContactAddButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    return contactAddButton;
}

- (void)onAccessoryContactAddButtonTapped:(id)sender
{
    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Accessory View Button"
                                                        message:@"This view is optional and can be a UIButton, etc."
                                                       delegate:nil
                                              cancelButtonTitle:@"Okay"
                                              otherButtonTitles:nil];
    [alertView show];
}

-(IBAction)cancel:(id)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
}

@end
