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
    
    @IBOutlet weak var mainView: NSView!
    
    @IBOutlet weak var gearView: NSImageView!
    @IBOutlet weak var gearCenter: NSImageView!
    
    @IBOutlet weak var downloadProgressBar: NSProgressIndicator!
    @IBOutlet weak var completionText: NSTextField!
    
    let frameRate = 60.0 // FPS
    let rate = 1.0 // Rotations per second
    var currentAngle = 0.0
    
    var rotateGear : NSTimer = NSTimer()
    
    var oldImage : NSImage!
    
    func applicationDidFinishLaunching(aNotification: NSNotification) {
        oldImage = gearView.image!.copy() as! NSImage
        
        mainView.layer?.backgroundColor = CGColorCreateGenericRGB(CGFloat(0.086), CGFloat(0.086), CGFloat(0.086), CGFloat(1.0))
        
        rotateGear = NSTimer.scheduledTimerWithTimeInterval(1.0 / frameRate, target: self, selector: Selector("rotateGearFunction"), userInfo: nil, repeats: true)
    }
    
    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }
    
    func rotateGearFunction () {
        currentAngle += (360.0 / frameRate) * (Double)(rate)
        if (currentAngle > 360.0) {
            currentAngle -= 360.0
        }
        
        let queue = NSOperationQueue()
        
        queue.addOperationWithBlock() {
            let newImage = self.oldImage!.imageRotatedByDegreess(CGFloat(self.currentAngle))
            
            NSOperationQueue.mainQueue().addOperationWithBlock() {
                self.gearView.image = newImage
            }
        }
        
        downloadProgressBar.incrementBy(0.1)
        completionText.stringValue = String(format:"%.0f", downloadProgressBar.doubleValue) + "%"
    }
    
    @IBAction func helpToolbar(sender: NSMenuItem) {
        if let checkURL = NSURL(string: "http://onemoreblock.com/forum/viewtopic.php?f=24&t=3051") {
            if NSWorkspace.sharedWorkspace().openURL(checkURL) {
                print("URL successfully opened")
            }
        } else {
            print("Invalid URL")
        }
    }
}

