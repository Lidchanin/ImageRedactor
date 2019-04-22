using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace ImageRedactor
{
    public partial class SavedImagesPage : ContentPage
    {
        StackLayout stack;

        public SavedImagesPage()
        {
            InitializeComponent();

            var scroll = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
            };

            stack = new StackLayout
            {
                Orientation = StackOrientation.Vertical
            };

            scroll.Content = stack;

            Content = scroll;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadData();
        }

        void LoadData()
        {
            var files = Directory.GetFiles(Xamarin.Essentials.FileSystem.AppDataDirectory);

            foreach (var imagePath in files)
            {
                if (Path.GetExtension(imagePath) != ".png")
                    continue;

                var imageView = new Image();
                imageView.Source = ImageSource.FromFile(imagePath);

                using (var data = new StreamReader(imagePath))
                {
                }

                stack.Children.Add(imageView);
            }
        }
    }
}