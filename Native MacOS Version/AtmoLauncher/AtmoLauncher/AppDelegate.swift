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
    
    let frameRate = 60.0 // FPS
    let rate = 1.0 // Rotations per second
    var time = 0.0
    
    var rotateGear : NSTimer = NSTimer()
    
    var moveMainView : Bool = false
    var oldMove : Bool = false
    
    func applicationDidFinishLaunching(aNotification: NSNotification) {
        
        rotateGear = NSTimer.scheduledTimerWithTimeInterval(1.0 / frameRate, target: self, selector: Selector("rotateGearFunction"), userInfo: nil, repeats: true)
    }
    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }
    
    func rotateGearFunction () {
        gearView.image = gearView.image?.imageRotatedByDegreess(CGFloat(((360.0 / frameRate) * (Double)(rate)) * time))
        
        downloadProgressBar.incrementBy(1.0)
        
        if (oldMove != moveMainView) {
            if (moveMainView) {
                if (moveMainViewToValue((Double)(mainView.frame.size.height) - 100.0, rate: 20.0)) {
                    oldMove = moveMainView
                }
            } else {
                if (moveMainViewToValue(20.0, rate: -20.0)) {
                    oldMove = moveMainView
                }
            }
        }
        
        time += 1.0 / frameRate
    }
    
    @IBAction func pressedChangeLogButton(sender: NSButton) {
        moveMainView = !moveMainView
    }
    
    func moveMainViewToValue (yValue : Double, rate : Double) -> Bool {
        if (rate >= 0) {
            if ((Double)(-mainView.bounds.origin.y) >= yValue) {
                return true
            }
        } else {
            if ((Double)(-mainView.bounds.origin.y) < yValue) {
                return true
            }
        }
        
        mainView.translateOriginToPoint(NSPoint(x: 0.0, y: rate))
        
        return false
    }
}

