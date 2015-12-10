﻿using System;

using Xamarin.Forms;
using FFImageLoading.Forms.Sample.ViewModels;
using DLToolkit.PageFactory;
using FFImageLoading.Forms.Sample.Models;

namespace FFImageLoading.Forms.Sample.Pages
{
	public class ListExamplePage : PFContentPage<ListExampleViewModel>
	{
		public ListExamplePage()
		{
			Title = "List Demo";

			var listView = new ListView() {
				HorizontalOptions = LayoutOptions.FillAndExpand, 
				VerticalOptions = LayoutOptions.FillAndExpand,
				ItemTemplate = new DataTemplate(typeof(ListExampleCell)),
				HasUnevenRows = false,
				RowHeight = 210,
			};
			listView.SetBinding<ListExampleViewModel>(ListView.ItemsSourceProperty, v => v.Items);

            if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
                listView.ItemSelected += (sender, e) => { listView.SelectedItem = null; };

			var button = new Button() {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Text = "Duplicate list items",
				Command = ViewModel.DuplicateListItemsCommand
			};

			Content = new StackLayout() {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					listView, 
					button,
				}
			};
		}

		class ListExampleCell : ViewCell
		{
			public ListExampleCell()
			{
				var image = new CachedImage() {
					WidthRequest = 200,
					HeightRequest = 200,
					DownsampleHeight = 200,
					DownsampleUseDipUnits = true,
					TransparencyEnabled = false,
					Aspect = Aspect.AspectFill,
					CacheDuration = TimeSpan.FromDays(30),
					RetryCount = 3,
					RetryDelay = 500,
					LoadingPlaceholder = "loading.png",
				};
				image.SetBinding<ListExampleItem>(CachedImage.SourceProperty, v => v.ImageUrl);

				var fileName = new Label() {
					LineBreakMode = LineBreakMode.CharacterWrap,
					YAlign = TextAlignment.Center,
					XAlign = TextAlignment.Center,
				};
				fileName.SetBinding<ListExampleItem>(Label.TextProperty, v => v.FileName);

				var root = new AbsoluteLayout() {
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Padding = 5,
				};
					
				root.Children.Add(image, new Rectangle(0f, 0f, 200f, 200f));
				root.Children.Add(fileName, new Rectangle(200f, 0f, 150f, 200f));

				View = root;	
			}
		}
	}
}


