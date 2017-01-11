//
//  MelodyPickerController.m
//  InstaMelody
//
//  Created by Ahmed Bakir on 9/26/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "MelodyPickerController.h"
#import "ServerMelodyCell.h"
#import "UIFont+FontAwesome.h"
#import "NSString+FontAwesome.h"

@interface MelodyPickerController ()

@end

@implementation MelodyPickerController

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

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    // Return the number of rows in the section.
    
    return self.melodyList.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    ServerMelodyCell *cell = (ServerMelodyCell *)[tableView dequeueReusableCellWithIdentifier:@"MelodyCell" forIndexPath:indexPath];
    
    Melody *melody = (Melody *)[self.melodyList objectAtIndex:indexPath.row];
    
    cell.nameLabel.text = melody.melodyName;
    cell.lockLabel.font = [UIFont fontAwesomeFontOfSize:20.0f];
    cell.playButton.titleLabel.font = [UIFont fontAwesomeFontOfSize:20.0f];
    
    if ([melody.isPremiumContent boolValue]) {
        cell.lockLabel.text = [NSString fontAwesomeIconStringForEnum:FALock];
    } else {
        cell.lockLabel.text = [NSString fontAwesomeIconStringForEnum:FAUnlock];
    }
    
    [cell.playButton setTitle:[NSString fontAwesomeIconStringForEnum:FAPlayCircle] forState:UIControlStateNormal];
    cell.playButton.tag = [melody.melodyId integerValue];
    cell.tag = indexPath.row + 900;
    
    [cell.playButton addTarget:self action:@selector(preview:) forControlEvents:UIControlEventTouchUpInside];
    
    cell.backgroundColor = [UIColor clearColor];
        
    return cell;
}

-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    
    //pass back item
    
    Melody *melody = (Melody *)[self.melodyList objectAtIndex:indexPath.row];
    
    //NSDictionary *userDict = @{@"melodyId": melody.melodyId, @"groupId":  self.groupId};
    
    //NSDictionary *userDict = [NSDictionary dictionaryWithObject:melody.melodyId forKey:@"melodyId"];
    
    //[[NSNotificationCenter defaultCenter] postNotificationName:@"pickedMelody" object:nil userInfo:userDict];
    
    _delegate.melodyId = melody.melodyId;
    _delegate.groupId = self.groupId;
    
    [self done:nil];
}

-(IBAction)done:(id)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
}

@end
