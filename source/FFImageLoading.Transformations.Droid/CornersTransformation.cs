﻿using System;
using Android.Graphics;

namespace FFImageLoading.Transformations
{
	public class CornersTransformation : TransformationBase
	{
		private double _topLeftCornerSize;
		private double _topRightCornerSize;
		private double _bottomLeftCornerSize;
		private double _bottomRightCornerSize;
		private double _cropWidthRatio;
		private double _cropHeightRatio;
		private CornerTransformType _cornersTransformType;

		public CornersTransformation(double cornersSize, CornerTransformType cornersTransformType)
		{
			_topLeftCornerSize = cornersSize;
			_topRightCornerSize = cornersSize;
			_bottomLeftCornerSize = cornersSize;
			_bottomRightCornerSize = cornersSize;
			_cornersTransformType = cornersTransformType;
			_cropWidthRatio = 1f;
			_cropHeightRatio = 1f;
		}

		public CornersTransformation(double topLeftCornerSize, double topRightCornerSize, double bottomLeftCornerSize, double bottomRightCornerSize, 
			CornerTransformType cornersTransformType)
		{
			_topLeftCornerSize = topLeftCornerSize;
			_topRightCornerSize = topRightCornerSize;
			_bottomLeftCornerSize = bottomLeftCornerSize;
			_bottomRightCornerSize = bottomRightCornerSize;
			_cornersTransformType = cornersTransformType;
			_cropWidthRatio = 1f;
			_cropHeightRatio = 1f;
		}

		public CornersTransformation(double cornersSize, CornerTransformType cornersTransformType, double cropWidthRatio, double cropHeightRatio)
		{
			_topLeftCornerSize = cornersSize;
			_topRightCornerSize = cornersSize;
			_bottomLeftCornerSize = cornersSize;
			_bottomRightCornerSize = cornersSize;
			_cornersTransformType = cornersTransformType;
			_cropWidthRatio = cropWidthRatio;
			_cropHeightRatio = cropHeightRatio;
		}

		public CornersTransformation(double topLeftCornerSize, double topRightCornerSize, double bottomLeftCornerSize, double bottomRightCornerSize, 
			CornerTransformType cornersTransformType, double cropWidthRatio, double cropHeightRatio)
		{
			_topLeftCornerSize = topLeftCornerSize;
			_topRightCornerSize = topRightCornerSize;
			_bottomLeftCornerSize = bottomLeftCornerSize;
			_bottomRightCornerSize = bottomRightCornerSize;
			_cornersTransformType = cornersTransformType;
			_cropWidthRatio = cropWidthRatio;
			_cropHeightRatio = cropHeightRatio;
		}

		public override string Key
		{
			get { return string.Format("CornersTransformation, cornersSizes = {0}/{1}/{2}/{3}, cornersTransformType = {4}, cropWidthRatio = {5}, cropHeightRatio = {6}, ", 
				_topLeftCornerSize, _topRightCornerSize, _bottomRightCornerSize, _bottomLeftCornerSize, _cornersTransformType, _cropWidthRatio, _cropHeightRatio); }
		}

		protected override Bitmap Transform(Bitmap source)
		{
			return ToTransformedCorners(source, _topLeftCornerSize, _topRightCornerSize, _bottomLeftCornerSize, _bottomRightCornerSize, 
				_cornersTransformType, _cropWidthRatio, _cropHeightRatio);
		}

		public static Bitmap ToTransformedCorners(Bitmap source, double topLeftCornerSize, double topRightCornerSize, double bottomLeftCornerSize, double bottomRightCornerSize, 
			CornerTransformType cornersTransformType, double cropWidthRatio, double cropHeightRatio)
		{
			double sourceWidth = source.Width;
			double sourceHeight = source.Height;

			double desiredWidth = sourceWidth;
			double desiredHeight = sourceHeight;

			double desiredRatio = cropWidthRatio / cropHeightRatio;
			double currentRatio = sourceWidth / sourceHeight;

			if (currentRatio > desiredRatio)
			{
				desiredWidth = (cropWidthRatio * sourceHeight / cropHeightRatio);
			}
			else if (currentRatio < desiredRatio)
			{
				desiredHeight = (cropHeightRatio * sourceWidth / cropWidthRatio);
			}

			topLeftCornerSize = topLeftCornerSize * (desiredWidth + desiredHeight) / 2 / 100;
			topRightCornerSize = topRightCornerSize * (desiredWidth + desiredHeight) / 2 / 100;
			bottomLeftCornerSize = bottomLeftCornerSize * (desiredWidth + desiredHeight) / 2 / 100;
			bottomRightCornerSize = bottomRightCornerSize * (desiredWidth + desiredHeight) / 2 / 100;

			float cropX = (float)((sourceWidth - desiredWidth) / 2);
			float cropY = (float)((sourceHeight - desiredHeight) / 2);

			Bitmap bitmap = Bitmap.CreateBitmap((int)desiredWidth, (int)desiredHeight, Bitmap.Config.Argb8888);

			using (Canvas canvas = new Canvas(bitmap))
			using (Paint paint = new Paint())
			using (BitmapShader shader = new BitmapShader(source, Shader.TileMode.Clamp, Shader.TileMode.Clamp))
			using (Matrix matrix = new Matrix())
			using (var path = new Path())
			{
				if (cropX != 0 || cropY != 0)
				{
					matrix.SetTranslate(-cropX, -cropY);
					shader.SetLocalMatrix(matrix);
				}

				paint.SetShader(shader);
				paint.AntiAlias = true;

				// TopLeft
				if (cornersTransformType.HasFlag(CornerTransformType.TopLeftCut)) 
				{
					path.MoveTo(0, (float)topLeftCornerSize);
					path.LineTo((float)topLeftCornerSize, 0);
				}
				else if (cornersTransformType.HasFlag(CornerTransformType.TopLeftRounded)) 
				{
					path.MoveTo(0, (float)topLeftCornerSize);
					path.QuadTo(0, 0, (float)topLeftCornerSize, 0);
				}
				else
				{
					path.MoveTo(0, 0);
				}

				// TopRight
				if (cornersTransformType.HasFlag(CornerTransformType.TopRightCut)) 
				{
					path.LineTo((float)(desiredWidth - topRightCornerSize), 0);
					path.LineTo((float)desiredWidth, (float)topRightCornerSize);
				}
				else if (cornersTransformType.HasFlag(CornerTransformType.TopRightRounded))
				{
					path.LineTo((float)(desiredWidth - topRightCornerSize), 0);
					path.QuadTo((float)desiredWidth, 0, (float)desiredWidth, (float)topRightCornerSize);
				}
				else
				{
					path.LineTo((float)desiredWidth ,0);
				}

				// BottomRight
				if (cornersTransformType.HasFlag(CornerTransformType.BottomRightCut)) 
				{
					path.LineTo((float)desiredWidth, (float)(desiredHeight - bottomRightCornerSize));
					path.LineTo((float)(desiredWidth - bottomRightCornerSize), (float)desiredHeight);
				}
				else if (cornersTransformType.HasFlag(CornerTransformType.BottomRightRounded))
				{
					path.LineTo((float)desiredWidth, (float)(desiredHeight - bottomRightCornerSize));
					path.QuadTo((float)desiredWidth, (float)desiredHeight, (float)(desiredWidth - bottomRightCornerSize), (float)desiredHeight);
				}
				else
				{
					path.LineTo((float)desiredWidth, (float)desiredHeight);
				}

				// BottomLeft
				if (cornersTransformType.HasFlag(CornerTransformType.BottomLeftCut)) 
				{
					path.LineTo((float)bottomLeftCornerSize, (float)desiredHeight);
					path.LineTo(0, (float)(desiredHeight - bottomLeftCornerSize));
				}
				else if (cornersTransformType.HasFlag(CornerTransformType.BottomLeftRounded)) 
				{
					path.LineTo((float)bottomLeftCornerSize, (float)desiredHeight);
					path.QuadTo(0, (float)desiredHeight, 0, (float)(desiredHeight - bottomLeftCornerSize));
				}
				else
				{
					path.LineTo(0, (float)desiredHeight);
				}

				path.Close();
				canvas.DrawPath(path, paint);

				return bitmap;				
			}
		}
	}
}

