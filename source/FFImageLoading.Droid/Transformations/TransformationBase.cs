﻿using System;
using Android.Graphics;
using FFImageLoading.Work;

namespace FFImageLoading.Transformations
{
	public abstract class TransformationBase: ITransformation
	{
		public abstract string Key { get; }

		public IBitmap Transform(IBitmap source)
		{
			return new BitmapHolder(Transform(source.ToNative()));
		}

		protected abstract Bitmap Transform(Bitmap source);
	}
}

