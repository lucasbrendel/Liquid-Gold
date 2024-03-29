﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Telerik.Windows.Controls;

namespace LiquidGold
{
    public partial class App : Application
    {
        public ViewModel.VehicleDataContext Vehicles = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
        public ViewModel.FillUpDataContext FillUps = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);

        public bool FirstFill;

        public RadDiagnostics Diagnostics;

        public enum Units
        {
            Imperial,
            Metric
        };

        public Units UserUnits;
        public bool LocationAware;
        public bool AskUserForLocation;

        public string[] StatisticsArray = 
        {
            "Average Mileage",
            "Worst Mileage",
            "Best Mileage",
            "Average Distance",
            "Shortest Distance",
            "Longest Distance",
            "Total Distance",
            "Average Quantity",
            "Smallest Quanity",
            "Largest Quantity",
            "Total Quantity",
            "Average Total Cost",
            "Smallest Total Cost",
            "Largest Total Cost",
            "Overall Cost",
            "Average Cost/Gallon",
            "Smallest Cost/Gallon",
            "Largest Cost/Gallon"
        };

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            using (ViewModel.FillUpDataContext fillDB = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString))
            {
                if (fillDB.DatabaseExists() == false)
                {
                    fillDB.CreateDatabase();
                }
            }

            using (ViewModel.VehicleDataContext vehicleDB = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString))
            {
                if (vehicleDB.DatabaseExists() == false)
                {
                    vehicleDB.CreateDatabase();
                }
            }

            UserUnits = Units.Imperial;

            try
            {
                LocationAware = Boolean.Parse(IsolatedStorageSettings.ApplicationSettings["LocationAware"].ToString());
                AskUserForLocation = false;
            }
            catch (KeyNotFoundException)
            {
                AskUserForLocation = true;
                LocationAware = false;
            }

            try
            {
                FirstFill = Boolean.Parse(IsolatedStorageSettings.ApplicationSettings["FirstRun"].ToString());
            }
            catch (KeyNotFoundException)
            {
                FirstFill = true;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            ApplicationUsageHelper.Init("1.0");
            try
            {
                string val = IsolatedStorageSettings.ApplicationSettings["RemindAgain"].ToString();
            }
            catch(KeyNotFoundException)
            {
                RadRateApplicationReminder reminder = new RadRateApplicationReminder();
                reminder.AllowUsersToSkipFurtherReminders = true;
                reminder.MessageBoxInfo = new Telerik.Windows.Controls.Reminders.MessageBoxInfoModel()
                {
                    Title = "Rate Liquid Gold",
                    Buttons = MessageBoxButtons.YesNo,
                    Content = "Would you like to rate this app? It would be greatly appreciated.",
                    SkipFurtherRemindersMessage = "Skip further reminders"
                };
                reminder.ShowReminderMessage(reminder.MessageBoxInfo);
                reminder.ReminderClosed += new EventHandler<ReminderClosedEventArgs>(reminder_ReminderClosed);
            }

            Diagnostics = new RadDiagnostics()
            {
                EmailTo = "Liquidgoldapp@outlook.com",
                EmailSubject = "ERROR",
                IncludeScreenshot = true,
                HandleUnhandledException = true,
                ApplicationName = "Liquid Gold",
                ApplicationVersion = "1.0"
            };
            Diagnostics.MessageBoxInfo = new Telerik.Windows.Controls.Reminders.MessageBoxInfoModel()
            {
                Buttons = MessageBoxButtons.YesNo,
                Title = "ERROR",
                Content = "There seems to have been an error. Do you want to send a report to help resolve this issue?"
            };
            Diagnostics.Init();
            Diagnostics.ExceptionOccurred += new EventHandler<ExceptionOccurredEventArgs>(Diagnostics_ExceptionOccurred);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Diagnostics_ExceptionOccurred(object sender, ExceptionOccurredEventArgs e)
        {
            Diagnostics.ReportHandledException(e.Exception);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void reminder_ReminderClosed(object sender, ReminderClosedEventArgs e)
        {
            if (e.MessageBoxEventArgs.IsCheckBoxChecked)
            {
                IsolatedStorageSettings.ApplicationSettings["RemindAgain"] = "1";
            }
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}