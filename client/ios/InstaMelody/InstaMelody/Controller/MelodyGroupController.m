//
//  MelodyGroupController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "MelodyGroupController.h"

@interface MelodyGroupController ()

@property NSArray *groupList;

@end

@implementation MelodyGroupController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    [self refreshGroupList];
    
    
    //self.navigationController.navigationBar.translucent = YES;
    [(UIView*)[self.navigationController.navigationBar.subviews objectAtIndex:0] setAlpha:0.2f];
    
    NSDictionary *navbarTitleTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                               [UIColor darkGrayColor],
                                               NSForegroundColorAttributeName,
                                               [UIFont fontWithName:@"Century Gothic" size:18.0],
                                               NSFontAttributeName,
                                               nil];
    
    
    [self.navigationController.navigationBar setTitleTextAttributes:navbarTitleTextAttributes];
    //[self.navigationController.navigationBar setTintColor:[UIColor colorWithRed:191/255.0f green:139/255.0f blue:226/255.0f alpha:1.0f]];
    [self.navigationController.navigationBar setTintColor:[UIColor darkGrayColor]];
    
    
    NSDictionary *buttonTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                          [UIColor darkGrayColor],
                                          NSForegroundColorAttributeName,
                                          [UIFont fontWithName:@"FontAwesome" size:20.0],
                                          NSFontAttributeName,
                                          nil];
    
    
    [[UIBarButtonItem appearance] setTitleTextAttributes:buttonTextAttributes forState:UIControlStateNormal];
    
    if (self.groupId != nil) {
        
        MelodyGroup *melodyGroup = [MelodyGroup MR_findFirstByAttribute:@"groupId" withValue:self.groupId];
        
        
        UIStoryboard *sb = [UIStoryboard storyboardWithName:@"Main" bundle:nil];
        MelodyPickerController *vc = (MelodyPickerController *)[sb instantiateViewControllerWithIdentifier:@"MelodyPickerController"];
        
        
        NSArray *melodyArray = [melodyGroup.melodies allObjects];
        
        NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"melodyName" ascending:YES];
        NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
        
        NSArray *sortedArray = [melodyArray sortedArrayUsingDescriptors:descriptors];
        
        vc.melodyList = sortedArray;
        vc.groupId = melodyGroup.groupId;
        [self.navigationController pushViewController:vc animated:YES];
        
    }
    
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

-(void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];

}

-(IBAction)cancel:(id)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    
    return self.groupList.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"MelodyGroupCell" forIndexPath:indexPath];
    
    MelodyGroup *melodyGroup = (MelodyGroup *)[self.groupList objectAtIndex:indexPath.row];
    
    //NSDictionary *friendDict = [self.friendsList objectAtIndex:indexPath.row];
    
    cell.textLabel.text = melodyGroup.groupName;
    cell.backgroundColor = [UIColor clearColor];
    
    return cell;
}

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
    
    UITableViewCell *selectedCell = (UITableViewCell *)sender;
    
    NSIndexPath *indexPath = [self.tableView indexPathForCell:selectedCell];
    MelodyGroup *melodyGroup = (MelodyGroup *)[self.groupList objectAtIndex:indexPath.row];
    
    MelodyPickerController *vc = (MelodyPickerController *)segue.destinationViewController;
    
    NSArray *melodyArray = [melodyGroup.melodies allObjects];
    
    NSSortDescriptor *valueDescriptor = [[NSSortDescriptor alloc] initWithKey:@"melodyName" ascending:YES];
    NSArray *descriptors = [NSArray arrayWithObject:valueDescriptor];
    
    NSArray *sortedArray = [melodyArray sortedArrayUsingDescriptors:descriptors];
    
    vc.melodyList = sortedArray;
    vc.groupId = melodyGroup.groupId;
    vc.delegate = self.delegate;
    
}


#pragma mark - web actions

-(void)refreshGroupList {
    [self getGroupList];
}

-(void)getGroupList {
    //
    
    //add observer for fetching friend list asynchronously
    
    //get friends from core data
    self.groupList = [[DataManager sharedManager] melodyGroupList];
    
}


@end
