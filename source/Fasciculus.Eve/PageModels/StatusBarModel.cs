﻿using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class StatusBarModel : MainThreadObservable
    {
        private const string NoError = "No Error";

        private readonly IExceptions exceptions;

        [ObservableProperty]
        private string text = NoError;

        [ObservableProperty]
        private bool isError = false;

        public StatusBarModel(IExceptions exceptions)
        {
            this.exceptions = exceptions;
            this.exceptions.PropertyChanged += OnExceptionsChanged;
        }

        private void OnExceptionsChanged(object? sender, PropertyChangedEventArgs e)
        {
            Exception? exception = exceptions.Last;

            Text = exception?.Message ?? NoError;
            IsError = exception is not null;
        }
    }
}