//
//  AppDelegate.swift
//  AtmoLauncher
//
//  Created by Tarasik A. Palczynski on 6/12/16.
//  Copyright © 2016 OneMoreBlock. All rights reserved.
//

import Cocoa

@NSApplicationMain
class AppDelegate: NSObject, NSApplicationDelegate {

    @IBOutlet weak var window: NSWindow!
    
    @IBOutlet weak var gearView: NSView!
    
    let frameRate = 60.0 // FPS
    let rate = 1.0 // Rotations per second
    
    @IBOutlet weak var rateSlider: NSSlider!
    
    func applicationDidFinishLaunching(aNotification: NSNotification) {
        var rotateGear = NSTimer.scheduledTimerWithTimeInterval(1.0 / frameRate, target: self, selector: Selector("rotateGear"), userInfo: nil, repeats: true)
    }

    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }
    
    func rotateGear () {
        gearView.frameCenterRotation += CGFloat((360.0 / frameRate) * (Double)(rateSlider.floatValue))
        gearView.display()
    }
}

