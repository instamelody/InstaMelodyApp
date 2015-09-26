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
    vc.melodyList = [melodyGroup.melodies allObjects];
    
    
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
