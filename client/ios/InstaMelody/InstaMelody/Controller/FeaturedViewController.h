//
//  FeaturedViewController.h
//  InstaMelody
//
//  Created by Ahmed Bakir on 10/23/15.
//  Copyright © 2015 InstaMelody. All rights reserved.
//

#import "InstamelodyViewController.h"

@interface FeaturedViewController : InstamelodyViewController  <UICollectionViewDataSource, UICollectionViewDelegate>

@property IBOutlet UICollectionView *currentCollectionView;
@property IBOutlet UICollectionView *featuredCollectionView;
@property IBOutlet UICollectionView *adCollectionView;

@end
