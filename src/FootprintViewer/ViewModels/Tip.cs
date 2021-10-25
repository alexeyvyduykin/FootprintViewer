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
                var tip = new Tip
                {
                    Text = "Designer description for testing",
                    X = 20,
                    Y = 20,
                };

                return new ObservableCollection<Tip>() { tip };
            }
        }
    }
}
