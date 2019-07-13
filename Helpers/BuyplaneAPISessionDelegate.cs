using System;
using System.IO;
using AircraftForSale.PCL.Helpers;
using Foundation;
using SDWebImage;
using UIKit;

namespace AircraftForSale.Helpers
{
    public class BuyplaneAPISessionDelegate : NSUrlSessionDownloadDelegate
    {
        System.Action completionHandler;

        int downloadCount = 0;

        public BuyplaneAPISessionDelegate(System.Action completionHandler)
        {
            this.completionHandler = completionHandler;
        }

        public BuyplaneAPISessionDelegate()
        {
            //this.completionHandler = null;
        }

        public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
        {
            //throw new NotImplementedException();
            //SDImageCache.SharedImageCache.StoreImageDataToDisk(downloadTask., adListing.ImageURL);

            try {
                var byteArray = File.ReadAllBytes(location.Path);

                var dataBytes = NSData.FromArray(byteArray);

                SDImageCache.SharedImageCache.StoreImageDataToDisk(dataBytes, downloadTask.OriginalRequest.ToString());

                //var dataBytes = NSData.FromUrl(location);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem getting bytes " + e.Message);
            }


            //NSFileManager fileManager = NSFileManager.DefaultManager;

            
            //NSUrl originalURL = downloadTask.OriginalRequest.Url;
          
            //NSError removeCopy;


            //fileManager.Remove(originalURL, out removeCopy);


            Console.WriteLine(string.Format("File location path: {0}", downloadTask.OriginalRequest.ToString()));


        }

        public override void DidWriteData(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
        {
            float progress = totalBytesWritten / (float)totalBytesExpectedToWrite;
            Console.WriteLine($"Task: {downloadTask.TaskIdentifier}  progress: {progress}");
            // perform any UI updates here
        }

        public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
        {
            Console.WriteLine("DidFinishEventsForBackgroundSession");

            // Perform any processing on the data

            using (var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate)
            {
                var handler = appDelegate.BackgroundUrlCompletionHandler;
                if (handler != null)
                {
                    handler.Invoke();
                }
            }

           
        }

        public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            Console.WriteLine("DidCompleteWithError");

            if (completionHandler != null)
            {
                completionHandler.Invoke();
            }

            downloadCount++;
            Settings.LastImageCacheDump = DateTime.Now.ToString();

        }
    }
}
