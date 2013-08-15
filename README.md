DeltekReminder
==============

Reminds you to fill in your timesheet daily

Features:
-------------

* One click entry of hours
* One click to open active timesheet (no logging in)
* Configurable reminder time
* Close reminder to sleep 15 minutes
* No reminder if you're already entered time

Installation
-------------

[Download and install here](http://sirenofshame.com/DeltekReminderInstaller/publish.htm)

Release Notes
==============

1.0.0.12 - Aug 15, 2013
-------------

* Feature: Upon reminder popup can now quick save time against the project with the most hours
* Fixed: Null reference exception when setting the tray icon and the main form isn't initialized yet
* Fixed: If you navigated to a different page (e.g. expenses) the app was getting a null 
reference exception (b/c it incorrectly thought it was on the home page)
* Fixed: If you manually navigated to the home page the app displays the loading animation indefinitely

1.0.0.11 - Aug 11, 2013
-------------

* New Feature: Global error handling
* Fixed: Fail silently if the internet is unavailable
* Fixed: When users changed the time to check it wasn't immediatly using the new time, it is now
* Fixed: If there were multiple open timesheets it was using the furthest in the future, it now uses the closest
* Fixed: If I wasn't logged in at 5pm then it was telling me in the next morning that I haven't filled out my timesheet for the new day
* Fixed: If it missed the time it was supposed to check (another bug) then when I click "check now" it should recalculate the next check
* Minor UI updates

1.0.0.10 - Aug 8, 2013
-------------

* Feature: I want to be able to configure what time the reminder annoys me at
* Feature: The clickonce install app now uses a code signing certificate to make the install easier

1.0.0.0 - Aug 1, 2013
-------------

* Feature: When I start windows I want Deltek Reminder to auto-start (silently)
* Feature: I want the app to be able to auto-update itself with the latest version (ClickOnce)
* Feature: I want my password to be encrypted on disk
* Bug Fix: When I enter an incorrect u/p I want to see the error
* Feature: When the timer goes off and I haven't entered time I want to be able to go directly to my active timesheet within the app without logging in so I can enter my time
* Feature: When the timer goes off and I have entered time I want nothing to happen (but the app should re-schedule itself for tomorrow)
* Feature: When the timer goes off and I'm busy I want to close it or dismiss it and have it remind me in 15 minutes
* Feature: When I get a reminder I want it to hang around until I actively postpone or open the timesheet
* Feature: When I start the app and have successfully connected in the past I want the app to start minimized
* Feature: When I double click on the tray icon I want to see a page that shows when the app last check for status and when it will next check
* Feature: I want to be able to right-click and to exit the app
* Feature: When the timer goes off and I haven't entered time for today I want a dialog that pops up and allows me to open the webpage to add time
* Feature: When the timer goes off and there is no active timesheet I want a dialog that says so and allows me to open the webpage and add a timesheet
* Feature: When I enter a successful u/p I want a spinner and indication of what the app is doing
* Feature: When I enter a successful u/p I want an an indication of when the app will next remind me
* Feature: When I start the app for the first time and have never successfully authenticated I want a dialog that display username/password
