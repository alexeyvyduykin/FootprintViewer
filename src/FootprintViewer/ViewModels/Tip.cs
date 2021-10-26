using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace FootprintViewer.ViewModels
{
    public class Tip : ReactiveObject
    {
        [Reactive]
        public string Title { get; set; }

        [Reactive]
        public string Text { get; set; }
        
        [Reactive]
        public double X { get; set; }

        [Reactive]
        public double Y { get; set; }
    }

    public static class TipDesigner 
    {
        public static ObservableCollection<Tip> Items
        {
            get
            {
                var tip1 = new Tip
                {
                    Text = "Designer description for testing",
                    X = 20,
                    Y = 20,
                };

                var tip2 = new Tip
                {
                    Title = "Area: 45 546.34 km²",
                    Text = "Designer description for testing",
                    X = 20,
                    Y = 80,
                };

                var tip3 = new Tip
                {
                    Title = "Distance: 1 542.61 km²",
                    X = 20,
                    Y = 140,
                };

                return new ObservableCollection<Tip>() { tip1, tip2, tip3 };
            }
        }
    }
}
