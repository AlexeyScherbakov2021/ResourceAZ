﻿using ResourceAZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResourceAZ.Views
{
    /// <summary>
    /// Логика взаимодействия для CalcWindow.xaml
    /// </summary>
    public partial class CalcWindow : Window
    {
        private double minPot;


        public CalcWindow(KindGroup kg, KindCalc kc, double LastA, double LastR, double MinPot)
        {
            minPot = MinPot;

            InitializeComponent();
        }
    }
}