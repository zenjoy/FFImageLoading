﻿using FFImageLoading.Work;
using FFImageLoading.Helpers;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;

namespace FFImageLoading
{
    public static class TaskParameterExtensions
    {
        /// <summary>
        /// Loads the image into given imageView using defined parameters.
        /// </summary>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static IScheduledWork Into(this TaskParameter parameters, Image imageView)
        {
            var weakRef = new WeakReference<Image>(imageView);

            Func<Image> getNativeControl = () => {
                Image refView = null;

                if (!weakRef.TryGetTarget(out refView))
                    return null;

                return refView;
            };

            Action<WriteableBitmap, bool> doWithImage = (img, fromCache) => {
                Image refView = getNativeControl();
                if (refView == null)
                    return;

                var isFadeAnimationEnabled = parameters.FadeAnimationEnabled.HasValue ?
                    parameters.FadeAnimationEnabled.Value : ImageService.Config.FadeAnimationEnabled;

                if (isFadeAnimationEnabled && !fromCache)
                {
                    refView.Source = img;

                    //TODO !!!!!!!!!!!!!!
                    // fade animation
                    //DoubleAnimation fadeoutAnimation = new DoubleAnimation();
                    //fadeoutAnimation.Duration = TimeSpan.FromMilliseconds(500);
                    //fadeoutAnimation.From = 1.0d;
                    //fadeoutAnimation.To = 0.0d;
                    //Storyboard storyboard = new Storyboard();
                    //storyboard.Children.Add(fadeoutAnimation);
                    //refView.BeginAnimation(Image.OpacityProperty, fadeoutAnimation);
                }
                else
                {
                    refView.Source = img;
                }
            };

            return parameters.Into(getNativeControl, doWithImage);
        }

        /// <summary>
        /// Only use this method if you plan to handle exceptions in your code. Awaiting this method will give you this flexibility.
        /// </summary>
        /// <returns>An awaitable Task.</returns>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Image imageView)
        {
            return parameters.IntoAsync(param => param.Into(imageView));
        }

        private static IScheduledWork Into(this TaskParameter parameters, Func<Image> getNativeControl, Action<WriteableBitmap, bool> doWithImage)
        {
            var task = new ImageLoaderTask(ImageService.Config.DownloadCache, new MainThreadDispatcher(), ImageService.Config.Logger, parameters,
                getNativeControl, doWithImage);
            ImageService.LoadImage(task);
            return task;
        }

        private static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Action<TaskParameter> into)
        {
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<IScheduledWork>();

            parameters
                .Error(ex => {
                    userErrorCallback(ex);
                    tcs.SetException(ex);
                })
                .Finish(scheduledWork => {
                    finishCallback(scheduledWork);
                    tcs.TrySetResult(scheduledWork); // we should use TrySetResult since SetException could have been called earlier. It is not allowed to set result after SetException
                });

            into(parameters);

            return tcs.Task;
        }
    }
}
